using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloGeo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao.Interno;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoValidar
	{
		#region Propriedades

		EmpreendimentoDa _da = new EmpreendimentoDa();
		PessoaDa _pessoaDa = new PessoaDa();
		EmpreendimentoMsg Msg = new EmpreendimentoMsg();
		ListaBus _busLista = new ListaBus();
		PessoaBus _busPessoa = new PessoaBus();
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoord = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
		EnderecosMsg endMsg = new EnderecosMsg();
		String objPaiNome = "Empreendimento";
		String lstEnderecosNome = "Enderecos";

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		#endregion

		public bool Salvar(Empreendimento empreendimento, bool isImportar = false)
		{
			if (!isImportar && empreendimento.Id > 0 && !EmpreendimentoEmPosse(empreendimento.Id))
			{
				Validacao.Add(Mensagem.Empreendimento.Posse);
				return Validacao.EhValido;
			}

			ValidacoesBasicas(empreendimento, isImportar);
			VerificarResponsaveis(empreendimento.Responsaveis, isImportar);
			VerificarEnderecos(empreendimento);
			VerificarCoordenada(empreendimento.Coordenada, "Empreendimento");

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

		public bool ValidarLocalizarFiscalizacao(ListarEmpreendimentoFiltro filtros)
		{
			Msg.CampoPrefixo = "Filtros";
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
				return (_busLista.Segmentos.FirstOrDefault()).Denominador;
			}

			return (_busLista.Segmentos.SingleOrDefault(x => x.Id == segmento.ToString())??new Segmento()).Denominador;
		}

		private void ValidacoesBasicas(Empreendimento empreendimento, bool isImportar = false)
		{
			Msg.CampoPrefixo = "Empreendimento";

			if (empreendimento.Segmento == null || empreendimento.Segmento <= 0)
			{
				Validacao.Add(Msg.SegmentoObrigatorio);
			}

			if (!string.IsNullOrEmpty(empreendimento.CNPJ))
			{
				if (!isImportar)
				{
					bool retorno = false;

					if (empreendimento.Id > 0)
					{
						retorno = _da.ExisteCnpj(empreendimento.CNPJ, empreendimento.Id);
					}
					else
					{
						retorno = _da.ExisteCnpj(empreendimento.CNPJ);
					}

					if (retorno)
					{
						Validacao.Add(Msg.CnpjJaExistente);
					}
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

		private void VerificarResponsaveis(List<Responsavel> responsaveis, bool isImportar = false)
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
					if (resp.Id == null || !resp.Id.HasValue || resp.Id <= 0)
					{
						Validacao.Add(Msg.ResponsaveisObrigatorio(i));
						continue;
					}

					if (!isImportar && !_pessoaDa.ExistePessoa(resp.Id.Value))
					{
						Validacao.Add(Msg.ResponsaveisNaoCadastrado(i));
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
					}

					if (resp.Tipo == 9) /*Outro*/
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

			if(!Validacao.EhValido)
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

		public bool VerificarCodigo(ListarEmpreendimentoFiltro filtrosListar, Resultados<Empreendimento> resultados)
		{
			if (filtrosListar.Codigo.GetValueOrDefault() > 0 && resultados.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.Empreendimento.CodigoNaoEncontrado);
			}
			return Validacao.EhValido;
		}

		private bool VerificarCoordenadaComAbrangencia(Coordenada coordenada, string prefixo)
		{
			if (CoordenadaBus.Validar(coordenada, prefixo, true))
			{
				coordenada.Datum.Sigla = _busLista.Datuns.SingleOrDefault(x => Equals(x.Id, coordenada.Datum.Id)).Sigla;
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

			CaracterizacaoBus caractBus = new CaracterizacaoBus();

			IEnumerable<Dependencia> lstDepDominialidade = caractBus.ObterDependenciasAtual(empreendimento, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao)
				.Where(x => x.DependenciaId > 0);

			if (lstDepDominialidade != null && lstDepDominialidade.Count() > 0)
			{
				List<DependenciaLst> dependentes = caractBus.CaracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);
				Validacao.Add(Mensagem.Empreendimento.AssociadoDependencias(lstDepDominialidade.Select(x => dependentes.First(lstDep => lstDep.TipoId == x.DependenciaTipo).TipoTexto).ToList()));
			}
			
			List<Caracterizacao> lstCaractCadastradas = caractBus.ObterCaracterizacoesEmpreendimento(empreendimento) ?? new List<Caracterizacao>();
			lstCaractCadastradas = lstCaractCadastradas.Where(x => x.Id > 0).ToList();

			List<CaracterizacaoLst> caracterizacoes = caractBus.CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			if (lstCaractCadastradas.Count > 0)
			{
				Validacao.Add(Mensagem.Empreendimento.AssociadoCaracterizacoes(lstCaractCadastradas.Select(x => caracterizacoes.Single(c => c.Id == (int)x.Tipo).Texto ).ToList()));
			}

			List<String> listString = null;
			
			//Associado ao Documento
			listString = _da.VerificarEmpreendimentoDocumento(empreendimento);
			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoDocumento(x)));
			}

			//Associado ao Processo
			listString = _da.VerificarEmpreendimentoProcesso(empreendimento);
			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoProcesso(x)));
			}

			//Associado ao Título
			listString = _da.VerificarEmpreendimentoTitulo(empreendimento);
			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoTitulo(x)));
			}

			//Associado a Fiscalização
			listString = _da.VerificarEmpreendimentoFiscalizacoes(empreendimento);
			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoFiscalizacao(x)));
			}

			return Validacao.EhValido;
		}

	}
}