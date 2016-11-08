<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TramitacoesVM>" %>

<div class="visualizarTramitacaoPartial">
	<h1 class="titTela">Tramitações</h1>
	<br />

	<div class="block box">
		<input type="hidden" class="hdnMostrarSetor" value="<%= Html.Encode(Model.MostrarSetor) %>" />
		<input type="hidden" class="hdnMostrarFunc" value="<%= Html.Encode(Model.MostrarFuncionario) %>" />
		<input type="hidden" class="hdnSetorId" value="<%= Html.Encode(Model.SetorId) %>" />
		<input type="hidden" class="hdnFuncionario" value="<%= Html.Encode(Model.FuncionarioId) %>" />
		
		<div class="coluna48 divSetor">
			<label>Setor *</label>
			<%= Html.DropDownList("SetorId", Model.Setores, new { @class = "text ddlSetor" })%>
		</div>

		<div class="coluna45 prepend2 divFunc">
			<label>Funcionário*</label>
			<%= Html.DropDownList("FuncionarioId", Model.Funcionarios, new { @class = "text ddlFuncionario" })%>
		</div>
	</div>

	<div class="divConteudoTramitacao">
		<% Html.RenderPartial("TramitacaoConteudoPartial", Model.Tramitacao); %>
	</div>
</div>