
using System.ComponentModel;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eBarragemTipo
	{
		[Description("Terra")]
		Terra =  1,
		[Description("Concreto")]
		Concreto = 2,
		[Description("Mista")]
		Mista = 3
	}
}