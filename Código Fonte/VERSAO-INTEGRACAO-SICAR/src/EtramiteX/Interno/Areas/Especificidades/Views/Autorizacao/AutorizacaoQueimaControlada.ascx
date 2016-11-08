<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Autorizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AutorizacaoQueimaControladaVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Autorizacao/autorizacaoQueimaControlada.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>
<script type="text/javascript">
	AutorizacaoQueimaControlada.settings.urls.obterDadosAutorizacaoQueimaControlada = '<%= Url.Action("ObterDadosAutorizacaoDestinatarios", "Autorizacao", new {area="Especificidades"}) %>';
	AutorizacaoQueimaControlada.settings.urls.obterLaudoVistoria = '<%= Url.Action("Associar","Titulo", new {area=""}) %>';
	AutorizacaoQueimaControlada.settings.urls.validarAssociarVistoria = '<%= Url.Action("ValidarAssociarLaudoVistoria","Autorizacao", new {area="Especificidades"}) %>';
	AutorizacaoQueimaControlada.settings.modelos.LaudoVistoriaFlorestal = <%: (int)eEspecificidade.LaudoVistoriaFlorestal %>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna31">
			<label>Laudo de Vistoria Florestal *</label>
			<input type="hidden" name="Autorizacao.LaudoVistoriaFlorestalIdRelacionamento" class="hdnLaudoVistoriaFlorestalIdRelacionamento" value="<%= Model.TituloAssociado.IdRelacionamento %>" />
			<input type="hidden" name="Autorizacao.LaudoVistoriaFlorestal" class="hdnLaudoVistoriaFlorestalId" value="<%= Model.TituloAssociado.Id %>" />
			<%= Html.TextBox("Autorizacao.LaudoVistoriaFlorestalTexto", Model.LaudoVistoriaTextoTela, new { @class = "text txtLaudoVistoriaFlorestal disabled", @disabled = "disabled" })%>
		</div>
		<%if(!Model.IsVisualizar) {%>
		<div class="coluna20">
			<button class="inlineBotao <%: Model.TituloAssociado.Id > 0 ? "hide" : "" %> btnVistoriaAdicionar">Buscar</button>
			<button class="inlineBotao <%: Model.TituloAssociado.Id > 0 ? "" : "hide" %> btnVistoriaLimpar">Limpar</button>
		</div>
		<%} %>
	</div>

	<div class="block">
		<div class="coluna70">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Autorizacao.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text  ddlDestinatarios" }))%>
		</div>
	</div>
</fieldset>