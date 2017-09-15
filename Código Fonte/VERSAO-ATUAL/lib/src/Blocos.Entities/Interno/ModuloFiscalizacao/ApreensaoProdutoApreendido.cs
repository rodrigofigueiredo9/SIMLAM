using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ApreensaoProdutoApreendido
	{
		public Int32 Id { get; set; }
        public Int32 ProdutoId { get; set; }
        public String ProdutoTexto { get; set; }
        public String UnidadeTexto { get; set; }
        public Decimal Quantidade { get; set; }
        public Int32 DestinoId { get; set; }
        public String DestinoTexto { get; set; }
	}
}
