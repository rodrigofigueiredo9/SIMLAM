using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEscritura
{
	public class EscrituraPublicaCompraVenda : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public String Livro { get; set; }
		public String Folhas { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
	}
}