using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
{
    public class HistoricoEmissaoCFOCFOCVM
    {
        public List<HistoricoEmissaoCFOCFOC> ListaHistoricoHabilitacao { get; set; }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string NumeroHabilitacao { get; set; }

        //Tela
        public Boolean IsVisualizar { get; set; }
        public Boolean IsEditar { get; set; }
        public Boolean IsAjaxRequest { get; set; }

        public String ObterJSon(Object objeto)
        {
            return ViewModelHelper.JsSerializer.Serialize(objeto);
        }

        public HistoricoEmissaoCFOCFOCVM(List<HistoricoEmissaoCFOCFOC> listaEmissoes)
        {
            ListaHistoricoHabilitacao = listaEmissoes;
            IsVisualizar = false;
            IsEditar = false;
            IsAjaxRequest = false;
        }
        public HistoricoEmissaoCFOCFOCVM()
        {
            ListaHistoricoHabilitacao = new List<HistoricoEmissaoCFOCFOC>();
            IsVisualizar = false;
            IsEditar = false;
            IsAjaxRequest = false;
        }
    }
}