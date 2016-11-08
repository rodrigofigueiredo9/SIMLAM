<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Entregar Título</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/entrega.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">

		$(function () {
			Entrega.urlIncial = '<%= Url.Action("Entregar", "Titulo") %>';
			Entrega.urlSalvar = '<%= Url.Action("EntregarCriar", "Titulo") %>';
			Entrega.urlGerarPdfTitulo = '<%= Url.Action("GerarPdf", "Titulo") %>';
			Entrega.urlObterTitulosProcesso = '<%= Url.Action("ObterTitulosProcesso", "Titulo") %>';

			Entrega.urlObterProcessoTipos = '<%= Url.Action("ObterProcessoTipos", "Titulo") %>';
			Entrega.urlObterDocumentoTipos = '<%= Url.Action("ObterDocumentoTipos", "Titulo") %>';
			Entrega.urlObterTituloNumeroConcatenados = '<%= Url.Action("ObterTitulosSitucaoAssinado", "Titulo") %>';
			Entrega.urlEntregaConfirm = '<%= Url.Action("EntregaConfirm", "Titulo") %>';

			Entrega.urlObterPessoaEntrega = '<%= Url.Action("ObterPessoaEntrega", "Titulo") %>';
			Entrega.Mensagens = <%= Model.Mensagens %>;
			Entrega.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdfEntrega", "Titulo", new { id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Entregar Título</h1>
		<br />

		<div class="divConteudo">
			<% Html.RenderPartial("EntregaPartial"); %>
		</div>

		<div class="block box btnTituloContainer">
			<input class="btnEntregaSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Index") %>">Cancelar</a></span>
			<input class="btnGerarPdf floatRight hide" type="button" value="Gerar Pdf" />
		</div>
	</div>
</asp:Content>