

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class ListarEntregaFiltro
	{
		public String NumeroProtocolo { get; set; }		
		public String NumeroTitulo { get; set; }
		public int Modelo { get; set; }
		public String Empreendimento { get; set; }

		public ListarEntregaFiltro() { }
	}
}
