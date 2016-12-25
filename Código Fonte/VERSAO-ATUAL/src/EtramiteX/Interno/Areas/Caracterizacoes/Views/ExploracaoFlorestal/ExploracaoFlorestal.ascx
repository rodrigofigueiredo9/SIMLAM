<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ExploracaoFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	ExploracaoFlorestal.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	ExploracaoFlorestal.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	ExploracaoFlorestal.settings.textoMerge = '<%= Model.TextoMerge %>';
	ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';	
	ExploracaoFlorestal.settings.mensagens = <%=Model.Mensagens%>;
</script>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<fieldset class="block box" id="exploracaoFlorestalFinalidade">
	<div class="block">
		<label for="FinalidadeExploracao">Finalidade da Exploração *</label>
	</div>
	<div class="block">
		<% 
		foreach (var finalidade in Model.Finalidades){
			bool selecionado = ((Model.Caracterizacao.FinalidadeExploracao != null) && (Model.Caracterizacao.FinalidadeExploracao.Value & finalidade.Codigo) > 0); %>
			<label class="coluna32 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
				<input class="checkboxFinalidadeExploracao <%= Model.IsVisualizar ? "disabled" : "" %> checkbox<%= finalidade.Texto %>" type="checkbox" title="<%= finalidade.Texto %>" value="<%= finalidade.Codigo %>" <%= selecionado ? "checked=\"checked\"" : "" %> <%= Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
				<%= finalidade.Texto%>
			</label>
		<% } %>
	</div>
	<br />
	<div class="block">
		<div class="coluna30 divEspecificarFinalidade hide">
			<label for="FinalidadeEspecificar">Especificar *</label>
			<%= Html.TextBox("FinalidadeEspecificar", Model.Caracterizacao.FinalidadeEspecificar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFinalidadeEspecificar ", @maxlength = "50" }))%>
		</div>
	</div>
</fieldset>


<%foreach (var item in Model.ExploracaoFlorestalExploracaoVM){ %>
<fieldset class="block box exploracoesFlorestais" id="exploracao<%: item.ExploracaoFlorestal.Identificacao%>">
	<legend class="titFiltros">Exploração Florestal</legend>
	<%Html.RenderPartial("ExploracaoFlorestalExploracao", item); %>
</fieldset>
<%} %>