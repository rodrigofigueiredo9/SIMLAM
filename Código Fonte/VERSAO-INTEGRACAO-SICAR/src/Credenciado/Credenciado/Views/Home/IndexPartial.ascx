<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloSecurity" %>

<% EtramitePrincipal User = (HttpContext.Current.User as EtramitePrincipal); %>

<div id="central">
	<img src="<%= Url.Content("~/Content/_imgLogo/logo_entrada.png") %>" alt="Logar" style="display: block; margin: 150px auto 0px; width: 776px;" />
</div>