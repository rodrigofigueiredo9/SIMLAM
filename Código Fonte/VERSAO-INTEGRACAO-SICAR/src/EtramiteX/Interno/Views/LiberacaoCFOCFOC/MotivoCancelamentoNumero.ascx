<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotivoCancelamentoVM>" %>

<h1 class="titTela">Motivo</h1>
<br />

<div class="block box">
	<div class="block">
		<div class="coluna40">
			<label>Número</label>
			<label class="lblTipoDocumento"><%=Model.TipoDocumento%></label> : 
			<label class="lblNumero"><%=Model.Numero %></label>
		</div>
	</div>
	<div class="block">
		<div class="coluna99 ultima">
			<label>Motivo do cancelamento do número *</label>
			<%=Html.TextArea("Motivo", Model.Motivo, Tecnomapas.EtramiteX.Interno.ViewModels.ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@class="txtMotivo text", @maxlength="500"})) %>
		</div>
	</div>
</div>