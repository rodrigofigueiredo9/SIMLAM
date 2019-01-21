<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<BarragemDispensaLicencaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Barragem para Dispensa de Licença Ambiental</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/barragemDispensaLicenca.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			BarragemDispensaLicenca.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "BarragemDispensaLicenca") %>',
					associar: '<%= Url.Action("AssociarCaracterizacaoProjetoDigital", "BarragemDispensaLicenca") %>',
					desassociar: '<%= Url.Action("DesassociarCaracterizacaoProjetoDigital", "Caracterizacao")%>',
					visualizar: '<%= Url.Action("Visualizar", "BarragemDispensaLicenca")%>',
					editar: '<%= Url.Action("Editar", "BarragemDispensaLicenca")%>',

				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>,
				projetoDigitalId: <%: Request.Params["projetoDigitalId"] %>,
				empreendimentoId: <%: Model.Caracterizacao.EmpreendimentoID %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoID %>" />
		<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
		<%=Html.Hidden("ProjetoDigitalId", Request.Params["ProjetoDigitalId"], new { @class="hdnProjetoDigitalId" })%>
		<h1 class="titTela">Barragens Dispensadas de Licenciamento Ambiental</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("ListagemAssociadaBarragem", Model);%>
			<%Html.RenderPartial("ListagemCadastradaBarragem", Model); %>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoID, projetoDigitalId = Request.Params["projetoDigitalId"] }) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>