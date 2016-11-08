using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade
{
	public class ListarConfiguracaoVM
	{
		public List<SelectListItem> Modelos { get; set; }
		public List<SelectListItem> Setores { get; set; }
		public List<SelectListItem> Agrupadores { get; set; }

		public String UltimaBusca { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeVisualizar { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ListarConfigurarcaoFiltro _filtros = new ListarConfigurarcaoFiltro();
		public ListarConfigurarcaoFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<AtividadeConfiguracao> _resultados = new List<AtividadeConfiguracao>();
		public List<AtividadeConfiguracao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String AtividadesSolicitadas { get; set; }

		public ListarConfiguracaoVM() { }

		public ListarConfiguracaoVM(List<QuantPaginacao> quantPaginacao, List<TituloModeloLst> modelos, List<ProcessoAtividadeItem> atividades, List<Setor> setores, List<AtividadeAgrupador> agrupadores)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			Modelos = ViewModelHelper.CriarSelectList(modelos, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Agrupadores = ViewModelHelper.CriarSelectList(agrupadores, true);

			AtividadesSolicitadas = ViewModelHelper.Json(atividades.Select(x => x.Texto).ToList());
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}