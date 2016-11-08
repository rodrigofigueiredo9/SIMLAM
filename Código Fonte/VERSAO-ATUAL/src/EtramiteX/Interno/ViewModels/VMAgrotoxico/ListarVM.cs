using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }

		private List<SelectListItem> _situacao = new List<SelectListItem>();
		public List<SelectListItem> Situacao
		{
			get { return _situacao; }
			set { _situacao = value; }
		}

		private List<SelectListItem> _classeUso = new List<SelectListItem>();
		public List<SelectListItem> ClasseUso
		{
			get { return _classeUso; }
			set { _classeUso = value; }
		}

		private List<SelectListItem> _modalidadeAplicacao = new List<SelectListItem>();
		public List<SelectListItem> ModalidadeAplicacao
		{
			get { return _modalidadeAplicacao; }
			set { _modalidadeAplicacao = value; }
		}

		private List<SelectListItem> _grupoQuimico = new List<SelectListItem>();
		public List<SelectListItem> GrupoQuimico
		{
			get { return _grupoQuimico; }
			set { _grupoQuimico = value; }
		}

		private List<SelectListItem> _classificacaoToxicologica = new List<SelectListItem>();
		public List<SelectListItem> ClassificacaoToxicologica
		{
			get { return _classificacaoToxicologica; }
			set { _classificacaoToxicologica = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private AgrotoxicoFiltro _filtros = new AgrotoxicoFiltro();
		public AgrotoxicoFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<AgrotoxicoFiltro> _resultados = new List<AgrotoxicoFiltro>();
		public List<AgrotoxicoFiltro> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> classes, List<Lista> modalidades, List<Lista> grupos, List<Lista> classificacoes, List<Lista> situacao)
		{
			Paginacao = new Blocos.Entities.Etx.ModuloCore.Paginacao();
			Situacao = ViewModelHelper.CriarSelectList(situacao, true, false);
			ClasseUso = ViewModelHelper.CriarSelectList(classes, true, true);
			ModalidadeAplicacao = ViewModelHelper.CriarSelectList(modalidades, true, true);
			GrupoQuimico = ViewModelHelper.CriarSelectList(grupos, true, true);
			ClassificacaoToxicologica = ViewModelHelper.CriarSelectList(classificacoes, true, true);

			Filtros = new AgrotoxicoFiltro();
			UltimaBusca = string.Empty;
			Resultados = new List<AgrotoxicoFiltro>();

			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public ListarVM(){}
	}
}