using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }

		private List<FichaFundiaria> _resultados = new List<FichaFundiaria>();
		public List<FichaFundiaria> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
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

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}