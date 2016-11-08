using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa
{
	public class ListarProfissaoVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<ProfissaoLst> _resultados = new List<ProfissaoLst>();
		public List<ProfissaoLst> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private ProfissaoFiltro _filtros = new ProfissaoFiltro();
		public ProfissaoFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public String ProfissaoTexto { get; set; }

		public String UltimaBusca { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarProfissaoVM()
		{
		}

		public ListarProfissaoVM(List<QuantPaginacao> quantPaginacao)
		{
			SetListItens(quantPaginacao);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}