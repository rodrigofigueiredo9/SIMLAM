<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AcompanhamentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Acompanhamento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/acompanhamento.js") %>" ></script>

	<script>
		$(function () {
			Acompanhamento.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AcompanhamentoEditar", "Fiscalizacao") %>',
					concluirCadastro: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao") %>',
					enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "Fiscalizacao") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Editar Acompanhamento</h1><br />

		<% Html.RenderPartial("AcompanhamentoPartial", Model); %>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Acompanhamentos", "Fiscalizacao", new { id = Model.Acompanhamento.FiscalizacaoId}) %>">Cancelar</a></span>
			<span class="spnConcluirCadastro <%=Model.Acompanhamento.Id > 0 ? "" : "hide" %>"><input class="floatRight btnConcluirCadastro" type="button" value="Concluir Cadastro" /></span>
		</div>
	</div>
</asp:Content>
