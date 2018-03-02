<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<NotificacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Notificações / Financeiro</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/notificacao.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>
    <script>
		$(function () {
			Notificacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("NotificacaoCriar", "Fiscalizacao") %>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
		{%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls: {
					urlEditar: '<%= Url.Action("NotificacaoEditar", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
					urlListar: '<%= Url.Action("Notificacao", "Fiscalizacao", new {id = Request.Params["fiscalizacaoId"].ToString() }) %>'
				}
			});
			<%}%>
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">

        <h1 class="titTela">Notificações / Financeiro</h1>
        <br />

        <% Html.RenderPartial("NotificacaoPartial", Model); %>

        <div class="block box">
            <input class="floatLeft btnSalvar" type="button" value="Salvar" />
            <span class="floatRight spnCancelarCadastro"><a class="linkCancelar" href="<%= Url.Action("Index","Fiscalizacao") %>" title="Cancelar">Cancelar</a></span>
        </div>
    </div>
</asp:Content>
