<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SilviculturaPPFFVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar silvicultura – programa produtor florestal (fomento)</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/silviculturaPPFF.js") %>"></script>
	
	<script>
		$(function () {
			SilviculturaPPFF.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "SilviculturaPPFF") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Salvar silvicultura – programa produtor florestal (fomento)</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("Silvicultura", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>