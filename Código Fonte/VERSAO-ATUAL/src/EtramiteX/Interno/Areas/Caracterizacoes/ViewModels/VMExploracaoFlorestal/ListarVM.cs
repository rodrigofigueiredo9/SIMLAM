using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<SelectListItem> _tipoExploracaoList = new List<SelectListItem>();
		public List<SelectListItem> TipoExploracaoList
		{
			get { return _tipoExploracaoList; }
			set { _tipoExploracaoList = value; }
		}

		private ListarExploracaoFlorestalFiltro _filtros = new ListarExploracaoFlorestalFiltro();
		public ListarExploracaoFlorestalFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<ExploracaoFlorestal> _resultados = new List<ExploracaoFlorestal>();
		public List<ExploracaoFlorestal> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }

		public ListarVM() { }

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<Lista> tipoExploracao,List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			TipoExploracaoList = ViewModelHelper.CriarSelectList(tipoExploracao, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}