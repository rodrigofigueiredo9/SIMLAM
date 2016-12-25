<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CARSolicitacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Solicitação de Inscrição no CAR/ES</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Protocolo/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/CARSolicitacao/solicitacao.js") %>"></script>
	<script>
		$(function () {
			Solicitacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "CARSolicitacao") %>',
					associarProtocolo: '<%= Url.Action("Associar", "Protocolo") %>',
					visualizarProtocolo: '<%= Url.Action("Visualizar", "Protocolo") %>',
					obterProcessosDocumentos: '<%= Url.Action("ObterProcessosDocumentos", "CARSolicitacao") %>',
					obterAtividades: '<%= Url.Action("ObterAtividades", "CARSolicitacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Visualizar Solicitação de Inscrição no CAR/ES</h1>
		<br />

		<% if (Model.IsCredenciado) { %>
			<% Html.RenderPartial("SolicitacaoPartialCredenciado", Model); %>
		<% } else { %>
			<%Html.RenderPartial("SolicitacaoPartial", Model);%>
		<% } %>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
			</div>
		</div>
	</div>
</asp:Content>