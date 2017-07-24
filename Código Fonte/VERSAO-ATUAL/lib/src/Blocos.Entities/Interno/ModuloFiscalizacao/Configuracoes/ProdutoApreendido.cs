using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ProdutoApreendido
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
        public String Item { get; set; }
        public String Unidade { get; set; }
        public bool Ativo { get; set; }
        public bool Excluir { get; set; }
	}
}
