<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DespolpamentoCafeVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.DespolpamentoCafe%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	DespolpamentoCafe.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	DespolpamentoCafe.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	DespolpamentoCafe.settings.textoMerge = '<%= Model.TextoMerge %>';
	DespolpamentoCafe.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna80">
			<label for="DespolpamentoCafe_Atividade">Atividade *</label>
			<%= Html.DropDownList("DespolpamentoCafe.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna29 append2">
			<label for="DespolpamentoCafe_CapacidadeTotalInstalada">Capacidade instalada total (L/hora) *</label>
			<%= Html.TextBox("DespolpamentoCafe.CapacidadeTotalInstalada", Model.Caracterizacao.CapacidadeTotalInstalada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCapacidadeTotalInstalada maskDecimal", @maxlength = "9" }))%>
		</div>

		<div class="coluna25">
			<label for="DespolpamentoCafe_AguaResiduariaCafe">Água residuária de café (L/dia)</label>
			<%= Html.TextBox("DespolpamentoCafe.AguaResiduariaCafe", Model.Caracterizacao.AguaResiduariaCafe, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAguaResiduariaCafe maskDecimal", @maxlength = "9" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

