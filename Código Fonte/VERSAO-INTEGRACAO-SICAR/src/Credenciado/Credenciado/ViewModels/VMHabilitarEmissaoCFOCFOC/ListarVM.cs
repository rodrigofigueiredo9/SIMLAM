using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMHabilitarEmissaoCFOCFOC
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}


		private ListarFiltro _filtros = new ListarFiltro();
		public ListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<PragaHabilitarEmissao> _resultados = new List<PragaHabilitarEmissao>();
		public List<PragaHabilitarEmissao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public ListarVM() : this(new List<QuantPaginacao>()) { }

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{			
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}