<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoListarVM>" %>

<div class="dataGrid <%= ((Model.Resultados.Count > 0) ? string.Empty : "hide") %> ">
	<div class="block">
		<div class="coluna100">
			<h6 class="borderB padding0">Total de <strong><%= Model.Resultados.Count %></strong> <%= Model.QuantidadeItensTexto %></h6>
		</div>	</div>

	<% foreach (var item in Model.Resultados) { %>
		<div class="itemListaLarga block posRelative margem0">
			<div class="coluna77 border">
				<p class="margem0"><strong><%: item.Nome %></strong></p>
				<p class="quiet"><%: item.Descricao %></p>
			</div>
			<div class="ultima iconesListaRelatorios">
				<input type="hidden" class="itemId" value="<%= item.Id %>" />
				<% if(Model.PodeExecutar) { %><a class="icone opcoes btnExecutar" title="Gerar Relatório">Gerar Relatório</a><% } %>
				<% if(Model.PodeEditar) { %><a class="icone editar btnEditar" title="Editar">Editar</a><% } %>
				<% if(Model.PodeExcluir) { %><a class="icone excluir btnExcluir" title="Excluir">Excluir</a><% } %>
				<% if(Model.PodeExportar) { %><a class="icone selecEmpreendimento btnExportar" title="Exportar Relatório">Exportar Relatório</a><% } %>
				<% if(Model.PodeAtribuirExecutor) { %><a class="icone criarNovo btnAtribuirExecutor" title="Atribuir Executor">Atribuir Executor</a><% } %>
			</div>
		</div>
	<% } %>
</div>