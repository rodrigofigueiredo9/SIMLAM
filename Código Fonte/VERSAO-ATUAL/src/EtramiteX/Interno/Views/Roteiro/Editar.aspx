<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Roteiro Orientativo</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listar.js") %>"></script>
	<script>
		RoteiroSalvar.urlListarItem = '<%= Url.Action("ListarItem", "Roteiro") %>';
		RoteiroSalvar.urlAssociarRoteiroCopiar = '<%= Url.Action("Associar", "Roteiro") %>';
		RoteiroSalvar.urlRoteiroCopiar = '<%= Url.Action("CopiarRoteiro", "Roteiro") %>';
		RoteiroSalvar.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro","Roteiro") %>';
		RoteiroSalvar.urlValidarAssociarAtividade = '<%= Url.Action("ValidarAssociarAtividade", "Roteiro") %>';
		RoteiroSalvar.urlListarAtividade  = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
		RoteiroSalvar.urlObterModelosAtividades= '<%= Url.Action("ObterModelosAtividades", "Roteiro") %>';
		RoteiroSalvar.urlConfirmarEditar = '<%= Url.Action("ConfirmarEditar", "Roteiro") %>';
		RoteiroSalvar.Mensagens = <%= Model.Mensagens %>;
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">

	<h1 class="titTela">Editar Roteiro Orientativo</h1>
	<br />

	<% using (Html.BeginForm("Editar", "Roteiro", FormMethod.Post, new { @id = "RoteiroForm", enctype = "multipart/form-data" })) { %>
		<%= Html.Hidden("Roteiro.Id", Model.Roteiro.Id, new { @class = "hdnRoteiroId" })%>
		<%= Html.Hidden("Roteiro.Situacao", Model.Roteiro.Situacao, new { @class = "hdnSituacao" })%>
		<%= Html.Hidden("Roteiro.Versao", Model.Roteiro.Versao)%>
		<div class="block box">
			<div class="block">
				<div class="block">
					<div class="coluna23">
						<label for="Roteiro.Numero">Número *</label>
						<%= Html.TextBox("Roteiro.Numero", Model.Roteiro.Numero, new { @disabled = "disabled", @class = "text disabled RoteiroNumEditar txtNumero" })%>
					</div>
					<div class="coluna23 prepend1">
						<label for="Roteiro.Versao">Versão *</label>
						<%= Html.TextBox("Roteiro.Versao", Model.Roteiro.Versao, new { @disabled = "disabled", @class = "text disabled txtVersao" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna48" >
						<label for="Roteiro.Setor">Setor *</label>
						<%= Html.DropDownList("Roteiro.Setor", Model.Setores, new { @disabled = "disabled", @class = "text disabled ddlSetor" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna100">
						<label for="Roteiro.Nome">Nome *</label>
						<%= Html.TextBox("Roteiro.Nome", Model.Roteiro.Nome, new { @maxlength = "100", @class = "text  txtNome" })%>
					</div>
				</div>
			</div>
		</div>
	<div class="roteiroContent">

	<% Html.RenderPartial("RoteiroContentPartial"); %>

	</div>
	<div class="block box botoesSalvarCancelar">
		<div class="block">
			<button class="bntEditarRoteiro floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
<% } %>
</div>

</asp:Content>