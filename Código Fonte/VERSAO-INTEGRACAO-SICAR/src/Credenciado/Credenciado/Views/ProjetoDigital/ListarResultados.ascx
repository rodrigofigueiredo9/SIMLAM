<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Número</th>
				<th>Interessado</th>
				<th>Empreendimento</th>
				<th width="13%">Situação</th>
				<th class="semOrdenacao" width="17%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%: item.RequerimentoId %>"><%: item.RequerimentoId %></td>
				<td title="<%: item.Interessado %>"><%: item.Interessado %></td>
				<td title="<%: item.EmpreendimentoTexto %>"><%: item.EmpreendimentoTexto %></td>
				<td title="<%: item.SituacaoTexto%>"><%: item.SituacaoTexto%></td>
				<td> 
					<input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />

					<% if (Model.PodeVisualizar && Model.PodeAssociar) { %><input type="button" title="Visualizar requerimento" class="icone visualizar btnVisualizarRequerimento" /><% } %>
					<% if ((Model.PodeEditar || Model.PodeVisualizar) && !Model.PodeAssociar) { %><input type="button" title="Operar" class="icone opcoes btnOperar" /><% } %>
					<% if (Model.PodeExcluir && !Model.PodeAssociar) { %><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
					<% if (item.Situacao == (int)eProjetoDigitalSituacao.AguardandoImportacao && !Model.PodeAssociar) { %><input type="button" title="Cancelar envio" class="icone cancelar btnCancelarEnvio" /><% } %>
					<% if (Model.PodeVisualizar && !Model.PodeAssociar) { %><input type="button" title="Documentos gerados" class="icone anexos btnDocumentosGerados" /><% } %>
					<% if (!string.IsNullOrEmpty(item.MotivoRecusa) && !Model.PodeAssociar)	{ %><input type="button" title="Notificação para correção" class="icone pendencias btnNotificacaoCorrecao" /><% } %>
					<%if (Model.PodeAssociar){%><input type="button" class="icone associar btnAssociar" title="Associar"/><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>