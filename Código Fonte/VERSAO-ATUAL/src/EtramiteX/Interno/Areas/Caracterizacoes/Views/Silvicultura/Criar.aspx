<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SilviculturaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar Silvicultura</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/silvicultura.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/silviculturaSilvicult.js") %>"></script>
	
	<script>
		$(function () {
			Silvicultura.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "Silvicultura") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "Silvicultura") %>'
				}
			});

		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Salvar Silvicultura</h1>
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