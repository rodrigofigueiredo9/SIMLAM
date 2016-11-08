using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class ItemQuadroDeArea
    {
        public int Id { get; set; }
        public string Nome { get; set; }
		public string Descricao { get; set; }
        public string Area { get; set; }
        public bool IsSubArea { get; set; }
        public bool IsProcessada { get; set; }
    }
}