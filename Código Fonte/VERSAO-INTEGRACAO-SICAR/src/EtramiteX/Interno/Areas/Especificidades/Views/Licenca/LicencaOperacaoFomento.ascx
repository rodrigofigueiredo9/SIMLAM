<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LicencaOperacaoFomentoVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Licenca/licencaOperacaoFomento.js") %>"></script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>	
</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>