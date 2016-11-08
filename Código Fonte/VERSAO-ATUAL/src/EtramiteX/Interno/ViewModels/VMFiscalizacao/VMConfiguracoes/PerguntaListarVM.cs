using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class PerguntaListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private PerguntaInfracaoListarFiltro _filtros = new PerguntaInfracaoListarFiltro();
		public PerguntaInfracaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _perguntaLst = new List<SelectListItem>();
		public List<SelectListItem> PerguntaLst
		{
			get { return _perguntaLst; }
			set { _perguntaLst = value; }
		}

		private List<SelectListItem> _respostaLst = new List<SelectListItem>();
		public List<SelectListItem> RespostaLst
		{
			get { return _respostaLst; }
			set { _respostaLst = value; }
		}

		private List<PerguntaInfracaoListarResultado> _resultados = new List<PerguntaInfracaoListarResultado>();
		public List<PerguntaInfracaoListarResultado> Resultados
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

		public PerguntaListarVM() { }

		public PerguntaListarVM(List<QuantPaginacao> quantPaginacao, List<Item> perguntasLst, List<Item> respostasLst)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			PerguntaLst = ViewModelHelper.CriarSelectList(perguntasLst, true, true);
			RespostaLst = ViewModelHelper.CriarSelectList(respostasLst, true, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}