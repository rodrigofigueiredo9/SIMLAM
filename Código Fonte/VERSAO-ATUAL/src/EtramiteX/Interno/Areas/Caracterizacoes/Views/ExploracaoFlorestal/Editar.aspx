﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ExploracaoFlorestalListVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Exploração Florestal</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/exploracaoFlorestal.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/exploracaoFlorestalExploracao.js") %>"></script>
	
	<script>
		$(function () {
			ExploracaoFlorestal.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "ExploracaoFlorestal") %>',
					getCodigoExploracao: '<%= Url.Action("GetCodigoExploracao", "ExploracaoFlorestal") %>'
				},
				idsTela: <%= Model.IdsTela %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Exploração Florestal</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("ExploracaoFlorestal", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.ExploracaoFlorestalVM.Count > 0 ? Model.ExploracaoFlorestalVM.FirstOrDefault().Caracterizacao.EmpreendimentoId : 0 }) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>