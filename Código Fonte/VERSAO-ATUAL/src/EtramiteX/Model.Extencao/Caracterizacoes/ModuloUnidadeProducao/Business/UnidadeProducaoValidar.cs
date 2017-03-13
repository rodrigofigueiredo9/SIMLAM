using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using System.Text.RegularExpressions;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business
{
	public class UnidadeProducaoValidar
	{
		#region Propriedades

		UnidadeProducaoDa _da = new UnidadeProducaoDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		HabilitarEmissaoCFOCFOCBus _busHabilitacaoCFOCFOC = new HabilitarEmissaoCFOCFOCBus();

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		internal bool Salvar(UnidadeProducao caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.Empreendimento.Id))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.Empreendimento.Id, true) ?? new UnidadeProducao()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.Empreendimento.Id))
			{
				return false;
            }

            if ((caracterizacao.Id > 0 || caracterizacao.PossuiCodigoPropriedade) && caracterizacao.CodigoPropriedade.ToString().Length < 11)
            {
                Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeInvalido);
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
                    if (Convert.ToInt64(caracterizacao.CodigoPropriedade.ToString().Substring(7, 4)) > Convert.ToInt64(_configSys.Obter<String>(ConfiguracaoSistema.KeyUnidadeProducaoMaxCodigoPropriedade)))
					{
						Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeSuperiorMaximo);
					}
					else if (_da.CodigoPropriedadeExistente(caracterizacao.Id, caracterizacao.CodigoPropriedade))
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

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.UnidadeProducao))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		public List<Mensagem> SalvarItemUnidadeProducao(UnidadeProducaoItem unidade, int empreendimento)
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
					if (unidade.CodigoUP.ToString().Length < 17)
					{
						mensagens.Add(Mensagem.UnidadeProducao.CodigoUPInvalido);
					}
					else
					{
						if (_da.CodigoUPExistente(unidade.Id, unidade.CodigoUP))
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
				if (!_da.VerificarResponsavelEmpreendimento(empreendimento, x.Id.GetValueOrDefault()))
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

		internal bool CopiarDadosCredenciado(UnidadeProducao caracterizacao)
		{
			if (caracterizacao.CredenciadoID <= 0)
			{
				Validacao.Add(Mensagem.UnidadeProducao.CopiarCaractizacaoCadastrada);
			}

			string auxiliar = _da.CodigoPropriedadeExistenteImportar(caracterizacao);
			if(!string.IsNullOrEmpty(auxiliar))
			{
				Validacao.Add(Mensagem.UnidadeProducao.CodigoPropriedadeExistenteImportar(auxiliar));
			}

			caracterizacao.UnidadesProducao.ForEach(unidade => {
				EmpreendimentoCaracterizacao emp = _da.VerificarCodigoUPJaCadastrado(unidade.CodigoUP, caracterizacao.Empreendimento.Id);

				if (emp != null && emp.Id > 0)
				{
					Validacao.Add(Mensagem.UnidadeProducao.CodigoUPJaAssociado(emp.Codigo.GetValueOrDefault(), emp.Denominador));
				}
			});

			return Validacao.EhValido;
		}
	}
}