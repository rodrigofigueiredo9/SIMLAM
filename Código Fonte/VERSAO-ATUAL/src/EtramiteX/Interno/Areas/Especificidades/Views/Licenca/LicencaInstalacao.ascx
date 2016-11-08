﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LicencaInstalacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Licenca/LicencaInstalacao.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>
<script type="text/javascript">
	LicencaInstalacao.settings.urls.obterDadosLicencaInstalacao = '<%= Url.Action("ObterDadosLicencaPorteUsoMotosserra", "Licenca", new {area="Especificidades"}) %>';
	AtividadeEspecificidade.Barragem.settings.urls.urlEspBarragem = '<%= Url.Action("AtividadeCaracterizacao", "AtividadeEspecificidade", new {area="Especificidades"}) %>';
</script>
<%= Html.Hidden("hdnLicencaJSON", Model.GetLicencaJSON, new { @class = "hdnLicencaJSON" })%>
<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Licenca.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text  ddlDestinatarios" }))%>
		</div>
	</div>
	<div class="contextAtividadeCaracterizacao"></div>
</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>