

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloSetor
{
	public class ListarFiltro
	{
		public Int32 Agrupador { get; set; }
		public Int32 Setor { get; set; }
		public Int32? Municipio { get; set; }

		public ListarFiltro()
		{
		}
	}
}
