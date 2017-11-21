using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class FuncionarioLst : IListaValor
    {
        public int Id { set; get; }
        public string Texto { set; get; }
        public bool IsAtivo { set; get; }
    }
}
