<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Alterar Situação
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">
	<h1 class="titTela">Alterar Situação</h1>
	<br />

	<% using(Html.BeginForm(new { id = Model.Id})){ %>

	<div class="block box">
		<div class="block">
			<div class="coluna98">
				<div class="coluna30">
					<label for="CpfFuncionario">Nome</label>
					<%= Html.TextBox("Nome", Model.Nome, new { @class = "text disabled", disabled = true })%>
				</div>
				<div class="coluna30">
				</div>
			</div>
			<div class="coluna98">
				<div class="coluna30">
					<label for="CpfFuncionario">CPF</label>
					<%= Html.TextBox("CPF", Model.Cpf, new { @class = "text disabled", disabled = true })%>
				</div>
				<div class="coluna30">
				</div>
			</div>
			<div class="coluna98">
				<div class="coluna30">
					<label>Situação atual</label>
				</div>
				<div class="coluna30">
					<label>Nova situação*</label>
				</div>
			</div>
			<div class="coluna98">
				<div class="coluna30">
					<%= Html.TextBox("Situacao", Model.Situacao, new { @class = "text disabled", disabled = true })%>
				</div>
				<div class="coluna30">
					<%= Html.DropDownList("NovaSituacao", Model.Situacoes, new { @class = "selectLista" })%>
				</div>
			</div>

			<% bool isAusente = Model.SituacaoId == 4 || Model.NovaSituacaoId == 4; %>
			<div id="divMotivo" class="coluna60 <%= isAusente ? "" : "hide" %>">
				<label>Motivo*</label>
				<% if (Model.SituacaoId == 4) { %>
					<%= Html.TextBox("Motivo", Model.Motivo, new { maxlength = "80", @class = "text disabled", @disabled = "disabled" })%>
				<% } else { %>
					<%= Html.TextBox("Motivo", Model.Motivo, new { maxlength = "80", @class = "text" })%>
				<%} %>
			</div>
		</div>
			
	</div>

	<div class="block box">
		<input id="salvar" type="submit" value="Salvar" class="floatLeft" />
		<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Index") %>" >Cancelar</a></span>
	</div>

	<% } %>

</div>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Funcionario/alterarSituacao.js") %>" ></script>

<script type="text/javascript">
	$(function () {
		$('#NovaSituacao').focus();
	});
	</script>

</asp:Content>
