<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CARSolicitacao>" %>

<div class="block box">
	<h1 class="titTela">Notificação</h1>
	<br />

	<div class="coluna100">
		<label for="Solicitacao_Motivo">Motivo *</label>
		<%= Html.TextArea("Solicitacao.Motivo", Model.Motivo, ViewModelHelper.SetaDisabled(true, new { @class = "text txtMotivo media" }))%>
	</div>
</div>