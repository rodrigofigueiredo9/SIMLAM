

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ExploracaoFlorestalProduto
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 ProdutoId { get; set; }
		public String ProdutoTexto { get; set; }
		public String Quantidade { get; set; }
		public Int32? EspeciePopularId { get; set; }
		public String EspeciePopularTexto { get; set; }
		public Int32? DestinacaoMaterialId { get; set; }
		public String DestinacaoMaterialTexto { get; set; }
	}
}