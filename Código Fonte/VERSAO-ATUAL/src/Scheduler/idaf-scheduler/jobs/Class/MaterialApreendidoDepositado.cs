using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class MaterialApreendidoDepositario
    {
        public Int32? Id { get; set; }
        public Int32? Estado { get; set; }
        public Int32? Municipio { get; set; }
        public String NomeRazaoSocial { get; set; }
        public String CPFCNPJ { get; set; }
        public String Logradouro { get; set; }
        public String Bairro { get; set; }
        public String Distrito { get; set; }
    }
}
