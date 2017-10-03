<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>

<div id="abasNav" class="AbasFiscalizacao subContMenu">
	<ul>
		<li class="step1 ativo"><a class="primeira">Local da Infração</a> </li>
		<li class="step2"><a>Projeto Geográfico</a> </li>
		<li class="step3"><a>Infração</a></li>
        <li class="step4"><a>Multa</a></li>
		<li class="step5"><a>Interdição/Embargo</a> </li>
		<li class="step6"><a>Apreensão</a> </li>
        <li class="step7"><a>Outras Penalidades</a> </li>
		<li class="step8"><a>Considerações finais</a> </li>
		<li class="step9"><a class="ultima">Concluir Cadastro</a></li>
	</ul>
</div>

<input type="hidden" class="hdnFiscalizacaoId" id="hdnFiscalizacaoId" value="<%= Model.Id %>" />
<input type="hidden" class="hdnFiscalizacaoSituacaoId" id="hdnFiscalizacaoSituacaoId" value="<%= Model.Fiscalizacao.SituacaoId %>" />
<input type="hidden" class="hdnEmpreendimentoId" value="0" />

<div class="conteudoFiscalizacao block">	
	<%  Html.RenderPartial("LocalInfracao", Model.LocalInfracaoVM); %>
</div>
<div class="block box">
	<%  if (!Model.IsVisualizar) { %>
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
			<input class="floatLeft btnFinalizar" type="button" value="Concluir Cadastro" />
		</span>
	</span>
	<span class="spnCancelarEdicao cancelarCaixa hide"><span class="btnModalOu">ou</span> <a class="linkCancelar linkCancelarEdicao">Cancelar edição</a></span>
	<%}%>
	<span class="floatRight spnCancelarCadastro"><a class="linkCancelar" href="<%= Url.Action("Index","Fiscalizacao") %>" title="Cancelar">Cancelar</a></span>
</div>