using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario
{
	public class ConfiguracaoNumeracaoListarVM
	{	
        public List<SelectListItem> TiposDocumento { get; set; }

        public List<SelectListItem> TiposNumeracao { get; set; }

        public List<DocumentoFitossanitario> Intervalos { get; set; }

        private Paginacao _paginacao = new Paginacao();
        public Paginacao Paginacao
        {
            get { return _paginacao; }
            set { _paginacao = value; }
        }

        private DocumentoFitossanitarioListarFiltros _filtros = new DocumentoFitossanitarioListarFiltros();
        public DocumentoFitossanitarioListarFiltros Filtros
        {
            get { return _filtros; }
            set { _filtros = value; }
        }

        private List<DocumentoFitossanitario> _resultados = new List<DocumentoFitossanitario>();
        public List<DocumentoFitossanitario> Resultados
        {
            get { return _resultados; }
            set { _resultados = value; }
        }

        public String UltimaBusca { get; set; }

        public ConfiguracaoNumeracaoListarVM() { }

        public ConfiguracaoNumeracaoListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public bool IsAssociar { get; set; }
       
		public ConfiguracaoNumeracaoListarVM(List<Lista> tipoDocumento, List<Lista> tipoNumeracao, List<DocumentoFitossanitario> intervalos = null)
		{
			TiposDocumento = ViewModelHelper.CriarSelectList(tipoDocumento);
            TiposNumeracao = ViewModelHelper.CriarSelectList(tipoNumeracao);
            Intervalos = intervalos ?? new List<DocumentoFitossanitario>();
		}
	}
}