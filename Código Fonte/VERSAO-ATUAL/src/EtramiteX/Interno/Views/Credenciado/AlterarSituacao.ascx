<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlterarSituacaoVM>" %>

<div class="credAlterarSituacao">

	<h1 class="titTela">Alterar Situação</h1>
	<br />


	<input type="hidden" class="credenciadoId" name="credenciadoId" value="<%= Model.Id %>"  />

	<div class="block box">
		<div class="block">
			<div class="coluna98">
				<div class="coluna30">
					<label for="nomeFuncionario">Nome</label>
					<%= Html.TextBox("Nome", Model.Nome, new { @class = "text disabled nomeCred", disabled = true })%>
				</div>
				<div class="coluna30">
					<label for="CpfFuncionario">CPF/CNPJ</label>
					<%= Html.TextBox("CPFCNPJ", Model.CpfCnpj, new { @class = "text disabled", disabled = true })%>
				</div>
			</div>
			<div class="coluna98">
				<div class="coluna30">
					<label>Situação atual</label>
					<%= Html.TextBox("Situacao", Model.Situacao, new { @class = "text disabled", disabled = true })%>
				</div>
				<div class="coluna30">
					<label>Nova situação*</label>
					<%= Html.DropDownList("NovaSituacao", Model.Situacoes, new { @class = "selectLista novaSituacaoCred" })%>
				</div>
			</div>
			<div id="divMotivo" class="coluna60">
				<label>Motivo*</label>
				<%= Html.TextBox("Motivo", Model.Motivo, new { @maxlength = "80", @class = "text motivoCred" })%>
			</div>
		</div>
	</div>
</div>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/alterarSituacao.js") %>" ></script>

<script type="text/javascript">
	$(function () {
		$('#NovaSituacao').focus();
	});
	</script>