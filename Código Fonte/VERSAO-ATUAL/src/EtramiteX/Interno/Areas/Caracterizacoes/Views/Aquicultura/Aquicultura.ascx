<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AquiculturaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.Aquicultura%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />
<input type="hidden" class="hdnQtdAtividade" value="<%: Model.QtdAtividade %>" />

<script>
	Aquicultura.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Aquicultura.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	Aquicultura.settings.textoMerge = '<%= Model.TextoMerge %>';
	AquiculturaAquicult.settings.idsTela =  <%=Model.IdsTela %>;
	AquiculturaAquicult.settings.mensagens = <%= Model.Mensagens%>;
	Aquicultura.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	CoordenadaAtividade.settings.mensagens = <%= Model.Mensagens%>;
	CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "Aquicultura") %>';
	CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "Aquicultura") %>';
</script>


<div class="containerAquicultura">
	<%if(Model.AquiculturaAquicultVM.Count > 0) {
		  foreach (AquiculturaAquicultVM vm in Model.AquiculturaAquicultVM)
		  {%>
			<%Html.RenderPartial("AquiculturaAquicult", vm);%>
		<%}
	}else{ %>
			<%Html.RenderPartial("AquiculturaAquicult", Model.AquiculturaAquicultTemplateVM);%>
	<%} %>
</div>

<%if(!Model.IsVisualizar) {%>
<div class="block divAdicionarAtividade">
	<div class="coluna10 direita">
		<button type="button" style="width:100px;" class="inlineBotao btnAdicionarAtividade btnAddItem" title="Adicionar">+ Atividade</button>
	</div>
</div>
<%} %>