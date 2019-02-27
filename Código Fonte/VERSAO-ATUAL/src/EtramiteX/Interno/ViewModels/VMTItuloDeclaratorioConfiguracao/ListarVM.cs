using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private RelatorioTituloDecListarFiltro _filtros = new RelatorioTituloDecListarFiltro();
		public RelatorioTituloDecListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _situacaoTipo = new List<SelectListItem>();
		public List<SelectListItem> SituacaoTipo
		{
			get { return _situacaoTipo; }
			set { _situacaoTipo = value; }
		}

		private List<RelatorioTituloDecListarResultado> _resultados = new List<RelatorioTituloDecListarResultado>();
		public List<RelatorioTituloDecListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }

		public ListarVM() { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> situacoes)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			SituacaoTipo = ViewModelHelper.CriarSelectList(situacoes, true, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}