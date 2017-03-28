using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario
{
	public class DocumentoFitossanitarioListarFiltros
	{
        public bool EhConsolidado { get; set; }
        public bool EhIntervalo { get; set; }
		public string TipoDocumentoID { get; set; }
		public string TipoNumeracaoID { get; set; }
        public string Ano { get; set; }
        public string AnoConsolidado { get; set; }
	}
}