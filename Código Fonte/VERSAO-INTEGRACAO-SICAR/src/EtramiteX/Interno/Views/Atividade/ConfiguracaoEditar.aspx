<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Configuração de Atividade Solicitada</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/atividadeConfiguracao.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			AtividadeConfiguracao.urlObterAtividade = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
			AtividadeConfiguracao.urlValidarAtividadeConfigurada = '<%= Url.Action("ValidarAtividadeConfigurada", "Atividade") %>';
			AtividadeConfiguracao.urlConfiguracaoSalvar = '<%= Url.Action("ConfiguracaoEditar", "Atividade") %>';
			AtividadeConfiguracao.Mensagens = <%= Model.Mensagens %>;
			AtividadeConfiguracao.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Configuração de Atividade Solicitada</h1>
		<br />

		<% Html.RenderPartial("ConfiguracaoPartial"); %>

		<div class="block box">
			<input id="salvar" type="button" value="Salvar" class="floatLeft btnSalvar" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("")  %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>