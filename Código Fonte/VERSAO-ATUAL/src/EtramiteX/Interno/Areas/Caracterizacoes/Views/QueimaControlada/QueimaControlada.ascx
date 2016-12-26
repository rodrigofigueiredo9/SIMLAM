<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QueimaControladaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<script>
	QueimaControlada.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	QueimaControlada.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	QueimaControlada.settings.textoMerge = '<%= Model.TextoMerge %>';
	QueimaControlada.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<%foreach (var item in Model.QueimaControladaQueimaVM){ %>
<fieldset class="block box" id="queima<%=item.Caracterizacao.Identificacao%>">
	<legend class="titFiltros">Características da queima</legend>
	<%Html.RenderPartial("QueimaControladaQueima", item); %>
</fieldset>
<%} %>