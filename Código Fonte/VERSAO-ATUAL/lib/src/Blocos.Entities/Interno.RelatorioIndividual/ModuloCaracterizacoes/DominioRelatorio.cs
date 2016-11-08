using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes
{
	public class DominioRelatorio
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Decimal AreaCroqui { get; set; }
		public Decimal AreaDocumento { get; set; }
		public Int64? NumeroCCIR { get; set; }
		public Decimal AreaCCIR { get; set; }
		public Decimal APPCroqui { get; set; }
		public Decimal? ARLDocumento { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }

		private List<ReservaLegalRelatorio> _reservasLegais = new List<ReservaLegalRelatorio>();
		public List<ReservaLegalRelatorio> ReservasLegais
		{
			get { return _reservasLegais; }
			set { _reservasLegais = value; }
		}
	}
}