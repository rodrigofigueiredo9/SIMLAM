<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotivoInvalidacaoVM>" %>

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
			<label>Motivo da invalidação do número*</label>
			<%=Html.TextArea("Motivo", Model.Motivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@class="txtMotivo text", @maxlength="500"})) %>
		</div>
	</div>
</div>