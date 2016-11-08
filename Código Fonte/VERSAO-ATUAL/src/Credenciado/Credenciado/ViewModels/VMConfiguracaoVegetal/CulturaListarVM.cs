using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMConfiguracaoVegetal
{
    public class CulturaListarVM
    {
        public String UltimaBusca { get; set; }
        public Boolean PodeVisualizar { get; set; }
        public Boolean Associar { get; set; }
        public Boolean PodeEditar { get; set; }
        public Boolean PodeExcluir { get; set; }
        public Boolean PodeAlterarSituacao { get; set; }

        #region StraggCultivar

        private bool _straggCultivar = true;
        public bool StraggCultivar
        {
            get { return _straggCultivar; }
            set { _straggCultivar = value; }
        }

        #endregion

        private Paginacao _paginacao = new Paginacao();
        public Paginacao Paginacao
        {
            get { return _paginacao; }
            set { _paginacao = value; }
        }

        private CulturaListarFiltro _filtros = new CulturaListarFiltro();
        public CulturaListarFiltro Filtros
        {
            get { return _filtros; }
            set { _filtros = value; }
        }

        private List<CulturaListarResultado> _resultados = new List<CulturaListarResultado>();
        public List<CulturaListarResultado> Resultados
        {
            get { return _resultados; }
            set { _resultados = value; }
        }

        public CulturaListarVM(List<QuantPaginacao> quantPaginacao)
        {
            Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
        }
        
        public CulturaListarVM() { }
        
        public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
        {
            Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
        }

    }
}