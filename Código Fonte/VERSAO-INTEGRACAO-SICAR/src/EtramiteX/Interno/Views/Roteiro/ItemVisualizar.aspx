<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ItemRoteiroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Item de Roteiro</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Item de Roteiro</h1>
		<br />

		<div class="block box">
			<div class="block">
				<div class="coluna40">
					<p><label for="Item.Tipo">Tipo *</label></p>
					<label><%= Html.RadioButton("Tipo", RoteiroTipo.TECNICO, Model.ItemRoteiro.Tipo != RoteiroTipo.ADMINISTRATIVO, new { @class = "radio disabled", @disabled = "disabled" })%> Técnico</label>
					<label class="append5"><%= Html.RadioButton("Tipo", RoteiroTipo.ADMINISTRATIVO, Model.ItemRoteiro.Tipo == RoteiroTipo.ADMINISTRATIVO, new { @class = "radio disabled", @disabled = "disabled" })%> Administrativo</label>
				</div>
			</div>

			<div class="block">
				<div class="coluna70">
					<label>Nome *</label>
					<%= Html.TextBox("Item_Nome", Model.ItemRoteiro.Nome, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna70">
					<label>Condicionante </label>
					<%= Html.TextBox("Item_Condicionante", Model.ItemRoteiro.Condicionante, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna100">
					<label>Procedimentos de análise</label>
					<%= Html.TextArea("ProcedimentoAnalise", Model.ItemRoteiro.ProcedimentoAnalise, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</div>

		<div class="block box">
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("IndexItem", "Roteiro") %>">Cancelar</a>
		</div>
	</div>
</asp:Content>