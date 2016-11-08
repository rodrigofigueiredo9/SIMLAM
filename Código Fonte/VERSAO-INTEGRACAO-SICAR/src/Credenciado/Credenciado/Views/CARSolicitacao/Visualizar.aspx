<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CARSolicitacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Solicitação de Inscrição no CAR/ES</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CARSolicitacao/solicitacao.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	
    <script type="text/javascript">
        $(function () {
            Solicitacao.load($('#central'), {
                urls: {
                    salvar: '<%= Url.Action("Criar", "CARSolicitacao") %>',
				    associarProjetoDigital: '<%= Url.Action("Associar", "ProjetoDigital") %>',
				    obterProjetoDigital: '<%= Url.Action("ObterProjetoDigital", "CARSolicitacao") %>',
                    obterAtividades: '<%= Url.Action("ObterAtividades", "CARSolicitacao") %>',
                    visualizarRequerimento: '<%= Url.Action("Visualizar", "Requerimento") %>'
				}
			});
        });
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Solicitação de Inscrição no CAR/ES</h1>
		<br />

		<% if (Model.IsInterno) { %>
			<% Html.RenderPartial("SolicitacaoPartialInterno", Model); %>
		<% } else { %>
			<% Html.RenderPartial("SolicitacaoPartial", Model); %>
		<% } %>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
			</div>
		</div>
	</div>
</asp:Content>