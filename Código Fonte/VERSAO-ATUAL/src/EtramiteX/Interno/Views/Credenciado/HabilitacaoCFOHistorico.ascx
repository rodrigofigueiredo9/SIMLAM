<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HistoricoEmissaoCFOCFOCVM>" %>

<%--<script>
	$.extend(HabilitacaoCFOAlterarSituacao.settings, {
		urls: {
			alterarSituacao: '<%= Url.Action("AlterarSituacaoHabilitacaoCFO", "Credenciado") %>'
		},
	    situacaoMotivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Inativo %>',
	    situacaoMotivoAtivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Ativo %>',
	    motivoSuspenso: '<%= (int)eHabilitacaoCFOCFOCMotivo.Suspensao %>',
	    motivoDescredenciado: '<%= (int)eHabilitacaoCFOCFOCMotivo.Descredenciamento %>'
	});
</script>--%>

<h1 class="titTela">Histórico de Habilitações para Emissão de CFO e CFOC</h1>
<br />

<%= Html.Hidden("HabilitarEmissao.Id", Model.Id, new { @class = "hdnHabilitacaoId" })%>
<div class="box block">
	<div class="block ultima">
		<label for="HistoricoEmissao.Nome">Nome</label>
		<%= Html.TextBox("HistoricoEmissao.Nome", Model.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
	</div>

</div>