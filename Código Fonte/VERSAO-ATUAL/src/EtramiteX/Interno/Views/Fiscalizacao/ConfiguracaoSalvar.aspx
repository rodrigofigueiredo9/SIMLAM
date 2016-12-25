<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoSalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Salvar configuração
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>"></script>

<script>

	$(function () {
		FiscalizacaoConfiguracao.load($('#central'), {
			urls: {
				criar: '<%= Url.Action("ConfiguracaoCriar", "Fiscalizacao") %>'
			},
			Mensagens: <%= Model.Mensagens %>
		});
	});

</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central" class="configFiscalizacao">


		<h1 class="titTela">Salvar configuração</h1><br />

		<div class="configFiscalizacaoPartial">
			<% Html.RenderPartial("ConfiguracaoPartial", Model); %>
		</div>
		<div class="block box btnContainer">
			<span class="spnSalvar"><input class="btnSalvar floatLeft" type="button" value="Salvar" /></span>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span>
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("ConfiguracaoIndex", "Fiscalizacao") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>


