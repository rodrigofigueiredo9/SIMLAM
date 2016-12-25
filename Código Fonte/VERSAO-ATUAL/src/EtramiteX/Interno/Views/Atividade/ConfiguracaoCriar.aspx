<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AtividadeConfiguracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Atividade Solicitada</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Atividade/atividadeConfiguracao.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>

	<script>
		$(function () {
			AtividadeConfiguracao.urlObterAtividade = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
			AtividadeConfiguracao.urlValidarAtividadeConfigurada = '<%= Url.Action("ValidarAtividadeConfigurada", "Atividade") %>';
			AtividadeConfiguracao.urlConfiguracaoSalvar = '<%= Url.Action("ConfiguracaoCriar", "Atividade") %>';
			AtividadeConfiguracao.Mensagens = <%= Model.Mensagens %>;
			AtividadeConfiguracao.load($('#central'));
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Atividade Solicitada</h1>
		<br />

		<% Html.RenderPartial("ConfiguracaoPartial"); %>

		<div class="block box">
			<input id="salvar" type="button" value="Salvar" class="floatLeft btnSalvar" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("")  %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>