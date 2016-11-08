<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ComplementacaoDadosVM>" %>

<div class="FiscalizacaoComplementacaoDadosContainer">
	<input type="hidden" class="hdnTipoAutuado" value="<%:Model.Entidade.AutuadoTipo %>" />
	<input type="hidden" class="hdnAutuadoId" value="<%:Model.Entidade.AutuadoId %>" />
	<input type="hidden" class="hdnEmpreendimentoId" value="<%:Model.Entidade.EmpreendimentoId %>" />
	<input type="hidden" class="hdnComplementacaoDadosId" value="<%:Model.Entidade.Id %>" />
	<fieldset class="box">
		<legend>Dados do autuado</legend>
		<div class="block">
			<div class="coluna25 append2">
				<label for="Complementacao_ResidePropriedadeTipo">Reside na propriedade? *</label>
				<%= Html.DropDownList("Complementacao.ResidePropriedadeTipo", Model.ResidePropriedadeTipoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar,  new { @class = "text ddlResidePropriedadeTipo" }))%>
			</div>

			<div class="coluna28 append2">
				<label for="Complementacao_RendaMensalFamiliarTipo">Renda mensal familiar informada *</label>
				<%= Html.DropDownList("Complementacao.RendaMensalFamiliarTipo", Model.RendaMensalTipoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlRendaMensalFamiliarTipo" }))%>
			</div>

			<div class="coluna19">
				<label for="Complementacao_NivelEscolaridadeTipo">Nível de escolaridade *</label>
				<%= Html.DropDownList("Complementacao.NivelEscolaridadeTipo", Model.NivelEscolaridadeTipoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlNivelEscolaridadeTipo" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25 append2">
				<label for="Complementacao_VinculoComPropriedadeTipo">Vínculo com a propriedade *</label>
				<%= Html.DropDownList("Complementacao.VinculoComPropriedadeTipo", Model.VinculoPropriedadeTipoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.AutuadoTipo == (int)eTipoAutuado.Empreendimento, new { @class = "text ddlVinculoComPropriedadeTipo" }))%>
			</div>

			<div class="coluna48 divEspecificarVinculoPropriedade <%=Model.Entidade.VinculoComPropriedadeTipo == (int)eVinculoPropriedade.Outro ? "" : "hide" %>">
				<label for="Complementacao_VinculoPropriedadeEspecificarTexto">Especificar *</label>
				<%= Html.TextBox("Complementacao.VinculoComPropriedadeEspecificarTexto", Model.Entidade.VinculoComPropriedadeEspecificarTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.AutuadoTipo == (int)eTipoAutuado.Empreendimento, new { @class = "text txtVinculoComPropriedadeEspecificarTexto", @maxlength = "100" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25">
				<label for="Complementacao_ConhecimentoLegislacaoTipo">Tem conhecimento quanto a legislação? *</label>
				<%= Html.DropDownList("Complementacao.ConhecimentoLegislacaoTipo", Model.ConhecimentoLegislacaoTipoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlConhecimentoLegislacaoTipo" }))%>
			</div>
		</div>

		<div class="block divJustificativa <%=Model.Entidade.ConhecimentoLegislacaoTipo == (int)eRespostasDefault.NaoSeAplica ? "hide" : "" %>">
			<div class="coluna76">
				<label for="Complementacao_Justificativa">Justificativa *</label>
				<%= Html.TextArea("Complementacao.Justificativa", Model.Entidade.Justificativa, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtJustificativa", @maxlength = "500" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="box fsDadosPropriedade <%=Model.Entidade.AutuadoTipo == (int)eTipoAutuado.Empreendimento ? "" : "hide"%>">
		<legend>Dados da propriedade</legend>

		<div class="block">
			<div class="coluna30 append4">
				<label for="Complementacao_AreaTotalInformada">Área total informada (ha)</label>
				<%= Html.TextBox("Complementacao.AreaTotalInformada", Model.Entidade.AreaTotalInformada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaTotalInformada maskArea44", @maxlength = "15" }))%>
			</div>

			<div class="coluna50">
				<label for="Complementacao_AreaCoberturaFlorestalNativa">Área com cobertura florestal nativa informada/ estimada (ha)</label>
				<%= Html.TextBox("Complementacao.AreaCoberturaFlorestalNativa", Model.Entidade.AreaCoberturaFlorestalNativa, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCoberturaFlorestalNativa maskArea44", @maxlength = "15" }))%>
			</div>
		</div>

		<div class="block">
			<label for="Complementacao_ReservalegalTipo">Possui area de reserva legal? *</label>
		</div>
		<div class="block">
			<%foreach (var reserva in Model.ReservaLegalTipoLst){
				bool selecionado = ((Model.Entidade.ReservalegalTipo != null) && (Model.Entidade.ReservalegalTipo.Value & reserva.Codigo) > 0); %>
				<label class="coluna16 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
					<input class="checkboxReservasLegais <%= Model.IsVisualizar ? "disabled" : "" %> checkbox<%= reserva.Texto %> checkbox<%= reserva.Id %>" type="checkbox" title="<%= reserva.Texto %>" value="<%= reserva.Codigo %>" <%= selecionado ? "checked=\"checked\"" : "" %> <%= Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					<%= reserva.Texto%>
				</label>
			<% } %>
		</div>	
	</fieldset>
</div>