<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<fieldset class="block box">
	<legend>Parceiro/ Convênio</legend>
	<div class="block">
		<div class="coluna98">
			<label for="Pessao.Cidade">Nome do órgão parceiro/ conveniado *</label>
			<%= Html.DropDownList("Credenciado.OrgaoParceiroId", Model.OrgaosParceiros, new { @class = "text ddlOrgaoParceiro" })%> 
		</div>
	</div>

	<div class="block">
		<div class="coluna98">
			<label for="Pessao.Cidade">Unidade *</label>
			<%= Html.DropDownList("Credenciado.OrgaoParceiroUnidadeId", Model.OrgaosParceirosUnidades, new { @class = "text ddlOrgaoParceiroUnidade" })%> 
		</div>
	</div>
</fieldset>