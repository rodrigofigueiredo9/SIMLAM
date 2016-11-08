<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AgrotoxicoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Agrotóxico</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Agrotoxico/agrotoxico.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Agrotoxico/agrotoxicoCultura.js") %>" ></script>

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/IngredienteAtivo/listar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			Agrotoxico.load($("#central"), {
				urls: {
					obterMensagemAgrotoxicoDesativado: '<%=Url.Action("ObterMensagemAgrotoxicoDesativado", "Agrotoxico")%>',
					associarPessoa: '<%=Url.Action("PessoaModal", "Pessoa", new { @tipoCadastro = "2" })%>',
					ingredientesAtivos: '<%=Url.Action("IngredientesAtivos", "ConfiguracaoVegetal", new { @associar = true})%>',
					agrotoxicoCulturaCriar: '<%=Url.Action("CriarCultura", "Agrotoxico")%>',
					agrotoxicoCulturaEditar: '<%=Url.Action("EditarCultura", "Agrotoxico")%>',
					agrotoxicoCulturaVisualizar: '<%=Url.Action("VisualizarCultura", "Agrotoxico")%>',
					enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
					salvar: '<%= Url.Action("Criar", "Agrotoxico") %>'
				},
				ingredienteAtivoUnidadeMedida: <%= Model.IngredienteAtivoUnidadeMedida %>,
				mensagens: <%= Model.Mensagens %>,
				idsTelaIngredienteAtivoSituacao: <%= Model.IdsTelaIngredienteAtivoSituacao %>,
				tiposArquivo: <%= Model.TiposArquivoValido %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar Agrotóxico</h1>
		<br />

		<%Html.RenderPartial("AgrotoxicoPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "Agrotoxico") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>