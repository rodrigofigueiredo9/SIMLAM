<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMHabilitarEmissaoCFOCFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<fieldset class="block box">
	<legend>Pragas</legend>
	<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">	
		<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="20%">Nome científico</th>
					<th width="20%">Nome comum</th>
					<th width="20%">Cultura</th>
					<th width="15%">Data inicial</th>
					<th width="15%">Data final</th>
				</tr>
			</thead>

			<tbody>
			<% foreach (var item in Model.Resultados) { %>
				<tr>
					<td>
						<label class="lblNomeCientifico" title="<%=item.Praga.NomeCientifico%>"><%=item.Praga.NomeCientifico%> </label>
					</td>
					<td>
						<label class="lblNomeComun" title="<%=item.Praga.NomeComum%>"><%=item.Praga.NomeComum%> </label>
					</td>
					<td>
						<label class="lblCultura" title="<%=item.Cultura%>"><%=item.Cultura%> </label>
					</td>
					<td>
						<label class="lblDataInicialHabilitacao" title="<%=item.DataInicialHabilitacao%>"><%=item.DataInicialHabilitacao%> </label>
					</td>
					<td>
						<label class="lblDataFinalHabilitacao" title="<%=item.DataFinalHabilitacao%>"><%=item.DataFinalHabilitacao%> </label>
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>