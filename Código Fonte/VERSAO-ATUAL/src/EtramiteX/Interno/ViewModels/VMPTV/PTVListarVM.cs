using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class PTVListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
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

		private PTVListarFiltro _filtros = new PTVListarFiltro();
		public PTVListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<PTVListarResultado> _resultados = new List<PTVListarResultado>();
		public List<PTVListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public List<SelectListItem> Situacoes { get; set; }

		public List<SelectListItem> TiposDocumentoOrigem { get; set; }

		public PTVListarVM(List<Lista> lstSituacoes, List<Lista> lstTipoDocumentoOrigem)
		{
			this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes);

			if (Convert.ToBoolean(this.Situacoes?.Exists(x => x.Value == ((int)eSolicitarPTVSituacao.Cadastrado).ToString())))
				this.Situacoes.Remove(this.Situacoes.FirstOrDefault(x => x.Value == ((int)eSolicitarPTVSituacao.Cadastrado).ToString()));
			if (Convert.ToBoolean(this.Situacoes?.Exists(x => x.Value == ((int)eSolicitarPTVSituacao.Editado).ToString())))
				this.Situacoes.Remove(this.Situacoes.FirstOrDefault(x => x.Value == ((int)eSolicitarPTVSituacao.Editado).ToString()));
			this.TiposDocumentoOrigem = ViewModelHelper.CriarSelectList(lstTipoDocumentoOrigem);
		}

		public PTVListarVM() { }

		public PTVListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}