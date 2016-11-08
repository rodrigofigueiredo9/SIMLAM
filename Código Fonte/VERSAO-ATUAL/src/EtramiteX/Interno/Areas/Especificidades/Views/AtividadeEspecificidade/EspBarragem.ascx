<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.AtividadeEspecificidade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EspBarragemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="contextBarragem">
	<div class="block">
		<div class="coluna75">
			<label for="BarragemId">Barragem da Licença *</label>
			<%= Html.DropDownList("BarragemId", Model.Barragens, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlBarragens", @id = "ddlBarragens" }))%>
		</div>
	</div>
</div>