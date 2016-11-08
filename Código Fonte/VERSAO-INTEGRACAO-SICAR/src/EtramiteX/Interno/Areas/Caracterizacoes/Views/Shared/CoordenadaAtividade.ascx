<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CoordenadaAtividadeVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<fieldset class="box">
	<legend class="titFiltros">Coordenada da atividade para a licença</legend>

	<div class="block">
		<div class="coluna30 append2">
			<label for="CoordenadaAtividade_Tipo">Tipo geométrico *</label>
			<%= Html.DropDownList("CoordenadaAtividade.Tipo", Model.TipoGeometrico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaTipoGeometria " }))%>
		</div>

		<div class="coluna40">
			<label for="CoordenadaAtividade_CoordenadaAtividade">Coordenada da atividade *</label>
			<%= Html.DropDownList("CoordenadaAtividade.CoordenadaAtividade", Model.CoordenadasAtividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaAtividade " }))%>
		</div>
	</div>
</fieldset>