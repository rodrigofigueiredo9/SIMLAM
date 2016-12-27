using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro
{
    public class PTVOutroDeclaracao
    {
        public int  IdPraga { get; set;}
        public string NomeCientifico { get; set; }
        public string NomeComum { get; set; }
        public string DeclaracaoAdicional { get; set; }
        public int IdDeclaracao { get; set; }
        public int IdCultivar { get; set; }

    }
}