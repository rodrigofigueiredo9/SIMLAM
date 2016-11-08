<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMAgrotoxico" %>

<h1 class="titTela">Consultar Agrotóxicos</h1>
<br />
<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>	
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna88 append1">
					<label for="Filtros_NomeComercial">Nome comercial</label>
					<%= Html.TextBox("Filtros.NomeComercial", null, new { @class = "text txtNomeComercial"})%>
				</div>
				<div class="coluna5">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna28 append1">
					<label for="NumeroCadastro">Nº do cadastro</label>
					<%= Html.TextBox("Filtros.NumeroCadastro", null, new { @class = "text txtNumeroCadastro maskNumInt", @maxlength="15"} )%>
				</div>

				<div class="coluna28 append1">
					<label for="NumeroRegistroMinisterio">Nº do registro no ministério</label>
					<%= Html.TextBox("Filtros.NumeroRegistroMinisterio", null, new { @class = "text txtNumeroRegistroMinisterio maskNumInt", @maxlength="30"})%>
				</div>

				<div class="coluna27">
					<label for="Filtros_Situacao">Situação do agrotóxico</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.Situacao, new { @class = "text ddlSituacao" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna58 append1">
					<label for="Filtros_TitularRegistro_NomeRazaoSocial">Titular do registro</label>
					<%= Html.TextBox("Filtros.TitularRegistro", null, new { @class = "text"})%>
				</div>

				<div class="coluna27">
					<label for="NumeroProcessoSep">Nº do processo SEP</label>
					<%= Html.TextBox("Filtros.NumeroProcessoSep", null, new { @class = "text txtNumeroProcessoSep"})%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna88">
					<label for="Filtros_IngredienteAtivo">Ingrediente ativo</label>
					<%= Html.TextBox("Filtros.IngredienteAtivo", null, new { @class = "text txtIngredienteAtivo"})%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna88">
					<label for="Filtros_Cultura">Cultura</label>
					<%= Html.TextBox("Filtros.Cultura", null, new { @class = "text txtCultura"})%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna88">
					<label for="Filtros_Praga">Praga</label>
					<%= Html.TextBox("Filtros.Praga", null, new { @class = "text txtPraga"})%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna43 append1">
					<label for="Filtros_ClasseUso">Classe de Uso</label>
					<%= Html.DropDownList("Filtros.ClasseUso", Model.ClasseUso, new { @class = "text ddlClasseUso" })%>
				</div>

				<div class="coluna43">
					<label for="Filtros_ModalidadeAplicacao">Modalidade de aplicação</label>
					<%= Html.DropDownList("Filtros.ModalidadeAplicacao", Model.ModalidadeAplicacao, new { @class = "text ddlModalidadeAplicacao" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna43 append1">
					<label for="Filtros_GrupoQuimico">Grupo Quimíco</label>
					<%= Html.DropDownList("Filtros.GrupoQuimico", Model.GrupoQuimico, new { @class = "text ddlGrupoQuimico" })%>
				</div>

				<div class="coluna43">
					<label for="Filtros_ClassificacaoToxicologica">Classificação toxicológica</label>
					<%= Html.DropDownList("Filtros.ClassificacaoToxicologica", Model.ClassificacaoToxicologica, new { @class = "text ddlClassificacaoToxicologica" })%>
				</div>
			</div>
		</div>
	</div>
</div>
<div class="gridContainer"></div>