using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional
{
    public class DeclaracaoAdicional
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string TextoFormatado { get; set; }
        public int? OutroEstado { get; set; }
        public bool chkOutroEstado { get; set; }
    }
}
