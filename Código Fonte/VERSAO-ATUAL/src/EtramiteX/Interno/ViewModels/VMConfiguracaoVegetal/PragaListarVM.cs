using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
	public class PragaListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean Associar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private Praga _filtros = new Praga();
		public Praga Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<Praga> _resultados = new List<Praga>();
		public List<Praga> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public PragaListarVM() { }

		public PragaListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
        	
	}
}