using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario
{
	public class DocumentoFitossanitario
	{
		public int ID { get; set; }
		public string TID { get; set; }
		public int TipoDocumentoID { get; set; }
		public string TipoDocumentoTexto { get; set; }
		public int Tipo { get; set; }
		public long NumeroInicial { get; set; }
		public long NumeroFinal { get; set; }
        public string Serie { get; set; }
	}
}