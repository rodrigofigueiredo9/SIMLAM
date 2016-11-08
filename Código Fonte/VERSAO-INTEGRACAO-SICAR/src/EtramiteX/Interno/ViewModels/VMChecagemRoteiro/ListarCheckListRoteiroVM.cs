using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro
{
	public class ListarCheckListRoteiroVM
	{
		public List<SelectListItem> ListaSituacao { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ChecagemRoteiroListarFiltro _filtros = new ChecagemRoteiroListarFiltro();
		public ChecagemRoteiroListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<ChecagemRoteiro> _resultados = new List<ChecagemRoteiro>();
		public List<ChecagemRoteiro> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarCheckListRoteiroVM()
		{
		}

		public ListarCheckListRoteiroVM(List<QuantPaginacao> quantPaginacao, List<Situacao> listaSituacao)
		{
			ListaSituacao = ViewModelHelper.CriarSelectList(listaSituacao, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}