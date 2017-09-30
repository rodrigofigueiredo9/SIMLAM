using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Municipio : IListaValor
    {
        public int Id { get; set; }
        public String Texto { get; set; }
        public String Cep { get; set; }
        public int Ibge { get; set; }
        public bool IsAtivo { get; set; }

        private Estado _estado = new Estado();
        public Estado Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
    }
}
