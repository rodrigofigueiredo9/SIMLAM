using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class ContatoLst : IListaValor
    {
        public int Id { get; set; }
        public bool IsAtivo { get; set; }
        public string Mascara { get; set; }
        public string Texto { get; set; }
        public string Tid { get; set; }
    }
}
