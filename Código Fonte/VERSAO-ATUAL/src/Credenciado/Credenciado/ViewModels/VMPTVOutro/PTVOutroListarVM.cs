using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTVOutro
{
	public class PTVOutroListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
        public Boolean PodeEditar { get; set; }
		
		public Boolean PodeCancelar { get; set; }
		public int RT { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private PTVOutroListarFiltro _filtros = new PTVOutroListarFiltro();
		public PTVOutroListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<PTVOutroListarResultado> _resultados = new List<PTVOutroListarResultado>();
		public List<PTVOutroListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public List<SelectListItem> Situacoes { get; set; }

		public PTVOutroListarVM(List<Lista> lstSituacoes)
		{
			this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes);
		}

		public PTVOutroListarVM() { }

		public PTVOutroListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}