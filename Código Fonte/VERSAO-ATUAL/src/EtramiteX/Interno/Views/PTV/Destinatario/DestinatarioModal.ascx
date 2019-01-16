<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVVM>" %>

<script src="<%= Url.Content("~/Scripts/PTV/destinatario.js") %>"></script>
<script>
	$.extend(DestinatarioPTV.settings,{
		urls: {
			verificarCPFCNPJ:'<%= Url.Action("VerificarDestinatarioCPFCNPJ", "PTV") %>',
			salvar: '<%= Url.Action("DestinatarioSalvar", "PTV") %>',
			verificarExportacao:'<%= Url.Action("VerificarDestinatarioExportacao", "PTV") %>'
		}
	});
</script>
<div id="central">
	<h1 class="titTela">Cadastrar Destinatário</h1>
	<br />
	<% Html.RenderPartial("Destinatario/DestinatarioPartial", Model); %>
</div>