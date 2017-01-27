<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PecaTecnicaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Gerar Peça Técnica
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%: Url.Content("~/Scripts/AnaliseItens/pecaTecnica.js") %>" ></script>

	<script>
		$(function () {
			PecaTecnica.load($('#central'), {
				urls: {
					obterPartialRequerimentos: '<%: Url.Action("ObterPecaTecnicaRequerimentos","AnaliseItens") %>',
					obterPartialAtividades: '<%: Url.Action("ObterPecaTecnicaAtividades","AnaliseItens") %>',
					obterPartialConteudo: '<%: Url.Action("ObterPecaTecnicaConteudo","AnaliseItens") %>',
					obterElaboradores: '<%: Url.Action("ObterPecaTecnicaElaboradores","AnaliseItens") %>',
					pdfRequerimento: '<%: Url.Action("GerarPdf","Requerimento") %>',
					pdfPecaTecnica: '<%:Url.Action("GerarPdfPecaTecnica","AnaliseItens") %>',
					obterSetores: '<%:Url.Action("ObterSetores","AnaliseItens") %>',
					salvarPecaTecnica: '<%: Url.Action("SalvarPecaTecnica","AnaliseItens") %>'
				}
			});

			PecaTecnica.mensagens = <%= Model.Mensagens %>;
			PecaTecnica.ElaboradorTipo = <%= Model.ElaboradorTipos %>;

		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Gerar Peça Técnica</h1>
		<br />
		<% Html.RenderPartial("PecaTecnicaPartial", Model); %>
	</div>

</asp:Content>

