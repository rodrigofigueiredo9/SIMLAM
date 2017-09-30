using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public interface IListaValor
    {
        int Id { get; }
        string Texto { get; }
        bool IsAtivo { get; }
    }
}
