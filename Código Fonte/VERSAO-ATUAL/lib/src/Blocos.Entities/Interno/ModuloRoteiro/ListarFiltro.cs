

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro
{
	public class ListarFiltro
	{
		public String Nome { get; set; }
		public String PalavaChave { get; set; }
		public Int32? Numero { get; set; }
		public Int32 Setor { get; set; }
		public Int32 Situacao { get; set; }
		public String Atividade { get; set; }

		public ListarFiltro()
		{
		}
	}
}