using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }

		private List<SetorLocalizacao> _resultados = new List<SetorLocalizacao>();
		public List<SetorLocalizacao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private List<SelectListItem> _agrupadores = new List<SelectListItem>();
		public List<SelectListItem> Agrupadores
		{
			get { return _agrupadores; }
			set { _agrupadores = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

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

		private ListarFiltro _filtros = new ListarFiltro();
		public ListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public ListarVM(){}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<SetorAgrupador> agrupador, List<Setor> setor, List<Municipio> listaMunicipios)
		{
			ListaMunicipios = ViewModelHelper.CriarSelectList(listaMunicipios, true, true);
			Agrupadores = ViewModelHelper.CriarSelectList(agrupador, true, true);
			Setores = ViewModelHelper.CriarSelectList(setor, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}