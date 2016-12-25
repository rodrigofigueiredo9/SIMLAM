<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Oficio" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OficioNotificacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Oficio/oficioNotificacao.js") %>"></script>
<script>
	OficioNotificacao.urlObterDadosOficioNotificacao = '<%= Url.Action("ObterDadosOficioNotificacao", "Oficio", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="Oficio_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Oficio.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>
</fieldset>