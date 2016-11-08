using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class DestinatarioPTVListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean Associar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private DestinatarioListarFiltro _filtros = new DestinatarioListarFiltro();
		public DestinatarioListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<DestinatarioListarResultado> _resultados = new List<DestinatarioListarResultado>();
		public List<DestinatarioListarResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public DestinatarioPTVListarVM() { }

		public DestinatarioPTVListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}