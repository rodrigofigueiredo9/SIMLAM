﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ExploracaoFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	ExploracaoFlorestal.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	ExploracaoFlorestal.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	ExploracaoFlorestal.settings.textoMerge = '<%= Model.TextoMerge %>';
	ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';	
	ExploracaoFlorestal.settings.mensagens = <%=Model.Mensagens%>;
</script>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<fieldset class="block box localizador">
    <legend class="titLocalizador">Localizador</legend>
    <div class="coluna22 append2">
        <label for="CodigoExploracao">Código Exploração</label>
        <%= Html.TextBox("CodigoExploracao", Model.Caracterizacao.CodigoExploracao, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCodigoExploracao" }))%>
    </div>
    <div class="coluna22 append2">
        <label for="CodigoExploracao">Tipo de Exploração *</label>
        <%= Html.DropDownList("TipoExploracao", Model.TipoExploracao, new { @class = "text ddlTipoExploracao" })%>
    </div>
	<div class="coluna15">
        <label>Data Cadastro</label>
        <%= Html.TextBox("DataCadastro", Model.Caracterizacao.DataCadastro.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataCadastro maskData" }))%>
    </div>
</fieldset>

<%foreach (var item in Model.ExploracaoFlorestalExploracaoVM){ %>
<fieldset class="block box exploracoesFlorestais" id="exploracao<%: item.ExploracaoFlorestal.Identificacao%>">
	<legend class="titFiltros">Exploração Florestal</legend>
	<%Html.RenderPartial("ExploracaoFlorestalExploracao", item); %>
</fieldset>
<%} %>