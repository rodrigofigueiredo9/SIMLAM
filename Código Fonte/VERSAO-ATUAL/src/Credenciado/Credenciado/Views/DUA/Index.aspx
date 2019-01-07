<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<DUAVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Título Declaratório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/DUA/dua.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			TituloDeclaratorio.load($('#central'), {
				urls: {
					<%--TituloListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio") %>';--%>
				},
				<%--Mensagens: <%= Model.Mensagens %>,
				carregarEspecificidade: <%= Model.CarregarEspecificidade.ToString().ToLower()--%> %>
			});

			<%--<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>' },
					{ label: 'Alterar Situação', url: '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio", new {id = Request.Params["acaoId"].ToString() }) %>' },
					{ label: 'Editar', url: '<%= Url.Action("Editar", "TituloDeclaratorio", new {id = Request.Params["acaoId"].ToString() }) %>' }
				]
			});
			<% } %>--%>
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Emissão de DUA</h1>
		<br />
		<% Html.RenderPartial("EmitirDuaListar", Model); %>
	</div>
</asp:Content>