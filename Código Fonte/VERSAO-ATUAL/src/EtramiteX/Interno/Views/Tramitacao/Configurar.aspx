<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Tramitação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Funcionario/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Tramitacao/configurar.js") %>"></script>
	<script>
		$(function () {
			TramitacaoConfigurar.load($('#central'), {
				urls: {
					associarFuncionario: '<%= Url.Action("Associar", "Funcionario") %>',
					validarFuncContidoSetor: '<%= Url.Action("ValidarFuncionarioContidoSetor", "Tramitacao") %>',
					salvar: '<%= Url.Action("Configurar", "Tramitacao") %>'
				},
				Mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Configurar Tramitação</h1>
		<br />

		<div>
			<% Html.RenderPartial("ConfigurarPartial"); %>
		</div>

		<div class="block box">
			<input class="btnTramitConfigSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>