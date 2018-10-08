
namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloExploracaoFlorestalExploracao
	{
		public TituloExploracaoFlorestalExploracao() { }

		public int? Id { get; set; }
		public int TituloExploracaoFlorestalId { get; set; }
		public int ExploracaoFlorestalExploracaoId { get; set; }
		public string ExploracaoFlorestalExploracaoTexto { get; set; }
		public int? AutorizacaoSinaflorId { get; set; }
	}
}