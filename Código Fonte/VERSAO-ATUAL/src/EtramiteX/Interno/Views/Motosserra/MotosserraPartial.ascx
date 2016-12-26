<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraVM>" %>

<script>
	Motosserra.settings.possuiRegistro = <%= Model.Motosserra.PossuiRegistro.ToString().ToLower() %>;
	Motosserra.settings.registroNumero = <%= Model.Motosserra.RegistroNumero %>;
</script>

<input type="hidden" class="hdnArtefatoId" value="<%= Model.Motosserra.Id %>" />
<input type="hidden" class="hdnSituacaoId" value="<%= Model.Motosserra.SituacaoId %>" />

<div class="block box">
	<div class="block">
		<div class="coluna60">
			<p><label>Já possui nº de registro *</label></p>
			<label><%= Html.RadioButton("PossuiRegistro", true, Model.Motosserra.PossuiRegistro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbPossuiRegistro" }))%> Sim</label>
			<label class="append5"><%= Html.RadioButton("PossuiRegistro", false, !Model.Motosserra.PossuiRegistro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbPossuiRegistro" }))%> Não</label>
		</div>
	</div>

	<div class="block">
		<div class="coluna20">
			<label for="RegistroNumero">Nº Registro *</label>
			<%= Html.TextBox("RegistroNumero", (Model.Motosserra.RegistroNumero <= 0 ? "Gerado automaticamente" : Model.Motosserra.RegistroNumero.ToString()), ViewModelHelper.SetaDisabled((Model.IsVisualizar || !Model.Motosserra.PossuiRegistro), new { @class = "text txtRegistroNumero maskNumInt", @maxlength = "7" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna20">
			<label for="NotaFiscalNumero">Nº Nota fiscal *</label>
			<%= Html.TextBox("NotaFiscalNumero", Model.Motosserra.NotaFiscalNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNotaFiscalNumero", @maxlength = "12" }))%>
		</div>

		<div class="coluna60 prepend2">
			<label for="Modelo">Marca/Modelo *</label>
			<%= Html.TextBox("Modelo", Model.Motosserra.Modelo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtModelo", @maxlength = "80" }))%>
		</div>
	</div>
</div>

<fieldset class="block box" id="Proprietario_Container">
	<legend>Proprietário</legend>

	<div class="block">
		<div class="coluna53 append2">
			<label for="Proprietario_NomeRazaoSocial">Nome/Razão social *</label>
			<%= Html.Hidden("Proprietario.Id", Model.Motosserra.Proprietario.Id, new { @class = "hdnPessoaId" })%>
			<%= Html.TextBox("Proprietario.NomeRazaoSocial", Model.Motosserra.Proprietario.NomeRazaoSocial, new { @class = "text disabled txtPessoaNomeRazaoSocial", @disabled = "disabled" })%>
		</div>

		<div class="coluna20">
			<label for="Proprietario_CPFCNPJ">CPF/CNPJ *</label>
			<%= Html.TextBox("Proprietario.CPFCNPJ", Model.Motosserra.Proprietario.CPFCNPJ, new { @class = "text disabled txtPessoaCPFCNPJ", @disabled = "disabled" })%>
		</div>

		<div class="coluna15">
			<span class="spanVisualizarPessoa <%= (Model.Motosserra.Proprietario.Id > 0) ? "" : "hide" %>"><button type="button" class="icone visualizar inlineBotao btnVisualizarPessoa" title="Visualizar proprietário"></button></span>

			<% if(!Model.IsVisualizar) { %>
				<button type="button" title="Buscar proprietário" class="inlineBotao btnAssociarPessoa <%= (Model.Motosserra.Proprietario.Id > 0) ? "hide" : "" %>">Buscar</button>
				<span class="btnLimparContainerPessoa <%= Model.Motosserra.Proprietario.Id > 0 ? "" : "hide" %>"><button type="button" class="inlineBotao btnLimparPessoa">Limpar</button></span>
			<% } %>
		</div>
	</div>
</fieldset>