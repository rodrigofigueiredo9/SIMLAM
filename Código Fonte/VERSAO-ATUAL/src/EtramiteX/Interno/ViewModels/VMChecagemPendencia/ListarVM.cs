using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia
{
	public class ListarVM
	{
		private List<SelectListItem> _lstSituacoesDeChecagem = new List<SelectListItem>();
		public List<SelectListItem> LstSituacoesDeChecagem
		{
			get { return _lstSituacoesDeChecagem; }
			set { _lstSituacoesDeChecagem = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ListarFiltroChecagemPendencia _filtros = new ListarFiltroChecagemPendencia();
		public ListarFiltroChecagemPendencia Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<ChecagemPendencia> _resultados = new List<ChecagemPendencia>();
		public List<ChecagemPendencia> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeGerarPdf { get; set; }

		public ListarVM() { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> lstSituacoesDeChecagem) 
		{
			LstSituacoesDeChecagem = ViewModelHelper.CriarSelectList(lstSituacoesDeChecagem, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}