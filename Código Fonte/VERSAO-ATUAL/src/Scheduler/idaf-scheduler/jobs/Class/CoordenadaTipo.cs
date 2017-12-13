using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class CoordenadaTipo : IListaValor
    {
        public int Id { get; set; }
        public bool IsAtivo { get; set; }
        [Display(Name = "Sistema de coordenada")]
        public string Texto { get; set; }

        public CoordenadaTipo() { }
    }
}
