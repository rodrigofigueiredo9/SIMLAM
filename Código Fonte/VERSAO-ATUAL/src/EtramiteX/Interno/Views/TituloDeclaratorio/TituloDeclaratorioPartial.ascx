<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<%= Html.Hidden("TituloId", Model.Titulo.Id, new { @class = "hdnTituloId" })%>
<%= Html.Hidden("TituloSituacao", Model.Titulo.Situacao.Id, new { @class = "hdnTituloSituacao" })%>

<div class="block box">
	<div class="block">
		<div class="coluna75">
			<label for="AutorNome">Autor *</label>
			<%= Html.TextBox("AutorNome", Model.Titulo.Autor.Nome, new { @maxlength = "80", @class = "disabled text txtAutor", @disabled = "disabled" })%>
		</div>
		<div class="coluna18 prepend2">
			<label for="DataCriacao">Data de criação *</label>
			<%= Html.TextBox("DataCriacao", Model.Titulo.DataCriacao.DataTexto, new { @maxlength = "80", @class = "disabled text txtDataCriacao", @disabled = "disabled" })%>
		</div>	
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="LocalEmissao">Local da emissão *</label>
			<%= Html.DropDownList("LocalEmissao", Model.LstLocalEmissao, new { @class = "text ddlLocal" })%>
		</div>	
		<div class="coluna18 prepend2">
			<label for="SituacaoTexto">Situação *</label>
			<%= Html.TextBox("SituacaoTexto", Model.Titulo.Situacao.Texto, new { @maxlength = "80", @class = "disabled text txtSituacao", @disabled = "disabled" })%>
		</div>	
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Modelos">Modelo *</label>
			<% if(Model.Titulo.Id > 0) { %>
			<%= Html.DropDownList("Modelos", Model.LstModelos, new { @class = "text ddlModelos disabled", @disabled = "disabled" })%>
			<% } else { %>
			<%= Html.DropDownList("Modelos", Model.LstModelos, new { @class = "text ddlModelos" })%>
			<% } %>
		</div>
		<div class="coluna18 prepend2">
			<label for="SituacaoTexto">Origem *</label>
			<%= Html.TextBox("OrigemTexto", "Institucional", new { @class = "disabled text txtOrigem", @disabled = "disabled" })%>
		</div>	
	</div>
</div>

<div class="block box tituloRequerimento <%= (Model.Titulo.Modelo.Id > 0)?"":"hide" %>">
	<% Html.RenderPartial("TituloDeclaratorioRequerimento"); %>
</div>

<div class="divTituloEspContent">
</div>

<div class="tituloValoresModelo">
	<% Html.RenderPartial("TituloCamposModelo"); %>
</div>

<div class="block box btnTituloContainer">
	<input class="btnTituloSalvar floatLeft" type="button" value="Salvar" />
	<span class="cancelarCaixa">ou <a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
</div>