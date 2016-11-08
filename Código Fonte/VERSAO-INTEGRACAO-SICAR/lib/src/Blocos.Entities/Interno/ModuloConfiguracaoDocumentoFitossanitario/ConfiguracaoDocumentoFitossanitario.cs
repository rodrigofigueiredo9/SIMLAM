using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario
{
	public class ConfiguracaoDocumentoFitossanitario
	{
		public int ID { get; set; }
		public string Tid { get; set; }
		public List<DocumentoFitossanitario> DocumentoFitossanitarioIntervalos { get; set; }

		public ConfiguracaoDocumentoFitossanitario()
		{
			DocumentoFitossanitarioIntervalos = new List<DocumentoFitossanitario>();
		}
	}
}