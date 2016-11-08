<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Fichas Fundiárias</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlGerarPdf", Url.Action("GerarPdf"), new { @class = "urlGerarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna40 append2">
					<label for="Filtros_ProtocoloGeral">Protocolo geral</label>
					<%= Html.TextBox("Filtros.ProtocoloGeral", String.Empty, new { @class = "text txtProtocoloGeral", @maxlength="80" })%>
				</div>

				<div class="coluna40 append1">
					<label for="Filtros_ProtocoloRegional">Protocolo regional</label>
					<%= Html.TextBox("Filtros.ProtocoloRegional", String.Empty, new { @class = "text txtProtocoloRegional", @maxlength = "80" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna40 append2">
					<label for="Filtros_TipoDocumento">Tipo do documento</label>
					<%= Html.TextBox("Filtros.DocumentoTipo", String.Empty, new { @class = "text txtDocumentoTipo", @maxlength = "150" })%>
				</div>

				<div class="coluna40">
					<label for="Filtros_DocumentoNumero">Número do documento</label>
					<%= Html.TextBox("Filtros.DocumentoNumero", String.Empty, new { @class = "text txtDocumentoNumero", @maxlength = "80" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna40 append2">
					<label for="Filtros_Municipio">Município</label>
					<%= Html.TextBox("Filtros.Municipio", String.Empty, new { @class = "text txtMunicipio", @maxlength = "150" })%>
				</div>

				<div class="coluna40">
					<label for="Filtros_Distrito">Distrito</label>
					<%= Html.TextBox("Filtros.Distrito", String.Empty, new { @class = "text txtDistrito", @maxlength = "150" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna83">
					<label for="Filtros_Requerente">Nome do requerente</label>
					<%= Html.TextBox("Filtros.Requerente", String.Empty, new { @class = "text txtRequerente", @maxlength = "150" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>