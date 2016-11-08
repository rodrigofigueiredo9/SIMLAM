using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados
{
    public class AlterarSituacaoVM
    {
        private OrgaoParceiroConveniado _orgaoParceiroConveniado;
        public OrgaoParceiroConveniado OrgaoParceiroConveniado
        {
            get { return _orgaoParceiroConveniado; }
            set { _orgaoParceiroConveniado = value; }
        }

        public List<SelectListItem> Situacoes { get; set; }

        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    ConfirmBloquear = Mensagem.OrgaoParceiroConveniado.ConfirmBloquear,
                    TituloConfirmBloquear = Mensagem.OrgaoParceiroConveniado.TituloConfirmBloquear,
                    ConfirmAtivar = Mensagem.OrgaoParceiroConveniado.ConfirmAtivar,
                    TituloConfirmAtivar = Mensagem.OrgaoParceiroConveniado.TituloConfirmAtivar
                });
            }
        }
    }
}