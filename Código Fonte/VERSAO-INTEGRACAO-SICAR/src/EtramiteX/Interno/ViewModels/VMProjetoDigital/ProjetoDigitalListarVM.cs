using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeImportar { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private RequerimentoListarFiltro _filtros = new RequerimentoListarFiltro();
		public RequerimentoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<Requerimento> _resultados = new List<Requerimento>();
		public List<Requerimento> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public ProjetoDigitalListarVM() { }

		public ProjetoDigitalListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}