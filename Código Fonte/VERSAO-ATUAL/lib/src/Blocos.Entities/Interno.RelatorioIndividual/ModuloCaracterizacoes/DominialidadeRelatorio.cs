using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes
{
	public class DominialidadeRelatorio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public List<DominioRelatorio> Dominios { get; set; }

		private List<DominialidadeAreaRelatorio> _areas = new List<DominialidadeAreaRelatorio>();
		public List<DominialidadeAreaRelatorio> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}

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

		public String TotalCroquiHa { get { return TotalCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.AreaCroqui);
				}
				return Decimal.Zero;
			}
		}

		public String TotalVegetacaoNativaHa { get { return TotalVegetacaoNativa.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalVegetacaoNativa
		{
			get
			{
				return Areas.Where(x => 
					x.Tipo == (int)eDominialidadeAreaRelatorio.AVN_I || 
					x.Tipo == (int)eDominialidadeAreaRelatorio.AVN_M || 
					x.Tipo == (int)eDominialidadeAreaRelatorio.AVN_A || 
					x.Tipo == (int)eDominialidadeAreaRelatorio.AVN_D)
					.Sum(x=>x.Valor);
			}
		}

		public String TotalFlorestaPlantadaHa { get { return TotalFlorestaPlantada.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalFlorestaPlantada
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeAreaRelatorio.AA_USO_FLORES_PLANT).Sum(x => x.Valor);
			}
		}

		public String TotalFlorestaHa { get { return TotalFloresta.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalFloresta
		{
			get
			{
				return TotalVegetacaoNativa + TotalFlorestaPlantada;
			}
		}

		public String TotalAreaDocumentoHa { get { return TotalAreaDocumento.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAreaDocumento
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.AreaDocumento);
				}
				return Decimal.Zero;
			}
		}

		public String TotalAreaCCRIHa { get { return TotalAreaCCRI.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAreaCCRI
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.AreaCCIR);
				}
				return Decimal.Zero;
			}
		}

		public String TotalAPPCroquiHa { get { return TotalAPPCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAPPCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.APPCroqui);
				}
				return Decimal.Zero;
			}
		}

		public String TotalAPPPreservadaCroquiHa { get { return TotalAPPPreservadaCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAPPPreservadaCroqui
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AVN).Sum(x => x.Valor);
			}
		}

		public String TotalAPPRecuperacaoCroquiHa { get { return TotalAPPRecuperacaoCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAPPRecuperacaoCroqui
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_REC).Sum(x => x.Valor);
			}
		}

		public String TotalAPPUsoCroquiHa { get { return TotalAPPUsoCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAPPUsoCroqui
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_USO).Sum(x => x.Valor);
			}
		}

		public String TotalAPPNaoCaracterizadaHa { get { return TotalAPPNaoCaracterizada.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalAPPNaoCaracterizada
		{
			get
			{
				Decimal APP_APMP = 0;
				Decimal APP_AVN = 0;
				Decimal APP_AA_REC = 0;
				Decimal APP_AA_USO = 0;

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_APMP))
				{
					APP_APMP = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_APMP).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AVN))
				{
					APP_AVN = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AVN).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_REC))
				{
					APP_AA_REC = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_REC).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_USO))
				{
					APP_AA_USO = Areas.LastOrDefault(x => x.Tipo == (int)eDominialidadeAreaRelatorio.APP_AA_USO).Valor;
				}

				return APP_APMP - APP_AVN - APP_AA_REC - APP_AA_USO;
			}
		}

		public String TotalARLDocumentoHa { get { return TotalARLDocumento.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLDocumento
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ARLDocumento.GetValueOrDefault());
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLCroquiHa { get { return TotalARLCroqui.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLCroqui
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Sum(reserva => reserva.ARLCroqui));
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
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetalRelatorio.Preservada)
							.Sum(reserva => reserva.ARLCroqui));
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
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetalRelatorio.EmRecuperacao)
							.Sum(reserva => reserva.ARLCroqui));
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
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetalRelatorio.EmUso)
							.Sum(reserva => reserva.ARLCroqui));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLNaoCaracterizadaHa { get { return TotalARLNaoCaracterizada.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARLNaoCaracterizada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetalRelatorio.NaoCaracterizada)
							.Sum(reserva => reserva.ARLCroqui));
				}
				return Decimal.Zero;
			}
		}

		public String TotalARLHa { get { return TotalARL.Convert(eMetrica.M2ToHa).ToStringTrunc(4); } }
		public Decimal TotalARL
		{
			get
			{
				return TotalARLCroqui + TotalARLDocumento;
			}
		}

		public DominialidadeRelatorio()
		{
			Dominios = new List<DominioRelatorio>();
		}
	}
}