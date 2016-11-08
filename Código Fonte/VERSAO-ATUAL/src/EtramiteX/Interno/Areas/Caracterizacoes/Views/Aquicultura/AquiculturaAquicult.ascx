<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AquiculturaAquicultVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnIdentificador" value="<%=Model.Caracterizacao.Identificador%>" />

<fieldset id="fsAquicultura<%:Model.Caracterizacao.Identificador%>" class="box fsAquicultura">
	<p class="block">
		<%if (!Model.IsVisualizar) { %>
			<a class="btnAsmExcluir btnExluirAtividade" title="Excluir Aquicultura">Excluir</a>
		<%} %>
	</p>

	<input type="hidden" class="hdnAquiculturaId" value="<%:Model.Caracterizacao.Id %>" />
	<input type="hidden" class="hdnIdentificador" value="<%:Model.Caracterizacao.Identificador %>" />

	<fieldset class="boxBranca">
		<div class="block">
			<div class="coluna80">
				<label for="Aquicultura_Atividade<%:Model.Caracterizacao.Identificador%>">Atividade *</label>
				<%= Html.DropDownList("Aquicultura.Atividade"+Model.Caracterizacao.Identificador, Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAtividade " }))%>
			</div>
		</div>

		<div class="block divGrupo1 <%:(Model.IsVisualizar && Model.MostrarGrupo01) ? "" : "hide"%>">
			<div class="coluna22 append2">
				<label for="Aquicultura_AreaInundadaTotal<%:Model.Caracterizacao.Identificador%>">Área total inundada (ha) *</label>
				<%= Html.TextBox("Aquicultura.AreaInundadaTotal" + Model.Caracterizacao.Identificador, Model.Caracterizacao.AreaInundadaTotal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaInundadaTotal maskDecimal", @maxlength = "8" }))%>
			</div>

			<div class="coluna25">
				<label for="Aquicultura_NumViveiros<%:Model.Caracterizacao.Identificador%>">Nº de viveiros escavados *</label>
				<%= Html.TextBox("Aquicultura.NumViveiros" + Model.Caracterizacao.Identificador, Model.Caracterizacao.NumViveiros, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumViveiros maskNum3", @maxlength = "3" }))%>
			</div>
		</div>
	</fieldset>

	<div class="block divGrupo2 <%:(Model.IsVisualizar && Model.MostrarGrupo02) ? "" : "hide"%>">
		<fieldset class="boxBranca">
			<div class="block">
				<div class="coluna22">
					<label for="Aquicultura_NumUnidadeCultivos<%:Model.Caracterizacao.Identificador%>">Nº de unidade de cultivo *</label>
					<%= Html.TextBox("Aquicultura.NumUnidadeCultivos" + Model.Caracterizacao.Identificador, Model.Caracterizacao.NumUnidadeCultivos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumUnidadeCultivos maskNum3", @maxlength = "3" }))%>
				</div>
			</div>

			<%Html.RenderPartial("Cultivo", Model);%>
		</fieldset>
	</div>

	<fieldset class="boxBranca divGrupo3 <%:(Model.IsVisualizar && Model.MostrarGrupo03) ? "" : "hide"%>">
		<div class="block">
			<div class="coluna20">
				<label for="Aquicultura_AreaCultivo<%:Model.Caracterizacao.Identificador%>">Área de cultivo (m²) *</label>
				<%= Html.TextBox("Aquicultura.AreaCultivo" + Model.Caracterizacao.Identificador, Model.Caracterizacao.AreaCultivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCultivo maskDecimal", @maxlength = "10" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="boxBranca">
		<legend class="titFiltros">Coordenada da atividade para a licença</legend>

		<div class="block">
			<div class="coluna30 append2">
				<label for="CoordenadaAtividade_Tipo<%:Model.Caracterizacao.Identificador%>">Tipo geométrico *</label>
				<%= Html.DropDownList("CoordenadaAtividade.Tipo" + Model.Caracterizacao.Identificador, Model.CoodernadaAtividade.TipoGeometrico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaTipoGeometria " }))%>
			</div>

			<div class="coluna40">
				<label for="CoordenadaAtividade_CoordenadaAtividade<%:Model.Caracterizacao.Identificador%>">Coordenada da atividade *</label>
				<%= Html.DropDownList("CoordenadaAtividade.CoordenadaAtividade" + Model.Caracterizacao.Identificador, Model.CoodernadaAtividade.CoordenadasAtividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaAtividade " }))%>
			</div>
		</div>
	</fieldset>
</fieldset>