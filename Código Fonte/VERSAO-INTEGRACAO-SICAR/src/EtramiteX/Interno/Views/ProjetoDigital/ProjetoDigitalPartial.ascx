<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalVM>" %>

<div id="abasNav" class="containerAbas">
	<ul class="ui-tabs-nav">
		<li class="ui-tabs-nav-item step1 ui-tabs-selected"><a class="primeira">Objetivo do Pedido</a> </li>
		<li class="ui-tabs-nav-item step2"><a>Interessado</a> </li>
		<li class="ui-tabs-nav-item step3"><a>Responsável Técnico</a> </li>
		<li class="ui-tabs-nav-item step4"><a>Empreendimento</a> </li>
		<li class="ui-tabs-nav-item step5"><a class="ultima">Importar</a></li>
	</ul>
</div>
<br />

<input type="hidden" id="hdnRequerimentoId" value="<%= Model.Id %>" />

<div class="conteudoProjetoDigital block">
	<% Html.RenderPartial("ObjetivoPedido"); %>
</div>

<div class="block box">
	<% if(!Model.IsVisualizar) { %>
	<span class="spanBotoes divAvancar">
		<input class="floatLeft btnAvancar" type="button" value="Avançar" />
	</span>

	<span class="spanBotoes divFinalizar hide">
		<input class="floatLeft btnFinalizar" type="button" value="Importar Requerimento" />
		<input class="floatLeft btnRecusar" type="button" value="Recusar Importação" />
	</span>
	<% } %>

	<span class="floatRight spnCancelarCadastro"><a class="linkCancelar" href="<%= Url.Action("Index", "ProjetoDigital") %>">Cancelar</a></span>
</div>