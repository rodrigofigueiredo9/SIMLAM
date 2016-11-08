<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoSalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Visualizar configuração
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central" class="configFiscalizacao">		

		
		<h1 class="titTela">Visualizar configuração</h1><br />

		<div class="configFiscalizacaoPartial">
			<% Html.RenderPartial("ConfiguracaoPartial", Model); %>
		</div>
		<div class="block box btnContainer">			
			<span><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("ConfiguracaoIndex", "Fiscalizacao") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>


