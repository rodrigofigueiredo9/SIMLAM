<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrosReciboEntregaCopiaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Outros/outrosReciboEntregaCopia.js") %>"></script>
<script>
	OutrosReciboEntregaCopia.settings.urls.obterDadosOutros = '<%= Url.Action("ObterDadosOutros", "Outros", new {area="Especificidades"}) %>';
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

</fieldset>
