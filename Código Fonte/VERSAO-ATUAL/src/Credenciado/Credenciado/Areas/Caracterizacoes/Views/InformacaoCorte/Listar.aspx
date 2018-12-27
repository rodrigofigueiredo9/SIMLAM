<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Informação de Corte</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorte.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			InformacaoCorte.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "InformacaoCorte") %>'
				},
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">	
	<div id="central">
		<h1 class="titTela">Visualizar Informação de Corte</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("ListarPartial", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnVoltarInformacao" type="button" value="Voltar" onclick="window.history.back();"/>
			<input class="floatRight btnAdicionarInformacao" type="button" value="Incluir Informação de Corte" />
		</div>
	</div>
</asp:Content>