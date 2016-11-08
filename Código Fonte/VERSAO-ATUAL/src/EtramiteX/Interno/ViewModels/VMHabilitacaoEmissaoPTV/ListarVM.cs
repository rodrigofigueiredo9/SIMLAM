using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private HabilitacaoEmissaoPTVFiltro _filtros = new HabilitacaoEmissaoPTVFiltro();
		public HabilitacaoEmissaoPTVFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<HabilitacaoEmissaoPTVFiltro> _resultados = new List<HabilitacaoEmissaoPTVFiltro>();
		public List<HabilitacaoEmissaoPTVFiltro> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Setor> setores)
		{
			Paginacao = new Blocos.Entities.Etx.ModuloCore.Paginacao();
			Setores = ViewModelHelper.CriarSelectList(setores);
			Filtros = new HabilitacaoEmissaoPTVFiltro();
			UltimaBusca = string.Empty;
			Resultados = new List<HabilitacaoEmissaoPTVFiltro>();

			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public ListarVM(){}
	}
}