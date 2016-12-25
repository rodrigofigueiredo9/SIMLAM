<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação de Título Declaratório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorio/alterarSituacao.js") %>" ></script>
	<script>
		$(function () {
			TituloAlterarSituacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio") %>',
					redirecionar: '<%= Url.Action("Index", "TituloDeclaratorio") %>'
			}
		});
	});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("AlterarSituacaoPartial", Model); %>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>