using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario
{
	public class DocumentoFitossanitarioConsolidado
	{
        public string Texto { get; set; }
        public int QtdBlocoCFO { get; set; }
        public int QtdBlocoCFOC { get; set; }
        public int QtdBlocoPTV { get; set; }
        public int QtdDigitalCFO { get; set; }
        public int QtdDigitalCFOC { get; set; }
        public int QtdDigitalPTV { get; set; }
	}
}