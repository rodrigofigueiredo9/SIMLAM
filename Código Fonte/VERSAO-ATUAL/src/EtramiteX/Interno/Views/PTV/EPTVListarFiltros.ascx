<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVListarVM>" %>

<h1 class="titTela">EPTVs</h1>
<br />

<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
    <div class="filtroCorpo filtroSerializarAjax block">
        <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
        <input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />
        <input type="hidden" class="hdnAlerta" name="Alerta" value="<%= Request.Params["alerta"] %>" />
        <%= Html.Hidden("UrlVisualizar", Url.Action("EPTVVisualizar"), new { @class = "urlVisualizar" })%>
        <%= Html.Hidden("UrlAnalisar", Url.Action("EPTVAnalisar"), new { @class = "urlAnalisar" })%>

        <%= Html.Hidden("UrlFiltrar", Url.Action("EPTVFiltrar"), new { @class = "urlFiltrar" })%>
        <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
        <%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

        <div class="ultima">
            <div class="block fixado">
                <div class="coluna60">
                    <label for="Filtros_Numero">Número PTV</label>
                    <%= Html.TextBox("Filtros.Numero", string.Empty, new { @class = "text setarFoco maskNumInt", @maxlength="10" })%>
                </div>
                <div class="coluna20">
                    <label>Situação</label>
                    <%=Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class="text" }) %>
                </div>
                <div class="coluna10">
                    <button class="inlineBotao btnBuscar">Buscar</button>
                </div>
            </div>
            <div class="block hide">
                <div class="block">
                    <div class="coluna81 append1">
                        <label for="Filtros_Nome">Interessado</label>
                        <%= Html.TextBox("Filtros.Interessado", string.Empty, new { @class = "text", @maxlength="100", @style="width:100%;"  })%>
                    </div>
                </div>
                <div class="block">
                    <div class="coluna60">
                        <label for="Filtros_Numero">Número DUA</label>
                        <%= Html.TextBox("Filtros.DUANumero", string.Empty, new { @class = "text setarFoco maskNumInt", @maxlength="10" })%>
                    </div>

                    <div class="coluna20">
                        <label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.DUAIsCPF", true, true, new { @class = "radio radioCpfCnpj radioCPF" })%>CPF</label>
                        <label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.DUAIsCPF", false, false, new { @class = "radio radioCpfCnpj" })%>CNPJ</label>
                        <%= Html.TextBox("Filtros.DUACPFCNPJ", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
                    </div>
                </div>
                <div class="block">
                    <div class="coluna81">
                        <label for="Filtros_Nome">Empreendimento</label>
                        <%= Html.TextBox("Filtros.Empreendimento", string.Empty, new { @class = "text", @maxlength="100", @style="width:100%;"  })%>
                    </div>
                </div>
                <div class="block">
                    <div class="coluna60">
                        <label for="Filtros_Destinatario">Destinatário</label>
                        <%= Html.TextBox("Filtros.Destinatario", string.Empty, new { @class = "text", @maxlength="100" })%>
                    </div>
                    <div class="coluna20">
                        <label>Cultura/Cultivar</label>
                        <%=Html.TextBox("Filtros.CulturaCultivar", string.Empty, new { @class="text ", @maxlength="100"})%>
                    </div>
                </div>
                <div class="block">
                    <div class="coluna60">
                        <label>Tipo do documento de origem</label>
                        <%=Html.DropDownList("Filtros.TipoDocumento", Model.TiposDocumentoOrigem, new { @class="text ddlTipoDocumento" }) %>
                    </div>
                    <div class="coluna20  txtNumeroDocumento hide">
                        <label>N° do documento de origem</label>
                        <%=Html.TextBox("Filtros.NumeroDocumento", string.Empty, new { @class="text", @maxlength="100"})%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="gridContainer"></div>
