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
			<label for="SetorCadastro">Setor de cadastro *</label>
			<% if (Model.SetoresEditar) { %>
				<%= Html.DropDownList("SetorCadastro", Model.LstSetores, new { @class = "disabled text ddlSetores", @disabled = "disabled" })%>
			<% } else  {%>
				<%= Html.DropDownList("SetorCadastro", Model.LstSetores, new { @class = "text ddlSetores" })%>
			<% } %>
		</div>	
		<div class="coluna18 prepend2">
			<label for="SituacaoTexto">Situação *</label>
			<%= Html.TextBox("SituacaoTexto", Model.Titulo.Situacao.Texto, new { @maxlength = "80", @class = "disabled text txtSituacao", @disabled = "disabled" })%>
		</div>	
	</div>
	
	<div class="block">
		<div class="coluna75">
			<label for="LocalEmissao">Local da emissão *</label>
			<%= Html.DropDownList("LocalEmissao", Model.LstLocalEmissao, new { @class = "text ddlLocal" })%>
		</div>	
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Modelos">Modelo *</label>
			<% if(Model.Titulo.Id > 0 || Model.LstModelos.Count <= 1) { %>
			<%= Html.DropDownList("Modelos", Model.LstModelos, new { @class = "text ddlModelos disabled", @disabled = "disabled" })%>
			<% } else { %>
			<%= Html.DropDownList("Modelos", Model.LstModelos, new { @class = "text ddlModelos" })%>
			<% } %>
		</div>
	</div>
</div>

<div class="block box tituloProtocolo <%= (Model.Titulo.Modelo.Id > 0)?"":"hide" %>">
	<% Html.RenderPartial("TituloProtocolo"); %>
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