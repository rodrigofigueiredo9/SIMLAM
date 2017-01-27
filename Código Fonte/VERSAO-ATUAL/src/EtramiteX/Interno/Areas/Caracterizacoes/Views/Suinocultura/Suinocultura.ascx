<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SuinoculturaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.Suinocultura%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	Suinocultura.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Suinocultura.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	Suinocultura.settings.textoMerge = '<%= Model.TextoMerge %>';
	Suinocultura.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna75">
			<label for="Suinocultura_Atividade">Atividade *</label>
			<%= Html.DropDownList("Suinocultura.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna25 append2">
			<label for="Suinocultura_Fase">Fase *</label>
			<%= Html.DropDownList("Suinocultura.Fase", Model.Fases, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFases" }))%>
		</div>

		<div class="coluna26 append1">
			<label for="Suinocultura_ExisteBiodigestor">Existe utilização de biodigestor? *</label><br />
			<%if (Model.IsEditar || Model.IsVisualizar){%>
				<label><%= Html.RadioButton("Suinocultura.ExisteBiodigestor", 0, (Model.Caracterizacao.ExisteBiodigestor == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteBiodigestor"}))%>Não</label>
				<label><%= Html.RadioButton("Suinocultura.ExisteBiodigestor", 1, (Model.Caracterizacao.ExisteBiodigestor == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteBiodigestor" }))%>Sim</label>
			<%} else{%>
				<label><%= Html.RadioButton("Suinocultura.ExisteBiodigestor", 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteBiodigestor", @name = "rdbExisteBiodigestor" }))%>Não</label>
				<label><%= Html.RadioButton("Suinocultura.ExisteBiodigestor", 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteBiodigestor", @name = "rdbExisteBiodigestor" }))%>Sim</label>
			<%} %>
		</div>

		<div class="coluna21">
			<label for="Suinocultura_PossuiFabricaRacao">Possui fábrica de ração? *</label><br />
			<%if (Model.IsEditar || Model.IsVisualizar){%>
				<label><%= Html.RadioButton("Suinocultura.PossuiFabricaRacao", 0, (Model.Caracterizacao.PossuiFabricaRacao == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPossuiFabricaRacao" }))%>Não</label>
				<label><%= Html.RadioButton("Suinocultura.PossuiFabricaRacao", 1, (Model.Caracterizacao.PossuiFabricaRacao == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPossuiFabricaRacao" }))%>Sim</label>
			<%} else{%>
				<label><%= Html.RadioButton("Suinocultura.PossuiFabricaRacao", 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPossuiFabricaRacao"}))%>Não</label>
				<label><%= Html.RadioButton("Suinocultura.PossuiFabricaRacao", 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPossuiFabricaRacao"}))%>Sim</label>
			<%} %>
		</div>
	</div>

	<div class="block">
		<div class="coluna25 <%:(Model.IsVisualizar && Model.MostrarNumeroCabecas) ? "" : "hide"%> divNumeroCabecas">
			<label for="Suinocultura_NumeroCabecas">Número máximo de cabeças *</label>
			<%= Html.TextBox("Suinocultura.NumeroCabecas", Model.Caracterizacao.NumeroCabecas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroCabecas maskInteger", @maxlength = "9" }))%>
		</div>

		<div class="coluna25 <%:(Model.IsVisualizar && Model.MostrarNumeroMatrizes) ? "" : "hide"%> divNumeroMatrizes">
			<label for="Suinocultura_NumeroMatrizes">Número máximo de matrizes *</label>
			<%= Html.TextBox("Suinocultura.NumeroMatrizes", Model.Caracterizacao.NumeroMatrizes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroMatrizes maskInteger", @maxlength = "9" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

