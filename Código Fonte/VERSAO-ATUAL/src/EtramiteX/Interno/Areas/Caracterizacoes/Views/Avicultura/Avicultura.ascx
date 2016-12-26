<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AviculturaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.Avicultura%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	Avicultura.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Avicultura.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	Avicultura.settings.textoMerge = '<%= Model.TextoMerge %>';
	Avicultura.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna70">
			<label for="Avicultura_Atividade">Atividade *</label>
			<%= Html.DropDownList("Avicultura.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("ConfinamentosPartial", Model); %>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

