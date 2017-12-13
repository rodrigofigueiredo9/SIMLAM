using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class GerenciadorConfiguracao
    {
        IConfiguracao _config;

        public GerenciadorConfiguracao(IConfiguracao confg)
        {
            _config = confg;
        }

        public TRet Obter<TRet>(String idx)
        {
            return (TRet)_config[idx];
        }

        public Object Obter(String idx)
        {
            return _config[idx];
        }
    }

    public class GerenciadorConfiguracao<T> : GerenciadorConfiguracao
    {
        public GerenciadorConfiguracao(IConfiguracao confg)
            : base(confg)
        {
        }

        public T Instancia { get; set; }
    }
}
