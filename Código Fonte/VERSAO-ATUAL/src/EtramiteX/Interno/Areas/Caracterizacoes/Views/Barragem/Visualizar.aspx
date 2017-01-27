<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<BarragemVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Barragem</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/barragem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	<script>
		$(function () {
			Barragem.load($('#central'), {
				urls: {
					criar: '<%= Url.Action("Criar", "Barragem") %>',
					editarBarragemItem: '<%= Url.Action("EditarBarragemItem", "Barragem") %>',
					visualizarBarragemItem: '<%= Url.Action("VisualizarBarragemItem", "Barragem") %>'
				},
				mensagens: <%= Model.Mensagens %>			
			});
		});
	</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Barragem</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("BarragemPartial", Model);%>
		</div>
		<div class="divBarragemItem">
		
		</div>
		<div class="block box">
			<span class="cancelarCaixa">
				<a class="linkCancelar" id="linkVoltar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Barragem.EmpreendimentoId}) %>">Voltar</a>
				<a class="linkCancelar hide" id="linkCancelar" href="#">Cancelar</a>
			</span>
		</div>
	</div>
</asp:Content>
