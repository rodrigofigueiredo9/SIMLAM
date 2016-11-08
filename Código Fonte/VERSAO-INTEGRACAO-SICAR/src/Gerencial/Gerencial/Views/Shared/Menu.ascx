<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.ViewModels.VMMenu" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MenuVM>" %>

<nav class="navExp">
	<!-- EXPANDIR E RECOLHER O MENU  -->
	<div id="menuCmd"><a class="exp" title="Recolher/Expandir o Menu">Recolher/Expandir o Menu</a></div>
	<!-- =========================== -->

	<!-- MENU  -->
	<ul class="menu" runat="server" id="ulMenu">
		<%  foreach (var item in Model.Menus) { %>
			<li class="itemMenu<%= (item.IsAtivo)? " ativo":"" %>">
				<a href="<%= item.Url %>" class="<%= item.Css %>" title="<%= item.Nome %>"><img src="<%= Url.Content("~/Content/_img/sprite_menu_principal.png") %>" alt="<%= item.Nome %>" /><span><%= item.Nome %></span></a>
			</li>
		<% } %>
	</ul>
	<!-- ==== -->
</nav>