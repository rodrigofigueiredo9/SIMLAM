<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>	
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<% if (Model.IsExibirCamposModelo) {%>

	<% if (Model.IsExibirAssinate) {%> 
		<% Html.RenderPartial("Assinantes", Model.AssinantesVM); %>
	<%} %>

<%} %>