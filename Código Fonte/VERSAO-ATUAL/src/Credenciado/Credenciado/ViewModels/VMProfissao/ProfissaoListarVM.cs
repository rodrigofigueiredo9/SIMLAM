using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProfissao
{
	public class ProfissaoListarVM
	{
		private ProfissaoListarFiltros _filtros = new ProfissaoListarFiltros();
		private List<QuantPaginacao> _lstPaginacao = new List<QuantPaginacao>();
		private Paginacao _paginacao = new Paginacao();
		private List<Profissao> _resultados = new List<Profissao>();

		public List<Profissao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}
 
		public ProfissaoListarFiltros Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public List<QuantPaginacao> LstPaginacao
		{
			get { return _lstPaginacao; }
			set { _lstPaginacao = value; }
		}
		
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}
		
		public bool IsAssociar { get; set; }
		public string UltimaBusca { get; set; }

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public ProfissaoListarVM(List<QuantPaginacao> lstPaginacao)
		{
			this._lstPaginacao = lstPaginacao;
		}

		public ProfissaoListarVM()
		{
		}
	}
}