<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProducaoCarvaoVegetalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.ProducaoCarvaoVegetal%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	ProducaoCarvaoVegetal.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	ProducaoCarvaoVegetal.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	ProducaoCarvaoVegetal.settings.textoMerge = '<%= Model.TextoMerge %>';
	ProducaoCarvaoVegetal.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna80">
			<label for="ProducaoCarvaoVegetal_Atividade">Atividade *</label>
			<%= Html.DropDownList("ProducaoCarvaoVegetal.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna14">
			<label for="ProducaoCarvaoVegetal_NumeroFornos">Nº de fornos *</label>
			<%= Html.TextBox("ProducaoCarvaoVegetal.NumeroFornos", Model.Caracterizacao.NumeroFornos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumFornos maskNum3", @maxlength = "4" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("Fornos", Model); %>

<% Html.RenderPartial("MateriaPrima", Model.MateriaPrimaFlorestalConsumida); %>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>
