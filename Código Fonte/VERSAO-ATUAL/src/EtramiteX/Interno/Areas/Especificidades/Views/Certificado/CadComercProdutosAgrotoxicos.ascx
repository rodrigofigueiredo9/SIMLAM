<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CadComercProdutosAgrotoxicosVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>


<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certificado/cadComercProdutosAgrotoxicos.js") %>"></script>
<script>
	CadComercProdutosAgrotoxicos.urlObterDadosCadComercProdutosAgrotoxicos = '<%= Url.Action("ObterDadosCertificadoDestinatarios", "Certificado", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="CadComercProdutosAgrotoxicos_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("CadComercProdutosAgrotoxicos.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>


</fieldset>