<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>
<div id="abasNav" class="AbasRequerimento">
	<ul class="ui-tabs-nav">
		<li class="ui-tabs-nav-item step1 ui-tabs-selected"><a class="primeira">Objetivo do Pedido</a> </li>
		<li class="ui-tabs-nav-item step2"><a>Interessado</a> </li>
		<li class="ui-tabs-nav-item step3"><a>Responsável Técnico</a> </li>
		<li class="ui-tabs-nav-item step4"><a>Empreendimento</a> </li>
		<li class="ui-tabs-nav-item step5"><a class="ultima">Finalizar Requerimento</a></li>
	</ul>
</div>
<br />
<input type="hidden" id="hdnRequerimentoId" value="<%= Model.Id %>" />
<div class="conteudoRequerimento block">
	<%  Html.RenderPartial((Model.IsVisualizar) ? "ObjetivoPedidoVisualizar" : "ObjetivoPedido"); %>
</div>
<div class="block box">
	<span class="modoVisualizar">
		<span class="spanBotoes divSalvar">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
		</span>
		<span class="spanBotoes divEditar hide">
			<input class="floatLeft btnEditar" type="button" value="Editar" />
		</span>
		<span class="spanBotoes divIntNovo hide">
			<input class="floatLeft btnIntAssNovo" type="button" value="Buscar Novo" />
		</span>
		<span class="spanBotoes divEmpAvancar hide">
			<input class="floatLeft btnEmpAvancar" type="button" value="Novo" />
		</span>
		<span class="spanBotoes divEmpNovo hide">
			<input class="floatLeft btnEmpAssNovo" type="button" value="Buscar Novo" />
		</span>
		<span class="spanBotoes divFinalizar hide">
			<input class="floatLeft btnFinalizar" type="button" value="Finalizar" />
		</span>
	</span>
	<span class="spnCancelarEdicao cancelarCaixa hide"><span class="btnModalOu">ou</span> <a class="linkCancelar">Cancelar edição</a></span>
	<span class="floatRight spnCancelarCadastro"><a class="linkCancelar" href="<%= Url.Action("") %>" title="Cancelar">Cancelar</a></span>
</div>
