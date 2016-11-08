using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV
{
    public class PTVHistoricoVM
    {
        private PTVHistorico ptvHistorico = new PTVHistorico();

        public string NumeroEPTV { get { return ptvHistorico.NumeroEPTV; } }

        public string DataEmissao { get { return ptvHistorico.DataEmissao; } }

        public List<PTVItemHistorico> Resultados { get { return ptvHistorico.ListaHistoricos; } }

        public List<SelectListItem> Situacoes { get; set; }

        public string Titulo { get; set; }

        public PTVHistoricoVM(PTVHistorico _ptvHistorico, List<Lista> lstSituacoes)
        {
            ptvHistorico = _ptvHistorico; 

            this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, true, true, _ptvHistorico.SituacaoAtual.ToString());
        }
    }
}