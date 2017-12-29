using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class ProdutoApreendidoLst : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
        public string Unidade { get; set; }
	}
}