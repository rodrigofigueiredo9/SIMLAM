<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="20%">Requerente</th>
				<th width="14%">Documento</th>
				<th width="18%">Município</th>
				<th width="13%">Protocolo geral</th>
				<th width="17%">Protocolo regional</th>
				<th class="semOrdenacao" width="18%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="FichaFundiariaRequerente" title="<%= Html.Encode(item.Requerente.Nome)%>"><%= Html.Encode(item.Requerente.Nome)%></td>
				<td class="FichaFundiariaDocumento" title="<%= Html.Encode(item.Requerente.DocumentoTipoNumero)%>"><%= Html.Encode(item.Requerente.DocumentoTipoNumero)%></td>
				<td class="FichaFundiariaMunicipio" title="<%= Html.Encode(item.Terreno.Municipio)%>"><%= Html.Encode(item.Terreno.Municipio)%></td>
				<td class="FichaFundiariaProtocoloGeral" title="<%= Html.Encode(item.ProtocoloGeral)%>"><%= Html.Encode(item.ProtocoloGeral)%></td>
				<td class="FichaFundiariaProtocoloRegional" title="<%= Html.Encode(item.ProtocoloRegional)%>"><%= Html.Encode(item.ProtocoloRegional)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<%if (Model.PodeVisualizar) { %><input type="button" title="PDF" class="icone pdf btnGerarPdf" /><%}%>
					<%if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><%}%>
					<%if (Model.PodeEditar) { %><input type="button" title="Editar" class="icone editar btnEditar" /><%}%>
					<%if (Model.PodeExcluir) { %><input type="button" title="Excluir" class="icone excluir btnExcluir" /><%}%>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>