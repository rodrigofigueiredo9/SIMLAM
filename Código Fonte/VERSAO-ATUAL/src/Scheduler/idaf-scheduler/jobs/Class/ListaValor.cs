using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class ListaValor : IListaValor
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public bool IsAtivo { get; set; }
    }
}
