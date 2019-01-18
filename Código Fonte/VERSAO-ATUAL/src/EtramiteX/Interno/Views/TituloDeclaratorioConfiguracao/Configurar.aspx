<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfigurarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorioConfiguracao/configurar.js") %>"></script>

	<script>
		$(function () {
			TituloDeclaratorioConfiguracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</h1>
		<br />
		<fieldset class="block box">
			<legend>Área alagada na soleira do vertedouro (ha)</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Configurar_ValorMaximoAtual">Valor máximo atual:</label>
					<%= Html.TextBox("Configurar_ValorMaximoAtual", Model.ValorMaximoAtual, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>
	</div>
</asp:Content>