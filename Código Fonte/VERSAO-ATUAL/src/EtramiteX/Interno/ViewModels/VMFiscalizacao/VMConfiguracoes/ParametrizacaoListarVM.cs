using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class ParametrizacaoListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ParametrizacaoListarFiltro _filtros = new ParametrizacaoListarFiltro();
		public ParametrizacaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _ParametrizacaoLst = new List<SelectListItem>();
		public List<SelectListItem> ParametrizacaoLst
		{
			get { return _ParametrizacaoLst; }
			set { _ParametrizacaoLst = value; }
		}

		private List<ParametrizacaoListarResultado> _resultados = new List<ParametrizacaoListarResultado>();
		public List<ParametrizacaoListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }

		public ParametrizacaoListarVM() { }

		public ParametrizacaoListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> parametrizacaoLst)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			ParametrizacaoLst = ViewModelHelper.CriarSelectList(parametrizacaoLst, true, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}