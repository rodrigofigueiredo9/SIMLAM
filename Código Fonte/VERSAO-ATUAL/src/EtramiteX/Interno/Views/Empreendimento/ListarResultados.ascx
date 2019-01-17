<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels"%>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>"%>

<input type="hidden" class="paginaAtual" value=""/>
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>"/>

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="17%">Código</th>
				<th width="17%">Segmento</th>
				<th>Razão social/Denominação/Nome</th>
				<th width="15%">CNPJ</th>
				<th class="semOrdenacao" width=" <%= (Model.PodeAssociar) ? "9%" : "17%" %>">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="itemCodigo" title="<%= Html.Encode(item.Codigo.GetValueOrDefault().ToString("N0"))%>"> <%= Html.Encode(item.Codigo.GetValueOrDefault().ToString("N0"))%></td>
				<td title="<%= Html.Encode(item.SegmentoTexto)%>"><%= Html.Encode(item.SegmentoTexto)%></td>
				<td title="<%= Html.Encode(item.Denominador)%>" ><%= Html.Encode(item.Denominador)%></td>
				<td title="<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CNPJ))%>" ><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CNPJ))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.Denominador %>" class="itemDenominador" />
					<input type="hidden" value="<%= item.CNPJ %>" class="itemCnpj" />
					
					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button" title="Editar" class="icone editar btnEditar"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
					<%if (Model.PodeCaracterizar){%><button type="button" title="Caracterização" class="icone caracterizacao btnCaracterizacao"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>
