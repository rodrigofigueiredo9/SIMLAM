using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados
{
    public class ListarVM
    {
        public String UltimaBusca { get; set; }
        public Boolean PodeVisualizar { get; set; }
        public Boolean PodeGerenciar { get; set; }
        public Boolean PodeEditar { get; set; }
        public Boolean PodeAlterarSituacao { get; set; }

        private List<SelectListItem> _listaSituacoes = new List<SelectListItem>();
        public List<SelectListItem> ListaSituacoes
        {
            get { return _listaSituacoes; }
            set { _listaSituacoes = value; }
        }

        private Paginacao _paginacao = new Paginacao();
        public Paginacao Paginacao
        {
            get { return _paginacao; }
            set { _paginacao = value; }
        }

        private OrgaoParceiroConveniadoListarFiltros _filtros = new OrgaoParceiroConveniadoListarFiltros();
        public OrgaoParceiroConveniadoListarFiltros Filtros
        {
            get { return _filtros; }
            set { _filtros = value; }
        }

        private List<OrgaoParceiroConveniadoListarResultados> _resultados = new List<OrgaoParceiroConveniadoListarResultados>();
        public List<OrgaoParceiroConveniadoListarResultados> Resultados
        {
            get { return _resultados; }
            set { _resultados = value; }
        }

        public ListarVM() : this(new List<QuantPaginacao>(), new List<Lista>()) { }

        public ListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> listaSituacoes)
        {
            ListaSituacoes = ViewModelHelper.CriarSelectList(listaSituacoes, true, true);
            Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
        }

        public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
        {
            Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
        }

		public String Mensagens 
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@OrgaoParceiroBloqueado = Mensagem.OrgaoParceiroConveniado.OrgaoBloqueado
				});
			}
		}
    }
}