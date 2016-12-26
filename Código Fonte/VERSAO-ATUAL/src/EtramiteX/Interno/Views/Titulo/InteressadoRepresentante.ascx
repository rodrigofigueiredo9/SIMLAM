<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InteressadoRepresentanteVM>" %>

<script src="<%= Url.Content("~/Scripts/Titulo/interessadoRepresentante.js") %>"></script>

<div class="tituloInteressadoRepresentanteContainer block <%= (Model.Representantes.Count > 0 ? "" : "hide") %>">
	<input type="hidden" class="hdnInteressadoRepresentanteId" value="<%: Model.InteressadoId %>" />
	<input type="hidden" class="hdnInteressadoRepresentanteIdRelacionamento" value="<%: Model.IdRelacionamento %>" />
	<div class="coluna63">
		<label for="RepresentanteInteressado">Representante do interessado PJ *</label>
		<%= Html.DropDownList("RepresentanteInteressado", Model.Representantes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlRepresentantesInteressado" }))%>
	</div>
</div>
