<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<UnidadeConsolidacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Unidade de Consolidação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Unidade de Consolidação</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("UnidadeConsolidacaoPartial", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.UnidadeConsolidacao.Empreendimento.Id, projetoDigitalId = Request.Params["projetoDigitalId"],visualizar = Model.RetornarVisualizar}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
