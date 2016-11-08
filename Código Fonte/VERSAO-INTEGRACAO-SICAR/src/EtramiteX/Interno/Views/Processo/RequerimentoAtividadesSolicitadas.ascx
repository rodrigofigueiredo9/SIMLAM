<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>

<% foreach (AtividadeSolicitadaVM atividadeVM in Model.AtividadesSolicitadasVM) { %>
	<div class="asmItemContainer" style="border:0px">
		<% Html.RenderPartial("~/Views/Shared/AtividadeSolicitada.ascx", atividadeVM); %>
	</div>
<% } %>