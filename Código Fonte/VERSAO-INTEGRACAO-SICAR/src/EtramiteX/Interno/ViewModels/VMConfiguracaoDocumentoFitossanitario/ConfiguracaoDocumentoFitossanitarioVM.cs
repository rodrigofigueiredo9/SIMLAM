using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario
{
	public class ConfiguracaoDocumentoFitossanitarioVM
	{
		public ConfiguracaoDocumentoFitossanitario Configuracao { get; set; }
		public List<SelectListItem> TiposDocumento { get; set; }

		public List<DocumentoFitossanitario> NumerosBloco
		{
			get { return Configuracao.DocumentoFitossanitarioIntervalos.Where(x => x.Tipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco).ToList(); }
		}

		public List<DocumentoFitossanitario> NumerosDigitais
		{
			get { return Configuracao.DocumentoFitossanitarioIntervalos.Where(x => x.Tipo == (int)eDocumentoFitossanitarioTipoNumero.Digital).ToList(); }
		}

		public ConfiguracaoDocumentoFitossanitarioVM(ConfiguracaoDocumentoFitossanitario configuracao, List<Lista> tipoDocumento)
		{
			Configuracao = configuracao;
			TiposDocumento = ViewModelHelper.CriarSelectList(tipoDocumento);
		}
	}
}