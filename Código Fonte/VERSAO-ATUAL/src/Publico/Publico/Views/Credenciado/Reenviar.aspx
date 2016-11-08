<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<PessoaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%if (Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado){ %>
		Atualizar E-mail do Credenciado
	<%}else{ %>
		Reenviar Chave de Acesso
	<%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/chaveReenviar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			ChaveReenviar.load($('#central'), {
				urls: {
					reenviar: '<%= Url.Action("Reenviar", "Credenciado") %>',
					atualizarDados: '<%= Url.Action("AtualizarDados", "Credenciado") %>'
				}
			});
		}); 
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Mensagem"); %>
		<h1 class="titTela"> 
			<%=Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado ? "Atualizar E-mail do Credenciado" : "Reenviar Chave de Acesso"%> 
		</h1>
		<br />

		<div class="block box">
			<div class="block">
				<div class="coluna25">
					<p><label for="Pessoa.Tipo">Tipo *</label></p>
					<label><%= Html.RadioButton("Pessoa.Tipo", PessoaTipo.FISICA, Model.Pessoa.Tipo != PessoaTipo.JURIDICA, new { @class = "radio pessoaf rdbPessoaTipo disabled", @disabled = "disabled" })%> Física</label>
					<label><%= Html.RadioButton("Pessoa.Tipo", PessoaTipo.JURIDICA, Model.Pessoa.Tipo == PessoaTipo.JURIDICA, new { @class = "radio pessoaj rdbPessoaTipo disabled", @disabled = "disabled" })%> Jurídica</label>
				</div>
			</div>
			<div class="block">
				<div class="coluna30 append2">
					<% if(Model.Pessoa.IsFisica) { %>
					<div class="CpfPessoaContainer <%= Model.Pessoa.Tipo != PessoaTipo.JURIDICA ? "" : "hide" %> ">
						<label for="Pessoa.Fisica.Cpf">CPF *</label>
						<%= Html.TextBox("Pessoa.Fisica.Cpf", Model.Pessoa.Fisica.CPF, new { @class = "text maskCpf inputCpfPessoa txtCpfCnpj disabled", @disabled = "disabled" })%>
					</div>
					<% } else { %>
					<div class="CnpjPessoaContainer <%= Model.Pessoa.Tipo == PessoaTipo.JURIDICA ? "" : "hide" %> ">
						<label for="Pessoa.Juridica.Cnpj">CNPJ *</label>
						<%= Html.TextBox("Pessoa.Juridica.Cnpj", Model.Pessoa.Juridica.CNPJ, new { @class = "text maskCnpj inputCnpjPessoa txtCpfCnpj disabled", @disabled = "disabled" })%>
					</div>
					<% } %>
				</div>

				<div class="coluna60">
					<label for="Contato.Email">E-mail *</label>
					<%= Html.TextBox("Contato.Email", Model.Credenciado.Email, new { @maxlength = "40", @class = "text maskEmail txtEmail" })%>
				</div>
			</div>
		</div>

		<div class="block box">
			<%if (Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado){ %>
				<input class="btnAtualizar floatLeft" type="button" value="Atualizar" />
			<%}else{ %>
				<input class="btnReenviar floatLeft" type="button" value="Reenviar" />
			<%} %>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Criar", "Credenciado") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>