<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemRoteiroVM>" %>

<div class="modalAddItemRotCheck">

<h2 class="titTela"><%= (Model.ModoTela == 1 ? "Adicionar " : "Editar ")%> Item</h2>
<br />
<input type="hidden" class="modo" value="<%= Model.ModoTela%>" />
<div class="block box">
	<div class="block">
		<input type="hidden" class="tipo" value="<%= Model.ItemRoteiro.Tipo%>" />
		<div class="coluna30">
			<label for="Tipo">Tipo *</label>
			<label><%= Html.RadioButton("Tipo", RoteiroTipo.TECNICO, true, new { @class = "radio rdbItemTipo" })%>Técnico</label>
			<label class="append5"><%= Html.RadioButton("Tipo", RoteiroTipo.ADMINISTRATIVO, false, new { @class = "radio rdbItemTipo" })%>Adminstrativo</label>
		</div>
	</div>
	<div class="block">
		<div class="coluna100">
			<label for="ItemNome">Nome *</label>
			<%= Html.TextBox("Nome", Model.ItemRoteiro.Nome, new { @class = "text txtNome", @maxlength = "200" })%>
		</div>
	</div>
	<input type="hidden" value="" />
	<div class="block">
		<div class="coluna100">
			<label for="ItemProcedimento">Procedimentos de análise</label>
			<%= Html.TextArea("ProcedimentoAnalise", Model.ItemRoteiro.ProcedimentoAnalise, new { @class = "text txtItemProcAnalise", @maxlength = "500" })%>
		</div>
	</div>
</div>
</div>