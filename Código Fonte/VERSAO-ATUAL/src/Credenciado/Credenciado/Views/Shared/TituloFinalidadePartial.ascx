<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FinalidadeVM>" %>
<h1 class="titTela">Adicionar Título</h1>
<br />
<br />
<input type="hidden" value="<%= Model.AtividadeId%>" class="hdnAtividade" />
<fieldset class="block box">
	<div class="coluna30">
		<label for="Nome">Finalidade *</label>
		<%= Html.DropDownList("Finalidade", Model.Finalidades, new { @class = "text ddlFinalidade" })%>
	</div>
	<div class="coluna60">
		<label for="Nome">Título *</label>
		<%= Html.DropDownList("Titulo", Model.Titulos, new { @class = "text ddlTitulo" })%>
	</div>
</fieldset>

<fieldset class="block box modeloTituloConteudo hide">
	<legend>Título Anterior</legend>
	<div class="block">
		<label><%= Html.RadioButton("Emitido", 1, true, new { @class = "orgaoEmissor emitidoPorInterno" })%> Emitido pelo <%= Model.SiglaOrgao %></label>
		<label><%= Html.RadioButton("Emitido", 2, false, new { @class = "orgaoEmissor radio" })%> Emitido por outro órgão</label>
	</div>
	<div class="divOrgao">
		<div class="coluna60">
			<%/*Cuidado ao alterar label com class, pois estao sendo usados pelo js, ex: lbTitulo, lbNumero e lbNumerosDocumentoAnterior*/ %>
			<label for="Nome" class="lbTitulo">Título *</label>
			<%= Html.DropDownList("TituloAnterior", Model.TitulosAnterior, new { @class = "text ddltituloAnterior" })%>
			<input type="hidden" class="hdnFaseAnteriorObrigatoria" value="false" />
		</div>
		<div class="coluna10 divTxtNumero">
			<label for="Nome" class="lbNumero">Número *</label>
			<%= Html.TextBox("NumeroDocumento", null, new { @maxlength = "12", @class = "text txtNumeroDocAnterior" })%>
		</div>
		<div class="coluna25 divDdlNumero hide">
			<label for="Nome" class="lbNumerosDocumentoAnterior">Número *</label>
			<%= Html.DropDownList("DdlNumeroDocumento", Model.NumerosDocumentoAnterior , new { @class = "text ddlNumeroAnterior" })%>
		</div>
		<div class="coluna10">
			<button type="button" class="inlineBotao btnBuscarNumero" title="Buscar Números">Buscar</button>
			<button type="button" class="inlineBotao btnLimpar hide" title="Limpar">Limpar</button>
		</div>
	</div>
	<div class="divOrgaoExterno hide">
		<div>
			<%/*Cuidado ao alterar label com class, pois estao sendo usados pelo js, ex: lbTitulo, lbNumero e lbOrgaoExpedidor*/ %>
			<div class="coluna60">
				<label for="Nome" class="lbTitulo">Título *</label>
				<%= Html.TextBox("Titulo", null, new { @maxlength = "100", @class = "text txtTituloOrgaoExterno" })%>
			</div>
			<div class="coluna10">
				<label for="Nome" class="lbNumero">Número *</label>
				<%= Html.TextBox("Numero", null, new { @maxlength = "12", @class = "text txtNumeroOrgaoExterno" })%>
			</div>
		</div>
		<div>
			<div class="coluna60">
				<label for="Nome" class="lbOrgaoExpedidor">Órgão Expedidor *</label>
				<%= Html.TextBox("OrgaoExpedidor", null, new { @maxlength = "100", @class = "text txtOrgaoExpedidor" })%>
			</div>
		</div>
	</div>
</fieldset>