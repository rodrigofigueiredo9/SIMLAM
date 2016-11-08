<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Titulos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/condicionanteSituacaoAlterar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			TituloListar.urlExcluir = '<%= Url.Action("Excluir", "Titulo") %>';
			TituloListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Titulo") %>';
			TituloListar.urlEditar = '<%= Url.Action("Editar", "Titulo") %>';
			TituloListar.urlVisualizar = '<%= Url.Action("Visualizar", "Titulo") %>';
			TituloListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "Titulo") %>';
			TituloListar.urlCondicionanteSituacaoAlterar = '<%= Url.Action("CondicionanteSituacaoAlterar", "Titulo") %>';
			TituloListar.urlValidarAlterarSituacao = '<%= Url.Action("ValidarAlterarSituacao", "Titulo") %>';
			TituloListar.urlValidarPossuiCondicionantes = '<%= Url.Action("ValidarPossuiCondicionante", "Titulo") %>';
			TituloListar.urlValidarAlterarAutorSetor = '<%= Url.Action("ValidarAbrirAlterarAutorSetor", "Titulo") %>';
			TituloListar.urlAlterarAutorSetor = '<%= Url.Action("AlterarAutorSetor", "Titulo") %>';
			TituloListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
			
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdf", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlAlterarSituacao: '<%= Url.Action("AlterarSituacao", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlEditar: '<%= Url.Action("Editar", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});

			<%}%>
		});
		
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("TituloListarFiltros"); %>
	</div>
</asp:Content>