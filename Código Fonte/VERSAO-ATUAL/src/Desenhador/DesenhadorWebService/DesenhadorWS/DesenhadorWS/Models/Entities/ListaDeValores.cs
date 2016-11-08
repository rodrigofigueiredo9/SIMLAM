using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class ListaDeValores
    {
        public string TabelaReferenciada { get; set; }
        public string ColunaReferenciada { get; set; }
        public List<Item> Itens;
    }
}