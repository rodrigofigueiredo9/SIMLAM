using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Enquadramento
    {
        public Int32 Id { get; set; }
        public String Tid { get; set; }
        public Int32 FiscalizacaoId { get; set; }

        private List<Artigo> _artigos = new List<Artigo>();
        public List<Artigo> Artigos
        {
            get { return _artigos; }
            set { _artigos = value; }
        }
    }
}
