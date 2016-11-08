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

<fieldset class="block box">
	<div class="block">
		<div class="coluna71 append1">
			<label for="Solicitacao_Requerimento_Id">Nº do Projeto Digital *</label>
			<%= Html.TextBox("Solicitacao.Requerimento.Id", (Model.Solicitacao.Requerimento.Id > 0 ? Model.Solicitacao.Requerimento.Id.ToString() : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtProjetoId" }))%>
			<input type="hidden" class="hdnProjetoId" value="<%= Model.Solicitacao.ProjetoId %>" />
		</div>

		<div class="ultima divBotoesProjetoDigital">
			<span class="spnBuscarProjeto append1 <%= (Model.Solicitacao.ProjetoId > 0) || Model.IsVisualizar ? "hide" : "" %>">
				<button type="button" class="inlineBotao botaoBuscar btnBuscarProjeto">Buscar</button>
			</span>

			<span class="append1 spnLimparContainer <%= (Model.Solicitacao.ProjetoId > 0) && !Model.IsVisualizar? "" :"hide" %>">
				<button type="button" class="floatLeft inlineBotao btnLimparProjeto">Limpar</button>
			</span>

            <br />
            <span class="spnVisualizarRequerimento <%=(Model.Solicitacao.ProjetoId > 0) ? "" : "hide" %>">
				<input type="button" title="Visualizar Requerimento" class="icone visualizar btnVisualizarRequerimento"/>
			</span>			
		</div>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_RequerimentoLst">Nº do Requerimento Digital*</label>
			<%= Html.TextBox("Solicitacao.Requerimento.Id", (Model.Solicitacao.Requerimento.Id > 0 ? Model.Solicitacao.Requerimento.Id.ToString() : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtRequerimento" }))%>		
		</div>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Solicitacao_Requerimento_Atividade">Atividade solicitada *</label>
			<%= Html.DropDownList("Solicitacao_Requerimento_Atividade", Model.AtividadeLst, ViewModelHelper.SetaDisabled(Model.AtividadeLst.Count <= 1 || Model.IsVisualizar, new { @class = "text ddlAtividade" }))%>
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
			<%= Html.DropDownList("Solicitacao.Declarante.Id", Model.DeclaranteLst, ViewModelHelper.SetaDisabled(Model.DeclaranteLst.Count <= 1 || Model.IsVisualizar, new { @class = "text ddlDeclarante" }))%>
		</div>
	</div>
</fieldset>