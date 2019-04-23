using System.ComponentModel;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloDUA
{
	public enum eSituacaoDua
	{
		[Description("Dua Emitido")]
		Emitido,
		[Description("Dua Pago")]
		Pago,
		[Description("Dua Vencido")]
		Vencido,
		[Description("Dua Cancelado")]
		Cancelado,
		[Description("Reemitido")]
		Reemitido,
		[Description("Indefinido")]
		Indefinido = 99
	}
}
