using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
    public class PTVComunicador
    {

        public int Id { get; set; }
        public string Tid { get; set; }

        public int PTVId { get; set; }

        public Int64 PTVNumero { get; set; }

        public bool liberadoCredenciado { get; set; }

        public Arquivo.Arquivo ArquivoInterno { get; set; }

        public Arquivo.Arquivo ArquivoCredenciado { get; set; }

        public int ArquivoInternoId { get; set; }

        public int ArquivoCredenciadoId { get; set; }

        public List<PTVConversa> Conversas { get; set; }

		public bool IsDesbloqueio { get; set; } = false;

        public PTVComunicador()
        {
            ArquivoInterno = new Arquivo.Arquivo();
            ArquivoCredenciado = new Arquivo.Arquivo();
            Conversas = new List<PTVConversa>();

        }

    }
}
