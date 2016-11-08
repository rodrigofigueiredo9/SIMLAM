<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnaliseItemVM>" %>

<div class="dataGrid">
	<table class="dataGridTable" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th width="15%">Data de criação</th>
				<th width="15%">Origem</th>
				<th width="10%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var item in Model.Requerimentos){ %>
			<tr class="cursorPointer <%=Model.RequerimentoSelecionado == item.Id ? "AnalisandoRequerimento" : "" %>">
				<td class="checkRequerimento">
					<%= Html.RadioButton("RequerimentoNumero", 1, (Model.RequerimentoSelecionado == item.Id), new { @class = "radio rdbRequerimentoNumero" })%>
					<label class="lbRequerimentoNumero" title="<%= Html.Encode(item.Numero) %>"><%= Html.Encode(item.Numero) %></label>
				</td>
				<td class="checkRequerimento">
					<span class="lbRequerimentoData" title="<%= Html.Encode(item.DataCadastro.ToShortDateString()) %>"><%= Html.Encode(item.DataCadastro.ToShortDateString()) %></span>
				</td>

				<td class="checkRequerimento">
					<span class="lbRequerimentoOrigem" title="<%= Html.Encode(item.Origem) %>"><%= Html.Encode(item.Origem) %></span>
				</td>

				<td>
					<input type="hidden" class="hdnAnaliseId" value="<%=Html.Encode(Model.AnaliseId) %>" />
					<input type="hidden" class="hdnProjetoDigitalId" value="<%=Model.ProjetoDigitalId%>" />
					<input type="hidden" class="hdnIsProjetoDigitalImportado" value="<%=Model.IsProjetoDigitalImportado.ToString().ToLower()%>" />
					<input type="hidden" class="hdnChecagemNumero" value="<%= Html.Encode(item.Checagem) %>" />
					<input type="hidden" class="hdnRequerimentoNumero" value="<%= Html.Encode(item.Numero) %>" />
					<input type="hidden" class="hdnRequerimentoId" value="<%= Html.Encode(item.Id) %>" />
					<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(item.ProtocoloId) %>" />
					<input type="hidden" class="hdnProtocoloTipo" value="<%= Html.Encode(item.ProtocoloTipo) %>" />
					<input type="hidden" class="hdnRequerimentoData" value="<%= Html.Encode(item.DataCadastro.ToShortDateString()) %>" />
					<input title="Atividades solicitadas" type="button" class="icone ativSolicitadas btnVisualizarAtividades" />
					<input title="PDF do requerimento" type="button" class="icone pdf btnPDFRequerimento" />
				</td>
			</tr>
			<%} %>
		</tbody>
	</table>
</div>