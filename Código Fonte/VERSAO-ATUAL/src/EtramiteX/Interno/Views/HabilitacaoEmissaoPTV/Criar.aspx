<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<HabilitacaoEmissaoPTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Habilitar Emissão de PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/HabilitacaoEmissaoPTV/habilitacaoEmissaoPTV.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Funcionario/listar.js") %>" ></script>

	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>

	<script src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>

	<script>
		$(function () {
			HabilitacaoEmissaoPTV.load($("#central"), {
				urls: {
					verificarCPF: '<%= Url.Action("VerificarCPF", "HabilitacaoEmissaoPTV") %>',
					associarFuncionario: '<%= Url.Action("Associar", "Funcionario") %>',
					enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
					associarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
					buscarMunicipios: '<%= Url.Action("BuscarMunicipios", "HabilitacaoEmissaoPTV") %>',
					validarOperador: '<%= Url.Action("ExisteOperador", "HabilitacaoEmissaoPTV")%>',
					salvar: '<%= Url.Action("Criar", "HabilitacaoEmissaoPTV") %>'
				},
				estadoDefaultId: <%= ViewModelHelper.EstadoDefaultId() %>,
				tiposArquivo: <%= Model.TiposArquivoValido %>,
				mensagens: <%= Model.Mensagens %>,
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Habilitar Emissão de PTV</h1>
		<br />

		<%Html.RenderPartial("HabilitacaoEmissaoPTVPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "HabilitacaoEmissaoPTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>