using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade
{
	public class Dominialidade
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 InternoID { get; set; }
		public String InternoTID { get; set; }
		public Int32 CredenciadoID { get; set; }
		public String CredenciadoTID { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public int? EmpreendimentoInternoId { get; set; }
		public Int32 EmpreendimentoLocalizacao { get; set; }
		public Int32 ProjetoDigitalId { get; set; }
		public String ConfrontacaoNorte { get; set; }
		public String ConfrontacaoSul { get; set; }
		public String ConfrontacaoLeste { get; set; }
		public String ConfrontacaoOeste { get; set; }
		public List<Dependencia> Dependencias { get; set; }
		public List<Dominio> Dominios { get; set; }
		public Int32? PossuiAreaExcedenteMatricula { get; set; }
		public bool AlteradoCopiar { get; set; }

		private List<DominialidadeArea> _areas = new List<DominialidadeArea>();
		public List<DominialidadeArea> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}

		public Decimal ATPCroqui { get; set; }

		public Decimal AreaVegetacaoNativa
		{
			get
			{
				Decimal retorno = 0;
				List<DominialidadeArea> _areasAux = Areas.Where(x => x.Tipo == (int)eDominialidadeArea.AVN_I || x.Tipo == (int)eDominialidadeArea.AVN_M || x.Tipo == (int)eDominialidadeArea.AVN_A || x.Tipo == (int)eDominialidadeArea.AVN_D).ToList();
				foreach (DominialidadeArea area in _areasAux)
				{
					retorno += area.Valor;
				}

				return retorno;
			}
		}
		public string AreaVegetacaoNativaString { get { return AreaVegetacaoNativa.ToStringTrunc(); } }

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

		public Decimal APPPreservada
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeArea.APP_AVN).Sum(x => x.Valor);
			}
		}

		public Decimal APPRecuperacao
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeArea.APP_AA_REC).Sum(x => x.Valor);
			}
		}

		public Decimal APPUso
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeArea.APP_AA_USO).Sum(x => x.Valor);
			}
		}

		public Decimal APPARL
		{
			get
			{
				return Areas.Where(x => x.Tipo == (int)eDominialidadeArea.APP_ARL).Sum(x => x.Valor);
			}
		}

		public Decimal AreaFlorestaPlantada
		{

			get
			{
				if (Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.AA_USO_FLORES_PLANT))
				{
					return Areas.Where(x => x.Tipo == (int)eDominialidadeArea.AA_USO_FLORES_PLANT).Sum(x => Convert.ToDecimal(x.Valor));
				}
				return 0;
			}
		}
		public string AreaFlorestaPlantadaString { get { return AreaFlorestaPlantada.ToStringTrunc(); } }

		public Decimal TotalFloresta
		{
			get
			{
				return AreaVegetacaoNativa + AreaFlorestaPlantada;
			}
		}
		public string TotalFlorestaString { get { return TotalFloresta.ToStringTrunc(); } }

		public Decimal AreaCroqui
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
		public string AreaCroquiString { get { return AreaCroqui.ToStringTrunc(); } }

		public Decimal AreaDocumento
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
		public string AreaDocumentoString { get { return AreaDocumento.ToStringTrunc(); } }

		public Decimal AreaCCRI
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
		public string AreaCCRIString { get { return AreaCCRI.ToStringTrunc(); } }

		public Decimal APPCroqui
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
		public string APPCroquiString { get { return APPCroqui.ToStringTrunc(); } }

		public Decimal ARLCroqui
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
		public string ARLCroquiString { get { return ARLCroqui.ToStringTrunc(); } }

		public Decimal ARLDocumento
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
		public string ARLDocumentoString { get { return ARLDocumento.ToStringTrunc(); } }

		public Decimal ARLCedente
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.CompensacaoTipo == eCompensacaoTipo.Cedente)
							.Sum(reserva => reserva.ARLCroqui));
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
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCroqui)
						+ 
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora).Sum(reserva => reserva.ARLCedida)
						+ 
						x.ReservasLegais.Where(reserva =>
							reserva.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora &&
							(reserva.SituacaoId == (int)eReservaLegalSituacao.Proposta && Convert.ToBoolean(reserva.CedentePossuiEmpreendimento))).Sum(reserva => reserva.ARLCroqui)
					);
				}

				return Decimal.Zero;
			}
		}

		public Decimal ARLPreservadaCompensada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => 
						x.ReservasLegais.Where(reserva => 
							reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.Preservada && reserva.Compensada).Sum(reserva => reserva.ARLCroqui)
						+
						x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.Preservada).Sum(reserva => reserva.ARLCedida)
					);
				}

				return Decimal.Zero;
			}
		}

		public Decimal ARLRecuperacaoCompensada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					decimal valor = Dominios.Sum(x =>
						x.ReservasLegais.Where(reserva =>
							reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmRecuperacao && reserva.Compensada).Sum(reserva => reserva.ARLCroqui)
						+
						x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmRecuperacao).Sum(reserva => reserva.ARLCedida)
					);

					return valor;
				}

				return Decimal.Zero;
			}
		}

		public Decimal ARLUsoCompensada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmUso && reserva.Compensada)
							.Sum(reserva => reserva.ARLCroqui));
				}

				return Decimal.Zero;
			}
		}

		public Decimal ARLNaoCaracterizadaCompensada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais
						.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.NaoCaracterizada && reserva.Compensada)
						.Sum(reserva => reserva.ARLCroqui));
				}

				return Decimal.Zero;
			}
		}

		public Decimal ARLNaoCaracterizada
		{
			get
			{
				if (Dominios != null && Dominios.Count > 0)
				{
					return Dominios.Sum(x => x.ReservasLegais
						.Where(reserva => reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.NaoCaracterizada)
						.Sum(reserva => reserva.ARLCroqui));
				}

				return Decimal.Zero;
			}
		}

		public Decimal TotalARL
		{
			get
			{
				return ARLCroqui + ARLDocumento;
			}
		}
		public string TotalARLString { get { return TotalARL.ToStringTrunc(); } }

		public Boolean PossuiPorcentagemMinimaARL
		{
			get 
			{
				return (ARLCroqui + ARLReceptor - ARLCedente) < (ATPCroqui * (decimal)0.2);
			}
		}

		public Dominialidade()
		{
			Dependencias = new List<Dependencia>();
			Dominios = new List<Dominio>();
		}
	}
}