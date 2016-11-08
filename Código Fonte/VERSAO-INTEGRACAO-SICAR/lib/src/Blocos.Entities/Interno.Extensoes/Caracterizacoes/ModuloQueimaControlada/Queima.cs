

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada
{
	public class Cultivo
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CultivoTipo { get; set; }
		public String CultivoTipoTexto { get; set; }
		public String FinalidadeNome { get; set; }
		public String AreaQueima { get; set; }
		public String AreaQueimaTexto { get; set; }

		public String AreaQueimaHa
		{
			get
			{
				Decimal aux = 0;
				if (Decimal.TryParse(AreaQueima, out aux))
				{
					if (aux > 0)
					{
						return (aux / 10000M).ToString("N4");
					}
				}

				return String.Empty;
			}
		}
	}
}
