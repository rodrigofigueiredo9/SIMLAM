<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Notificacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NotificacaoEmbargoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Titulo/destinatarioEspecificidade.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Notificacao/notificacaoEmbargo.js") %>"></script>
<script>
	NotificacaoEmbargo.settings.urls.obterDadosNotificacao = '<%= Url.Action("ObterDadosNotificacao", "Notificacao", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna75">
			<label>Atividade a ser embargada *</label>
			<%: Html.DropDownList("Notificacao.AtividadeEmbargo", Model.AtividadesEmbargo, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.AtividadesEmbargo.Count <= 1, new { @class = "text ddlAtividadesEmbargo" }))%>
		</div>
	</div>

	<% Html.RenderPartial("DestinatarioEspecificidade", Model.DestinatariosVM); %>

</fieldset>
