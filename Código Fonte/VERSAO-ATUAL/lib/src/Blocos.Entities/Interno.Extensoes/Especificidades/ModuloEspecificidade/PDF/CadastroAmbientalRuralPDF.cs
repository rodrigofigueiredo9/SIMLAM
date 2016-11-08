using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CadastroAmbientalRuralPDF
	{
		private List<Area> _areas = new List<Area>();
		public List<Area> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}

		#region TotalAreaCroqui

		public String TotalAreaCroqui
		{
			get
			{
				return TotalAreaCroquiDecimal.ToStringTrunc();
			}
		}
		public String TotalAreaCroquiHa
		{
			get
			{
				if (TotalAreaCroquiDecimal > 0)
				{
					return TotalAreaCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalAreaCroquiDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		#endregion

		#region AreaUsoAlternativo

		public String AreaUsoAlternativo
		{
			get
			{
				return AreaUsoAlternativoDecimal.ToStringTrunc();
			}
		}
		public String AreaUsoAlternativoHa
		{
			get
			{
				if (AreaUsoAlternativoDecimal > 0)
				{
					return AreaUsoAlternativoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal AreaUsoAlternativoDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.AREA_USO_ALTERNATIVO).Valor;
				}

				return 0;
			}
		}

		#endregion

		#region AreaDeclividade

		public String AreaDeclividade
		{
			get
			{
				return AreaDeclividadeDecimal.ToStringTrunc();
			}
		}
		public String AreaDeclividadeHa
		{
			get
			{
				if (AreaDeclividadeDecimal > 0)
				{
					return AreaDeclividadeDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal AreaDeclividadeDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.AREA_USO_RESTRITO_DECLIVIDADE).Valor;
				}

				return 0;
			}
		}

		#endregion

		#region TotalAPPCroqui

		public String TotalAPPCroqui
		{
			get
			{
				return TotalAPPCroquiDecimal.ToStringTrunc();
			}
		}
		public String TotalAPPCroquiHa
		{
			get
			{
				if (TotalAPPCroquiDecimal > 0)
				{
					return TotalAPPCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalAPPCroquiDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_TOTAL_CROQUI).Valor;
				}

				return 0;
			}
		}

		public String TotalAPPCroquiPorc { get; set; }

		#endregion

		#region TotalARLCroqui

		public String TotalARLCroqui
		{
			get
			{
				return TotalARLCroquiDecimal.ToStringTrunc();
			}
		}
		public String TotalARLCroquiHa
		{
			get
			{
				if (TotalARLCroquiDecimal > 0)
				{
					return TotalARLCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalARLCroquiDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CROQUI).Valor;
				}

				return 0;
			}
		}

		public String TotalARLCroquiPorcSemAsterisco { get; set; }

		public String TotalARLCroquiPorc
		{
			get
			{
				if (DispensaARL)
				{
					return TotalARLCroquiPorcSemAsterisco + " **";
				}

				return TotalARLCroquiPorcSemAsterisco;
			}
		}

		#endregion

		#region APPPreservada

		public String APPPreservada
		{
			get
			{
				return APPPreservadaDecimal.ToStringTrunc();
			}
		}
		public String APPPreservadaHa
		{
			get
			{
				if (APPPreservadaDecimal > 0)
				{
					return APPPreservadaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal APPPreservadaDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_PRESERVADA).Valor;
				}

				return 0;
			}
		}

		public String APPPreservadaPorc { get; set; }

		#endregion

		#region TotalARLPreservada

		public String TotalARLPreservada
		{
			get
			{
				return TotalARLPreservadaDecimal.ToStringTrunc();
			}
		}
		public String TotalARLPreservadaHa
		{
			get
			{
				if (TotalARLPreservadaDecimal > 0)
				{
					return TotalARLPreservadaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalARLPreservadaDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_PRESERVADA).Valor;
				}

				return 0;
			}
		}

		public String TotalARLPreservadaPorc { get; set; }

		#endregion

		#region APPRecuperacao

		public String APPRecuperacao
		{
			get
			{
				return APPRecuperacaoDecimal.ToStringTrunc();
			}
		}
		public String APPRecuperacaoHa
		{
			get
			{
				if (APPRecuperacaoDecimal > 0)
				{
					return APPRecuperacaoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal APPRecuperacaoDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_RECUPERACAO).Valor;
				}

				return 0;
			}
		}

		public String APPRecuperacaoPorc { get; set; }

		#endregion

		#region TotalARLRecuperacao

		public String TotalARLRecuperacao
		{
			get
			{
				return TotalARLRecuperacaoDecimal.ToStringTrunc();
			}
		}
		public String TotalARLRecuperacaoHa
		{
			get
			{
				if (TotalARLRecuperacaoDecimal > 0)
				{
					return TotalARLRecuperacaoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalARLRecuperacaoDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERACAO).Valor;
				}

				return 0;
			}
		}

		public String TotalARLRecuperacaoPorc { get; set; }

		#endregion

		#region APPEmUso

		public String APPEmUso
		{
			get
			{
				return APPEmUsoDecimal.ToStringTrunc();
			}
		}
		public String APPEmUsoHa
		{
			get
			{
				if (APPEmUsoDecimal > 0)
				{
					return APPEmUsoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal APPEmUsoDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_USO).Valor;
				}

				return 0;
			}
		}

		public String APPEmUsoPorc { get; set; }

		#endregion

		#region ARLEmUso

		public String ARLEmUso
		{
			get
			{
				return ARLEmUsoDecimal.ToStringTrunc();
			}
		}
		public String ARLEmUsoHa
		{
			get
			{
				if (ARLEmUsoDecimal > 0)
				{
					return ARLEmUsoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal ARLEmUsoDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					//return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.USO).Valor;
				}

				return 0;
			}
		}

		#endregion

		#region ARLEmpreendimento

		public string ARLEmpreendimento { get { return ARLEmpreendimentoDecimal.ToStringTrunc(); } }
		public string ARLEmpreendimentoHa { get { return (ARLEmpreendimentoDecimal > 0 ? ARLEmpreendimentoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4) : "0"); } }
		public Decimal ARLEmpreendimentoDecimal
		{
			get
			{
				decimal arlCroqui = 0;
				decimal totalARLReceptora = 0;
				decimal totalARLCedente = 0;

				if (Areas.Exists(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CROQUI))
				{
					arlCroqui = Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CROQUI).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA))
				{
					totalARLReceptora = Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE))
				{
					totalARLCedente = Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE).Valor;
				}

				arlCroqui = arlCroqui + totalARLReceptora - totalARLCedente;

				return arlCroqui;
			}
		}

		public string ARLEmpreendimentoiPorc { get; set; }

		#endregion

		#region APPRecuperarCalculada

		public String APPRecuperarCalculada
		{
			get
			{
				return APPRecuperarCalculadaDecimal.ToStringTrunc();
			}
		}
		public String APPRecuperarCalculadaHa
		{
			get
			{
				if (APPRecuperarCalculadaDecimal > 0)
				{
					return APPRecuperarCalculadaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal APPRecuperarCalculadaDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO).Valor;
				}

				return 0;
			}
		}

		public String APPRecuperarCalculadaPorc { get; set; }

		#endregion

		#region TotalARLRecuperar

		public String TotalARLRecuperar
		{
			get
			{
				return TotalARLRecuperarDecimal.ToStringTrunc();
			}
		}
		public String TotalARLRecuperarHa
		{
			get
			{
				if (TotalARLRecuperarDecimal > 0)
				{
					return TotalARLRecuperarDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalARLRecuperarDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERAR).Valor;
				}

				return 0;
			}
		}

		public String TotalARLRecuperarPorc { get; set; }

		#endregion

		#region APPRecuperarEfetiva

		public String APPRecuperarEfetiva
		{
			get
			{
				return APPRecuperarEfetivaDecimal.ToStringTrunc();
			}
		}
		public String APPRecuperarEfetivaHa
		{
			get
			{
				if (APPRecuperarEfetivaDecimal > 0)
				{
					return APPRecuperarEfetivaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal APPRecuperarEfetivaDecimal
		{
			get
			{
				if (Areas != null && Areas.Count > 0)
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA).Valor;
				}

				return 0;
			}
		}

		public String APPRecuperarEfetivaPorc { get; set; }

		#endregion

		#region TotalRLCompensada

		public decimal TotalRLCompensadaDecimal { get; set; }
		public string TotalRLCompensada { get { return TotalRLCompensadaDecimal.ToStringTrunc(); } }
		public string TotalRLCompensadaHa { get { return (TotalRLCompensadaDecimal > 0 ? TotalRLCompensadaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4) : "0"); } }

		public String TotalARLCompensadaPorc { get; set; }

		#endregion

		public bool ReservaLegalEmOutroCAR { get; set; }
		public int? EmpreendimentoCodigoNoCAR { get; set; }
		public bool ReservaLegalDeOutroCAR { get; set; }
		public int? EmpreendimentoCodigoDoCAR { get; set; }
		public string TipoCompensacao { get; set; }
		public int ProjetoGeoId { get; set; }
		public bool DispensaARL { get; set; }

		public CadastroAmbientalRuralPDF() { }

		public CadastroAmbientalRuralPDF(CadastroAmbientalRural car)
		{
			ProjetoGeoId = car.ProjetoGeoId;
			DispensaARL = car.DispensaARL;

			Areas = car.Areas;
			TotalAPPCroquiPorc = car.PercentTotalAPP.ToStringTrunc();
			TotalARLCroquiPorcSemAsterisco = car.PercentARLCroqui.ToStringTrunc();
			APPPreservadaPorc = car.PercentTotalAPPPreservada.ToStringTrunc();
			TotalARLPreservadaPorc = car.PercentARLPreservada.ToStringTrunc();
			APPRecuperacaoPorc = car.PercentTotalAPPRecuperacao.ToStringTrunc();
			TotalARLRecuperacaoPorc = car.PercentARLRecuperacao.ToStringTrunc();
			APPEmUsoPorc = car.PercentTotalAPPUso.ToStringTrunc();
			TotalARLRecuperarPorc = car.PercentARLRecuperar.ToStringTrunc();
			TotalARLCompensadaPorc = (car.PercentARLCedenteEmRecuperacao + car.PercentARLCedentePreservada + car.PercentARLReceptoraEmRecuperacao + car.PercentARLReceptoraPreservada).ToStringTrunc();
			APPRecuperarCalculadaPorc = car.PercentTotalAPPRecuperarCalculado.ToStringTrunc();
			APPRecuperarEfetivaPorc = car.PercentTotalAPPRecuperarEfetiva.ToStringTrunc();
			ARLEmpreendimentoiPorc = car.PercentARLEmpreendimento.ToStringTrunc();
			ReservaLegalDeOutroCAR = car.ReservaLegalDeOutroCAR.GetValueOrDefault();
			ReservaLegalEmOutroCAR = car.ReservaLegalEmOutroCAR.GetValueOrDefault();
			EmpreendimentoCodigoNoCAR = car.CodigoEmpreendimentoReceptor;
			EmpreendimentoCodigoDoCAR = car.CodigoEmpreendimentoCedente;
		}
	}
}