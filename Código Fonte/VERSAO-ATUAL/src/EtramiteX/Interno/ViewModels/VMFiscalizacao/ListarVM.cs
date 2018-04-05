using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private FiscalizacaoListarFiltro _filtros = new FiscalizacaoListarFiltro();
		public FiscalizacaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _infracaoTipo = new List<SelectListItem>();
		public List<SelectListItem> InfracaoTipo
		{
			get { return _infracaoTipo; }
			set { _infracaoTipo = value; }
		}

		private List<SelectListItem> _itens = new List<SelectListItem>();
		public List<SelectListItem> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private List<SelectListItem> _situacaoTipo = new List<SelectListItem>();
		public List<SelectListItem> SituacaoTipo
		{
			get { return _situacaoTipo; }
			set { _situacaoTipo = value; }
		}

		private List<SelectListItem> _series = new List<SelectListItem>();
		public List<SelectListItem> Series
		{
			get { return _series; }
			set { _series = value; }
		}

		private List<SelectListItem> _areasFiscalizacao = new List<SelectListItem>();
		public List<SelectListItem> AreasFiscalizacao
		{
			get { return _areasFiscalizacao; }
			set { _areasFiscalizacao = value; }
		}

		private List<SelectListItem> _classificacoes = new List<SelectListItem>();
		public List<SelectListItem> Classificacoes
		{
			get { return _classificacoes; }
			set { _classificacoes = value; }
		}

		private List<Fiscalizacao> _resultados = new List<Fiscalizacao>();
		public List<Fiscalizacao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeParcelarMulta { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }
		public Boolean PodeVisualizarAcompanhamentos { get; set; }

		public ListarVM() { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Setor> setores, List<Lista> infracaoTipo, List<Lista> infracaoItem, List<Lista> situacao, List<Lista> series, List<Lista> classificacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			Setores = ViewModelHelper.CriarSelectList(setores, true, true);
			InfracaoTipo = ViewModelHelper.CriarSelectList(infracaoTipo, true, true);
			Itens = ViewModelHelper.CriarSelectList(infracaoItem, true, true);
			SituacaoTipo = ViewModelHelper.CriarSelectList(situacao, true, true);
			Series = ViewModelHelper.CriarSelectList(series, true, true);
			Classificacoes = ViewModelHelper.CriarSelectList(classificacao, true, true);
			AreasFiscalizacao = ViewModelHelper.CriarSelectList(new List<Lista>() {
				new Lista() { Texto = "DDSIA", Id = "3", IsAtivo = true },
				new Lista() { Texto = "DDSIV", Id = "1", IsAtivo = true },
				new Lista() { Texto = "DRNRE", Id = "2", IsAtivo = true }
			}, true, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}