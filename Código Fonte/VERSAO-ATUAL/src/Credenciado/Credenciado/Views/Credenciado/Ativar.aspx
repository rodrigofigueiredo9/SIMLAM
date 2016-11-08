<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<CredenciadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Acesso Credenciado</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/ativar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/masterpage.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Lib/jquery-globalize/globalize.js")%>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Lib/jquery-globalize/cultures/globalize.culture.pt-BR.js")%>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Lib/mask/jquery.maskedinput.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Lib/mask/jquery.maskMoney.js") %>" ></script> 
	<script type="text/javascript">
	$(function(){ 
		CredenciadoAtivar.load($('#central'), {
			urls: {
				verificar: '<%= Url.Action("Verificar", "Credenciado") %>',
				salvar: '<%= Url.Action("GerenciarAcesso", "Credenciado") %>'
			}
		});
	});
	</script>

	<div id="central">
		<h1 class="titTela">Acesso Credenciado</h1>
		<br />

		<div class="block box">
			<div class="coluna90">
				<label for="Chave">Chave *</label>
				<%= Html.TextArea("Chave", Model.Chave, new { @class = "text media setarFoco txtChave", @maxlength = "128" })%>
				<input type="hidden" name="Credenciado.Situacao" id="Credenciado_Situacao" value="<%=Model.Credenciado.Situacao%>" />
			</div>

			<div class="coluna8">
				<button type="button" class="inlineBotao floatRight btnValidar">Validar</button>
			</div>
		</div>

		<div class="containerDados"></div>

		<div class="block box">
			<input class="btnSalvar hide floatLeft" type="button" value="<%= (Model.Credenciado.Situacao != 1) ? "Reativar" : "Ativar" %>" />
			<span class="cancelarCaixa"><span class="btnModalOu hide">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("LogOn", "Autenticacao") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>