using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural
{
	public class CadastroAmbientalRural
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 ProjetoGeoId { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 OcorreuAlteracaoApos2008 { get; set; }
		public int VistoriaAprovacaoCAR { get; set; }
		public DateTecno DataVistoriaAprovacao { get; set; }
		public Decimal? ATPDocumento2008 { get; set; }
		public Situacao SituacaoProcessamento { get; set; }
		public Situacao Situacao { get; set; }
		public Int32 MunicipioId { get; set; }
		public String MunicipioTexto { get; set; }
		public Int32 ModuloFiscalId { get; set; }
		public Decimal ModuloFiscalHA { get; set; }
		public Decimal ATPQuantidadeModuloFiscal{get; set;}
		public Decimal APPRecuperarCalculada { get; set; }
		public List<Dependencia> Dependencias { get; set; }
		public List<Area> Areas { get; set; }
		public ArquivoProjeto Arquivo { get; set; }
		public Decimal PercentMaximaRecuperacaoAPP { get; set; }
		public bool DispensaARL { get; set; }

		private List<ArquivoProjeto> arquivosProjeto = new List<ArquivoProjeto>();
		public List<ArquivoProjeto> ArquivosProjeto
		{
			get { return arquivosProjeto; }
			set { arquivosProjeto = value; }
		}

		public ReservaLegal ReservaLegal { get; set; }
		public Area ObterArea(eCadastroAmbientalRuralArea tipo)
		{
			return Areas.SingleOrDefault(x => x.Tipo == (int)tipo) ?? new Area();
		}

		public decimal PercentTotalAPP
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_TOTAL_CROQUI);

				if (area!= null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPPreservada
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_PRESERVADA);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPRecuperacao
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERACAO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPUso
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_USO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPRecuperarCalculado
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPRecuperarEfetiva
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentTotalAPPRecuperarUsoConsolidado
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.APP_USO_CONSOLIDADO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentARLCroqui
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_CROQUI);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentARLDocumento
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_DOCUMENTO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}

		public decimal PercentARLAPP
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_APP);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLPreservada
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_PRESERVADA);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLRecuperacao
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_RECUPERACAO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLRecuperar
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_RECUPERAR);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLEmpreendimento 
		{ 
			get 
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_EMPREENDIMENTO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0; 
			} 
		}
		public decimal PercentTotalARLCompensadaCedente
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE);

				if (area != null && area.Valor > 0)
				{
					return ((area.Valor / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor) * 100);
				}

				return 0;
			}
		}
		public decimal PercentTotalARLCompensadaReceptor
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA);

				if (area != null && area.Valor > 0)
				{
					return ((area.Valor / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor) * 100);
				}

				return 0;
			}
		}
		public decimal PercentARLCedentePreservada
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA);

				if (area != null && area.Valor > 0)
				{
					return ((area.Valor / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor) * 100);
				}

				return 0;
			}
		}
		public decimal PercentARLCedenteEmRecuperacao
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLCedenteRecuperar
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLReceptoraPreservada
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLReceptoraEmRecuperacao
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public decimal PercentARLReceptoraRecuperar
		{
			get
			{
				Area area = ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR);

				if (area != null && area.Valor > 0)
				{
					return (area.Valor * 100) / ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI).Valor;
				}

				return 0;
			}
		}
		public CadastroAmbientalRural()
		{
			DataVistoriaAprovacao = new DateTecno();
			DispensaARL = false;
			Situacao = new Situacao();
			SituacaoProcessamento = new Situacao();
			Arquivo = new ArquivoProjeto();
			Dependencias = new List<Dependencia>();
			Areas = new List<Area>();
		}

		public bool? ReservaLegalEmOutroCAR { get; set; }
		public int? CodigoEmpreendimentoReceptor { get; set; }
		public bool? ReservaLegalDeOutroCAR { get; set; }
		public int? CodigoEmpreendimentoCedente { get; set; }
		public int? EmpreendimentoCedenteId { get; set; }
		public int? EmpreendimentoReceptorId { get; set; }

		public bool IsCedente { get { return ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE).Valor > 0; } }
		public bool IsReceptor { get { return ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA).Valor > 0; } }

	}
}