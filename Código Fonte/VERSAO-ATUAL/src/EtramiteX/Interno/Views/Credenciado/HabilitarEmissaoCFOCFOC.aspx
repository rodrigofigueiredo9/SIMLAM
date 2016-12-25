<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<HabilitarEmissaoCFOCFOCVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Habilitar Emissão de CFO e CFOC</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/Pessoa.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Credenciado/habilitarEmissaoCFOCFOC.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Credenciado/renovarDataHabilitacaoCFO.js") %>"></script>

	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			HabilitarEmissaoCFOCFOC.load($('#central'),
			{
				urls:
				{
					salvar: '<%= Url.Action("SalvarHabilitarEmissao", "Credenciado") %>',
					visualizarResponsavel: '<%= Url.Action("Visualizar", "Credenciado") %>',
					pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					verificar: '<% = Url.Action("CriarVerificarCpfCnpj", "Pessoa") %>',
					visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
					criar: '<% = Url.Action("Criar", "Pessoa") %>',
					obter: '<% = Url.Action("Obter", "Credenciado") %>',
					renovarDatasPragas: '<% = Url.Action("RenovarPraga", "Credenciado") %>',
					validarPraga: '<% = Url.Action("ValidarAdicionarPraga", "Credenciado") %>',
					associarPragas: '<%= Url.Action("AssociarPraga", "ConfiguracaoVegetal") %>'
				},
				Mensagens:<%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%= Html.Hidden("HabilitarEmissao.Id", Model.HabilitarEmissao.Id, new { @class = "hdnHabilitarEmissaoId" })%>
		<% Html.RenderPartial("HabilitarEmissaoCFOCFOCPartial"); %>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<input class="btnSalvar floatLeft <%= (Model.IsVisualizar? "hide" : "") %>" type="button" value="Salvar" />
				<span class="cancelarCaixa"><%= (Model.IsVisualizar? "" : "ou") %> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("IndexHabilitarEmissaoCFOCFOC", "Credenciado") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>