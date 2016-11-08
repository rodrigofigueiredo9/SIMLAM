using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
   public class PTVHistorico
    {
        public string NumeroEPTV { get; set; }
        public string DataEmissao { get; set; }
        public int SituacaoAtual { get; set; }

        public List<PTVItemHistorico> ListaHistoricos = new List<PTVItemHistorico>(); 
    }
}
