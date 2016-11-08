using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo
{
	public class ListarVM
	{
		public string UltimaBusca { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<Titulo> _resultados = new List<Titulo>();
		public List<Titulo> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private TituloFiltro _filtros = new TituloFiltro();
		public TituloFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public List<SelectListItem> Modelos = new List<SelectListItem>();
		public List<SelectListItem> Situacoes = new List<SelectListItem>();
		public List<SelectListItem> Setores = new List<SelectListItem>();
		public List<SelectListItem> AtividadesSolicitada = new List<SelectListItem>();
		public List<SelectListItem> Origens = new List<SelectListItem>();

		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeVisualizar { get; set; }
		public bool PodeAssociar { get; set; }
		public bool PodeAlterarSituacao { get; set; }
		public bool PodeAlterarSituacaoCondicionante { get; set; }

		public ListarVM() { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<TituloModeloLst> modelos, List<Situacao> situacoes, List<Setor> setores, List<Lista> origens = null)
		{
			Modelos = ViewModelHelper.CriarSelectList(modelos, true);
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);

			if (origens != null)
			{
				Origens = ViewModelHelper.CriarSelectList(origens, true);
			}
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public String ObterJSon(Titulo titulo)
		{
			object objeto = new
			{
				@Id = titulo.Id,
				@Tid = titulo.Tid,
				@Numero = titulo.Numero.Texto,
				@Modelo = titulo.Modelo.Nome,
				@ModeloTipo = titulo.Modelo.Tipo,
				@ModeloSigla = titulo.Modelo.Sigla
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}
	}
}