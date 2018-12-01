using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class RetificacaoNFCaixaVM
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
		public int RT { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private RetificacaoNFCaixaListarFiltro _filtros = new RetificacaoNFCaixaListarFiltro();
		public RetificacaoNFCaixaListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<RetificacaoNFCaixaListarResultado> _resultados = new List<RetificacaoNFCaixaListarResultado>();
		public List<RetificacaoNFCaixaListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public NotaFiscalCaixa NotaFiscalDeCaixa { get; set; }

		public RetificacaoNFCaixaVM() { }
	}
}