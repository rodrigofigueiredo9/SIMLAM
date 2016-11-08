

using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro
{
	public class ListarFiltroItem
	{
		public Int32? Id { get; set; }
		public String Nome { get; set; }
		public String TipoTexto { get; set; }
		public Int32? TipoId { get; set; }
		public List<int> TiposPermitidos { get; set; }
		public String Procedimento { get; set; }
		public String Tid { get; set; }

		public ListarFiltroItem() {
			TiposPermitidos = new List<int>();
		}
	}
}