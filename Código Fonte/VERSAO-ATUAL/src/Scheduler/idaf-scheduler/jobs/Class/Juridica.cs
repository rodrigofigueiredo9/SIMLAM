using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Juridica
    {
        public String CNPJ { get; set; }
        public String RazaoSocial { get; set; }
        [Display(Order = 11, Name = "Nome Fantasia")]
        public String NomeFantasia { get; set; }
        public String IE { get; set; }

        public int RepresentanteId { get; set; }
        public String RepresentanteTexto { get; set; }

        private List<Pessoa> _representantes = new List<Pessoa>();
        public List<Pessoa> Representantes
        {
            get { return _representantes; }
            set { _representantes = value; }

        }
    }
}
