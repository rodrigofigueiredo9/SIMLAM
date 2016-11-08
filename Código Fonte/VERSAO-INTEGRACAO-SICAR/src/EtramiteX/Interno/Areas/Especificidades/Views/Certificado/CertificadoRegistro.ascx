<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertificadoRegistroVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certificado/certificadoRegistro.js") %>"></script>
<script type="text/javascript">
	CertificadoRegistro.urlObterDadosCertificadoRegistro = '<%= Url.Action("ObterDadosCertificadoDestinatarios", "Certificado", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />
	
	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Certificado.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Classificacao">Classificação *</label>
			<%= Html.TextBox("Classificacao", Model.Certificado.Classificacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "100", @class = "text txtClassificacao" }))%>
		</div>

		<div class="coluna20 prepend2">
			<label for="Registro">Nº do Registro *</label>
			<%= Html.TextBox("Registro", Model.Certificado.Registro, ViewModelHelper.SetaDisabled(Model.IsVisualizar,  new { @maxlength = "20", @class = "text txtRegistro" }))%>
		</div>
	</div>
</fieldset>