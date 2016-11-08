using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Tecnomapas.Blocos.Entities.Home
{
    public class Log
    {
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        public string Source { get; set; }
        public string Mensagem { get; set; }

        public List<SelectListItem> LstSource { get; set; }
        public List<Dictionary<string, string>> Resultados { get; set; }

        public Log()
        {
            LstSource = new List<SelectListItem>();
            Resultados = new List<Dictionary<string, string>>();
        }        
    }
}
