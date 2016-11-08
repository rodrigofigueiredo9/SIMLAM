<%@ Import Namespace="Tecnomapas.Blocos.Etx.ModuloCore.Entities" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Paginacao>" %>

<div class="dataGridControle">
	<div class="dcCaixa">
		<%= Html.DropDownList("Paginacao.QuantPaginacao", Model.ListaQuantPaginacao, new { @class = "selectLista comboItensPorPagina quantPaginacao", type = "text" })%>
		<label for="numListar">Itens por página</label>
	</div>

	<div id="totalRegistros" class="dcCaixa">
		<span> Total de <strong><%= Html.Encode(Model.QuantidadeRegistros)%></strong> itens encontrados</span>
	</div>

	<div class="paginacaoCaixa">
		<a class="1 paginar comeco" title="Primeira Página">Primeira página</a>
		<a class="<%= Math.Max(1, Model.PaginaAtual-1).ToString()  %> paginar anterior" title="Página Anterior">Página anterior</a>

		<span class="numerosPag">
			<% for (int i = Model.PaginaInicial; i <= Model.PaginaFinal; i++) { %>
				<a class="<%= i.ToString() %> paginar pag <%= ((Model.PaginaAtual == (i))?"ativo":string.Empty) %>"><%= Html.Encode(i)%></a>
			<% } %>
		</span>

		<a class="<%= Math.Min(Model.NumeroPaginas, Model.PaginaAtual+1).ToString()  %> paginar proxima" title="Próxima Página">Próxima página</a>
		<a class="<%= Html.Encode(Model.NumeroPaginas.ToString()) %> paginar final" title="Ultima Página">Ultima página</a>
	</div>
</div>