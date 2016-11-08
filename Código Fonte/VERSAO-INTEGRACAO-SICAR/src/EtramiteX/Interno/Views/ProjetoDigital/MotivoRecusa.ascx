<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotivoRecusaVM>" %>

<div class="motivoContainer">
	<%=Html.Hidden("ProjetoDigitalId", Model.ProjetoId, new{ @class="hdnProjetoDigitalId"}) %>
	<h1 class="titTela">Recusa da importação do Requerimento</h1>
	<br />
	<div class="block box">
		<div class="coluna99">
			<label>Motivo *</label>
			<%=Html.TextArea("MotivoRecusa", Model.Motivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{@class="txtMotivoRecusa media", @maxlenght="3000"})) %>
		</div>
	</div>
	
</div>