<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CobrancaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Cobrança</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/cobranca.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoListar.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			Cobranca.load($('#central'), {
				urls: {
					visualizar: '<%= Url.Action("CobrancaVisualizar", "Fiscalizacao") %>',
					carregar: '<%= Url.Action("Cobranca", "Fiscalizacao") %>',
					cancelar: '<%= Url.Action("NotificacaoVisualizar", "Fiscalizacao") %>',

					editarAutuadoPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					fiscalizacaoPessoaModal: '<%= Url.Action("FiscalizacaoPessoaModal", "Fiscalizacao") %>',
					associarAutuado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					associarFiscalizacao: '<%= Url.Action("Associar", "Fiscalizacao") %>',
					obterFiscalizacao: '<%= Url.Action("ObterFiscalizacao", "Fiscalizacao") %>',
					visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
					recalcular: '<%= Url.Action("CobrancaRecalcular", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <h1 class="titTela">Visualizar Cobrança</h1>
        <br />

        <% Html.RenderPartial("CobrancaPartial", Model); %>
    </div>
</asp:Content>