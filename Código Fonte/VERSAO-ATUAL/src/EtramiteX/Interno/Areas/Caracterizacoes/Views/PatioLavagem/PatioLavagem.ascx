<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PatioLavagemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.PatioLavagem%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	PatioLavagem.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	PatioLavagem.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	PatioLavagem.settings.textoMerge = '<%= Model.TextoMerge %>';
	PatioLavagem.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna70">
			<label for="PatioLavagem_Atividade">Atividade *</label>
			<%= Html.DropDownList("PatioLavagem.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna13">
			<label for="PatioLavagem_AreaTotal">Área total (m²) *</label>
			<%= Html.TextBox("PatioLavagem.AreaTotal", Model.Caracterizacao.AreaTotal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaTotal maskDecimalPonto", @maxlength="14"}))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

