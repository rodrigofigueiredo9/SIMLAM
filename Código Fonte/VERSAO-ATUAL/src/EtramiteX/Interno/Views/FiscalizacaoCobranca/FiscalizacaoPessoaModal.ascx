<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CobrancaVM>" %>

<h2 class="titTela"><%= "Verificar Fiscalização/Pessoa" %></h2>

<br />

<fieldset class="block box">
    <div class="block">
        <div class="coluna10">
            <button type="button" class="inlineBotao btnVerificarFiscalizacao" onclick="Modal.fechar(this); Cobranca.abrirModalFiscalizacao();">Fiscalização</button>
        </div>
        <div class="coluna10">
            <button type="button" class="inlineBotao btnVerificarPessoa" onclick="Modal.fechar(this); Cobranca.abrirModalPessoa();">Pessoa</button>
        </div>
    </div>
</fieldset>
<div class="block box">
    <a class="linkCancelar" title="Cancelar" onclick="Modal.fechar(this);">Cancelar</a>
</div>
