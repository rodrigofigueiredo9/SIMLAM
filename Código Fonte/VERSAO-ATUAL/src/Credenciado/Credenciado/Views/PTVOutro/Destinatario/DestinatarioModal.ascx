
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTVOutro/destinatario.js") %>"></script>
<script type="text/javascript">
	$.extend(DestinatarioPTV.settings,{
		urls: {
			verificarCPFCNPJ:'<%= Url.Action("VerificarDestinatarioCPFCNPJ", "PTVOutro") %>',
			salvar: '<%= Url.Action("DestinatarioSalvar", "PTVOutro") %>'
		}
	});	
</script>
<div id="central">
	<h1 class="titTela">Cadastrar Destinatário</h1>
	<br />
	<% Html.RenderPartial("Destinatario/DestinatarioPartial", Model); %>
</div>