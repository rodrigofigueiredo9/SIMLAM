<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EmpreendimentoVM>" %>

<div class="empreendimentoPartial">

	<% if (Model.SalvarVM.IsEditar) { %>
			<% Html.RenderPartial("Salvar", Model.SalvarVM); %>
	<% } else if (Model.SalvarVM.IsVisualizar) { %>
			<% Html.RenderPartial("VisualizarPartial", Model.SalvarVM); %>
	<% } else { %>
			<% Html.RenderPartial("Localizar", Model.LocalizarVM); %>
	<% } %>
</div>