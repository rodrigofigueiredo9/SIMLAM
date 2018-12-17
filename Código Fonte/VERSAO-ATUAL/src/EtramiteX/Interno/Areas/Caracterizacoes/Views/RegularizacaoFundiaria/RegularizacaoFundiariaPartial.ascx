<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<input type="hidden" class="hdnPosseId" value="<%= Model.Caracterizacao.Posse.Id %>" />
<input type="hidden" class="hdnDominioId" value="<%= Model.Caracterizacao.Posse.Dominio %>" />
<input type="hidden" class="hdnZonaLocalizacaoId" value="<%= Model.Caracterizacao.Posse.Zona %>" />

<% Html.RenderPartial("DadosDominialidadePartial", Model); %>
<% Html.RenderPartial("CaracteristicasOcupacaoPartial", Model); %>
<% Html.RenderPartial("ConfrontacoesDominio", Model); %>
<div class="block box">
	<% if(!Model.IsVisualizar){ %>
		<input class="floatLeft btnConfirmarPosse" type="button" value="Confirmar Dados"/>
		<span class="cancelarCaixa"><span class="btnModalOu">ou </span> <a class="linkCancelar btnCancelarPosse" title="Cancelar">Cancelar</a></span>
	<% }else{ %>
		<span class="cancelarCaixa"><a class="linkCancelar btnCancelarPosse" title="Cancelar">Cancelar</a></span>
	<%} %>
</div>		