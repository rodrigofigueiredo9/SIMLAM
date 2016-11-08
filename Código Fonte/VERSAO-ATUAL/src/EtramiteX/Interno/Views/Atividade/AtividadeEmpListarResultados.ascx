<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<%= Html.Hidden("Paginacao.PaginaAtual", null, new { @class = "paginaAtual" })%>
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<div class="dataGridControle">
		<div class="dcCaixa">
		<%= Html.DropDownList("Paginacao.QuantPaginacao", Model.ListaQuantPaginacao, new { @class = "selectLista comboItensPorPagina quantPaginacao", type = "text" })%>
			<label for="numListar">Itens por página</label>	
		</div>

		<div id="totalRegistros" class="dcCaixa">
			<span> Total de <strong><%= Html.Encode(Model.Paginacao.QuantidadeRegistros)%></strong> itens encontrados</span>
		</div>

		<div class="paginacaoCaixa">
			<a class="1 paginar comeco" title="Primeira Página">Primeira página</a>
			<a class="<%= Math.Max(1, Model.Paginacao.PaginaAtual-1).ToString()  %> paginar anterior" title="Página Anterior">Página anterior</a>

			<span class="numerosPag">
				<% for (int i = Model.Paginacao.PaginaInicial; i <= Model.Paginacao.PaginaFinal; i++) { %>
					<a class="<%= i.ToString() %> paginar pag <%= ((Model.Paginacao.PaginaAtual == (i))?"ativo":string.Empty) %>">
						<%= Html.Encode(i)%>
					</a>
					<% } %>
			</span>

			<a class="<%= Math.Min(Model.Paginacao.NumeroPaginas, Model.Paginacao.PaginaAtual+1).ToString()  %> paginar proxima" title="Próxima Página">Próxima página</a>
			<a class="<%= Html.Encode(Model.Paginacao.NumeroPaginas.ToString()) %> paginar final" title="Ultima Página">Ultima página</a>
		</div>
	</div>
	
	<table class="dataGridTable ordenavel" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">CNAE</th>
				<th>Atividade</th>
				<th class="semOrdenacao" width="7%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% for (int i = 0; i < Model.Resultados.Count; i++){ %>
			<tr>
				<td class="atividadeEmpCNAE" title="<%= Html.Encode(Model.Resultados[i].CNAE)%>"><%= Html.Encode(Model.Resultados[i].CNAE)%></td>
				<td class="atividadeEmpTexto" title="<%= Html.Encode(Model.Resultados[i].Atividade)%>"><%= Html.Encode(Model.Resultados[i].Atividade)%></td>
				<td>
					<%= Html.HiddenFor(x => x.Resultados[i].Id, new { @class="atividadeEmpId" } )%>
					<input type="button" name="associar" title="Associar" class="icone associar inlineBotao btnAssociarAtividadeEmp"/>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>