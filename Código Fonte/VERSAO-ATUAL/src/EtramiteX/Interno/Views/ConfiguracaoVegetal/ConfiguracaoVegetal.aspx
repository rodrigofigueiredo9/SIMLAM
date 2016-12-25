<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoVegetalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server"><%= Model.Titulo %></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/configuracaoVegetal.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
			ConfiguracaoVegetal.load($('#central'), {
				urls: {
					salvar: '<%= Model.UrlSalvar %>',
					obter: '<%= Model.UrlEditar %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela"><%= Model.Titulo %></h1>
		<br />

		<%Html.RenderPartial("ConfiguracaoVegetalPartial", Model);%>

        <div class="block box botoesSalvarCancelar">
            <button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<%if (!string.IsNullOrEmpty(Model.UrlCancelar)){%>
			<span class="cancelarCaixa cancelarCaixaPrincipal"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Model.UrlCancelar %>">Cancelar</a></span>
			<%} %>
        </div>

        <% if(Model.MostrarGrid) { %>
		<div class="gridContainer">
			<%Html.RenderPartial("GridConfiguracaoVegetal", Model);%>
		</div>
		<% } %>
    </div>
</asp:Content>