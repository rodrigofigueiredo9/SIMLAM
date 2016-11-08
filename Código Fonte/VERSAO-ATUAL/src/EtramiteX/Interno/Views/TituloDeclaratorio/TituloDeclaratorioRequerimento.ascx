<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="block">
	<input type="hidden" class="hdnAtividadeEspecificidadeCaracterizacao" value='<%= Model.AtividadeEspecificidadeCaracterizacaoJSON %>' />
	<input type="hidden" class="hdnAtividadesID" value='<%= Model.AtividadesIDJSON %>' />

	<div class="coluna22">
		<label for="Numero">Número *</label>
		<%= Html.TextBox("Numero", Model.Titulo.Numero.Inteiro.GetValueOrDefault() > 0 ? Model.Titulo.Numero.Texto : "Gerado Automaticamente", new { @class = "disabled text txtNumero", @disabled = "disabled" })%>
	</div>
	<div class="coluna45 prepend1">
		<input type="hidden" class="hdnRequerimentoId" value="<%= Html.Encode(Model.Titulo.RequerimetoId) %>" />
		<label for="Requerimento">N° do requerimento *</label>
		<%= Html.TextBox("Requerimento", Model.Titulo.RequerimetoId > 0 ? Model.Titulo.RequerimetoId.ToString() : "", new { @class = "text disabled txtRequerimentoNumero", @disabled = "disabled" })%>
	</div>
	<div class="block ultima divBotoesRequerimento">
		<% if (!Model.IsVisualizar) { %>
			<input type="button" title="Buscar" value="Buscar" class="floatLeft inlineBotao btnBuscarRequerimento <%= (Model.Titulo.RequerimetoId > 0)?"hide":"" %>" />
			<div class="coluna10 prepend3"><input type="button" title="Limpar" value="Limpar" class="floatLeft inlineBotao btnLimparRequerimento <%= (Model.Titulo.RequerimetoId > 0)?"":"hide" %>" /></div>
		<% } %>
	</div>
</div>

<div class="block divEmpTitulo">
	<div class="coluna70">
		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(Model.Titulo.EmpreendimentoId) %>" />
		<label class="Empreendimento">Empreendimento</label>
		<%= Html.TextBox("Empreendimento", Model.Titulo.EmpreendimentoTexto, new { @class = "text disabled txtEmpDenominador", @disabled = "disabled" })%>
	</div>
</div>
