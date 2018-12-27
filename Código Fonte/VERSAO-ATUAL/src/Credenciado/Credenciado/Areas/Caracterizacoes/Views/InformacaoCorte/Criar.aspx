﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<InformacaoCorteVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Informação de Corte</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorte.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			InformacaoCorte.load($('#central'), {
				urls: {
					criarInformacaoCorte: '<%= Url.Action("InformacaoCorteCriar", "InformacaoCorte") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">	
	<div id="central">
		<h2>Visualizar Informação de Corte</h2>
		</br>
		<div class="divCaracterizacao">
			<%Html.RenderPartial("InformacaoCorte", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Empreendimento.EmpreendimentoId , Model.ProjetoDigitalId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>