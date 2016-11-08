

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class Produto
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		public Int32 ProdutoTipo { get; set; }
		public String ProdutoTipoTexto { get; set; }

		public Int32 DestinacaoTipo { get; set; }
		public String DestinacaoTipoTexto { get; set; }

		public String Quantidade { get; set; }
	}
}
