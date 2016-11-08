<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<PTVOutro>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cancelar PTV de Outro Estado</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTVOutro/cancelar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			PTVCancelar.load($("#central"), {
				urls: {
					urlSalvar: '<%= Url.Action("PTVCancelar", "PTVOutro") %>'
				},
			});
		});
	</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<%Html.RenderPartial("CancelarPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTVOutro") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>