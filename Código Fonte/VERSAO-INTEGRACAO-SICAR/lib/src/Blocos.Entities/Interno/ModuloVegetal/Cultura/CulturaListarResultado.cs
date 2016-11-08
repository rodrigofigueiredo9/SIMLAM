using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura
{
    public class CulturaListarResultado
    {
        public int Id { get; set; }
        public string Cultura { get; set; }
        public string Cultivar { get; set; }
		public string CultivarId { get; set; }

        public CulturaListarResultado() 
        {

        }

    }
}
