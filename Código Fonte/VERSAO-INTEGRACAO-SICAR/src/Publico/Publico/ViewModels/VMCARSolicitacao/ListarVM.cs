using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMCARSolicitacao
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }

		private List<SelectListItem> _listaMunicipios = new List<SelectListItem>();
		public List<SelectListItem> ListaMunicipios
		{
			get { return _listaMunicipios; }
			set { _listaMunicipios = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private SolicitacaoListarFiltro _filtros = new SolicitacaoListarFiltro();
		public SolicitacaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SolicitacaoListarResultados> _resultados = new List<SolicitacaoListarResultados>();
		public List<SolicitacaoListarResultados> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public ListarVM() : this(new List<QuantPaginacao>(), new List<Municipio>()) { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Municipio> listaMunicipios)
		{
			ListaMunicipios = ViewModelHelper.CriarSelectList(listaMunicipios, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}