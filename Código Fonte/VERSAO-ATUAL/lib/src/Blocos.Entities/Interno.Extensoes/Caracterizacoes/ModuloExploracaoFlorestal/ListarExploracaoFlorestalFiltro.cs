using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ListarExploracaoFlorestalFiltro
	{
		public int EmpreendimentoId { get; set; }
		public String TipoExploracao { get; set; }
		public String CodigoExploracao { get; set; }
		public String DataExploracao { get; set; }
		public bool IsVisualizar { get; set; }

		public ListarExploracaoFlorestalFiltro() { }
	}
}