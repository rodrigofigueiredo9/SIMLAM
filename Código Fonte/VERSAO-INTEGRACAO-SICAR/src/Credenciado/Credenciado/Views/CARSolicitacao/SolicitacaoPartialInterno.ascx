<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CARSolicitacaoVM>" %>

<input type="hidden" class="hdnSolicitacaoId" value="<%=Model.Solicitacao.Id%>" />

<div class="block box">
	<div class="coluna24 append2">
		<label for="Solicitacao_Numero">Nº de controle da solicitação *</label>
		<%= Html.TextBox("Solicitacao.Numero", Model.Solicitacao.NumeroTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumero", @maxlength="7" }))%>
	</div>

	<div class="coluna24 append2">
		<label for="Solicitacao_DataEmissao_DataTexto">Data de emissão *</label>
		<%= Html.TextBox("Solicitacao.DataEmissao.DataTexto", Model.Solicitacao.DataEmissao.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataEmissao" }))%>
	</div>

	<div class="coluna24">
		<label for="Solicitacao_Situacao">Situação *</label>
		<%= Html.DropDownList("Solicitacao.Situacao", Model.SituacaoLst, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSituacao" }))%>
	</div>
</div>

<div class="block box">
	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_Protocolo_Numero">Nº de registro do protocolo *</label>
			<%= Html.TextBox("Solicitacao.Protocolo.Numero", Model.Solicitacao.Protocolo.Numero, ViewModelHelper.SetaDisabled(true, new { @class = "text txtProtocoloNumero" }))%>
			<input type="hidden" class="hdnProtocoloId" value="<%=Model.Solicitacao.Protocolo.Id%>" />
		</div>

		<div class="ultima prepend2 divBotoesProtocolo">
			<span class="spnBuscarProtocolo <%= (Model.Solicitacao.Protocolo.Id > 0) || Model.IsVisualizar ? "hide" : "" %>">
				<button type="button" class="floatLeft inlineBotao botaoBuscar btnBuscarProtocolo">Buscar</button>
			</span>

			<span class="prepend1 spnLimparContainer <%= (Model.Solicitacao.Protocolo.Id > 0) && !Model.IsVisualizar? "" :"hide" %>">
				<button type="button" class="inlineBotao btnLimparProtocolo">Limpar</button>
			</span>
		</div>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_RequerimentoLst">Requerimento padrão *</label>
			<%= Html.DropDownList("Solicitacao.RequerimentoLst", Model.RequerimentoLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlRequerimento" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_Requerimento_Atividade">Atividade solicitada *</label>
			<%= Html.DropDownList("Solicitacao.Requerimento.Atividade", Model.AtividadeLst, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividade" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna25 append2">
			<label for="Solicitacao_Empreendimento_Codigo">Código do empreendimento *</label>
			<%= Html.TextBox("Solicitacao.Empreendimento.Codigo", Model.Solicitacao.Empreendimento.Codigo, ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmpreendimentoCodigo" }))%>
		</div>

		<div class="coluna43">
			<label for="Solicitacao_Empreendimento_NomeRazao">Empreendimento *</label>
			<%= Html.TextBox("Solicitacao.Empreendimento.NomeRazao", Model.Solicitacao.Empreendimento.NomeRazao, ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmpreendimentoNomeRazao" }))%>
			<input type="hidden" class="hdnEmpreendimentoId" value="<%=Model.Solicitacao.Empreendimento.Id%>" />
		</div>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_Declarante_Id">Nome/ Razão Social do Declarante *</label>
			<%= Html.DropDownList("Solicitacao.Declarante.Id", Model.DeclaranteLst, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlDeclarante" }))%>
		</div>
	</div>
</div>