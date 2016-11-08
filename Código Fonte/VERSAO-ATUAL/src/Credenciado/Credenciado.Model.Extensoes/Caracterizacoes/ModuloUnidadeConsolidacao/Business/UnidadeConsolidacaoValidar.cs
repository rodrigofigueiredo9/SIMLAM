using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business
{
	public class UnidadeConsolidacaoValidar
	{
		#region Propriedades

		UnidadeConsolidacaoDa _da = new UnidadeConsolidacaoDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		internal bool Salvar(UnidadeConsolidacao unidade)
		{
			if (!_caracterizacaoValidar.Basicas(unidade.Empreendimento.Id))
			{
				return false;
			}

			UnidadeConsolidacao auxiliar = _da.ObterPorEmpreendimento(unidade.Empreendimento.Id, true) ?? new UnidadeConsolidacao();
			unidade.InternoId = auxiliar.InternoId;
			unidade.InternoTid = auxiliar.InternoTid;

			if (unidade.Id <= 0 && auxiliar.Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (unidade.PossuiCodigoUC)
			{
				if (unidade.CodigoUC < 1)
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.CodigoUCObrigatorio);
				}
				else
				{
					if (unidade.CodigoUC.ToString().Length < 11)
					{
						Validacao.Add(Mensagem.UnidadeConsolidacao.CodigoUCInvalido);
					}
					else
					{
						if (Convert.ToInt32(unidade.CodigoUC.ToString().Substring(7)) > Convert.ToInt32(_configSys.Obter<String>(ConfiguracaoSistema.KeyUnidadeConsolidacaoMaxCodigoUC)))
						{
							Validacao.Add(Mensagem.UnidadeConsolidacao.CodigoUCSuperiorMaximo);
						}
						else if (_da.CodigoUCExiste(unidade))
						{
							Validacao.Add(Mensagem.UnidadeConsolidacao.CodigoUCJaExistente);
						}
					}
				}
			}

			if (string.IsNullOrWhiteSpace(unidade.LocalLivroDisponivel))
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.LocalLivroDisponivelObrigatorio);
			}

			#region Cultivar

			if (unidade.Cultivares.Count < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CapacidadeProcessamentoObrigatorioAdicionar);
			}
			else
			{
				if (unidade.Cultivares.Exists(x => x.CulturaId < 1) ||
					unidade.Cultivares.Exists(x => x.CapacidadeMes <= 0) ||
					unidade.Cultivares.Exists(x => x.UnidadeMedida < 1))
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.PossuiCultivarInvalido);
				}

				if (unidade.Cultivares.Any(x => unidade.Cultivares.Count(y => y.Id == x.Id && y.Id > 0) > 1))
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.CultivarJaAdicionado);
				}
			}

			#endregion

			#region Responsável Técnico

			if (unidade.ResponsaveisTecnicos.Count < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoObrigatorioAdicionar);
			}
			else
			{
				if (unidade.ResponsaveisTecnicos.Exists(x => x.Id < 1) ||
					unidade.ResponsaveisTecnicos.Exists(x => string.IsNullOrEmpty(x.CFONumero)) ||
					unidade.ResponsaveisTecnicos.Exists(x => string.IsNullOrEmpty(x.NumeroArt)) ||
					unidade.ResponsaveisTecnicos.Exists(x => !x.ArtCargoFuncao && !ValidacoesGenericasBus.ValidarData(x.DataValidadeART)))
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.PossuiResponsavelTecnicoInvalido);
				}
				else
				{
					unidade.ResponsaveisTecnicos.ForEach(responsavel =>
					{
						ValidarAssociarResponsavelTecnicoHabilitado(new HabilitarEmissaoCFOCFOC { Responsavel = new CredenciadoIntEnt() { Id = responsavel.Id } }, unidade.Cultivares);
					});
				}

				if (unidade.ResponsaveisTecnicos.Any(x => unidade.ResponsaveisTecnicos.Count(y => y.Id == x.Id) > 1))
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoJaAdicionado);
				}
			}

			#endregion

			if (string.IsNullOrWhiteSpace(unidade.TipoApresentacaoProducaoFormaIdentificacao))
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.TipoApresentacaoProducaoFormaIdentificacaoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public void ValidarCultivar(List<Cultivar> cultivarLista, Cultivar cultivar)
		{
			cultivarLista = cultivarLista ?? new List<Cultivar>();

			if (cultivar.CulturaId < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CulturaObrigatorio);
			}

			if (cultivar.Id < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CultivarObrigatorio);
			}

			if (cultivar.CapacidadeMes <= 0)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CapacidadeMesObrigatorio);
			}

			if (cultivar.UnidadeMedida < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.UnidadeMedidaObrigatorio);
			}

			if (cultivarLista.Count(x => x.Id == cultivar.Id && cultivar.Id > 0) > 0)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CultivarJaAdicionado);
			}

			if (cultivarLista.Count(x => x.CulturaId == cultivar.CulturaId && cultivar.Id == 0) > 0)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CulturaJaAdicionada);
			}
		}

		public bool ValidarAssociarResponsavelTecnicoHabilitado(HabilitarEmissaoCFOCFOC habilitacao, List<Cultivar> cultivares)
		{
			#region Configurar

			cultivares = cultivares ?? new List<Cultivar>();

			if (habilitacao.Pragas == null || habilitacao.Pragas.Count < 1)
			{
				habilitacao = _caracterizacaoBus.ObterHabilitacaoPorCredenciado(habilitacao.Responsavel.Id);
				if (Validacao.EhValido)
				{
					habilitacao.Pragas.ForEach(x =>
					{
						x.Praga.Culturas = _caracterizacaoBus.ObterCulturas(x.Praga.Id);
					});
				}
				else
				{
					return false;
				}
			}

			#endregion

			if (habilitacao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.HabilitacaoInativa);
				return false;
			}

			if (!habilitacao.Pragas.SelectMany(p => p.Praga.Culturas).ToList().Exists(c => cultivares.Exists(x => x.CulturaId == c.Id)))
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoNaoHabilitadoParaCultura);
				return false;
			}

			foreach (var habPraga in habilitacao.Pragas)
			{
				foreach (var cultivarItem in habPraga.Praga.Culturas.SelectMany(x => x.LstCultivar))
				{
					foreach (var item in cultivares)
					{
						if (cultivarItem.Id == item.Id && DateTime.Parse(habPraga.DataFinalHabilitacao) < DateTime.Today)
						{
							Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoHabilitacaoPragaVencida);
							return false;
						}
					}
				}
			}

			return Validacao.EhValido;
		}

		public void ValidarResponsavelTecnico(List<ResponsavelTecnico> responsavelTecnicoLista, ResponsavelTecnico responsavelTecnico)
		{
			responsavelTecnicoLista = responsavelTecnicoLista ?? new List<ResponsavelTecnico>();

			if (responsavelTecnico.Id < 1)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoObrigatorio);
			}

			if (string.IsNullOrEmpty(responsavelTecnico.CFONumero))
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.NumeroHabilitacaoCFOCFOCObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(responsavelTecnico.NumeroArt))
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.NumeroARTObrigatorio);
			}

			if (!responsavelTecnico.ArtCargoFuncao)
			{
				ValidacoesGenericasBus.DataMensagem(new DateTecno() { DataTexto = responsavelTecnico.DataValidadeART }, "DataValidadeART", "validade da ART", false);
			}

			if (responsavelTecnicoLista.Count(x => x.Id == responsavelTecnico.Id) > 0)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.ResponsavelTecnicoJaAdicionado);
			}
		}

		public bool Acessar(int empreendimentoId, int projetoDigitalId)
		{
			return _caracterizacaoValidar.Dependencias(empreendimentoId, projetoDigitalId, (int)eCaracterizacao.UnidadeConsolidacao);
		}

		internal bool CopiarDadosInstitucional(UnidadeConsolidacao caracterizacao)
		{
			if (caracterizacao.InternoId <= 0)
			{
				Validacao.Add(Mensagem.UnidadeConsolidacao.CopiarCaractizacaoCadastrada);
			}

			return Validacao.EhValido;
		}
	}
}