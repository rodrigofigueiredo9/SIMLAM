<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AcompanhamentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Acompanhamento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/acompanhamento.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			Acompanhamento.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AcompanhamentoCriar", "Fiscalizacao") %>',
					concluirCadastro: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao") %>',
					enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "Fiscalizacao") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls:{
					urlGerarPdf: '<%= Url.Action("LaudoAcompanhamentoFiscalizacaoPdf", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
					urlEditar: '<%= Url.Action("AcompanhamentoEditar", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
					urlAlterarSituacao: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
					urlListar: '<%= Url.Action("Acompanhamentos", "Fiscalizacao", new {id = Request.Params["fiscalizacaoId"].ToString() }) %>'
				}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Cadastrar Acompanhamento</h1><br />

		<% Html.RenderPartial("AcompanhamentoPartial", Model); %>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Acompanhamentos", "Fiscalizacao", new { id = Model.Acompanhamento.FiscalizacaoId}) %>">Cancelar</a></span>
			<span class="spnConcluirCadastro <%=Model.Acompanhamento.Id > 0 ? "" : "hide" %>"><input class="floatRight btnConcluirCadastro" type="button" value="Concluir Cadastro" /></span>
		</div>
	</div>
</asp:Content>
