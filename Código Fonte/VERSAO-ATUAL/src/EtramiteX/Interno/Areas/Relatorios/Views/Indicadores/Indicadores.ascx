<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels.IndicadoresVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloSecurity" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Security" %>
<div id="central">

	<%  EtramitePrincipal user = HttpContext.Current.User  as EtramitePrincipal; %>

	<% using (Html.BeginForm("Indicadores", "Indicadores", FormMethod.Get))
		{ %>

		<div id="container">

			<% if (user.IsInAnyRole(String.Join(",", new[] { ePermissao.TituloRelatorioIndicadoresTitulos.ToString(), ePermissao.TituloRelatorioIndicadoresTitulosCondicionantes.ToString() }))) { %>
				<% Html.RenderPartial("~/Areas/Relatorios/Views/Titulo/IndicadoresTitulo.ascx", Model); %>
			<%} %>
		</div>
	<% } %>
</div>

<script src="<%= Url.Content("~/Scripts/Areas/Relatorios/Indicadores.js") %>" ></script>

<script>
	Indicadores.urlModalIndicadores = '<%= Url.Action("InidicadoresModal", "Indicadores", new {area="Relatorios"})%>';
</script>