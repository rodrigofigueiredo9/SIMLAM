using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao
{
	public class ProfissaoListarVM
	{
		public String UltimaBusca { get; set; }

		private List<Profissao> _resultados = new List<Profissao>();
		public List<Profissao> Resultados
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

		private ProfissaoListarFiltros _filtros = new ProfissaoListarFiltros();
		public ProfissaoListarFiltros Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public ProfissaoListarVM() { }

		public ProfissaoListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public bool IsAssociar { get; set; }
	}
}