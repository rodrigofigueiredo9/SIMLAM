<%@ Import Namespace="Tecnomapas.Blocos.RelatorioPersonalizado.Entities" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<div class="block">
	<div class="coluna25">
		<label class="labelCheckBox">
			<input type="checkbox" class="cbContarRegistros" <%= Model.ConfiguracaoRelatorio.ContarRegistros ? "checked=\"checked\"" : "" %> />
			<span>Contar número de registros</span>
		</label>
	</div>
</div>
<div class="block">
	<table width="100%" border="0" cellspacing="0" cellpadding="0" class="dataGridTable ordenavel tabSumarios">
		<thead>
			<tr>
				<th class="semOrdenacao">Coluna</th>
				<th width="7%">Contar</th>
				<th width="7%">Somar</th>
				<th width="7%">Média</th>
				<th width="12%">Valor Máximo</th>
				<th width="12%">Valor Mínimo</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.ConfiguracaoRelatorio.CamposSelecionados) { %>
		<% Sumario sumario = Model.ConfiguracaoRelatorio.Sumarios.SingleOrDefault(x=> x.Campo.Id == item.Campo.Id) ?? new Sumario(); %>
			<tr>
				<td title="<%: item.Campo.DimensaoNome + " - " + item.Campo.Alias %>">
					<input type="hidden" class="itemId" value="<%= item.Campo.Id %>" />
					<%: item.Campo.DimensaoNome + " - " + item.Campo.Alias %>
				</td>
				<td><input type="checkbox" class="cbConta" <%= sumario.Contar ? "checked=\"checked\"" : "" %> /></td>
				<td><input type="checkbox" class="cbSoma" <%= sumario.Somar ? "checked=\"checked\"" : "" %> /></td>
				<td><input type="checkbox" class="cbMedia" <%= sumario.Media ? "checked=\"checked\"" : "" %> /></td>
				<td><input type="checkbox" class="cbMaximo" <%= sumario.Maximo ? "checked=\"checked\"" : "" %> /></td>
				<td><input type="checkbox" class="cbMinimo" <%= sumario.Minimo ? "checked=\"checked\"" : "" %> /></td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>