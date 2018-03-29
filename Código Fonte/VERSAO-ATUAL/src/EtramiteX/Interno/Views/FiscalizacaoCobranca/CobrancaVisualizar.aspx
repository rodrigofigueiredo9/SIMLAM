<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CobrancaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Cobrança</asp:Content>

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
					confirm: '<%= Url.Action("Confirm", "FiscalizacaoCobranca") %>',
					visualizar: '<%= Url.Action("CobrancaVisualizar", "FiscalizacaoCobranca") %>',
					carregar: '<%= Url.Action("Cobranca", "FiscalizacaoCobranca") %>',
					lista: '<%= Url.Action("CobrancaListar", "FiscalizacaoCobranca") %>',
					notificacao: '<%= Url.Action("NotificacaoVisualizar", "FiscalizacaoNotificacao") %>',

					editarAutuadoPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					associarAutuado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					associarFiscalizacao: '<%= Url.Action("Associar", "Fiscalizacao") %>',
					visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
					fiscalizacaoPessoaModal: '<%= Url.Action("FiscalizacaoPessoaModal", "FiscalizacaoCobranca") %>',
					obterFiscalizacao: '<%= Url.Action("ObterFiscalizacao", "FiscalizacaoCobranca") %>',
					recalcular: '<%= Url.Action("CobrancaRecalcular", "FiscalizacaoCobranca") %>'
				}
			});
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <h1 class="titTela">Visualizar Cobrança</h1>
        <br />

        <input type="hidden" class="hdnOrigem" value="<%= Request.QueryString["origem"] %>" />
        <div class="cobrancaPartial">
            <% Html.RenderPartial("CobrancaPartial", Model); %>
        </div>
    </div>
</asp:Content>
