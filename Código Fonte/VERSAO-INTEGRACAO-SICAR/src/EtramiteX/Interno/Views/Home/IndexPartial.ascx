<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IndicadoresVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloSecurity" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>


<% EtramitePrincipal User = (HttpContext.Current.User as EtramitePrincipal); %>

<% if (User != null && User.Identity.IsAuthenticated && Model.Exibir) {%>
	<% Html.RenderPartial("~/Areas/Relatorios/Views/Indicadores/Indicadores.ascx"); %>
<% } %>
<% else { %>
	<div id="central">
		<img src="<%= Url.Content("~/Content/_imgLogo/logo_entrada.png") %>" alt="Logar" style="display: block; margin: 150px auto 0px; width: 776px;" />
	</div>
<% } %>