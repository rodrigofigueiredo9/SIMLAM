<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<% if (Model.IsAjaxRequest) { %>
<script>
	$.extend(HabilitarEmissaoCFOCFOC.settings, {
		urls: {
			voltar: '<%= Url.Action("CriarHabilitarEmissaoCFOCFOC", "Credenciado") %>',
			salvar: '<%= Url.Action("SalvarHabilitarEmissao", "Credenciado") %>',
			editar: '<%= Url.Action("EditarHabilitarEmissaoCFOCFOC", "Credenciado") %>',
			visualizar: '<%= Url.Action("VisualizarHabilitarEmissaoCFOCFOC", "Credenciado") %>'
		},
		Mensagens: <%= Model.Mensagens %>
	});
</script>
<% } %>

<div class="modalVisualizarHabilitarEmissaoCFOCFOC">
	<%= Html.Hidden("HabilitarEmissao.Id", Model.HabilitarEmissao.Id, new { @class = "hdnHabilitarId" })%>
	<%= Html.Hidden("HabilitarEmissao.Tid", Model.HabilitarEmissao.Tid, new { @class = "hdnHabilitarTid" })%>
	<%= Html.Hidden("EstadoDefault", Model.HabilitarEmissao.UF, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.HabilitarEmissao.UFTexto, new { @class = "hdnEstadoDefaultSigla" })%>
	<%= Html.Hidden("hdnResponsavelId", Model.HabilitarEmissao.Responsavel.Id, new { @class = "hdnResponsavelId" })%>

	<h1 class="titTela">Visualizar Habilitar Emissão de CFO e CFOC</h1>
	<br/>

	<fieldset class="divResponsavel block box">
		<legend>Responsável Técnico</legend>

		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna30">
				<div class="CpfPessoaContainer">
					<label for="HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ">CPF</label>
					<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ", Model.HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ, new { @class = "text maskCpf txtResponsavelCpf disabled" })%>
				</div>
			</div>
			<div class="block ultima">
				<a title="Visualizar responsável" class="icone visualizar inlineBotao btnVisualizarResponsavel <%= (Model.HabilitarEmissao.Responsavel.Id > 0 ? "" : "hide") %>">Visualizar Responsável</a>
			</div>
		</div>

		<!-- Linha 2 -->
		<div class="block">
			<div class="coluna80">
				<label for="HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial">Nome</label>
				<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial", Model.HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial, new { @class = "txtResponsavelNome text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<!-- Linha 3 -->
		<div class="block">
			<span class="floatLeft inputFileDiv coluna80">
				<label for="ArquivoTexto">Arquivo complementar</label>
				<% if(Model.HabilitarEmissao.Arquivo.Id.GetValueOrDefault() > 0) { %>
					<%= Html.ActionLink(Tecnomapas.EtramiteX.Interno.ViewModels.ViewModelHelper.StringFit(Model.HabilitarEmissao.Arquivo.Nome, 43), "Baixar", "Arquivo", new { @id = Model.HabilitarEmissao.Arquivo.Id }, new { @Style = "display: block", @title = Model.HabilitarEmissao.Arquivo.Nome })%>
				<% } else { %>
					<input type="text" value="*** Nenhum arquivo associado ***" class="text txtArquivoNome disabled" disabled="disabled" />
				<% } %>
			</span>
		</div>
	</fieldset>

	<fieldset class="divregistro block box">
		<legend>Registro e Habilitações</legend>

		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Nº da habilitação</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroHabilitacao", Model.HabilitarEmissao.NumeroHabilitacao, new { @maxlength = "8", @class = "txtNumeroHabilitacao text disabled maskNumInt", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao.ValidadeRegistro">Validade da taxa de registro</label>
				<%= Html.TextBox("HabilitarEmissao.ValidadeRegistro", Model.HabilitarEmissao.ValidadeRegistro, new { @maxlength = "10", @class = "txtValidadeRegistro text maskData disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao.SituacaoTexto">Situação</label>
				<%= Html.TextBox("HabilitarEmissao.SituacaoTexto", Model.HabilitarEmissao.SituacaoTexto, new {@class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<!-- Linha 2 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Número do DUA</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroDua", Model.HabilitarEmissao.NumeroDua, new { @maxlength = "30", @class = "txtNumeroDua text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<!-- Linha 3 -->
		<div class="block">
			<div class="coluna30">
				<p><label for="HabilitarEmissao.ExtensaoHabilitacao">Extensão de habilitação?</label></p>
				<label class=""><%= Html.RadioButton("HabilitarEmissao.ExtensaoHabilitacao", "Sim", (Model.HabilitarEmissao.ExtensaoHabilitacao == 1), new { @class = "radio rdbExtensaoHabilitacao disabled" , @disabled = "disabled"})%> Sim</label>
				<label class=""><%= Html.RadioButton("HabilitarEmissao.ExtensaoHabilitacao", "Nao", (Model.HabilitarEmissao.ExtensaoHabilitacao == 0), new { @class = "radio rdbExtensaoHabilitacao disabled" , @disabled = "disabled"})%> Não</label>
			</div>

			<div class="coluna30 prepend2 divNumeroHabilitacaoOrigem <%= (Model.HabilitarEmissao.ExtensaoHabilitacao == 1) ? "" : "hide" %>">
				<label for="HabilitarEmissao.NumeroHabilitacaoOrigem">Número da habilitação de origem</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroHabilitacaoOrigem", Model.HabilitarEmissao.NumeroHabilitacaoOrigem, new { @maxlength = "8", @class = "text txtHabOrigem disabled maskNumInt", @disabled = "disabled" })%>
			</div>
		</div>

		<!-- Linha 4 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Registro no CREA</label>
				<%= Html.TextBox("HabilitarEmissao.RegistroCrea",  Model.HabilitarEmissao.RegistroCrea, new { @maxlength = "30", @class = "txtRegistroCrea text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao.UF">UF</label>
				<%= Html.DropDownList("HabilitarEmissao.UF", Model.Estados, new { @class = "text ddlUf disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend2 divRegistroCrea <%= (Model.HabilitarEmissao.UF == 8 ? "hide" : "") %>">
				<label for="HabilitarEmissao.RegistroCrea">Nº visto CREA/ES</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroVistoCrea", Model.HabilitarEmissao.NumeroVistoCrea, new { @maxlength = "30", @class = "txtNumeroVistoCrea text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="divregistro block box">
		<legend>Praga</legend>

		<!-- Linha 3 -->
		<div class="block">
			<div class="gridContainer">
				<table class="dataGridTable gridPraga" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Nome científico</th>
							<th>Nome comum</th>
							<th width="20%">Cultura</th>
							<th width="15%">Data inicial</th>
							<th width="15%">Data final</th>
						</tr>
					</thead>
					<tbody>
						<% Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado.PragaHabilitarEmissao item = null;
							for (int i = 0; i < Model.HabilitarEmissao.Pragas.Count; i++) {
							item = Model.HabilitarEmissao.Pragas[i];
						%>
						<tr>
							<td>
								<label class="lblNomeCientifico" title="<%=item.Praga.NomeCientifico%>"><%=item.Praga.NomeCientifico%> </label>
							</td>
							<td>
								<label class="lblNomeComun" title="<%=item.Praga.NomeComum%>"><%=item.Praga.NomeComum%> </label>
							</td>
							<td>
								<label class="lblCultura" title="<%=item.Cultura%>"><%=item.Cultura%> </label>
							</td>
							<td>
								<label class="lblDataInicialHabilitacao" title="<%=item.DataInicialHabilitacao%>"><%=item.DataInicialHabilitacao%> </label>
							</td>
							<td>
								<label class="lblDataFinalHabilitacao" title="<%=item.DataFinalHabilitacao%>"><%=item.DataFinalHabilitacao%> </label>
							</td>
						</tr>
						<% } %>

						<!-- Template -->
						<tr class="hide tr_template">
							<td>
								<label class="lblNomeCientifico"></label>
							</td>
							<td>
								<label class="lblNomeComun"></label>
							</td>
							<td>
								<label class="lblCultura"></label>
							</td>
							<td>
								<label class="lblDataInicialHabilitacao"></label>
							</td>
							<td>
								<label class="lblDataFinalHabilitacao"></label>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>