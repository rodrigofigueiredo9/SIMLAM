using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro
{
    public class PTVOutroListarResultado
	{
		public int ID { get; set; }
		public string Numero { get; set; }
		public string Interessado { get; set; }
		public string Destinatario { get; set; }
		public string CulturaCultivar { get; set; }
        public int SituacaoID { get; set; }
        public string SituacaoTexto { get; set; }
		public int ResponsavelTecnicoId { get; set; }		
	}
}