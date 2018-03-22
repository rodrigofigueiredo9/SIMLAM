<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CobrancaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar Cobrança</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/cobranca.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoListar.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>
    <script>
		$(function () {
			Cobranca.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("CobrancaCriar", "Fiscalizacao") %>',
					carregar: '<%= Url.Action("Cobranca", "Fiscalizacao") %>',
					cancelar: '<%= Url.Action("NotificacaoVisualizar", "Fiscalizacao") %>',
					novoParcelamento: '<%= Url.Action("CobrancaNovoParcelamento", "Fiscalizacao") %>',

					editarAutuadoPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					fiscalizacaoPessoaModal: '<%= Url.Action("FiscalizacaoPessoaModal", "Fiscalizacao") %>',
					associarAutuado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					associarFiscalizacao: '<%= Url.Action("Associar", "Fiscalizacao") %>',
					obterFiscalizacao: '<%= Url.Action("ObterFiscalizacao", "Fiscalizacao") %>',
					visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
					recalcular: '<%= Url.Action("CobrancaRecalcular", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <h1 class="titTela">Salvar Cobrança</h1>
        <br />

        <div class="cobrancaPartial">
            <% Html.RenderPartial("CobrancaPartial", Model); %>
        </div>
    </div>
</asp:Content>
