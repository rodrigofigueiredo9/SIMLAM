<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Roteiro Orientativo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		RoteiroSalvar.urlCriarRoteiro = '<%= Url.Action("Criar", "Roteiro") %>';
		RoteiroSalvar.urlListarItem  = '<%= Url.Action("ListarItem", "Roteiro") %>';
		RoteiroSalvar.urlAssociarRoteiroCopiar = '<%= Url.Action("Associar", "Roteiro") %>';
		RoteiroSalvar.urlRoteiroCopiar = '<%= Url.Action("CopiarRoteiro", "Roteiro") %>';
		RoteiroSalvar.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro","Roteiro") %>';
		RoteiroSalvar.urlValidarAssociarAtividade = '<%= Url.Action("ValidarAssociarAtividade", "Roteiro") %>';
		RoteiroSalvar.urlListarAtividade  = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
		RoteiroSalvar.urlObterModelosAtividades= '<%= Url.Action("ObterModelosAtividades", "Roteiro") %>';
		RoteiroSalvar.Mensagens = <%= Model.Mensagens %>;
	</script>

	<script>
		$(function () {
			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("RelatorioRoteiro", "Roteiro", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">

	<h1 class="titTela">Cadastrar Roteiro Orientativo</h1>
	<br />

<% using(Html.BeginForm("Criar", "Roteiro", FormMethod.Post, new { @id = "RoteiroForm", enctype = "multipart/form-data"})) { %>

	<div class="block box">
		<div class="block">
			<div class="block">
				<div class="coluna23">
					<label for="Roteiro.Numero">Número *</label>
					<%= Html.TextBox("Roteiro.Numero", "Gerado automaticamente", new { disabled = "disabled", @class = "text txtNumero disabled" })%>
				</div>
				<div class="coluna23 prepend1">
					<label for="Roteiro.Versao">Versão *</label>
					<%= Html.TextBox("Roteiro.Versao", "Gerado automaticamente", new { disabled = "disabled", @class = "text txtVersao disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna48">
					<label for="Roteiro.Setor">Setor *</label>
					<%= Html.DropDownList("Roteiro.Setor", Model.Setores, new { @maxlength = "", @class = "text ddlSetor"} )%>
				</div>
			</div>
			<div class="block">
				<div class="coluna100">
					<label for="Roteiro.Nome">Nome *</label>
					<%= Html.TextBox("Roteiro.Nome", Model.Roteiro.Nome, new { @maxlength = "150", @class = "text txtNome " })%>
				</div>
			</div>
		</div>
	</div>
<%= Html.Hidden("Roteiro.Id", Model.Roteiro.Id, new { @class = "hdnRoteiroId" })%>
<div class="roteiroContent <%= ((Model.CadastrandoItens)? "" : "hide"  )%>">
		<% Html.RenderPartial("RoteiroContentPartial"); %>
</div>

<div class="block box botoesSalvarCancelar <%= ((Model.CadastrandoItens)? "" : "hide"  )%>">
	<div class="block">
		<input class="bntSalvarRoteiro floatLeft" type="button" value="Salvar" />
		<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
	</div>
</div>

<div class="block box botoesNovoCopiar <%= ((Model.CadastrandoItens)? "hide" : "" )%>" >
	<button type="button" value="Novo" class="btnRoteiroNovo">Novo roteiro</button>
	<button type="button" value="Copiar Roteiro" class="btnRoteiroCopiar">Copiar roteiro</button>
</div>
	<% } %>
</div>

</asp:Content>