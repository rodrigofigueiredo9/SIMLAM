<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Titulos Declaratórios</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorio/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			TituloListar.urlVisualizar = '<%= Url.Action("Visualizar", "TituloDeclaratorio") %>';
			TituloListar.urlEditar = '<%= Url.Action("Editar", "TituloDeclaratorio") %>';
			TituloListar.urlExcluir = '<%= Url.Action("Excluir", "TituloDeclaratorio") %>';
			TituloListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "TituloDeclaratorio") %>';
			TituloListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio") %>';
			TituloListar.urlEmitirDua = '<%= Url.Action("Listar", "DUA") %>';
			TituloListar.load($('#central'));

			
			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
			{
				if (Request.Params["modelo"].ToString() == "61")
				{
					%>
				ContainerAcoes.load($(".containerAcoes"), {
						
					botoes: [
						{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "Titulo", new { id = Request.Params["acaoId"].ToString() })%>' },
						{ label: 'Editar', url: '<%= Url.Action("Editar", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() })%>' },
						{ label: 'Emitir DUA', url: '<%= Url.Action("Listar", "DUA", new { id = Request.Params["acaoId"].ToString() })%>' }
					]
					});
			<% }
			else
			{ %>
					ContainerAcoes.load($(".containerAcoes"), {
						urls:{
							urlGerarPdf: '<%= Url.Action("GerarPdf", "Titulo", new { id = Request.Params["acaoId"].ToString() }) %>',
							urlAlterarSituacao: '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() }) %>',
							urlEditar: '<%= Url.Action("Editar", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() }) %>'
						}
					});
				<%}
			}%>
		});

	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("TituloListarFiltros"); %>
	</div>
</asp:Content>