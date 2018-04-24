<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EmpreendimentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Empreendimento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>

<script src="<%= Url.Content("~/Scripts/Empreendimento/tela.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/listar.js") %>"></script>

<!-- DEPENDENCIAS DE MODAL PESSOA -->
<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>" ></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE MODAL PESSOA -->

<script>
		$(function () {
			EmpreendimentoTela.load($('#central'), {
				urls: {
					avancar: '<%= Url.Action("Salvar", "Empreendimento") %>',
					voltar: '<%= Url.Action("LocalizarMontar", "Empreendimento") %>',
					salvarCadastrar: '<%= Url.Action("SalvarCadastrar", "Empreendimento") %>',
					editar: '<%= Url.Action("Editar", "Empreendimento") %>',
					visualizar: '<%= Url.Action("Visualizar", "Empreendimento") %>',
					associarAtividadeModal: '<%= Url.Action("AtividadeEmpListarFiltros", "Atividade") %>',
					associarResponsavelModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					associarResponsavelEditarModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					verificarCnpj: '<%= Url.Action("VerificarCnpj", "Empreendimento") %>',
					pessoaAssociar: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					coordenadaGeo: '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento"}) %>',
					obterEstadosMunicipiosPorCoordenada: '<%= Url.Action("obterEstadosMunicipiosPorCoordenada", "Mapa", new {area="GeoProcessamento"}) %>',
					obterEnderecoResponsavel: '<%= Url.Action("ObterEnderecoResponsavel", "Empreendimento") %>',
					obterListaResponsaveis: '<%= Url.Action("ObterListaResponsaveis", "Empreendimento") %>',
					obterListaPessoasAssociada: '<%= Url.Action("ObterListaPessoasAssociada", "Empreendimento") %>',
					verificarLocalizaoEmpreendimento: '<%= Url.Action("VerificarLocalizacaoEmpreendimento", "Empreendimento") %>',
					obterListaResponsaveisCnpj: '<%= Url.Action("ObterListaResponsaveisCnpj", "Empreendimento") %>'
				},
				msgs: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela%>,
				denominadoresSegmentos: '<%= Model.SalvarVM.DenominadoresSegmentos %>'
			});
		});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central" class="empreendimentoCriar">

	<% Html.RenderPartial("EmpreendimentoPartial", Model); %>
	<div class="block box btnEmpContainer">
		<input class="floatLeft btnEmpAvancar " type="button" value="Novo" />
		<span style="float: left">&nbsp;&nbsp;</span>
		<input class="floatLeft btnEmpSalvar" type="button" value="Salvar" />
		<span style="float: left">&nbsp;&nbsp;</span>
		<input class="floatLeft btnEmpVoltar" type="button" value="Voltar" />
		<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
	</div>
</div>
</asp:Content>