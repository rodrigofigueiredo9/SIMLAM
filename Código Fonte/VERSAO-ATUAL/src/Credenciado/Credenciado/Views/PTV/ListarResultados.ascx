<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="18%">Número PTV</th>
				<th>Empreendimento</th>
				<th width="20%">Cultura/Cultivar</th>
				<th width="10%">Situação</th>
				<th class="semOrdenacao" width="23%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
				<tr>
					<td title="<%= Html.Encode(item.Numero)%>"><%= Html.Encode(item.Numero)%></td>
					<td title="<%= Html.Encode(item.Empreendimento)%>"><%= Html.Encode(item.Empreendimento)%></td>
					<td title="<%= Html.Encode(item.CulturaCultivar)%>"><%= Html.Encode(item.CulturaCultivar)%></td>
					<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
					<td>
						<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.ID, NumeroTipo = item.NumeroTipo , Numero = item.Numero, CulturaCultivar = item.CulturaCultivar, SituacaoTexto = item.SituacaoTexto, Situacao = item.Situacao } ) %>" />
						<% if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
						<% if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
						<% if (Model.PodeExcluir) { %><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
						<% if (item.Situacao == 3) { %><input type="button" title="PDF" class="icone pdf btnPDF" /><% } %>
						<% if (Model.PodeEnviar) { %><input type="button" title="Enviar" class="icone recebido btnEnviar" /><% } %>
						<% if (item.Situacao == (int)eSolicitarPTVSituacao.AguardandoAnalise) { %><input type="button" title="Cancelar envio" class="icone cancelar btnCancelarEnvio" /><% } %>
						<% if (Model.PodeSolicitarDesbloqueio) { %><input type="button" title="Solicitar Desbloqueio" class="icone comparar btnSolicitarDesbloqueio" /><% } %>
						<% if (	item.Situacao == (int)eSolicitarPTVSituacao.Rejeitado ||
								item.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao ||
								item.Situacao == (int)eSolicitarPTVSituacao.Bloqueado) { %>
							<input type="button" title="Comunicador" class="icone comunicador btnComunicador" />
						<% } %>
					</td>
				</tr>
			<% } %>
		</tbody>
	</table>
</div>
