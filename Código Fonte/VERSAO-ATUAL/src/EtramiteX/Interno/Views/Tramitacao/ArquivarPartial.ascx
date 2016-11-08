<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ArquivarVM>" %>

<div class="enviarPartial">
	<h1 class="titTela">Arquivar Processo/Documento</h1>
	<br />

	<fieldset class="block box">
		<legend>Remetente</legend>
		<div class="block divDropDown">
			<div class="coluna45">
				<label for="FuncionarioNome">Funcionário *</label>
				<%= Html.TextBox("FuncionarioNome", Model.Arquivar.Funcionario.Nome, new { @class = "text disabled txtFuncionarioNome", @disabled = "disabled" })%>
				<input type="hidden" class="hdnFuncionarioId" value="<%= Model.Arquivar.Funcionario.Id %>" />
			</div>

			<div class="block ultima prepend2">
				<label for="Arquivar_SetorId">Setor de origem *</label>
				<%= Html.DropDownList("Arquivar.SetorId", Model.SetoresOrigem, new { @class = "text ddlSetoresOrigem" })%>
			</div>
		</div>
	</fieldset>
	
	<fieldset id="Container_Processos_Documentos" class="block box containerProcessos <%= Model.Arquivar.SetorId <= 0 ? "hide" : "" %>">
		<legend>Processo/Documento em Posse</legend>
		<div class="block">
			<div class="coluna50">
				<label class="append5"><%= Html.RadioButton("FiltroTipo", 1, false, new { @class = "radio radFiltroTipo" })%>Listar todos</label>
				<label class="append5"><%= Html.RadioButton("FiltroTipo", 2, false, new { @class = "radio radFiltroTipo" })%>Nº Registro Processo/Documento</label>
			</div>
		</div>

		<div class="block divNumeroProcessoDoc hide">
			<div class="coluna15">
				<label for="NumeroProcessoDoc">Nº Registro *</label>
				<%= Html.TextBox("NumeroProcessoDoc", string.Empty, new { @maxlength = "12", @class = "text txtNumeroProcDoc" })%>
			</div>

			<div class="coluna10 prepend2">
				<button type="button" class="inlineBotao botaoAdicionarIcone btnAddProcDoc" style="width:35px" title="Adicionar Processo/Documento">Adicionar</button>
			</div>
		</div>

		<div class="block divItens">
			<table class="dataGridTable tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="25%"><input type="checkbox" class="ckbCheckAllInMyColumn" value="false" /> Processo/Documento</th>
						<th width="16%">Enviado em</th>
						<th width="6%">Origem</th>
						<th width="22%">Motivo</th>
						<th width="16%">Recebido em</th>
						<th width="15%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% Html.RenderPartial("ArquivarItens"); %>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset class="block box containerDadosArquivamento hide">
		<legend>Dados do Arquivamento</legend>
		<div class="block">
			<div class="coluna25">
				<label for="Arquivar_DataArquivamento_DataTexto">Data *</label>
				<%= Html.TextBox("Arquivar.DataArquivamento.DataTexto", Model.Arquivar.DataArquivamento.DataTexto, new { @maxlength = "80", @class = "text maskData txtData disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna42 prepend2">
				<label for="Arquivar_ObjetivoId">Motivo *</label>
				<%= Html.DropDownList("Arquivar.ObjetivoId", Model.Objetivos, new { @class = "text ddlObjetivos" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna45 divArquivos">
				<label for="Arquivar_ArquivoId">Arquivo *</label>
				<%= Html.DropDownList("Arquivar.ArquivoId", Model.Arquivos, new { @class = "text ddlArquivo" })%>
			</div>

			<div class="block ultima prepend2 divEstantes">
				<label for="Arquivar_EstanteId">Estante *</label>
				<%= Html.DropDownList("Arquivar.EstanteId", Model.Estantes, new { @class = "text ddlEstantes disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25">
				<label for="Arquivar_PrateleiraId">Prateleira/Pasta suspensa *</label>
				<%= Html.DropDownList("Arquivar.PrateleiraId", Model.PrateleiraModos, new { @class = "text ddlPrateleirasModo disabled", @disabled = "disabled" })%>
			</div>

			<div class="block ultima prepend2">
				<label for="Arquivar_Identificacao">Identificação *</label>
				<%= Html.DropDownList("Arquivar.Identificacao", Model.PrateleirasIdentificacoes, new { @class = "text ddlIdentificacao disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block ultima">
			<label for="Arquivar_Despacho">Despacho</label>
			<%= Html.TextArea("Arquivar.Despacho", null, new { @class = "text media txtDespacho", @maxlength = "500" })%>
		</div>
	</fieldset>
</div>