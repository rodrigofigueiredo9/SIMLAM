<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaPPFFVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.SilviculturaPPFF%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<fieldset class="box">
	<div class="block">
		<div class="coluna70">
			<label>Atividade *</label>
			<%= Html.DropDownList("Silvicultura.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<div class="block">
		<div class="coluna35">
			<label>Tipo de fomento *</label><br />
			<label class="append2"><%= Html.RadioButton("Silvicultura.FomentoTipo", (int)eFomentoTipo.FomentoFibriaDois, (Model.Caracterizacao.FomentoTipo == eFomentoTipo.FomentoFibriaDois), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rboFomentoTipo " }))%>Fomento Fibria 2</label>
			<label class="append2"><%= Html.RadioButton("Silvicultura.FomentoTipo", (int)eFomentoTipo.FomentoFibriaUm, (Model.Caracterizacao.FomentoTipo == eFomentoTipo.FomentoFibriaUm), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rboFomentoTipo " }))%>Fomento Fibria 1</label>
			<label ><%= Html.RadioButton("Silvicultura.FomentoTipo", (int)eFomentoTipo.FomentoSuzano, (Model.Caracterizacao.FomentoTipo == eFomentoTipo.FomentoSuzano), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rboFomentoTipo " }))%>Fomento Suzano</label>
		</div>
		<div class="coluna15">
			<label>Área total (ha)*</label>
			<%= Html.TextBox("Silvicultura.AreaTotal", Model.Caracterizacao.AreaTotal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaTotal maskDecimal", @maxlength = "10" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("MunicipiosPartial", Model); %>

