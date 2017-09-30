using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public interface IConfiguracao
    {
        object this[string idx] { get; }
        T Obter<T>(String key);
        object Atual(String key);
    }
}
