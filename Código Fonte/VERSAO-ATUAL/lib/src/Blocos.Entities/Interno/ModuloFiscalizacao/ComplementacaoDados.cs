

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ComplementacaoDados
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 FiscalizacaoId { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 AutuadoId { get; set; }
		public Int32 AutuadoTipo { get; set; }
		
		public Int32 ResidePropriedadeTipo { get; set; }
		public String ResidePropriedadeTipoTexto { get; set; }
		
		public Int32 RendaMensalFamiliarTipo { get; set; }
		public String RendaMensalFamiliarTipoTexto { get; set; }
		
		public Int32 NivelEscolaridadeTipo { get; set; }
		public String NivelEscolaridadeTipoTexto { get; set; }

		public Int32 VinculoComPropriedadeTipo { get; set; }
		public String VinculoComPropriedadeTipoTexto { get; set; }
		public String VinculoComPropriedadeEspecificarTexto { get; set; }

		public Int32 ConhecimentoLegislacaoTipo { get; set; }
		public String ConhecimentoLegislacaoTipoTexto { get; set; }
		public String Justificativa { get; set; }

		public String AreaTotalInformada { get; set; }
		public String AreaCoberturaFlorestalNativa { get; set; }
		public Int32? ReservalegalTipo { get; set; }
	}
}
