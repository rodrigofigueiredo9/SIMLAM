<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<InformacaoCorteVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Informação de Corte</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorte.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			InformacaoCorte.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Salvar", "InformacaoCorte") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">	
	<div id="central">
		<h1 class="titTela">Visualizar Informação de Corte</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("InformacaoCortePartial", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("Listar", "InformacaoCorte", new { id = Model.Empreendimento.EmpreendimentoId , Model.ProjetoDigitalId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>