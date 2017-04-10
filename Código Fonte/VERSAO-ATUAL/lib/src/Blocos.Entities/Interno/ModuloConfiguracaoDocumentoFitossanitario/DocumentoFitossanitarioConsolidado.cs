using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario
{
	public class DocumentoFitossanitarioConsolidado
	{
        public string Texto { get; set; }
        public string QtdBlocoCFO { get; set; }
        public string QtdBlocoCFOC { get; set; }
        public string QtdBlocoPTV { get; set; }
        public string QtdDigitalCFO { get; set; }
        public string QtdDigitalCFOC { get; set; }
        public string QtdDigitalPTV { get; set; }
	}
}