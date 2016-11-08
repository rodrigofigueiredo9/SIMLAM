using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class DescricaoLicenciamentoAtividade
	{
		public int Id { get; set; }
		public int EmpreendimentoId { get; set; }
		public int Tipo { get; set; }
		public eCaracterizacao GetTipo { get { return (eCaracterizacao)this.Tipo; } }
		public int RespAtividade { get; set; }
		public string BaciaHidrografica { get; set; }
		public bool ExisteAppUtil { get; set; }
		public int TipoVegetacaoUtilCodigo { get; set; }
		public string TipoVegetacaoUtilTexto { get; set; }
		public bool? ZonaAmortUC { get; set; }
		public string ZonaAmortUCNomeOrgaoAdm { get; set; }
		public bool LocalizadaUC { get; set; }
		public string LocalizadaUCNomeOrgaoAdm { get; set; }
		public bool? PatrimonioHistorico { get; set; }
		public bool? ResidentesEntorno { get; set; }
		public decimal? ResidentesEnternoDistancia { get; set; }
		public decimal? AreaTerreno { get; set; }
		public decimal? AreaUtil { get; set; }
		public int? TotalFuncionarios { get; set; }
		public string HorasDias { get; set; }
		public int? DiasMes { get; set; }
		public int? TurnosDia { get; set; }
		public decimal? ConsumoAguaLs { get; set; }
		public decimal? ConsumoAguaMh { get; set; }
		public decimal? ConsumoAguaMdia { get; set; }
		public decimal? ConsumoAguaMmes { get; set; }
		public int? TipoOutorgaId { get; set; }
		public string TipoOutorgaTexto { get; set; }
		public string Numero { get; set; }
		public int? FonteAbastecimentoAguaTipoId { get; set; }
		public int? PontoLancamentoTipoId { get; set; }		
		public string Tid { get; set; }

		public List<FonteAbastecimentoAgua> FontesAbastecimentoAgua { get; set; }
		public List<PontoLancamentoEfluente> PontosLancamentoEfluente { get; set; }
		public List<EfluenteLiquido> EfluentesLiquido { get; set; }
		public List<ResiduoSolidoNaoInerte> ResiduosSolidosNaoInerte { get; set; }
		public List<EmissaoAtmosferica> EmissoesAtmosfericas { get; set; }
		public List<Dependencia> Dependencias { get; set; }

		public DescricaoLicenciamentoAtividade()
		{
			this.Tipo = (int)eCaracterizacao.Nulo;
			this.FontesAbastecimentoAgua = new List<FonteAbastecimentoAgua>();
			this.PontosLancamentoEfluente = new List<PontoLancamentoEfluente>();
			this.EfluentesLiquido = new List<EfluenteLiquido>();
			this.ResiduosSolidosNaoInerte = new List<ResiduoSolidoNaoInerte>();
			this.EmissoesAtmosfericas = new List<EmissaoAtmosferica>();
			this.Dependencias = new List<Dependencia>();

			this.BaciaHidrografica = 
			this.TipoVegetacaoUtilTexto =
			this.ZonaAmortUCNomeOrgaoAdm =
			this.LocalizadaUCNomeOrgaoAdm =
			this.HorasDias =
			this.TipoOutorgaTexto = 
			this.Tid = "";
		}
	}
}

