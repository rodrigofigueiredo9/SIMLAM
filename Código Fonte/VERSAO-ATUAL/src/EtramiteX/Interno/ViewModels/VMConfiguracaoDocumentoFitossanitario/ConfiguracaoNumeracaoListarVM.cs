using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
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

        private ListarProtocoloFiltro _filtros = new ListarProtocoloFiltro();
        public ListarProtocoloFiltro Filtros
        {
            get { return _filtros; }
            set { _filtros = value; }
        }

        private List<Protocolo> _resultados = new List<Protocolo>();
        public List<Protocolo> Resultados
        {
            get { return _resultados; }
            set { _resultados = value; }
        }

        public String UltimaBusca { get; set; }
       
		public ConfiguracaoNumeracaoListarVM(List<Lista> tipoDocumento, List<Lista> tipoNumeracao, List<DocumentoFitossanitario> intervalos = null)
		{
			TiposDocumento = ViewModelHelper.CriarSelectList(tipoDocumento);
            TiposNumeracao = ViewModelHelper.CriarSelectList(tipoNumeracao);
            Intervalos = intervalos ?? new List<DocumentoFitossanitario>();
		}
	}
}