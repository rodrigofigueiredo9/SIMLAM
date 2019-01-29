<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Informação de Corte</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorteListar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			InformacaoCorte.load($('#central'), {
				urls: {
					adicionarInformacao: '<%= Url.Action("Criar", "InformacaoCorte") %>',
					editar: '<%= Url.Action("Editar", "InformacaoCorte") %>',
					visualizar: '<%= Url.Action("Visualizar", "InformacaoCorte") %>',
					visualizarAntigo: '<%= Url.Action("VisualizarAntigo", "InformacaoCorte") %>',
					excluirConfirm: '<%= Url.Action("ExcluirConfirm", "InformacaoCorte") %>',
					excluir: '<%= Url.Action("Excluir", "InformacaoCorte") %>',
					voltar: '<%= Url.Action("", "Caracterizacao", new { id = Model.Empreendimento.EmpreendimentoId, Model.ProjetoDigitalId }) %>'
				},
				empreendimentoID: '<%= Model.Empreendimento.EmpreendimentoId %>',
				projetoDigitalID: '<%= Model.ProjetoDigitalId %>',
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
			<input class="floatLeft btnVoltarInformacao" type="button" value="Voltar" />
			<input class="floatRight btnAdicionarInformacao" type="button" value="Incluir Informação de Corte" />
		</div>
	</div>
</asp:Content>