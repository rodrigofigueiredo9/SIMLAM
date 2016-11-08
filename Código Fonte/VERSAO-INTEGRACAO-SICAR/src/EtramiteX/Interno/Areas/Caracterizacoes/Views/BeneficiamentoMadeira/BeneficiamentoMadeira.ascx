<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BeneficiamentoMadeiraVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.BeneficiamentoMadeira%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />
<input type="hidden" class="hdnQtdAtividade" value="<%: Model.QtdAtividade %>" />

<script type="text/javascript">
	BeneficiamentoMadeira.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	BeneficiamentoMadeira.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	BeneficiamentoMadeira.settings.textoMerge = '<%= Model.TextoMerge %>';
	BeneficiamentoMadeiraBeneficiamento.settings.idsTela =  <%=Model.IdsTela %>;
	BeneficiamentoMadeiraBeneficiamento.settings.mensagens = <%= Model.Mensagens%>;
	BeneficiamentoMadeira.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	CoordenadaAtividade.settings.mensagens = <%= Model.Mensagens%>;
	CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "BeneficiamentoMadeira") %>';
	CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "BeneficiamentoMadeira") %>';
	MateriaPrimaFlorestalConsumida.settings.mensagens = <%= Model.Mensagens%>;
</script>


<div class="containerBeneficiamento">
	<%if(Model.BeneficiamentoMadeiraBeneficiamentosVM.Count > 0) {
		  foreach (BeneficiamentoMadeiraBeneficiamentoVM vm in Model.BeneficiamentoMadeiraBeneficiamentosVM)
		  {%>
			<%Html.RenderPartial("BeneficiamentoMadeiraBeneficiamento", vm);%>
		<%}
	}else{ %>
			<%Html.RenderPartial("BeneficiamentoMadeiraBeneficiamento", Model.BeneficiamentoMadeiraBeneficiamentosTemplateVM);%>
	<%} %>
</div>

<%if(!Model.IsVisualizar) {%>
<div class="block divAdicionarAtividade">
	<div class="coluna10 direita">
		<button type="button" style="width:100px;" class="inlineBotao btnAdicionarAtividade btnAddItem" title="Adicionar">+ Atividade</button>
	</div>
</div>
<%} %>