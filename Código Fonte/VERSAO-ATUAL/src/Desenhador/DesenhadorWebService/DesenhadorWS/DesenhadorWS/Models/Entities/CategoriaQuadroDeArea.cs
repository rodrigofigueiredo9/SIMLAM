using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class CategoriaQuadroDeArea
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<ItemQuadroDeArea> Itens { get; set; }
    }
}