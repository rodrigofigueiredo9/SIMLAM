<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Órgãos Parceiros/ Conveniados</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/OrgaosParceirosConveniados/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			OrgaoParceiroConveniadoListar.urlEditar = '<%= Url.Action("Editar", "OrgaosParceirosConveniados") %>';
			OrgaoParceiroConveniadoListar.urlVisualizar = '<%= Url.Action("Visualizar", "OrgaosParceirosConveniados") %>';
			OrgaoParceiroConveniadoListar.urlGerenciar = '<%= Url.Action("Gerenciar", "OrgaosParceirosConveniados") %>';
			OrgaoParceiroConveniadoListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "OrgaosParceirosConveniados") %>';
			OrgaoParceiroConveniadoListar.Mensagens = <%=Model.Mensagens%>;
			OrgaoParceiroConveniadoListar.load($('#central'));

			<% if (!string.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls: {
						urlAlterarSituacao: '<%= Url.Action("AlterarSituacao", "OrgaosParceirosConveniados", new { id = Request.Params["acaoId"] })%>'
						<% if (string.IsNullOrEmpty(Request.Params["ocutarEditar"])){%>
						,urlEditar: '<%= Url.Action("Editar", "OrgaosParceirosConveniados", new { id = Request.Params["acaoId"] }) %>'
						<%}%>
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>