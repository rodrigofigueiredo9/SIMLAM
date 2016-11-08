<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PecaTecnicaVM>" %>

<fieldset class="block box divRequerimentoPadrao">
	<legend>Requerimento Padrão</legend>
	<div class="coluna70">
		<table class="dataGridTable tabRequerimento" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Número</th>
					<th width="20%">Data de criação</th>
					<th width="15%">Ações</th>
				</tr>
			</thead>
			<tbody>
			<% foreach (var item in Model.Requerimentos) { %>
				<tr>
					<td title="<%= Html.Encode(item)%>"> 
						<label>
							<input type="radio" name="radioRequerimentoPadrao" class="radioRequerimento" value="<%: item.Id %>" /> <%: item.Numero %>
						</label> 
					</td>

					<td title="<%= Html.Encode(item)%>" ><%= item.DataCadastro.ToString("dd/MM/yyyy")%></td>
					<td>
						<input type="hidden" class="hdnItemId" value='<%: item.Id %>' />
						<input type="hidden" class="hdnChecagemNumero" value="<%= Html.Encode(item.Checagem) %>" />
						<input type="hidden" class="hdnRequerimentoNumero" value="<%= Html.Encode(item.Numero) %>" />
						<input type="hidden" class="hdnRequerimentoId" value="<%= Html.Encode(item.Id) %>" />
						<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(item.ProtocoloId) %>" />
						<input type="hidden" class="hdnProtocoloTipo" value="<%= Html.Encode(item.ProtocoloTipo) %>" />
						<input type="hidden" class="hdnRequerimentoData" value="<%= Html.Encode(item.DataCadastro.ToShortDateString()) %>" />

						<button type="button" title="Editar" class="icone pdf btnRequerimentoPDF"></button>
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<input type="hidden" class="hdnProtocoloPai" value="<%: Model.PecaTecnica.ProtocoloPai %>"/>

<div class="divAtividades">
</div>
