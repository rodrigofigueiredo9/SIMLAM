using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class CoordenadaDatum : IListaValor
    {
        public int Id { get; set; }
        public bool IsAtivo { get; set; }
        public string Sigla { get; set; }
        [Display(Name = "Datum")]
        public string Texto { get; set; }

        public CoordenadaDatum() { }
    }
}
