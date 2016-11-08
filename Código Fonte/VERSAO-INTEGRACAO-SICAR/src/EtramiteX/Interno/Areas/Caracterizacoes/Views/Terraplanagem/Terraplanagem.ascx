<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TerraplanagemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.Terraplanagem%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script type="text/javascript">
	Terraplanagem.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Terraplanagem.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	Terraplanagem.settings.textoMerge = '<%= Model.TextoMerge %>';
	Terraplanagem.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna80">
			<label for="Terraplanagem_Atividade">Atividade *</label>
			<%= Html.DropDownList("Terraplanagem.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna20 append2">
			<label for="Terraplanagem_Area">Área terraplanada (m²) *</label>
			<%= Html.TextBox("Terraplanagem.Area", Model.Caracterizacao.Area, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArea maskDecimal", @maxlength = "10" }))%>
		</div>

		<div class="coluna30">
			<label for="Terraplanagem_VolumeMovimentado">Volume de terra movimentado (m³) *</label>
			<%= Html.TextBox("Terraplanagem.VolumeMovimentado", Model.Caracterizacao.VolumeMovimentado, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVolumeMovimentado maskDecimal", @maxlength = "10" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

