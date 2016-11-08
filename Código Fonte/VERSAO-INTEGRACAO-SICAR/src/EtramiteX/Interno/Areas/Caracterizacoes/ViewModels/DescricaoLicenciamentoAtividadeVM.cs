using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class DescricaoLicenciamentoAtividadeVM
	{
		public DescricaoLicenciamentoAtividade DscLicAtividade { get; set; }

		public List<SelectListItem> ResponsaveisEmpreendimento { set; get; }
		public List<SelectListItem> FontesAbastecimentoAguaTipo { set; get; }
		public List<SelectListItem> PontosLancamentoEfluenteTipo { set; get; }
		public List<SelectListItem> OutorgaAguaTipo { set; get; }
		public List<SelectListItem> FontesGeracaoTipo { set; get; }
		public List<SelectListItem> UnidadeTipo { set; get; }
		public List<SelectListItem> CombustivelTipo { set; get; }

		public List<Lista> VegetacaoAreaUtil { set; get; }
		public List<Lista> Acondicionamento { set; get; }
		public List<Lista> Estocagem { set; get; }
		public List<Lista> Tratamento { set; get; }
		public List<Lista> DestinoFinal { set; get; }

		public string CaracterizacaoTipoTexto { get; set; }
		public bool IsCadastrarCaracterizacao { get; set; }
		public string UrlAvancar { get; set; }

		public string TextoMerge { get; set; }
		public string AtualizarDependenciasModalTitulo { get; set; }

		public bool IsVisualizar { get; set; }

		public string FontesAbastecimentoAgua 
		{ 
			get 
			{
				return this.GetJSON(new
				{
					@NaoPossui = new { @Id = (int)eFontesAbastecimentoAgua.NaoPossui },
					@RedePublica = new { @Id = (int)eFontesAbastecimentoAgua.RedePublica, @CampoRespectivo = "Empresa Fornecedora *" },
					@Pocos = new { @Id = (int)eFontesAbastecimentoAgua.Pocos, @CampoRespectivo = "Quantidade *" },
					@CursoDaguaRiosCorregosRiachos = new { @Id = (int)eFontesAbastecimentoAgua.CursoDaguaRiosCorregosRiachos, @CampoRespectivo = "Nome *" },
					@LagoLagoa = new { @Id = (int)eFontesAbastecimentoAgua.LagoLagoa, @CampoRespectivo = "Nome *" },
					@AguasCosteiras = new { @Id = (int)eFontesAbastecimentoAgua.AguasCosteiras, @CampoRespectivo = "Praia *" }
				});
			} 
		}

		public string PontosLancamentoEfluente
		{
			get
			{
				return this.GetJSON(new
				{
					@NaoPossui = new { @Id = (int)ePontosLancamentoEfluente.NaoPossui },
					@CursoDaguaRiosCorregosRiachos = new { @Id = (int)ePontosLancamentoEfluente.CursoDaguaRiosCorregosRiachos, @CampoRespectivo = "Nome *" },
					@LagoLagoa = new { @Id = (int)ePontosLancamentoEfluente.LagoLagoa, @CampoRespectivo = "Nome *" },
					@AguasCosteiras = new { @Id = (int)ePontosLancamentoEfluente.AguasCosteiras, @CampoRespectivo = "Praia *" },
					@RedePluvialPublica = new { @Id = (int)ePontosLancamentoEfluente.RedePluvialPublica, @CampoRespectivo = "Empresa Fornecedora *" },
					@RedeEsgoto = new { @Id = (int)ePontosLancamentoEfluente.RedeEsgoto, @CampoRespectivo = "Empresa Fornecedora *" }
				});
			}
		}

		public bool IsExibirEfluentesLiquidos
		{
			get 
			{
				if (DscLicAtividade == null)
				{
					return false;
				}

				return !(DscLicAtividade.FonteAbastecimentoAguaTipoId == (int)eFontesAbastecimentoAgua.NaoPossui);
			}
		}
		

		public int FontesGeracaoOutrosId { get { return (int)eFontesGeracao.Outros; } }
		public int TratamentoOutrasFormasId { get { return (int)eTratamento.OutrasFormas; } }
		public int DestinoFinalOutrosId { get { return (int)eDestinoFinal.Outros; } }

		public DescricaoLicenciamentoAtividadeVM()
		{
			this.DscLicAtividade = new DescricaoLicenciamentoAtividade();

			this.FontesAbastecimentoAguaTipo = new List<SelectListItem>();
			this.PontosLancamentoEfluenteTipo = new List<SelectListItem>();
			this.OutorgaAguaTipo = new List<SelectListItem>();
			this.FontesGeracaoTipo = new List<SelectListItem>();
			this.UnidadeTipo = new List<SelectListItem>();
			this.CombustivelTipo = new List<SelectListItem>();
			this.ResponsaveisEmpreendimento = new List<SelectListItem>();

			this.VegetacaoAreaUtil = new List<Lista>();
			this.Acondicionamento = new List<Lista>();
			this.Estocagem = new List<Lista>();
			this.Tratamento = new List<Lista>();
			this.DestinoFinal = new List<Lista>();

			this.UrlAvancar = 
			this.CaracterizacaoTipoTexto = string.Empty;

			this.IsVisualizar = false;
		}

		public bool IsChecked(int? id, int? codigo)
		{
			return (id & codigo) != 0;
		}

		public String GetJSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}		

		public String Mensagens()
		{
			return this.GetJSON(new {
				@SelecioneResponsavel = Mensagem.DescricaoLicenciamentoAtividadeMsg.SelecioneResponsavel,
				@InformePatrimonio = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformePatrimonio,
				@InformeResidencia = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeResidencia,
				@InformeDistancia = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeDistancia,
				@InformeAreaUtil = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeAreaUtil,
				@SelecioneTipoFonteAbastecimento = Mensagem.DescricaoLicenciamentoAtividadeMsg.SelecioneTipoFonteAbastecimento,
				@FonteAguaExistente = Mensagem.DescricaoLicenciamentoAtividadeMsg.FonteAguaExistente,
				@InformeFonteUso = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeFonteUso,				
				@AddFonteAbastecimento = Mensagem.DescricaoLicenciamentoAtividadeMsg.AddFonteAbastecimento,
				@InformeConsumo = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeConsumo,

				@AddPontoLancamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.AddPontoLancamento,
				@SelecionePontoLancamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.SelecionePontoLancamento,
				@InformePontoLancamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformePontoLancamento,
				@PontoLancamentoExistente = Mensagem.DescricaoLicenciamentoAtividadeMsg.PontoLancamentoExistente,

				@SelecioneTipoFonteGeracao = Mensagem.DescricaoLicenciamentoAtividadeMsg.SelecioneTipoFonteGeracao,
				@TipoFonteGeracaoExistente = Mensagem.DescricaoLicenciamentoAtividadeMsg.TipoFonteGeracaoExistente,
				@InformeVazao = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeVazao,
				@InformeUnidade = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeUnidade,
				@InformeSisTratamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeSisTratamento,
				@InformeOutraFonteGeracao = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeOutraFonteGeracao,
				@AddFonteGeracao = Mensagem.DescricaoLicenciamentoAtividadeMsg.AddFonteGeracao,

				@InformeClasseResiduo = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeClasseResiduo,
				@InformeTipoResiduo = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeTipoResiduo,
				@InformeAcondicionamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeAcondicionamento,
				@InformeEstocagem = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeEstocagem,
				@InformeTratamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeTratamento,
				@InformeTratamentoOutro = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeTratamentoOutro,
				@InformeDestinoFinal = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeDestinoFinal,
				@InformeDestinoFinalOutro = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeDestinoFinalOutro,
				@AddResiduoSolidoNaoInerte = Mensagem.DescricaoLicenciamentoAtividadeMsg.AddResiduoSolidoNaoInerte,

				@SelecioneCombustivel = Mensagem.DescricaoLicenciamentoAtividadeMsg.SelecioneCombustivel,
				@CombustivelExistente = Mensagem.DescricaoLicenciamentoAtividadeMsg.CombustivelExistente,
				@InformeSubstancia = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeSubstancia,
				@InformeEquipamento = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeEquipamento,
				@AddEmissoesAtm = Mensagem.DescricaoLicenciamentoAtividadeMsg.AddEmissoesAtm,

				@DscLicAtvSalvoSucesso = Mensagem.DescricaoLicenciamentoAtividadeMsg.DscLicAtvSalvoSucesso,
				@DscLicAtvExcluidoSucesso = Mensagem.DescricaoLicenciamentoAtividadeMsg.DscLicAtvExcluidoSucesso,

				@InformeZonaUC = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeZonaUC,
				@InformeVazaoValida = Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeVazaoValida
			});
		}
	}
}