using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Entities;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMRoteiro
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<Roteiro> _resultados = new List<Roteiro>();
		public List<Roteiro> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private ListarFiltro _filtros = new ListarFiltro();
		public ListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _listaAtividades = new List<SelectListItem>();
		public List<SelectListItem> ListaAtividades
		{
			get { return _listaAtividades; }
			set { _listaAtividades = value; }
		}

		public bool PodeAssociar { get; set; }
		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeDesativar { get; set; }
		public bool MostrarRelatorio { get; set; }

		public List<SelectListItem> Setores { get; set; }
		public String UltimaBusca { get; set; }

		public ListarVM() { }

		public ListarVM(List<Setor> setores, List<QuantPaginacao> quantPaginacao, List<ProcessoAtividadeItem> atividades)
		{
			Setores = ViewModelHelper.CriarSelectList(setores, true, true);
			ListaAtividades = ViewModelHelper.CriarSelectList(atividades, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}