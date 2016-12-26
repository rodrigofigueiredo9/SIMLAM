<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Fiscalizações</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoListar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			FiscalizacaoListar.ExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Fiscalizacao") %>';
			FiscalizacaoListar.urlExcluir = '<%= Url.Action("Excluir", "Fiscalizacao") %>';
			FiscalizacaoListar.urlEditar = '<%= Url.Action("Editar", "Fiscalizacao") %>';
			FiscalizacaoListar.urlEditarValidar = '<%= Url.Action("EditarValidar", "Fiscalizacao") %>';
			FiscalizacaoListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "Fiscalizacao") %>';
			FiscalizacaoListar.urlAcompanhamentos = '<%= Url.Action("Acompanhamentos", "Fiscalizacao") %>';
			FiscalizacaoListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlProcesso: '<%= Url.Action("Criar", "Processo") %>',
						urlFiscalizacao: '<%= Url.Action("Criar", "Fiscalizacao")%>',
						urlVisualizarPdfFiscalizacao: '<%= Url.Action("DocumentosGeradosPartial", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() })%>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ListarFiltros", Model);%>
	</div>
</asp:Content>