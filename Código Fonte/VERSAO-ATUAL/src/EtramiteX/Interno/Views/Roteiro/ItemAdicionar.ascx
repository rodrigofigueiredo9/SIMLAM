<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemRoteiroVM>" %>

<h1 class="titTela"><%= Model.ModoTela == 2 ? "Editar Item de Roteiro" : "Cadastrar Item de Roteiro"%></h1>
<br />

<% if(Model.IsModal) { %>
	<script type="text/javascript">
		$(function () {
			ItemSalvar.urlSalvarItem  = '<%= Url.Action("SalvarItemModal", "Roteiro") %>';
			ItemSalvar.urlModalConfirmar = '<%= Url.Action("ItemConfirmar", "Roteiro") %>';
			ItemSalvar.urlAtualizaRoteiro = '<%= Url.Action("AtualizarRoteiro", "Roteiro") %>';
			ItemSalvar.Mensagens = <%= Model.Mensagens %>;
		});
	</script>
<% } else { %>
	<script type="text/javascript">
		$(function () {
			ItemSalvar.urlSalvarItem = '<%= Url.Action("SalvarItem", "Roteiro") %>';
			ItemSalvar.urlModalConfirmar = '<%= Url.Action("ItemConfirmar", "Roteiro") %>';
			ItemSalvar.urlAtualizaRoteiro = '<%= Url.Action("AtualizarRoteiro", "Roteiro") %>';
		});
	</script>
<% } %>

<input type="hidden" class="hdnAnaliseId" value="<%= Html.Encode(Model.ItemRoteiro.AnaliseNumero) %>" />
<input type="hidden" class="hdnItemNumero" value="<%= Html.Encode(Model.ItemRoteiro.Id) %>" />
<input type="hidden" class="hdnEditar" value="<%= Html.Encode(Model.ModoTela == 2) %>" />
<input type="hidden" class="hdnTipo" value="<%= Html.Encode(Model.ItemRoteiro.Tipo) %>" />
<input type="hidden" class="hdnTid" value="<%= Html.Encode(Model.ItemRoteiro.Tid) %>" />

<div class="box">
	<div class="block">
		<div class="coluna40">
			<p><label for="Item.Tipo">Tipo *</label></p>
			<label><%= Html.RadioButton("Tipo", RoteiroTipo.TECNICO, Model.ItemRoteiro.Tipo != RoteiroTipo.ADMINISTRATIVO, new { @class = "radio rdbItemTipoTec rdbItemTipo" })%> Técnico</label>
			<label class="append5"><%= Html.RadioButton("Tipo", RoteiroTipo.ADMINISTRATIVO, Model.ItemRoteiro.Tipo == RoteiroTipo.ADMINISTRATIVO, new { @class = "radio rdbItemTipo" })%> Administrativo</label>
		</div>
	</div>

	<div class="block">
		<div class="coluna70">
			<label>Nome *</label>
			<%= Html.TextBox("Item_Nome", Model.ItemRoteiro.Nome, new { @class = "text Nome", @maxlength = "200" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna70">
			<label>Condicionante </label>
			<%= Html.TextBox("Item_Condicionante", Model.ItemRoteiro.Condicionante, new { @class = "text txtCondicionante", @maxlength = "200" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna100">
			<label>Procedimentos de análise</label>
			<%= Html.TextArea("ProcedimentoAnalise", Model.ItemRoteiro.ProcedimentoAnalise, new { @class = "text Procedimento", @maxlength = "500" })%>
		</div>
	</div>
</div>