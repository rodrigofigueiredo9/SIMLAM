<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CredenciadoVM>" %>

<fieldset class="block box">
	<legend>Parceiro/ Convênio</legend>
	<div class="block">
		<div class="coluna98">
			<label for="Pessao.Cidade">Nome do órgão parceiro/ conveniado *</label>
			<%= Html.DropDownList("Credenciado.OrgaoParceiroId", Model.OrgaosParceiros, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlOrgaoParceiro disabled"}))%> 
		</div>
	</div>

	<div class="block">
		<div class="coluna98">
			<label for="Pessao.Cidade">Unidade *</label>
			<%= Html.DropDownList("Credenciado.OrgaoParceiroUnidadeId", Model.OrgaosParceirosUnidades, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlOrgaoParceiroUnidade disabled"}))%> 
		</div>
	</div>
</fieldset>