using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ListarEmpreendimentoFiltro _filtros = new ListarEmpreendimentoFiltro();
		public ListarEmpreendimentoFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _selListSegmentos = new List<SelectListItem>();
		public List<SelectListItem> SelListSegmentos
		{
			get { return _selListSegmentos; }
			set { _selListSegmentos = value; }
		}

		private List<SelectListItem> _selListAtividades = new List<SelectListItem>();
		public List<SelectListItem> SelListAtividades
		{
			get { return _selListAtividades; }
			set { _selListAtividades = value; }
		}

		private List<Empreendimento> _resultados = new List<Empreendimento>();
		public List<Empreendimento> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeCaracterizar { get; set; }

		public ListarVM() { }

		public ListarVM(List<EmpreendimentoAtividade> atividades, List<Segmento> segmentos, List<QuantPaginacao> quantPaginacao)
		{
			SelListAtividades = ViewModelHelper.CriarSelectList(atividades);
			SelListSegmentos = ViewModelHelper.CriarSelectList(segmentos);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}