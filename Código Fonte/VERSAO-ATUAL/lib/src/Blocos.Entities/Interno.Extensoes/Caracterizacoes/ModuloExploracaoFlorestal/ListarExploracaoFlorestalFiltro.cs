using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ListarExploracaoFlorestalFiltro
	{
		public int EmpreendimentoId { get; set; }
		public String TipoAtividade { get; set; }
		public String CodigoExploracao { get; set; }
		public String DataExploracao { get; set; }

		public ListarExploracaoFlorestalFiltro() { }
	}
}