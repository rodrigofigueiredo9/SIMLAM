using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Situacao : IListaValor
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public String Nome { get; set; }
        public String Texto { get { return Nome; } }
        public bool IsAtivo { get; set; }
    }
}
