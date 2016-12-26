<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoVegetalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação do Ingrediente Ativo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/IngredienteAtivo/alterarSituacao.js") %>" ></script>

	<script>
		$(function () {
			IngredienteAtivoAlterarSituacao.load($('#central'), {
				urls: {
					alterarSituacao: '<%= Url.Action("AlterarSituacaoIngredienteAtivo", "ConfiguracaoVegetal") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Alterar Situação do Ingrediente Ativo</h1>
		<br />

		<input type="hidden" class="hdnIngredienteAtivoId" value="<%: Model.ConfiguracaoVegetalItem.Id %>" />
		<div class="block box">
			<div class="block ultima">
				<label for="Texto">Ingrediente ativo *</label>
				<%= Html.TextBox("Texto", Model.ConfiguracaoVegetalItem.Texto, new { @class = "disabled text", @disabled="disabled" })%>
			</div>

			<div class="block">
				<div class="coluna34">
					<label for="SituacaoTexto">Situação Atual *</label>
					<%= Html.TextBox("SituacaoTexto", Model.ConfiguracaoVegetalItem.SituacaoTexto, new { @class = "disabled text", @disabled="disabled" })%>
				</div>

				<div class="prepend2 coluna34">
					<label for="SituacaoNova">Nova situação *</label>
					<%= Html.DropDownList("SituacaoNova", Model.SituacoesLista, new {@class = "text ddlSituacaoNova"}) %>
				</div>
			</div>

			<div class="ultima divMotivo">
				<label for="Motivo">Motivo *</label>
				<%= Html.TextArea("Motivo", null, new { @class = "media text txtMotivo", @maxlength="250"})%>
			</div>
		</div>

		<div class="block box">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Model.UrlCancelar %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>