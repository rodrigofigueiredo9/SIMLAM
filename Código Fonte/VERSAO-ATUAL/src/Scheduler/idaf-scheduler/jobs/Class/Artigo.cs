using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Artigo
    {
        public Int32 Id { get; set; }
        public String Tid { get; set; }
        public String Identificador { get; set; }
        public Int32 EnquadramentoId { get; set; }

        public String ArtigoTexto { get; set; }
        public String ArtigoParagrafo { get; set; }
        public String CombinadoArtigo { get; set; }
        public String CombinadoArtigoParagrafo { get; set; }
        public String DaDo { get; set; }
    }
}
