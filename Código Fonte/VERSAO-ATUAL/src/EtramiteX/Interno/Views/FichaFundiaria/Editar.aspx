<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<FichaFundiariaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editar Ficha Fundiária
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/FichaFundiaria/fichaFundiaria.js") %>"></script>
	<script>
		$(function () {
			FichaFundiaria.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "FichaFundiaria") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Editar Ficha Fundiária</h1>
		<br />

		<%Html.RenderPartial("FichaFundiaria", Model);%>


		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>
