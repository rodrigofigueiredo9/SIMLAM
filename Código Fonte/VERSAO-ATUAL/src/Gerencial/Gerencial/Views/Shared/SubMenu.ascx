<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.ViewModels.VMMenu" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MenuVM>" %>

<% if (Model.Menus.Count > 0 ) { %>

<script language="JavaScript" type="text/javascript">
	function acaoSubMenu(title, url) {
		$.ajax({
			url: url,
			type: 'GET',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, settings.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
			}
		});
	}
</script>

<div class="menuSecCaixa">
	<ul class="menuSec">
		<% foreach (var item in Model.Menus) { %>

			<% if (item.Url.IndexOf("#AJAX", StringComparison.InvariantCultureIgnoreCase) > -1){ %>
				<li class="itemMenu<%= ((item.IsAtivo)? " ativo":"") + ((Model.Menus.Last() == item)?" ultimo":"") %>">
					<a href="javascript:acaoSubMenu('<%= item.Nome %>', '<%= VirtualPathUtility.ToAbsolute(item.Url) %>');" title="<%= item.Nome %>"><span><%= item.Nome %></span></a>
				</li>
			<% continue; } %>

			<li class="itemMenu<%= ((item.IsAtivo)? " ativo":"") + ((Model.Menus.Last() == item)?" ultimo":"") %>">
				<a href="<%= item.Url %>" title="<%= item.Nome %>"><span><%= item.Nome %></span></a>
			</li>
		<% } %>
	</ul>

	<div class="clearBoth"></div>
</div>
<% } %>