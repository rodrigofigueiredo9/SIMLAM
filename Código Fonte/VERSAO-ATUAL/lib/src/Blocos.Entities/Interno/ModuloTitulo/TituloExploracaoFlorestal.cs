using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloExploracaoFlorestal
	{
		public TituloExploracaoFlorestal() { }

		public int? Id { get; set; }
		public int? TituloId { get; set; }
		public int ExploracaoFlorestalId { get; set; }
		public string ExploracaoFlorestalTexto { get; set; }

		private List<TituloExploracaoFlorestalExploracao> list = new List<TituloExploracaoFlorestalExploracao>();
		public List<TituloExploracaoFlorestalExploracao> TituloExploracaoFlorestalExploracaoList { get { return list; } set { list = value; } }
	}
}