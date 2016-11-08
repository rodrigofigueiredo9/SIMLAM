
namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura
{
	public class CultivarConfiguracao
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int Cultivar { get; set; }
		public int PragaId { get; set; }
		public string PragaTexto { get; set; }
		public int TipoProducaoId { get; set; }
		public string TipoProducaoTexto { get; set; }
		public int DeclaracaoAdicionalId { get; set; }
		public string DeclaracaoAdicionalTexto { get; set; }
		
		public string DeclaracaoAdicionalTextoHtml { get; set; }
	}
}
