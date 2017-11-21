using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Dependencia
    {
        public Int32 Id { get; set; }
        public String Tid { get; set; }
        public Int32 DependenciaTipo { get; set; }
        public Int32 DependenciaCaracterizacao { get; set; }
        public Int32 DependenciaId { get; set; }
        public String DependenciaTid { get; set; }
        public String DependenciaCaracterizacaoTexto { get; set; }
    }
}
