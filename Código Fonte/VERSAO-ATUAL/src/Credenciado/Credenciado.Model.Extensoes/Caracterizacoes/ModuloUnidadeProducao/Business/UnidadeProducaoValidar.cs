using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business
{
	public class UnidadeProducaoValidar
	{
		#region Propriedades

		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		UnidadeProducaoDa _da = new UnidadeProducaoDa();
		Configuracao.ConfiguracaoSistema _configSys = new Configuracao.ConfiguracaoSistema();

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioConsulta); }
		}

		#endregion

		internal bool Salvar(UnidadeProducao caracterizacao, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.Empreendimento.Id))
			{
				return false;
			}

			UnidadeProducao auxiliar = _da.ObterPorEmpreendimento(caracterizacao.Empreendimento.Id, true) ?? new UnidadeProducao();

			if (caracterizacao.Id <= 0 && auxiliar.Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.Empreendimento.Id, projetoDigitalId))
			{
				return false;
			}

			if (caracterizacao.PossuiCodigoPropriedade)
			{
				if (caracterizacao.CodigoPropriedade < 1)
				{
					Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeObrigatorio);
				}
				else
				{
					if (caracterizacao.CodigoPropriedade > Convert.ToInt64(_configSys.Obter<String>(ConfiguracaoSistema.KeyUnidadeProducaoMaxCodigoPropriedade)))
					{
						Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeSuperiorMaximo);
					}
					else if (_da.CodigoPropriedadeExistente(caracterizacao))
					{
						Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeJaExiste);
					}
				}
			}

			if (string.IsNullOrEmpty(caracterizacao.LocalLivroDisponivel))
			{
				Validacao.Add(Mensagem.UnidadeProducao.LocalLivroDisponivelObrigatorio);
			}

			if (caracterizacao.UnidadesProducao.Count < 1)
			{
				Validacao.Add(Mensagem.UnidadeProducao.UnidadeProducaoObrigatorio);
			}

			for (int i = 0; i < caracterizacao.UnidadesProducao.Count; i++)
			{
				if (SalvarItemUnidadeProducao(caracterizacao.UnidadesProducao[i], caracterizacao.Empreendimento.Id).Count > 0)
				{
					Validacao.Add(Mensagem.UnidadeProducao.UnidadeProducaoItemIncorreto);
					break;
				}
			}

			return Validacao.EhValido;
		}

		public List<Mensagem> SalvarItemUnidadeProducao(UnidadeProducaoItem unidade, int empreendimentoID)
		{
			List<Mensagem> mensagens = new List<Mensagem>();

			if (unidade.PossuiCodigoUP)
			{
				if (unidade.CodigoUP < 1)
				{
					mensagens.Add(Mensagem.UnidadeProducao.CodigoUPObrigatorio);
				}
				else
				{
					if (unidade.CodigoUP.ToString().Length < 15)
					{
						mensagens.Add(Mensagem.UnidadeProducao.CodigoUPInvalido);
					}
					else
					{
						if (_da.CodigoUPExistente(unidade, empreendimentoID))
						{
							mensagens.Add(Mensagem.UnidadeProducao.CodigoUPJaExiste);
						}
					}
				}
			}

			if (unidade.TipoProducao == (int)eUnidadeProducaoTipoProducao.MaterialPropagacao)
			{
				if (String.IsNullOrEmpty(unidade.RenasemNumero))
				{
					mensagens.Add(Mensagem.UnidadeProducao.RenasemNumeroObrigatorio);
				}

				if (String.IsNullOrEmpty(unidade.DataValidadeRenasem))
				{
					mensagens.Add(Mensagem.Padrao.DataObrigatoria("UnidadeProducaoItem_DataValidadeRenasem", "validade"));
				}

				if (!String.IsNullOrEmpty(unidade.DataValidadeRenasem) && !ValidacoesGenericasBus.ValidarData(unidade.DataValidadeRenasem))
				{
					mensagens.Add(Mensagem.Padrao.DataInvalida("UnidadeProducaoItem_DataValidadeRenasem", "validade"));
				}


				if (!String.IsNullOrEmpty(unidade.DataValidadeRenasem) && ValidacoesGenericasBus.ValidarData(unidade.DataValidadeRenasem) && Convert.ToDateTime(unidade.DataValidadeRenasem) <= DateTime.Today)
				{
					mensagens.Add(Mensagem.UnidadeProducao.RenasemDataVencimentoMenorQueDataAtual);
				}
			}
			else
			{
				unidade.RenasemNumero = string.Empty;
				unidade.DataValidadeRenasem = string.Empty;
			}

			if (unidade.AreaHA <= 0)
			{
				mensagens.Add(Mensagem.UnidadeProducao.AreaHAObrigatorio);
			}

			if (unidade.Coordenada.EastingUtm.GetValueOrDefault() < 1 || unidade.Coordenada.NorthingUtm.GetValueOrDefault() < 1)
			{
				mensagens.Add(Mensagem.UnidadeProducao.CoordenadaObrigatorio);
			}

			if (unidade.Coordenada.EastingUtm <= 0)
			{
				mensagens.Add(Mensagem.UnidadeProducao.EastingUtmObrigatorio);
			}

			if (unidade.Coordenada.NorthingUtm <= 0)
			{
				mensagens.Add(Mensagem.UnidadeProducao.NorthingUtmObrigatorio);
			}

			if (unidade.CulturaId < 1)
			{
				mensagens.Add(Mensagem.UnidadeProducao.CulturaObrigatorio);
			}

			if (unidade.CultivarId < 1)
			{
				mensagens.Add(Mensagem.UnidadeProducao.CultivarObrigatorio);
			}

			if (string.IsNullOrEmpty(unidade.DataPlantioAnoProducao))
			{
				mensagens.Add(Mensagem.UnidadeProducao.DataPlantioAnoProducaoObrigatorio);
			}
			else
			{
				string[] mesAno = unidade.DataPlantioAnoProducao.Split('/');

				if (unidade.DataPlantioAnoProducao.Length != 7 || mesAno.Length < 2 || Convert.ToInt32(mesAno.GetValue(0)) <= 0 || Convert.ToInt32(mesAno.GetValue(0)) > 12 || Convert.ToInt32(mesAno.GetValue(1)) <= 0)
				{
					mensagens.Add(Mensagem.UnidadeProducao.DataPlantioAnoProducaoInvalida);
				}
				else
				{
					if (Convert.ToInt32(mesAno.GetValue(1)) > DateTime.Now.Year || (Convert.ToInt32(mesAno.GetValue(0)) > DateTime.Now.Month && Convert.ToInt32(mesAno.GetValue(1)) == DateTime.Now.Year))
					{
						mensagens.Add(Mensagem.UnidadeProducao.DataPlantioAnoProducaoMaiorAtual);
					}
				}
			}

			if (unidade.Produtores.Count < 1)
			{
				Validacao.Add(Mensagem.UnidadeProducao.ProdutorObrigatorio);
			}

			unidade.Produtores.ForEach(x =>
			{
				if (!_da.VerificarResponsavelEmpreendimento(empreendimentoID, x.Id.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.UnidadeProducao.ProdutorNaoEstaMaisVinculadoNoEmpreendimento(x.NomeRazao));
				}
			});

			if (unidade.ResponsaveisTecnicos.Count < 1)
			{
				mensagens.Add(Mensagem.UnidadeProducao.ResponsavelTecnicoObrigatorio);
			}
			else
			{
				foreach (var item in unidade.ResponsaveisTecnicos)
				{
					if (!item.ArtCargoFuncao)
					{
						if (!ValidacoesGenericasBus.ValidarData(item.DataValidadeART))
						{
							mensagens.Add(Mensagem.Padrao.DataInvalida("UnidadeProducaoItem_ResponsavelTecnico_DataValidadeART", "validade da ART"));
						}
					}
					else
					{
						item.DataValidadeART = string.Empty;
					}
				}
			}

			if (unidade.EstimativaProducaoQuantidadeAno <= 0)
			{
				mensagens.Add(Mensagem.UnidadeProducao.EstimativaProducaoQuantidadeAnoObrigatorio);
			}

			if (string.IsNullOrEmpty(unidade.EstimativaProducaoUnidadeMedida))
			{
				mensagens.Add(Mensagem.UnidadeProducao.EstimativaProducaoUnidadeMedidaObrigatorio);
			}

			return mensagens;
		}

		public bool ValidarResponsavelTecnicoHabilitado(HabilitarEmissaoCFOCFOC habilitacao, Cultura cultura)
		{
			if (habilitacao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo)
			{
				Validacao.Add(Mensagem.UnidadeProducao.HabilitacaoInativa);
				return false;
			}

			if (!habilitacao.Pragas.SelectMany(p => p.Praga.Culturas).ToList().Exists(c => c.Id == cultura.Id))
			{
				Validacao.Add(Mensagem.UnidadeProducao.ResponsavelTecnicoNaoHabilitadoParaCultura);
				return Validacao.EhValido;
			}

			List<PragaHabilitarEmissao> lista = habilitacao.Pragas.Where(p => p.Praga.Culturas.Exists(c => c.Id == cultura.Id)).ToList();
			foreach (var item in lista)
			{
				if (DateTime.Parse(item.DataFinalHabilitacao) < DateTime.Today)
				{
					Validacao.Add(Mensagem.UnidadeProducao.PragaCulturaDataFinalVencida);
					break;
				}
			}

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId, int projetoDigitalId)
		{
			_caracterizacaoValidar.Dependencias(empreendimentoId, projetoDigitalId, (int)eCaracterizacao.UnidadeProducao);

			return Validacao.EhValido;
		}

		internal bool CopiarDadosInstitucional(UnidadeProducao caracterizacao)
		{
			if (caracterizacao.InternoID <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.CopiarCaractizacaoCadastrada);
			}

			return Validacao.EhValido;
		}
	}
}