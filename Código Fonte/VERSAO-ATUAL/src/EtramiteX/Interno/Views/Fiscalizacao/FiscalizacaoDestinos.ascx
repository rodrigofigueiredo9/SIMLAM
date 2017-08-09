<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
	<div class="coluna60">
		<table class="dataGridTable tabDestinos" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Destino</th>
					<th width="35%">Ações</th>
				</tr>
			</thead>
			
			<tbody>
                <% foreach (var destino in Model.ListaDestinos){ %>
				    <tr>
					    <td>
						    <span class="nomeDestino" title="<%:destino.Destino%>"><%:destino.Destino%></span>
					    </td>
					    <td class="tdAcoes">
						    <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(destino)%>' />
						    <input type="hidden" value="<%= destino.Id %>" class="destinoId" />
                            <input type="hidden" value="<%= destino.Ativo %>" class="destinoAtivo" />

						    <input title="Editar destino" type="button" class="icone editar btnEditarDestino" value="" />
                            <% if (destino.Ativo){ %>
						        <input title="Desativar destino" type="button" class="icone cancelar btnDesativarDestino" value="" />
						        <input title="Ativar destino" type="button" class="icone recebido btnAtivarDestino" disabled="disabled" value="" />
                            <% }else{ %>
						        <input title="Desativar destino" type="button" class="icone cancelar btnDesativarDestino" disabled="disabled" value="" />
						        <input title="Ativar destino" type="button" class="icone recebido btnAtivarDestino" value="" />
                            <% } %>
						    <input title="Excluir destino" type="button" class="icone excluir btnExcluirDestino" value="" />
					    </td>
				    </tr>
                <%} %>

				<tr class="trTemplateRow hide">
					<td><span class="nomeDestino"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input type="hidden" value="" class="destinoId" />
                        <input type="hidden" value="" class="destinoAtivo" />

						<input title="Editar destino" type="button" class="icone editar btnEditarDestino" disabled="disabled" value="" />
						<input title="Desativar destino" type="button" class="icone cancelar btnDesativarDestino" disabled="disabled" value="" />
						<input title="Ativar destino" type="button" class="icone recebido btnAtivarDestino" disabled="disabled" value="" />
						<input title="Excluir destino" type="button" class="icone excluir btnExcluirDestino" value="" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>