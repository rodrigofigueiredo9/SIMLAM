<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RegularizacaoFundiariaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento - Regularização Fundiária</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/regularizacaoFundiaria.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/dominio.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>

	<script>
		$(function () {
			RegularizacaoFundiaria.load($('#central'), {
				urls: {
					criarPosse: '<%= Url.Action("CriarPosse", "RegularizacaoFundiaria")%>',
					editarPosse: '<%= Url.Action("EditarPosse", "RegularizacaoFundiaria")%>',
					visualizarPosse: '<%= Url.Action("VisualizarPosse", "RegularizacaoFundiaria")%>',
					visualizarDominio: '<%= Url.Action("DominioVisualizar", "Dominialidade") %>',
					validarDominioAvulso: '<%= Url.Action("ValidarDominioAvulso", "RegularizacaoFundiaria") %>',
					obterAreaTotalPosse: '<%= Url.Action("ObterAreaTotalPosse", "RegularizacaoFundiaria") %>',
					obterPerimetroPosse: '<%= Url.Action("ObterPerimetroPosse", "RegularizacaoFundiaria") %>',
					pessoaAssociar: '<%= Url.Action("PessoaModal", "Pessoa", new {area =""}) %>',
					pessoaVisualizar: '<%= Url.Action("Visualizar","Pessoa", new {area =""}) %>',
					validarTransmitente: '<%= Url.Action("ValidarTransmitente", "RegularizacaoFundiaria") %>',
					validarUsoAtualSolo: '<%= Url.Action("ValidarUsoAtualSolo", "RegularizacaoFundiaria") %>',
					validarEdificacao: '<%= Url.Action("ValidarEdificacao", "RegularizacaoFundiaria") %>',
					validarPosse: '<%= Url.Action("ValidarPosse", "RegularizacaoFundiaria") %>',
					mergiar: '<%= Url.Action("GeoMergiar","RegularizacaoFundiaria") %>',
					salvar: '<%= Url.Action("Criar", "RegularizacaoFundiaria") %>'
				},
				dependencias: '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>',
				textoMerge: '<%= Model.TextoMerge %>',
				atualizarDependenciasModalTitulo: '<%= Model.AtualizarDependenciasModalTitulo %>'
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Caracterização do Empreendimento - Regularização Fundiária</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("RegularizacaoFundiariaLista", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar hide" type="button" value="Salvar" />
			<span class="cancelarCaixa cancelarCaixaPrincipal"><span class="btnModalOu hide">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>