using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class SilviculturaPPFFPDF
	{
		public string AreaTotal { get; set; }
		public string MunicipiosAbrangencia { get; set; }

		public SilviculturaPPFFPDF(SilviculturaPPFF silvicultura)
		{
			this.AreaTotal = silvicultura.AreaTotal;
			this.MunicipiosAbrangencia = string.Join(", ", silvicultura.Itens.Select((s, i) => s.Municipio.Texto));
		}
	}
}