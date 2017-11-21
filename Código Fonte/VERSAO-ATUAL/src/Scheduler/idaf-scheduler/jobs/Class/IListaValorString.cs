using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public interface IListaValorString
    {
        string Id { get; set; }
        bool IsAtivo { get; set; }
        string Texto { get; set; }
    }
}
