using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<Pessoa> _resultados = new List<Pessoa>();
		public List<Pessoa> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private ListarFiltro _filtros = new ListarFiltro();
		public ListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public String UltimaBusca { get; set; }
		public String MsgExcluir { get; set; }

		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeAssociar { get; set; }

		public ListarVM()
		{
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			SetListItens(quantPaginacao);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}