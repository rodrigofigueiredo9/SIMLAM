using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class RetificacaoNFCaixaEditarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean IsVisualizar { get; set; }
		public Boolean Associar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeGerarPDF { get; set; }
		public Boolean PodeAtivar { get; set; }
		public Boolean PodeAnalisar { get; set; }

		public Boolean PodeCancelar { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private int _filtros = new int();
		public int Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<PTVNFCaixaResultado> _listaPTV = new List<PTVNFCaixaResultado>();
		public List<PTVNFCaixaResultado> ResultadosPTV
		{
			get { return _listaPTV; }
			set { _listaPTV = value; }
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public NotaFiscalCaixa NotaFiscalDeCaixa { get; set; }

		public int Id { get; set; }

		public RetificacaoNFCaixaEditarVM() { }
	}
}