<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertidaoAnuenciaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>


<script src="<%= Url.Content("~/Scripts/Titulo/destinatarioEspecificidade.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certidao/certidaoAnuencia.js") %>"></script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<% Html.RenderPartial("DestinatarioEspecificidade", Model.DestinatariosVM); %>

	<div class="block">
		<div class="coluna75">
			<label for="CertidaoAnuencia_Certificacao">Certificação *</label>
			<%= Html.TextArea("CertidaoAnuencia.Certificacao", Model.CertidaoAnuencia.Certificacao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtCertificacao", @maxlength = "1500" }))%>
		</div>
	</div>


</fieldset>