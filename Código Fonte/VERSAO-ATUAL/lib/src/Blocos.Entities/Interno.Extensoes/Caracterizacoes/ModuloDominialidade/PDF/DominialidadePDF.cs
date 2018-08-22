using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF
{
	public class DominialidadePDF
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		public String ConfrontacaoNorte { get; set; }
		public String ConfrontacaoSul { get; set; }
		public String ConfrontacaoLeste { get; set; }
		public String ConfrontacaoOeste { get; set; }
		public Decimal AreaDocumento { get; set; }

		public String NumeroCCIRStragg
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					List<String> numeros = Dominios.Select(x => x.NumeroCCIR.ToString()).Distinct().ToList();
					return String.Join("; ", numeros);
				}

				return String.Empty;
			}
		}

		private List<DominialidadeArea> _areas = new List<DominialidadeArea>();
		public List<DominialidadeArea> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}

		private List<DominioPDF> _dominios = new List<DominioPDF>();
		public List<DominioPDF> Dominios
		{
			get { return _dominios; }
			set { _dominios = value; }
		}

		public List<DominioPDF> DominiosCCIRs
		{
			get { return Dominios; }
		}

		public List<DominioPDF> DominiosReservas
		{
			get { return Dominios; }
		}

		public String VegetacaoNativaInicial { get { return VegetacaoNativaInicialDecimal.ToStringTrunc(); } }
		public String VegetacaoNativaInicialHa
		{
			get
			{
				if (VegetacaoNativaInicialDecimal > 0)
				{
					return VegetacaoNativaInicialDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}
				return "0";
			}
		}
		public Decimal VegetacaoNativaInicialDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.AVN_I))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.AVN_I).Valor;
				}
				return 0;
			}
		}

		public String VegetacaoNativaMedio { get { return VegetacaoNativaMedioDecimal.ToStringTrunc(); } }
		public String VegetacaoNativaMedioHa
		{
			get
			{
				if (VegetacaoNativaMedioDecimal > 0)
				{
					return VegetacaoNativaMedioDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}
				return "0";
			}
		}
		public Decimal VegetacaoNativaMedioDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.AVN_M))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.AVN_M).Valor;
				}
				return 0;
			}
		}

		public String VegetacaoNativaAvancado { get { return VegetacaoNativaAvancadoDecimal.ToStringTrunc(); } }
		public String VegetacaoNativaAvancadoHa
		{
			get
			{
				if (VegetacaoNativaAvancadoDecimal > 0)
				{
					return VegetacaoNativaAvancadoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal VegetacaoNativaAvancadoDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.AVN_A))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.AVN_A).Valor;
				}
				return 0;
			}
		}

		public String VegetacaoNativaNaoCaracterizado { get { return VegetacaoNativaNaoCaracterizadoDecimal.ToStringTrunc(); } }
		public String VegetacaoNativaNaoCaracterizadoHa
		{
			get
			{
				if (VegetacaoNativaNaoCaracterizadoDecimal > 0)
				{
					return VegetacaoNativaNaoCaracterizadoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal VegetacaoNativaNaoCaracterizadoDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.AVN_D))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.AVN_D).Valor;
				}
				return 0;
			}
		}

		public String VegetacaoNativaTotal { get { return VegetacaoNativaTotalDecimal.ToStringTrunc(); } }
		public String VegetacaoNativaTotalHa
		{
			get
			{
				if (VegetacaoNativaTotalDecimal > 0)
				{
					return VegetacaoNativaTotalDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal VegetacaoNativaTotalDecimal { get; set; }

		public String TotalFloresta { get { return TotalFlorestaDecimal.ToStringTrunc(); } }
		public String TotalFlorestaHa
		{
			get
			{
				if (TotalFlorestaDecimal > 0)
				{
					return TotalFlorestaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalFlorestaDecimal { get; set; }

		public String TotalFlorestaPlantada { get { return TotalFlorestaPlantadaDecimal.ToStringTrunc(); } }
		public String TotalFlorestaPlantadaHa
		{
			get
			{
				if (TotalFlorestaPlantadaDecimal > 0)
				{
					return TotalFlorestaPlantadaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal TotalFlorestaPlantadaDecimal { get; set; }

		public String AppVegetacaoNativa { get { return AppVegetacaoNativaDecimal.ToStringTrunc(); } }
		public String AppVegetacaoNativaHa
		{
			get
			{
				if (AppVegetacaoNativaDecimal > 0)
				{
					return AppVegetacaoNativaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal AppVegetacaoNativaDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AVN))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AVN).Valor;
				}
				return 0;
			}
		}

		public String AppSemVegetacaoNativa { get { return AppSemVegetacaoNativaDecimal.ToStringTrunc(); } }
		public String AppSemVegetacaoNativaHa
		{
			get
			{
				if (AppSemVegetacaoNativaDecimal > 0)
				{
					return AppSemVegetacaoNativaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal AppSemVegetacaoNativaDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AA_USO))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AA_USO).Valor;
				}
				return 0;
			}
		}

		public String AppEmRecuperacao { get { return AppEmRecuperacaoDecimal.ToStringTrunc(); } }
		public String AppEmRecuperacaoHa
		{
			get
			{
				if (AppEmRecuperacaoDecimal > 0)
				{
					return AppEmRecuperacaoDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal AppEmRecuperacaoDecimal
		{
			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AA_REC))
				{
					return Areas.SingleOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AA_REC).Valor;
				}
				return 0;
			}
		}

		public String TotalAreaCroqui
		{
			get
			{
				return TotalAreaCroquiDecimal.ToStringTrunc();
			}
		}

		public String ATPCroquiHa { get; set; }

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
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.AreaCroquiDecimal);
				}

				return Decimal.Zero;
			}
		}

		public String TotalAreaDocumento { get { return TotalAreaDocumentoDecimal.ToStringTrunc(); } }
		public Decimal TotalAreaDocumentoDecimal
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.AreaDocumentoDecimal);
				}
				return Decimal.Zero;
			}
		}

		public Decimal TotalAreaCCRI
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.AreaCCIRDecimal);
				}
				return Decimal.Zero;
			}
		}

		public String TotalAPPCroqui { get { return TotalAPPCroquiDecimal.ToStringTrunc(); } }
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
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.APPCroquiDecimal);
				}
				return Decimal.Zero;
			}
		}

		public string ARLCedenteHa { get { return ARLCedente.Convert(eMetrica.M2ToHa).ToStringTrunc(); } }

		public Decimal ARLCedente
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.Compensada)
							.Sum(reserva => Convert.ToDecimal(reserva.ARLCroqui)));
				}

				return Decimal.Zero;
			}
		}
		public Decimal ARLReceptor
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x =>
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => Convert.ToDecimal(reserva.ARLCroqui))
						+
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada && !Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCedida)
						+
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Proposta && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => Convert.ToDecimal(reserva.ARLCroqui))
					);
				}

				return Decimal.Zero;
			}
		}
		public String TotalARLCroqui { get { return TotalARLCroquiDecimal.ToStringTrunc(); } }
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
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.ReservasLegais.Sum(reserva => reserva.ARLCroquiDecimal));
				}
				return Decimal.Zero;
			}
		}

		public Decimal ARLReceptorDecimal
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x =>
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCroquiDecimal)
						+
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada && !Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCedida)
						+
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Proposta && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCroquiDecimal)
					);
				}

				return Decimal.Zero;
			}
		}

		public String TotalARLPreservadaCroquiHa { get { return TotalARLPreservadaCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLPreservadaCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.Preservada)
							.Sum(reserva => reserva.ARLCroquiDecimal));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLRecuperacaoCroquiHa { get { return TotalARLRecuperacaoCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLRecuperacaoCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmRecuperacao)
							.Sum(reserva => reserva.ARLCroquiDecimal));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLUsoCroquiHa { get { return TotalARLUsoCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLUsoCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmUso)
							.Sum(reserva => reserva.ARLCroquiDecimal));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLDocumento { get { return TotalARLDocumentoDecimal.ToStringTrunc(); } }
		public Decimal TotalARLDocumentoDecimal
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.ReservasLegais.Sum(reserva => reserva.ARLDocumentoDecimal));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLDocumentoDominio { get { return TotalARLDocumentoDominioDecimal.ToStringTrunc(); } }
		public Decimal TotalARLDocumentoDominioDecimal
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => (!string.IsNullOrEmpty(x.ARLDocumento) ? Convert.ToDecimal(x.ARLDocumento) : 0));
				}
				return Decimal.Zero;
			}
		}

		public Decimal TotalARLPCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(dominio => dominio.ReservasLegais.Sum(reserva => reserva.ARLDocumentoDecimal));
				}
				return Decimal.Zero;
			}
		}

		public Decimal TotalARLFCroqui
		{
			get
			{
				//throw new NotImplementedException();
				return 0;
			}
		}

		public String ARLPorcentagem
		{
			get { return ((TotalARLDocumentoDecimal / AreaDocumento) * 100).ToStringTrunc(); }
		}

		public string PorcentagemAreaCompensacao
		{
			get
			{
				return ((TotalARLCroquiDecimal + ARLReceptorDecimal - ARLCedente) * 100).ToStringTrunc();
			}
		}

		public string PorcentagemARLSobreCroqui
		{
			get
			{
				return ((TotalARLCroquiDecimal / TotalAreaCroquiDecimal) * 100).ToStringTrunc();
			}
		}

		public List<DominioPDF> Matricula
		{
			get { return this.Dominios.Where(x => x.Tipo == eDominioTipo.Matricula).ToList(); }
		}

		public List<DominioPDF> Posse
		{
			get { return this.Dominios.Where(x => x.Tipo == eDominioTipo.Posse).ToList(); }
		}

		public Decimal AreaAPPNaoCaracterizada
		{
			get
			{
				Decimal APP_APMP = 0;
				Decimal APP_AVN = 0;
				Decimal APP_AA_REC = 0;
				Decimal APP_AA_USO = 0;

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_APMP))
				{
					APP_APMP = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_APMP).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AVN))
				{
					APP_AVN = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AVN).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AA_REC))
				{
					APP_AA_REC = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AA_REC).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.APP_AA_USO))
				{
					APP_AA_USO = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeArea.APP_AA_USO).Valor;
				}

				return (APP_APMP - APP_AVN - APP_AA_REC - APP_AA_USO);
			}
		}
		public string AreaAPPNaoCaracterizadaString { get { return AreaAPPNaoCaracterizada.ToStringTrunc(); } }
		public String ATPReceptoraHa { get { return ATPReceptoraDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(); } }
		public Decimal ATPReceptoraDecimal { get; set; }

		public bool IsEmpreendimentoCedente
		{
			get
			{
				return Dominios.Exists(d => d.ReservasLegais.Exists(r => r.CompensacaoTipo.Equals(eCompensacaoTipo.Cedente)));
			}
		}

		public bool IsEmpreendimentoReceptor
		{
			get
			{
				return Dominios.Exists(d => d.ReservasLegais.Exists(r => r.CompensacaoTipo.Equals(eCompensacaoTipo.Receptora)));
			}
		}
		 
		public DominialidadePDF() { }

		public DominialidadePDF(Dominialidade dominialidade)
		{
			Id = dominialidade.Id;
			ConfrontacaoNorte = dominialidade.ConfrontacaoNorte;
			ConfrontacaoLeste = dominialidade.ConfrontacaoLeste;
			ConfrontacaoOeste = dominialidade.ConfrontacaoOeste;
			ConfrontacaoSul = dominialidade.ConfrontacaoSul;
			Areas = dominialidade.Areas;
			VegetacaoNativaTotalDecimal = dominialidade.AreaVegetacaoNativa;
			TotalFlorestaDecimal = dominialidade.TotalFloresta;
			TotalFlorestaPlantadaDecimal = dominialidade.AreaFlorestaPlantada;
			Dominios = dominialidade.Dominios.Select(x => new DominioPDF(x)).ToList();
			AreaDocumento = dominialidade.AreaDocumento;
			ATPCroquiHa = dominialidade.ATPCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
		}
	}
}