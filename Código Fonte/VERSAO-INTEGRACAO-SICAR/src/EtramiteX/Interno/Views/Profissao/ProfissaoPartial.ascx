<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProfissaoVM>" %>

<input type="hidden" class="hdnArtefatoId" value="<%= Model.Profissao.Id %>" />
<div class="block box">	
	<div class="block">
		<div class="coluna20">
			<label for="RegistroNumero">Código *</label>
			<%= Html.TextBox("Codigo", (String.IsNullOrEmpty(Model.Profissao.Codigo) ? "Gerado automaticamente" : Model.Profissao.Codigo), ViewModelHelper.SetaDisabled(true, new { @class = "text txtCodigo maskNumInt", @maxlength = "10" }))%>
		</div>
	</div>

	<div class="block">		
		<div class="coluna60">
			<label for="Modelo">Profissão *</label>
			<%= Html.TextBox("Texto", Model.Profissao.Texto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTexto", @maxlength = "80" }))%>
		</div>
	</div>
</div>