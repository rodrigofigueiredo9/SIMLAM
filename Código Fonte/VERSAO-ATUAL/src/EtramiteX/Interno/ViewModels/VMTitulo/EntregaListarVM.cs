using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class EntregaListarVM
	{
		public string UltimaBusca { get; set; }
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao { get { return _paginacao; } set { _paginacao = value; } }

		private List<Entrega> _resultados = new List<Entrega>();
		public List<Entrega> Resultados { get { return _resultados; } set { _resultados = value; } }

		private ListarEntregaFiltro _filtros = new ListarEntregaFiltro();
		public ListarEntregaFiltro Filtros { get { return _filtros; } set { _filtros = value; } }

		private List<SelectListItem> _modelos = new List<SelectListItem>();
		public List<SelectListItem> Modelos
		{
			get { return _modelos; }
			set { _modelos = value; }
		}

		public bool PodeAtivar { get; set; }
		public bool PodeDesativar{ get; set; }

		public EntregaListarVM() 
		{
			Paginacao = new Paginacao();
			Filtros = new ListarEntregaFiltro();
			Resultados = new List<Entrega>();
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, List<TituloModeloLst> modelos)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, Paginacao.QuantPaginacao.ToString());
			Modelos = ViewModelHelper.CriarSelectList(modelos, true);
		}
	}
}