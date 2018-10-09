<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Autorizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AutorizacaoExploracaoFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Autorizacao/AutorizacaoExploracaoFlorestal.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Titulo/tituloAutorizacaoExploracaoFlorestal.js") %>" ></script>
<script>
	AutorizacaoExploracaoFlorestal.settings.urls.obterDadosAutorizacaoExploracaoFlorestal = '<%= Url.Action("ObterDadosAutorizacaoDestinatarios", "Autorizacao", new {area="Especificidades"}) %>';
	AutorizacaoExploracaoFlorestal.settings.urls.obterDadosExploracao = '<%= Url.Action("ObterDadosExploracao", "Autorizacao", new {area="Especificidades"}) %>';
	AutorizacaoExploracaoFlorestal.settings.urls.obterLaudoVistoria = '<%= Url.Action("Associar","Titulo", new {area=""}) %>';
	AutorizacaoExploracaoFlorestal.settings.urls.validarAssociarVistoria = '<%= Url.Action("ValidarAssociarLaudoVistoria","Autorizacao", new {area="Especificidades"}) %>';
	AutorizacaoExploracaoFlorestal.settings.modelos.LaudoVistoriaFlorestal = <%: (int)eEspecificidade.LaudoVistoriaFlorestal %>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label>Laudo de Vistoria Florestal *</label>
			<input type="hidden" name="Autorizacao.LaudoVistoriaFlorestalIdRelacionamento" class="hdnLaudoVistoriaFlorestalIdRelacionamento" value="<%= Model.TituloAssociado.IdRelacionamento %>" />
			<input type="hidden" name="Autorizacao.LaudoVistoriaFlorestal" class="hdnLaudoVistoriaFlorestalId" value="<%= Model.TituloAssociado.Id %>" />
			<%= Html.TextBox("Autorizacao.LaudoVistoriaFlorestalTexto", Model.LaudoVistoriaTextoTela, new { @class = "text txtLaudoVistoriaFlorestal disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna20">
			<button class="inlineBotao <%: Model.TituloAssociado.Id > 0 || Model.IsVisualizar ? "hide" : "" %> btnVistoriaAdicionar">Buscar</button>
			<button class="inlineBotao <%: Model.TituloAssociado.Id > 0 && !Model.IsVisualizar ? "" : "hide" %> btnVistoriaLimpar">Limpar</button>
		</div>
	</div>

	<% Html.RenderPartial("~/Views/Titulo/TituloAutorizacaoExploracaoFlorestal.ascx", Model); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label>Observações *</label>
			<%= Html.TextArea("ExploracaoFlorestal.Observacoes", Model.Autorizacao.Observacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "500", @class = "text media txtObservacoes" }))%>
		</div>
	</div>
</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<legend>Arquivo</legend>
	<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>