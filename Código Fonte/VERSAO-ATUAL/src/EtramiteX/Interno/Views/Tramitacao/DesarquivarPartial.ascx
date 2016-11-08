<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DesarquivarVM>" %>

<div class="desarquivarPartial">
	<h1 class="titTela">Desarquivar Processo/Documento</h1>
	<br />

	<fieldset class="block box">
		<legend>Destinatário</legend>
		<div class="block divDropDown">
			<div class="coluna45">
				<label for="Executor.Nome">Funcionário *</label>
				<%= Html.TextBox("Executor.Nome", Model.Executor.Nome, new { @class = "text disabled txtExecutorNome", @disabled = "disabled" })%>
			</div>

			<div class="coluna48 prepend2">
				<label for="SetorRemetente.Id">Setor de origem *</label>
				<%= Html.DropDownList("SetorRemetente.Id", Model.SetoresOrigem, new { @class = "text ddlRemetenteSetor" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box fsDadosDesarquivamento hide">
		<legend>Dados do Desarquivamento</legend>
		<div class="block">
			<div class="coluna15">
				<label for="Data_Receber">Data *</label>
				<%= Html.TextBox("Data_Receber", Model.RecebimentoData.DataTexto, new { @class = "disabled text maskData txtDataRecebimento", @disabled = "disabled" })%>
			</div>

			<div class="coluna81 prepend2">
				<label for="Arquivar_ArquivoId">Arquivo *</label>
				<%= Html.DropDownList("Arquivar.ArquivoId", Model.ArquivosCadastrados, new { @class = "text ddlArquivos" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna31 append2">
				<label for="Arquivar_EstanteId">Estante</label>
				<%= Html.DropDownList("Arquivar.EstanteId", Model.Estantes, new { @class = "text ddlEstantes disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna31 append2">
				<label for="Desarquivar_PrateleiraId">Prateleira/ Pasta suspensa</label>
				<%= Html.DropDownList("Desarquivar.PrateleiraId", Model.PrateleiraModos, new { @class = "text ddlPrateleirasModo disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna31">
				<label for="Arquivar_Identificacao">Identificação</label>
				<%= Html.DropDownList("Arquivar.Identificacao", Model.PrateleirasIdentificacoes, new { @class = "text ddlIdentificacao disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset id="Container_Processos_Documentos" class="block box containerProcessos hide">
		<legend>Dados do arquivo</legend>
		<div class="block">
			<div class="coluna50">
				<label class="append5"><%= Html.RadioButton("FiltroTipo", 1, false, new { @class = "radio radFiltroTipo" })%>Buscar por filtros</label>
				<label class="append5"><%= Html.RadioButton("FiltroTipo", 2, false, new { @class = "radio radFiltroTipo" })%>Nº Registro Processo/Documento</label>
			</div>
		</div>

		<div class="block divNumeroProcessoDoc hide">
			<div class="coluna15">
				<label for="NumeroProtocolo">Nº Registro *</label>
				<%= Html.TextBox("NumeroProtocolo", string.Empty, new { @maxlength = "12", @class = "text txtNumeroProcDoc" })%>
			</div>

			<div class="coluna10 prepend2">
				<button type="button" class="inlineBotao botaoAdicionarIcone btnAddProcDoc" style="width:35px" title="Adicionar Processo/Documento">Adicionar</button>
			</div>
		</div>
	</fieldset>

	<div class="containerDadosFiltroDesarquivamento hide">
		<fieldset class="block box fsFiltrosDesarquivamento hide">
			<legend>Filtros</legend>
			<div class="block">
				<div class="coluna30">
					<label>Nº registro processo/documento</label>
					<%= Html.TextBox("NumeroProcessoDoc", null, new { @maxlength = "12", @class = "text txtNumeroProcDoc" })%>
				</div>
				<div class="coluna60 prepend2">
					<label for="SituacoesAtividade">Situação da atividade solicitada</label>
					<%= Html.DropDownList("SituacoesAtividade", Model.SituacoesAtividade, new { @class = "text ddlSituacaoAtividade" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna93">
					<label >Atividade solicitada</label>
					<%= Html.DropDownList("Atividade", Model.Atividades, new { @class = "text ddlAtividades" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna65">
					<label>Nome do interessado</label>
					<%= Html.TextBox("NomeInteressado", null, new {  @class = "text txtNomeInteressado" })%>
				</div>
				<div class="coluna25 prepend2">
					<label>CPF/CNPJ do interessado</label>
					<%= Html.TextBox("CPFCNPJInteressado", null, new { @maxlength = "19", @class = "text txtCpfCnpjInteressado" })%>
				</div>
			</div>

			<div class="block divEndereco">
				<div class="coluna30 ">
					<label for="SituacoesAtividade">UF</label>
					<%= Html.DropDownList("Estado", Model.Estados, new { @class = "text ddlEstado" })%>
				</div>
				<div class="coluna32 prepend2">
					<label>Município do empreendimento</label>
					<%= Html.DropDownList("MunicipioEmpreendimento", Model.MunicipiosEmpreendimento, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlMunicipio ddlMunicipioEmpreendimento" }))%>
				</div>
			</div>

			<div class="block">
				<div class="ultima prepend2">
					<button type="button" title="Buscar no Arquivo" class="inlineBotao floatRight botaoBuscar btnBuscarArquivo">Buscar</button>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Processo/Documento em Arquivo</legend>
			<div class="block divDesarquivarItens">
					<table class="dataGridTable tabTramitacoesArquivadas tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>
									<input type="checkbox" class="ckbIsTodosSelecionado" value="false" />
									Processo/Documento
								</th>
								<th width="15%">Enviado em</th>
								<th width="10%">Origem</th>
								<th width="20%">Motivo</th>
								<th width="15%">Ações</th>
							</tr>
						</thead>
						<tbody>
							<% Html.RenderPartial("DesarquivarItens"); %>
						</tbody>
					</table>
			</div>
		</fieldset>

		<div class="block box">
			<div class="coluna48">
				<label for="SetorDestinatario_Id">Setor de destino *</label>
				<%= Html.DropDownList("SetorDestinatario_Id", Model.SetoresDestino, new { @class = "text ddlDestinatarioSetor" })%>
			</div>
		</div>
	</div>
</div>