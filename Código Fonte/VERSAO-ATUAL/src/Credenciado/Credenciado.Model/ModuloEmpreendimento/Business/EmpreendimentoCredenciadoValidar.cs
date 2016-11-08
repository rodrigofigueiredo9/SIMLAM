using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloGeo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao.Interno;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoCredenciadoValidar
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		EmpreendimentoCredenciadoDa _da;
		EmpreendimentoMsg Msg;
		PessoaCredenciadoBus _busPessoa;

		EnderecosMsg endMsg;
		String objPaiNome;
		String lstEnderecosNome;

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public EmpreendimentoCredenciadoValidar()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new EmpreendimentoCredenciadoDa();
			Msg = new EmpreendimentoMsg();
			_busPessoa = new PessoaCredenciadoBus();

			endMsg = new EnderecosMsg();
			objPaiNome = "Empreendimento";
			lstEnderecosNome = "Enderecos";
		}

		public bool Salvar(Empreendimento empreendimento)
		{
			if (empreendimento.Id > 0 && !EmpreendimentoEmPosse(empreendimento.Id))
			{
				Validacao.Add(Mensagem.Empreendimento.Posse);
				return Validacao.EhValido;
			}

			ValidacoesBasicas(empreendimento);
			VerificarResponsaveis(empreendimento.Responsaveis);
			VerificarEnderecos(empreendimento);
			VerificarCoordenada(empreendimento.Coordenada, "Empreendimento");

			return Validacao.EhValido;
		}

		public bool VerificarCodigo(ListarEmpreendimentoFiltro filtrosListar, Resultados<Empreendimento> resultados)
		{
			if (filtrosListar.Codigo.GetValueOrDefault() > 0 && resultados.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.Empreendimento.CodigoNaoEncontrado);
			}
			return Validacao.EhValido;
		}

		public bool ValidarLocalizar(ListarEmpreendimentoFiltro filtros)
		{
			Msg.CampoPrefixo = "Filtros";

			if (filtros.PossuiCodigo)
			{
				if (filtros.Codigo.GetValueOrDefault() < 1)
				{
					Validacao.Add(Msg.CodigoObrigatorio);

				}
				return Validacao.EhValido;
			}

			//if ((filtros.EstadoId ?? 0) <= 0)
			//{
			//	Validacao.Add(Msg.EstadoObrigatorio);
			//}

			//if ((filtros.MunicipioId ?? 0) <= 0)
			//{
			//	Validacao.Add(Msg.MunicipioObrigatorio);
			//}

			VerificarCoordenadaComAbrangencia(filtros.Coordenada, "Filtros");

			if (String.IsNullOrWhiteSpace(filtros.AreaAbrangencia))
			{
				Validacao.Add(Msg.AreaAbrangenciaObrigatoria);
			}
			else if (Int32.Parse(filtros.AreaAbrangencia) <= 0)
			{
				Validacao.Add(Msg.AreaAbrangenciaMaiorZero);
			}

			return Validacao.EhValido;
		}

		public bool EmPosse(int empreendimento)
		{
			return _da.EmPosse(empreendimento);
		}

		public bool EmpreendimentoEmPosse(int empreendimentoId)
		{
			return EmPosse(empreendimentoId) || ((User as EtramitePrincipal).IsInRole(ePermissao.EmpreendimentoEditarSemPosse.ToString()));
		}

		private String BuscarSegmento(Int32 segmento)
		{
			if (segmento <= 0)
			{
				return (ListaCredenciadoBus.Segmentos.FirstOrDefault()).Denominador;
			}

			return (ListaCredenciadoBus.Segmentos.SingleOrDefault(x => x.Id == segmento.ToString()) ?? new Segmento()).Denominador;
		}

		private void ValidacoesBasicas(Empreendimento empreendimento)
		{
			Msg.CampoPrefixo = "Empreendimento";

			if (empreendimento.Segmento == null || empreendimento.Segmento <= 0)
			{
				Validacao.Add(Msg.SegmentoObrigatorio);
			}

			if (!string.IsNullOrEmpty(empreendimento.CNPJ))
			{
				bool retorno = false;

				if (empreendimento.Id > 0)
				{
					retorno = _da.ExisteCnpj(empreendimento.CNPJ, User.EtramiteIdentity.FuncionarioId, empreendimento.Id);
				}
				else
				{
					retorno = _da.ExisteCnpj(empreendimento.CNPJ, User.EtramiteIdentity.FuncionarioId);
				}

				if (retorno)
				{
					Validacao.Add(Msg.CnpjJaExistente);
				}

				if (!ValidacoesGenericasBus.Cnpj(empreendimento.CNPJ))
				{
					Validacao.Add(Msg.CnpjInvalido);
				}
			}

			if (String.IsNullOrWhiteSpace(empreendimento.Denominador))
			{
				Validacao.Add(Msg.DenominadorObrigatorio(BuscarSegmento(empreendimento.Segmento ?? 1)));
			}
		}

		private void VerificarResponsaveis(List<Responsavel> responsaveis)
		{
			if (responsaveis == null || responsaveis.Count <= 0)
			{
				Validacao.Add(Msg.ResponsavelObrigatorio);
			}
			else
			{
				int i = 0;
				foreach (Responsavel resp in responsaveis)
				{
					if (resp.Id.GetValueOrDefault() <= 0 && resp.InternoId <= 0)
					{
						Validacao.Add(Msg.ResponsaveisObrigatorio(i));
						continue;
					}

					if (resp.Tipo <= 0)
					{
						Validacao.Add(Msg.ResponsavelTipoObrigatorio(i, resp.NomeRazao));
					}

					if (resp.Tipo == 3)
					{
						if (resp.DataVencimento == DateTime.MinValue)
						{
							Validacao.Add(Msg.ResponsavelDataVencimentoObrigatorio(i));
						}

						if (resp.DataVencimento <= DateTime.Now)
						{
							Validacao.Add(Msg.ResponsavelDataVencimentoPassado(i));
						}
					}

					if (resp.Tipo == 9/*Outro*/)
					{
						if (String.IsNullOrWhiteSpace(resp.EspecificarTexto))
						{
							Validacao.Add(Msg.ResponsavelEspecificarTextoObrigatorio(i));
						}
					}

					i++;
				}
			}
		}

		private void VerificarEnderecos(Empreendimento empreendimento)
		{
			if (empreendimento.Enderecos == null || empreendimento.Enderecos.Count <= 0)
			{
				Validacao.Add(endMsg.EnderecoObrigatorio(objPaiNome, lstEnderecosNome, 0, "localização"));
			}
			else
			{
				if (empreendimento.Enderecos.Count > 0)
				{
					VerificarDadosEndereco(empreendimento.Enderecos[0], objPaiNome, lstEnderecosNome, 0, "localização", true);
				}

				//remove o segundo endereço quando não tem correspondência
				if (empreendimento.Enderecos.Count > 1 && empreendimento.TemCorrespondencia <= 0)
				{
					if (String.IsNullOrWhiteSpace(empreendimento.Enderecos[0].Cep))
					{
						Validacao.Add(endMsg.EnderecoCepObrigatorio(objPaiNome, lstEnderecosNome, 0, "localização"));
					}

					if (!String.IsNullOrWhiteSpace(empreendimento.Enderecos[0].Cep) && !(new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(empreendimento.Enderecos[0].Cep)))
					{
						Validacao.Add(endMsg.EnderecoCepInvalido(objPaiNome, lstEnderecosNome, 0, "localização"));
					}

					empreendimento.Enderecos.RemoveAt(1);
				}

				if (empreendimento.Enderecos.Count >= 2 && empreendimento.TemCorrespondencia > 0)
				{
					VerificarDadosEndereco(empreendimento.Enderecos[1], objPaiNome, lstEnderecosNome, 1, "correspondência");
				}
			}
		}

		private void VerificarDadosEndereco(Endereco endereco, string objPaiNome, string lstEndNome, int index, string nomeEndereco, bool localizacao = false)
		{
			if (!localizacao && String.IsNullOrWhiteSpace(endereco.Cep))
			{
				Validacao.Add(endMsg.EnderecoCepObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (!String.IsNullOrWhiteSpace(endereco.Cep) && !(new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(endereco.Cep)))
			{
				Validacao.Add(endMsg.EnderecoCepInvalido(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (localizacao && endereco.ZonaLocalizacaoId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(endMsg.EnderecoZonaLocalizacaoObrigatoria(objPaiNome, lstEnderecosNome, 0));
			}

			if (String.IsNullOrWhiteSpace(endereco.Logradouro))
			{
				Validacao.Add(endMsg.EnderecoLogradouroObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (String.IsNullOrWhiteSpace(endereco.Bairro))
			{
				Validacao.Add(endMsg.EnderecoBairroObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (localizacao && String.IsNullOrWhiteSpace(endereco.DistritoLocalizacao))
			{
				Validacao.Add(endMsg.EnderecoDistritoObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (endereco.EstadoId <= 0)
			{
				Validacao.Add(endMsg.EnderecoEstadoObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}
			else if (!_da.ExisteEstado(endereco.EstadoId))
			{
				Validacao.Add(endMsg.EnderecoEstadoInvalido(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (endereco.MunicipioId <= 0)
			{
				Validacao.Add(endMsg.EnderecoMunicipioObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}
			else if (!_da.ExisteMunicipio(endereco.MunicipioId))
			{
				Validacao.Add(endMsg.EnderecoMunicipioInvalido(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (endereco.MunicipioId > 0 && endereco.EstadoId > 0 && _da.ObterMunicipio(endereco.MunicipioId).Estado.Id != endereco.EstadoId)
			{
				Validacao.Add(endMsg.EnderecoMunicipioOutroEstado(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}

			if (localizacao && String.IsNullOrWhiteSpace(endereco.Complemento))
			{
				Validacao.Add(endMsg.EnderecoComplementoObrigatorio(objPaiNome, lstEnderecosNome, index, nomeEndereco));
			}
		}

		private bool VerificarCoordenada(Coordenada coordenada, string prefixo)
		{
			CoordenadaMsg msg = new CoordenadaMsg(prefixo);

			if (coordenada.LocalColeta.GetValueOrDefault() <= 0)
			{
				Validacao.Add(msg.LocalColetaObrigatorio);
			}

			if (coordenada.FormaColeta.GetValueOrDefault() <= 0)
			{
				Validacao.Add(msg.FormaColetaObrigatorio);
			}

			CoordenadaBus.Validar(coordenada, prefixo, true);

			if (!Validacao.EhValido)
			{
				return false;
			}

			if (_da.PontoForaMBR(coordenada.EastingUtm.GetValueOrDefault(), coordenada.NorthingUtm.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.Sistema.CoordenadaForaMBR);
				return false;
			}

			return Validacao.EhValido;
		}

		private bool VerificarCoordenadaComAbrangencia(Coordenada coordenada, string prefixo)
		{
			if (CoordenadaBus.Validar(coordenada, prefixo, true))
			{
				coordenada.Datum.Sigla = ListaCredenciadoBus.Datuns.SingleOrDefault(x => Equals(x.Id, coordenada.Datum.Id)).Sigla;
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			if (_da.PontoForaMBR(coordenada.EastingUtm.GetValueOrDefault(), coordenada.NorthingUtm.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.Sistema.CoordenadaForaMBR);
				return false;
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int empreendimento)
		{
			if (!EmPosse(empreendimento))
			{
				Validacao.Add(Mensagem.Empreendimento.Posse);
				return Validacao.EhValido;
			}

			List<String> requerimentosAssociados = _da.VerificarEmpreendimentoRequerimento(empreendimento);
			if (requerimentosAssociados.Count > 0)
			{
				foreach (String numero in requerimentosAssociados)
				{
					Validacao.Add(Mensagem.Empreendimento.EmpreedimentoAssociado("requerimento", numero));
				}
			}

			return Validacao.EhValido;
		}
	}
}