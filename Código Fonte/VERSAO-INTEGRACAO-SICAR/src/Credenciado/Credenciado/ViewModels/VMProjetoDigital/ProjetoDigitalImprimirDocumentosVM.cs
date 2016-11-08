using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalImprimirDocumentosVM
	{
		private DocumentosGeradosVM _documentosGeradosVM = new DocumentosGeradosVM();
		public DocumentosGeradosVM DocumentosGeradosVM
		{
			get { return _documentosGeradosVM; }
			set { _documentosGeradosVM = value; }
		}

		private ProjetoDigital _projetoDigital = new ProjetoDigital();
		public ProjetoDigital ProjetoDigital
		{
			get { return _projetoDigital; }
			set { _projetoDigital = value; }
		}

		public ProjetoDigitalImprimirDocumentosVM(){}
	}
}