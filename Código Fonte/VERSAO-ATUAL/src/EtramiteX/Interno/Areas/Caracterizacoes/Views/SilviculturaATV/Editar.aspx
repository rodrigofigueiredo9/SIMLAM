<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SilviculturaATVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Silvicultura - Implantação da Atividade de Silvicultura (Fomento)</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/silviculturaAtv.js") %>"></script>	
	<script>
		$(function () {
			SilviculturaATV.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "SilviculturaATV") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "SilviculturaATV") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Silvicultura - Implantação da Atividade de Silvicultura (Fomento)</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("SilviculturaATV", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>