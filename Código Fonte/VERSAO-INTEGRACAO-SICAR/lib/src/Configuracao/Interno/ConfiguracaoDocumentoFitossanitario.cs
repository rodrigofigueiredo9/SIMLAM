using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoDocumentoFitossanitario : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyDocumentosFitossanitario = "DocumentosFitossanitario";
		public List<Lista> DocumentosFitossanitario { get { return _daLista.ObterDocumentosFitossanitario(); } }

		public const string KeyDocFitossanitarioTipoNumero = "DocFitossanitarioTipoNumero";
		public List<Lista> DocFitossanitarioTipoNumero { get { return _daLista.ObterDocFitossanitarioTipoNumero(); } }

		public const string KeyDocFitossanitarioSituacao = "DocFitossanitarioSituacao";
		public List<Lista> DocFitossanitarioSituacao { get { return _daLista.ObterDocFitossanitarioSituacao(); } }

		public const string KeyCFOProdutoEspecificacao = "CFOProdutoEspecificacao";
		public List<Lista> CFOProdutoEspecificacao { get { return _daLista.ObterCFOProdutoEspecificacao(); } }

		public const string KeyCFOCLoteEspecificacao = "CFOCLoteEspecificacao";
		public List<Lista> CFOCLoteEspecificacao { get { return _daLista.ObterCFOCLoteEspecificacao(); } }
	}
}