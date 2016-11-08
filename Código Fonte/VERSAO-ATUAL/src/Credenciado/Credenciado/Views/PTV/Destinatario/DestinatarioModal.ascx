﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Credenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/destinatario.js") %>"></script>
<script type="text/javascript">
	$.extend(DestinatarioPTV.settings,{
		urls: {
			verificarCPFCNPJ:'<%= Url.Action("VerificarDestinatarioCPFCNPJ", "PTV") %>',
			salvar: '<%= Url.Action("DestinatarioSalvar", "PTV") %>'
		}
	});	
</script>
<div id="central">
	<h1 class="titTela">Cadastrar Destinatário</h1>
	<br />
	<% Html.RenderPartial("Destinatario/DestinatarioPartial", Model); %>
</div>