using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class DestinacaoProduto
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
        public String Destino { get; set; }
        public bool Ativo { get; set; }
        public bool Excluir { get; set; }
	}
}
