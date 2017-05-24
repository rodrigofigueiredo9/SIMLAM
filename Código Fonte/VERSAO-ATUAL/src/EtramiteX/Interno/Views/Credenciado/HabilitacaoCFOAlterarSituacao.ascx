<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<script>
	$.extend(HabilitacaoCFOAlterarSituacao.settings, {
		urls: {
			alterarSituacao: '<%= Url.Action("AlterarSituacaoHabilitacaoCFO", "Credenciado") %>'
		},
	    situacaoMotivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Inativo %>',
	    situacaoMotivoAtivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Ativo %>',
	    motivoSuspenso: '<%= (int)eHabilitacaoCFOCFOCMotivo.Suspensao %>',
	    motivoDescredenciado: '<%= (int)eHabilitacaoCFOCFOCMotivo.Descredenciamento %>'
	});
</script>

<h1 class="titTela">Alterar Situação da Habilitação</h1>
<br />

<%= Html.Hidden("HabilitarEmissao.Id", Model.HabilitarEmissao.Id, new { @class = "hdnHabilitacaoId" })%>
<div class="box block">
	<div class="block ultima">
		<label for="HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial">Responsável técnico</label>
		<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial", Model.HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
	</div>

	<div class="block">
		<div class="coluna30 append1">
			<label for="SituacaoTexto">Situação atual</label>
			<%= Html.TextBox("SituacaoTexto", Model.HabilitarEmissao.SituacaoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
		</div>

		<% if(Model.HabilitarEmissao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo) { %>
		<div class="coluna30 append1">
			<label for="MotivoTexto">Motivo atual</label>
			<%= Html.TextBox("MotivoTexto", Model.HabilitarEmissao.MotivoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
		</div>
		<% } %>

		<div class="coluna30">
			<label for="SituacaoData">Data da situação atual</label>
			<%= Html.TextBox("SituacaoDataAntiga", Model.HabilitarEmissao.SituacaoData, ViewModelHelper.SetaDisabled(true, new { @class = "text maskData" }))%>
		</div>

        <div class="coluna30 divDataFinalSituacao hide">
			<label for="HabilitarEmissao.SituacaoData">Data final da inativação *</label>
			<%= Html.TextBox("HabilitarEmissao.SituacaoData", DateTime.Now.ToShortDateString(), new { @class = "text maskData disabled txtFinalSituacaoData" })%>
		</div>

        <div class="coluna30 divNumeroDua append1 hide">
			<label for="NumeroDua">Número do DUA *</label>
			<%= Html.TextBox("NumeroDua", Model.HabilitarEmissao.NumeroDua, ViewModelHelper.SetaDisabled(false, new { @class = "text txtNumeroDua" }))%>
		</div>

        <div class="coluna30 divDataPagamento append1 hide">
			<label for="DataPagamento">Data do pagamento</label>
			<%= Html.TextBox("DataPagamento", Model.HabilitarEmissao.DataPagamentoDUA, ViewModelHelper.SetaDisabled(false, new { @class = "text maskData txtDataPagamentoDua" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30 append1">
			<label for="Situacao.Novo">Nova Situação *</label>
			<%= Html.DropDownList("Situacao.Novo", Model.Situacoes, new { @class = "text ddlSituacao" })%>
		</div>

		<div class="coluna30 divMotivo append1 hide">
			<label for="Motivo.Novo" class="labelNovoMotivo">Novo motivo *</label>
			<%= Html.DropDownList("Motivo.Novo", Model.SituacaoMotivos, new { @class = "text ddlMotivo" })%>
		</div>

		<div class="coluna30 divDataNovaSituacao hide">
			<label for="HabilitarEmissao.SituacaoData">Data da inativação *</label>
			<%= Html.TextBox("HabilitarEmissao.SituacaoData", DateTime.Now.ToShortDateString(), new { @class = "text maskData txtSituacaoData" })%>
		</div>
	</div>

	<div class="block ultima">
		<label for="HabilitarEmissao.Observacao">Observação *</label>
		<%= Html.TextArea("HabilitarEmissao.Observacao", Model.HabilitarEmissao.Observacao, new { @class = "text media txtObservacao", @maxlength = "250" })%>
	</div>
</div>