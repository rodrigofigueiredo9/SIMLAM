using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteDescricaoListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<SelectListItem> _selListItensPorPagina = new List<SelectListItem>();	
		public List<SelectListItem> SelListItensPorPagina
		{
		  get { return _selListItensPorPagina; }
		  set { _selListItensPorPagina = value; }
		}
		
		public string UltimaBusca { get; set; }

		private List<TituloCondicionante> _resultados = new List<TituloCondicionante>();
		public List<TituloCondicionante> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}
		
		// Filtros
		private TituloCondicionanteFiltro _filtros = new TituloCondicionanteFiltro();
		public TituloCondicionanteFiltro Filtros { get { return _filtros; } set { _filtros = value; } }


		public CondicionanteDescricaoListarVM() { }

		public CondicionanteDescricaoListarVM(List<QuantPaginacao> quantPaginacao) 
		{
 			SelListItensPorPagina = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		internal void SetResultados(List<TituloCondicionante> resultados)
		{
			Resultados = resultados;
		}
	}
}