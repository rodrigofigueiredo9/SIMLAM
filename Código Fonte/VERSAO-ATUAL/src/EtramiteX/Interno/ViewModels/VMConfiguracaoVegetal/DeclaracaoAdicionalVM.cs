using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
    public class DeclaracaoAdicionalVM
    {
        private List<DeclaracaoAdicional> _itens = new List<DeclaracaoAdicional>();

        private DeclaracaoAdicional _declaracao = new DeclaracaoAdicional();

        public DeclaracaoAdicional DeclaracaoAdicional {
            get { return _declaracao; }
            set { _declaracao = value; } 
        }

        public List<DeclaracaoAdicional> Itens
        {
            get { return _itens; }
            set { _itens = value; }
        }

    
    }
}