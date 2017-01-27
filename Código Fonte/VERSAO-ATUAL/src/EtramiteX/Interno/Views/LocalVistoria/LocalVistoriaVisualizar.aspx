<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<LocalVistoriaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Local de Vistoria</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/LocalVistoria/localVistoria.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
		    LocalVistoria.load($('#central'), {
		        Mensagens: <%= Model.Mensagens %>,
		    });
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Local de Vistoria</h1>
		<br />

		<%Html.RenderPartial("LocalVistoriaPartial", Model);%>

        <div class="block box botoesSalvarCancelar">

			<span class="cancelarCaixa cancelarCaixaPrincipal"><a class="linkCancelar" href="<%= Url.Action("LocalVistoriaListar", "LocalVistoria") %>">Cancelar</a></span>
        </div>

    </div>
</asp:Content>