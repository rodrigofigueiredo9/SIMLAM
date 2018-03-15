<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<NotificacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Notificações/Financeiro</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/Fiscalizacao/notificacao.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>
    <script type="text/javascript">
		$(function () {
			Notificacao.load($('#central'), {
				urls: {
					editar: '<%= Url.Action("Notificacao", "Fiscalizacao") %>',
					cobranca: '<%= Url.Action("Cobranca", "Fiscalizacao") %>',
					cobrancaVisualizar: '<%= Url.Action("CobrancaVisualizar", "Fiscalizacao") %>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
		{%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls: {
					urlListar: '<%= Url.Action("Notificacao", "Fiscalizacao", new {id = Request.Params["fiscalizacaoId"].ToString() }) %>'
				}
			});
			<%}%>
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">

        <h1 class="titTela">Visualizar Notificações/Financeiro</h1>
        <br />

        <% Html.RenderPartial("NotificacaoPartial", Model); %>

        <div class="block box">
            <div class="coluna10 append2">
				<% if (Model.UltimoParcelamento.DUAS.Count == 0 || Model.UltimoParcelamento.DUAS.Where(x => !x.Situacao.Contains("Cancelado")).Count() == 0) {%>
					<input class="floatLeft btnEditar" type="button" value="Editar Notificação" />
				<%} %>
			</div>
			<% if (Model.UltimoParcelamento.Id == 0) { %>
				<input class="floatLeft btnCadastrarCobranca" type="button" value="Cadastrar Cobrança" />
			<%}	else { %>
				<input class="floatLeft btnVisualizarCobranca" type="button" value="Visualizar Cobrança" />
			<%} %>
            <span class="floatRight spnCancelarCadastro"><a class="linkCancelar" href="<%= Url.Action("Index","Fiscalizacao") %>" title="Cancelar">Cancelar</a></span>
    </div>
    </div>
</asp:Content>
