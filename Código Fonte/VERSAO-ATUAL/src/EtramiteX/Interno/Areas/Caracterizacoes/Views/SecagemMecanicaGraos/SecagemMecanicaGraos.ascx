<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SecagemMecanicaGraosVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.SecagemMecanicaGraos%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	SecagemMecanicaGraos.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	SecagemMecanicaGraos.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	SecagemMecanicaGraos.settings.textoMerge = '<%= Model.TextoMerge %>';
	SecagemMecanicaGraos.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna70">
			<label for="SecagemMecanicaGraos_Atividade">Atividade *</label>
			<%= Html.DropDownList("SecagemMecanicaGraos.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna15">
			<label for="SecagemMecanicaGraos_numSecadores">Nº de secadores *</label>
			<%= Html.TextBox("SecagemMecanicaGraos.numSecadores", Model.Caracterizacao.NumeroSecadores, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumSecadores maskNum3", @maxlength = "3" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("Secadores", Model); %>

<% Html.RenderPartial("MateriaPrima", Model.MateriaPrimaFlorestalConsumida); %>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

