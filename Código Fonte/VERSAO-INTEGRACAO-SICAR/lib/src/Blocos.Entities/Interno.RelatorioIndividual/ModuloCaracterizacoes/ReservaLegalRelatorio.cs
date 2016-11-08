

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes
{
	public class ReservaLegalRelatorio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Decimal ARLCroqui { get; set; }
		public Int32? SituacaoVegetalId { get; set; }
		public String SituacaoVegetalTexto { get; set; }
	}
}