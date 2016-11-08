<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao.Tramitacao>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Processo/Documento em Posse</legend>
	<div class="block filtroCorpo">
		<div class="dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Processo/Documento</th>
						<th width="15%">Enviado em</th>
						<th width="10%">Origem</th>
						<th>Motivo</th>
						<th width="15%">Recebido em</th>
						<th width="16%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% if (Model.ProtocolosPosse.Count == 0) { %>
						<tr>
							<td colspan="6"><span>Não existem processos/documentos em posse</span></td>
						</tr>
					<% } %>
					<%foreach (var posse in Model.ProtocolosPosse) { %>
						<tr>
							<td>
								<span class="iconeInline <%= (posse.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(posse.Protocolo.Numero + " - " + posse.Protocolo.Tipo.Texto)%>"><%= Html.Encode(posse.Protocolo.Numero + " - " + posse.Protocolo.Tipo.Texto)%></span>
							</td>
							<td>
								<span title="<%= Html.Encode(posse.DataEnvio.DataHoraTexto) %>"><%= Html.Encode(posse.DataEnvio.DataHoraTexto) %></span>
							</td>
							<td>
								<span title="<%= Html.Encode(posse.RemetenteSetor.Texto)%>"><%= Html.Encode(posse.RemetenteSetor.Sigla)%></span>
							</td>
							<td>
								<span title="<%= Html.Encode(posse.Objetivo.Texto)%>"><%= Html.Encode(posse.Objetivo.Texto)%></span>
							</td>
							<td>
								<span title="<%= Html.Encode(posse.DataRecebido.DataHoraTexto)%>"><%= Html.Encode(posse.DataRecebido.DataHoraTexto)%></span>
							</td>
							<td>
								<input type="hidden" class="hdnProtocoloJSon" value='<%: ViewModelHelper.Json(new ProtocoloJsonVM(posse))%>' />
                                <% if (posse.IsExibirBotao) { %> <button title="Enviar" class="icone enviar btnEnviar" value="" type="button"></button><% } %>
								<button title="Visualizar" class="icone visualizar btnVisualizar" type="button"></button>
								<%if (posse.IsExisteHistorico) { %>
									<button title="Histórico" class="icone historico btnHistorico" type="button"></button>
									<button title="PDF de despacho" class="icone pdf btnPdf" type="button" onclick="Tramitacoes.onGerarPdf(true,this); return false;"></button>
								<%} %>
							</td>
						</tr>
				<% } %>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Processo/Documento a Receber</legend>
	<div class="block filtroCorpo">
		<label>*Enviados para mim</label>
		<div class="dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Processo/Documento</th>
						<th width="15%">Enviado em</th>
						<th width="10%">Origem</th>
						<th>Motivo</th>
						<th width="16%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% if (Model.ProtocolosReceber.Count == 0) {%>
					<tr>
						<td colspan="5"><span>Não existem processos/documentos enviados para o funcionário</span></td>
					</tr>
				<% } %>
				<%foreach (var receber in  Model.ProtocolosReceber) { %>
					<tr>
						<td>
							<span class="iconeInline <%= (receber.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(receber.Protocolo.Numero + " - " + receber.Protocolo.Tipo.Texto)%>"><%= Html.Encode(receber.Protocolo.Numero + " - " + receber.Protocolo.Tipo.Texto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receber.DataEnvio.DataHoraTexto)%>"><%= Html.Encode(receber.DataEnvio.DataHoraTexto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receber.RemetenteSetor.Texto)%>"><%= Html.Encode(receber.RemetenteSetor.Sigla)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receber.Objetivo.Texto)%>"><%= Html.Encode(receber.Objetivo.Texto)%></span>
						</td>
						<td>
							<input type="hidden" class="hdnProtocoloJSon" value='<%: ViewModelHelper.Json(new ProtocoloJsonVM(receber))%>' />
							<% if (receber.IsExibirBotao) { %> <button title="Receber Processo/Documento" alt="Receber Processo/Documento" class="icone receber btnReceber" type="button"></button> <%} %>
							<button title="Visualizar" class="icone visualizar btnVisualizar" type="button" ></button>
							<%if (receber.Id != 0) { %>
								<button title="Histórico" class="icone historico btnHistorico" type="button"></button>
								<button title="PDF de despacho" class="icone pdf btnPdf" type="button" onclick="Tramitacoes.onGerarPdf(false,this); return false;"></button>
							<%} %>
						</td>
					</tr>
				<% } %>
			</tbody>
		</table>
		</div>
	</div>

	<br />
	<div class="block filtroCorpo">
		<label>*Enviados para o meu setor</label>
		<div class="dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Processo/Documento</th>
						<th width="15%">Enviado em</th>
						<th width="10%">Origem</th>
						<th>Motivo</th>
						<th width="16%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% if (Model.ProtocolosReceberSetor.Count == 0) {%>
					<tr>
						<td colspan="5"><span>Não existem processos/documentos enviados para o setor</span></td>
					</tr>
				<% } %>
				<% foreach (var receberSetor in Model.ProtocolosReceberSetor) { %>
					<tr>
						<td>
							<span class="iconeInline <%= (receberSetor.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(receberSetor.Protocolo.Numero + " - " + receberSetor.Protocolo.Tipo.Texto)%>"><%= Html.Encode(receberSetor.Protocolo.Numero + " - " + receberSetor.Protocolo.Tipo.Texto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receberSetor.DataEnvio.DataHoraTexto)%>"><%= Html.Encode(receberSetor.DataEnvio.DataHoraTexto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receberSetor.RemetenteSetor.Texto)%>"><%= Html.Encode(receberSetor.RemetenteSetor.Sigla)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(receberSetor.Objetivo.Texto)%>"><%= Html.Encode(receberSetor.Objetivo.Texto)%></span>
						</td>
						<td>
							<input type="hidden" class="hdnProtocoloJSon" value='<%: ViewModelHelper.Json(new ProtocoloJsonVM(receberSetor))%>' />
							<% if (receberSetor.IsExibirBotao) { %> <button title="Receber Processo/Documento" class="icone receber btnReceber" type="button"></button> <% } %>
							<button title="Visualizar" class="icone visualizar btnVisualizar" type="button"></button>
							<%if (receberSetor.Id != 0) { %>
								<button title="Histórico" class="icone historico btnHistorico" type="button"></button>
								<button title="PDF de despacho" class="icone pdf btnPdf" type="button" onclick="Tramitacoes.onGerarPdf(false,this); return false;"></button>
							<%} %>
						</td>
					</tr>
				<% } %>
			</tbody>
			</table>
		</div>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Processo/Documento Enviado, Aguardando Recebimento</legend>
	<div class="block filtroCorpo">
		<div class="dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Processo/Documento</th>
						<th width="15%">Enviado em</th>
						<th width="10%">Motivo</th>
						<th>Destino</th>
						<th width="15%">Destinatário</th>
						<th width="16%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% if (Model.ProtocolosEnviado.Count == 0) {%>
					<tr>
						<td colspan="6"><span>Não existem processos/documentos enviados</span></td>
					</tr>
				<% } %>
				<% foreach (var enviado in  Model.ProtocolosEnviado) { %>
					<tr>
						<td>
							<span class="iconeInline <%= (enviado.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(enviado.Protocolo.Numero + " - " + enviado.Protocolo.Tipo.Texto)%>" ><%= Html.Encode(enviado.Protocolo.Numero + " - " + enviado.Protocolo.Tipo.Texto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(enviado.DataEnvio.DataHoraTexto)%>"><%= Html.Encode(enviado.DataEnvio.DataHoraTexto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(enviado.Objetivo.Texto)%>"><%= Html.Encode(enviado.Objetivo.Texto)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(enviado.DestinatarioSetor.Texto)%>"><%= Html.Encode(enviado.DestinatarioSetor.Sigla)%></span>
						</td>
						<td>
							<span title="<%= Html.Encode(enviado.Destinatario.Nome)%>"><%= Html.Encode(enviado.Destinatario.Nome)%></span>
						</td>
						<td>
                            <input type="hidden" class="hdnProtocoloJSon" value='<%: ViewModelHelper.Json(new ProtocoloJsonVM(enviado))%>' />
							<% if (enviado.IsExibirBotao) { %><button title="Cancelar tramitação" class="icone cancelar btnCancelarTramitacao" value="" type="button"></button> <%} %>
							<button title="Visualizar" class="icone visualizar btnVisualizar" type="button"></button>
							<%if (enviado.Id != 0) { %>
								<button title="Histórico" class="icone historico btnHistorico" type="button"></button>
								<button title="PDF de despacho" class="icone pdf btnPdf" type="button" onclick="Tramitacoes.onGerarPdf(false,this); return false;"></button>
							<%} %>
						</td>
					</tr>
				<% } %>
			</tbody>
			</table>
		</div>
	</div>
</fieldset>