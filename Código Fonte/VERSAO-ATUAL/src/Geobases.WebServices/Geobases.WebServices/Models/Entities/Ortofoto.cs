using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.Geobases.WebServices.Models.Entities
{
    public class Ortofoto
    {
        public string ArquivoNome { get; set; }
		public string ArquivoChave { get; set; }
		public DateTime ArquivoChaveData { get; set; }
    }
}