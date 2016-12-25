<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrosInformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Outros/outrosInformacaoCorte.js") %>"></script>
<script>
	OutrosInformacaoCorte.settings.urls.obterDadosOutrosInformacaoCorte = '<%= Url.Action("ObterDadosOutrosInformacaoCorte", "Outros", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna75">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Outros.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text  ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna15">
			<label>Informação de corte *</label>
			<%= Html.DropDownList("Outros.InformacaoCortes", Model.InformacaoCortes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlInformacaoCortes" }))%>
		</div>
	</div>

</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>