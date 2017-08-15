using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class CodigoReceita
	{
        public Int32 Id { get; set; }
        public String Tid { get; set; }
        public String Codigo { get; set; }
        public String Descricao { get; set; }
        public bool Ativo { get; set; } 
        public bool Excluir { get; set; }
        public bool? Editado { get; set; }
	}
}
