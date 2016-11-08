<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloAtividade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMAtividade" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarAtividadesSolicitadasVME>" %>

<div class="divConteudoAtividade associarMultiplo">
	<div class="asmConteudoFixo divRequerimento">
		<div class="block">
			<input type="hidden" class="hdnRequerimentoId asmItemId" value="<%= Html.Encode(Model.Requerimento.Id) %>" />
			<input type="hidden" class="hdnIsProcesso" value="<%= Html.Encode(Model.IsProcesso) %>" />
			<div class="coluna15">
				<label for="Numero">Número *</label>
				<%= Html.TextBox("Processo.Requerimento.Numero", Model.Requerimento.Numero, new { @class = "text txtRequerimentoNumero disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna15 prepend2">
				<label for="Numero">Data de criação *</label>
				<%= Html.TextBox("Processo.Requerimento.DataCadastroTexto", Model.Requerimento.DataCadastroTexto, new { @class = "text txtRequerimentoDataCadastro disabled", @disabled = "disabled" })%>
			</div>
			<%--<div class="coluna30 prepend2">
				<button class="icone pdf inlineBotao btnVisualizarPdf" title="PDF do requerimento"></button>
			</div>--%>
		</div>
	</div>

	<div class="asmConteudoLink">
		<div class="asmConteudoInterno hide">
			<div class="block dataGrid">
				<table class="dataGridTable tabAtividades" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Atividade Solicitada</th>
							<th width="15%">Situação</th>							
						</tr>
					</thead>
					<tbody>
					<%
						int i = 0;
						foreach (Atividade atividade in Model.Atividades) {
						i++;%>
						<tr>
							<td>
								<span class="lbAtividadeNome" title="<%= Html.Encode(atividade.NomeAtividade) %>"><%= atividade.NomeAtividade%></span>
								<input type="hidden" class="hdnItemIndex" name="Processo.Atividades.Index" value="<%= i %>" />
								<input type="hidden" class="hdnAtividadeNome" name="Processo.Atividades[<%= i %>].Nome" value="<%= Html.Encode(atividade.NomeAtividade) %>" />
								<input type="hidden" class="hdnAtividadeMotivo" value="<%= Html.Encode(atividade.Motivo) %>" />
								<input type="hidden" class="hdnAtividadeIdProtocolo" value="<%= Html.Encode(Model.ProtocoloId) %>" />
								<input type="hidden" class="hdnAtividadeIsProcesso" value="<%= Html.Encode(Model.IsProcesso) %>" />
								<input type="hidden" class="hdnAtividadeId" name="Processo.Atividades[<%= i %>].Id" value="<%= Html.Encode(atividade.Id) %>" />
								<input type="hidden" class="hdnAtividadeIdRelacionamento" name="Processo.Atividades[<%= i %>].IdRelacionamento" value="<%= Html.Encode(atividade.IdRelacionamento) %>" />
							</td>
							<td>
								<span class="spSituacaoTexto" title="<%= Html.Encode(atividade.SituacaoTexto) %>"><%= Html.Encode(atividade.SituacaoTexto) %></span>
								<input type="hidden" class="hdnSituacaoId" name="Processo.Atividades[<%= i %>].SituacaoId" value="<%= Html.Encode(atividade.SituacaoId) %>" />
							</td>
						</tr>
					<% } %>
					</tbody>
				</table>
			</div>
		</div>
		<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
	</div>
</div>