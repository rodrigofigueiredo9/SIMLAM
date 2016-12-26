<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>

<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacao.js") %>"></script>
<script>
	Fiscalizacao.salvarEdicao = false;
	$(function () {
		Fiscalizacao.load($('.fiscalizacaoModalContainer'));
		FiscalizacaoFinalizar.callBackObterFiscalizacaoFinalizar();
	});

	Fiscalizacao.modo = 3;//Visualizar
</script>

<div class="fiscalizacaoModalContainer">
	<h1 class="titTela">Visualizar Fiscalização</h1>
	<br />

	<input type="hidden" class="hdnFiscalizacaoId" id="hdnFiscalizacaoId" value="<%= Model.Id %>" />
	<input type="hidden" class="hdnFiscalizacaoSituacaoId" id="hdnFiscalizacaoSituacaoId" value="<%= Model.Fiscalizacao.SituacaoId %>" />
	<input type="hidden" class="hdnEmpreendimentoId" value="0" />

	<div class="conteudoFiscalizacao block">
		<%  Html.RenderPartial("ConcluirCadastro", Model); %>
	</div>
</div>