using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class CadastroAmbientalRuralVM
	{
		public Boolean IsVisualizar { get; set; }
		public Boolean IsFinalizado { get { return Caracterizacao.Situacao.Id == (int)eCadastroAmbientalRuralSituacao.Finalizado; } }

		public Boolean IsBloquearCampos
		{
			get
			{
				if (!MostrarProcessar && !MostrarReprocessar)
				{
					return true;
				}

				return false;
			}
		}

		public Boolean MostrarFinalizar
		{
			get
			{
				return !IsFinalizado && Caracterizacao.SituacaoProcessamento.Id == (int)eProjetoGeograficoSituacaoProcessamento.ProcessadoPDF;
			}
		}

		public Boolean MostrarReprocessar
		{
			get
			{
				if (!IsFinalizado && (new List<eProjetoGeograficoSituacaoProcessamento>(){
					eProjetoGeograficoSituacaoProcessamento.ErroProcessamento, 
					eProjetoGeograficoSituacaoProcessamento.Cancelado,
					eProjetoGeograficoSituacaoProcessamento.ErroGerarPDF})
					.Exists(x => (int)x == Caracterizacao.SituacaoProcessamento.Id))
				{
					return true;
				}

				return false;
			}
		}

		public Boolean MostrarProcessar
		{
			get
			{
				if (!IsFinalizado && (Caracterizacao.SituacaoProcessamento.Id == 0))
				{
					return true;
				}

				return false;
			}
		}

		public Boolean MostrarBtnOu { get { return MostrarReprocessar; } }

		public String IdsTelaProjetoGeograficoSituacao
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AguardandoProcessamento = (int)eProjetoGeograficoSituacaoProcessamento.AguardandoProcessamento,
					@Processando = (int)eProjetoGeograficoSituacaoProcessamento.Processando,
					@ErroProcessamento = (int)eProjetoGeograficoSituacaoProcessamento.ErroProcessamento,
					@Processado = (int)eProjetoGeograficoSituacaoProcessamento.Processado,
					@Cancelado = (int)eProjetoGeograficoSituacaoProcessamento.Cancelado,
					@AguardandoGeracaoPDF = (int)eProjetoGeograficoSituacaoProcessamento.AguardandoGeracaoPDF,
					@GerandoPDF = (int)eProjetoGeograficoSituacaoProcessamento.GerandoPDF,
					@ErroGerarPDF = (int)eProjetoGeograficoSituacaoProcessamento.ErroGerarPDF,
					@ProcessadoPDF = (int)eProjetoGeograficoSituacaoProcessamento.ProcessadoPDF,
					@CanceladaPDF = (int)eProjetoGeograficoSituacaoProcessamento.CanceladaPDF
				});
			}
		}

		public String IdsTelaArea
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ATP_CROQUI = (int)eCadastroAmbientalRuralArea.ATP_CROQUI,
					@AREA_DOCUMENTO = (int)eCadastroAmbientalRuralArea.AREA_DOCUMENTO,
					@AREA_CCRI = (int)eCadastroAmbientalRuralArea.AREA_CCRI,
					@AREA_USO_ALTERNATIVO = (int)eCadastroAmbientalRuralArea.AREA_USO_ALTERNATIVO,
					@AREA_USO_RESTRITO_DECLIVIDADE = (int)eCadastroAmbientalRuralArea.AREA_USO_RESTRITO_DECLIVIDADE,
					@APP_TOTAL_CROQUI = (int)eCadastroAmbientalRuralArea.APP_TOTAL_CROQUI,
					@APP_PRESERVADA = (int)eCadastroAmbientalRuralArea.APP_PRESERVADA,
					@APP_RECUPERACAO = (int)eCadastroAmbientalRuralArea.APP_PRESERVADA,
					@APP_USO = (int)eCadastroAmbientalRuralArea.APP_USO,
					@APP_RECUPERAR_CALCULADO = (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO,
					@APP_RECUPERAR_EFETIVA = (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA,
					@APP_USO_CONSOLIDADO = (int)eCadastroAmbientalRuralArea.APP_USO_CONSOLIDADO,
					@APP_NAO_CARACTERIZADA = (int)eCadastroAmbientalRuralArea.APP_NAO_CARACTERIZADA,
					@ARL_CROQUI = (int)eCadastroAmbientalRuralArea.ARL_CROQUI,
					@ARL_DOCUMENTO = (int)eCadastroAmbientalRuralArea.ARL_DOCUMENTO,
					@ARL_APP = (int)eCadastroAmbientalRuralArea.ARL_APP,
					@ARL_PRESERVADA = (int)eCadastroAmbientalRuralArea.ARL_PRESERVADA,
					@ARL_RECUPERACAO = (int)eCadastroAmbientalRuralArea.ARL_RECUPERACAO,
					@ARL_RECUPERAR = (int)eCadastroAmbientalRuralArea.ARL_RECUPERAR,
					@ARL_NAO_CARACTERIZADA = (int)eCadastroAmbientalRuralArea.ARL_NAO_CARACTERIZADA,
					@AA_USO = (int)eCadastroAmbientalRuralArea.AA_USO
				});
			}
		}

		public Boolean PossuiAPPNaoCaracterizada
		{
			get
			{
				if (Caracterizacao != null)
				{
					return Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_NAO_CARACTERIZADA).Valor.MaiorToleranciaM2();
				}

				return false;
			}
		}

		public Boolean PossuiARLNaoCaracterizada
		{
			get
			{
				if (Caracterizacao != null)
				{
					return Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_NAO_CARACTERIZADA).Valor.MaiorToleranciaM2();
				}

				return false;
			}
		}

		public string PercentMaximaRecuperacaoAPP
		{
			get
			{
				if (Caracterizacao.PercentMaximaRecuperacaoAPP == 0.1M)
				{
					return "10";
				}

				if (Caracterizacao.PercentMaximaRecuperacaoAPP == 0.2M)
				{
					return "20";
				}

				return "Não se aplica";
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ConfirmReprocessarProjeto = Mensagem.CadastroAmbientalRural.ConfirmReprocessarProjeto,
					@ConfirmFinalizarProjeto = Mensagem.CadastroAmbientalRural.ConfirmFinalizarProjeto
				});
			}
		}

		public String UrlsArquivo { get; set; }

		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CadastroAmbientalRural Caracterizacao { get; set; }
		public List<SelectListItem> ListaMunicipios { get; set; }
		public List<SelectListItem> ListaBoolean { get; set; }
		public MBR Abrangencia { get; set; }

		private List<ArquivoProcessamentoVM> _arquivosProcessamentoVM = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosProcessamentoVM
		{
			get { return _arquivosProcessamentoVM; }
			set { _arquivosProcessamentoVM = value; }
		}

		public CadastroAmbientalRuralVM(CadastroAmbientalRural caracterizacao, List<Municipio> municipiosLista, List<Lista> booleanLista, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;

			ListaMunicipios = ViewModelHelper.CriarSelectList(municipiosLista, true, selecionado: caracterizacao.MunicipioId.ToString());
			ListaBoolean = ViewModelHelper.CriarSelectList(booleanLista, true, false, selecionado: caracterizacao.OcorreuAlteracaoApos2008.ToString());
		}
	}
}