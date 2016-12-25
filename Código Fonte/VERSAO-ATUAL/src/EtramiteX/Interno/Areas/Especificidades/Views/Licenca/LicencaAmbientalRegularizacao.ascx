<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LicencaAmbientalRegularizacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Licenca/LicencaAmbientalRegularizacao.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>
<script>
	LicencaAmbientalRegularizacao.settings.urls.obterDadosLicencaAmbientalRegularizacao = '<%= Url.Action("ObterDadosLicencaPorteUsoMotosserra", "Licenca", new {area="Especificidades"}) %>';
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
			<%: Html.DropDownList("Licenca.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text  ddlDestinatarios" }))%>
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