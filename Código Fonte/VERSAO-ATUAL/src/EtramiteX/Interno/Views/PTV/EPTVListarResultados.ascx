<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>
    

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número PTV</th>
				<th>Empreendimento</th>
				<th width="20%">Cultura/Cultivar</th>
				<th width="18%">Situação</th>
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
					<% if (Model.PodeVisualizar) { %><input type="button" title="Visualizar PTV" class="icone visualizar btnVisualizar" /><% } %>
					<input type="button" title="PDF do PTV" class="icone pdf btnPDF" />
                    <% if (Model.PodeAnalisar) { %><input type="button" title="Analisar EPTV" class="icone recebido btnAnalisar" /><% } %>
					<% if (Model.PodeAnalisar)
					{ %><input type="button" title="Analisar Desbloqueio" class="icone comparar btnAnalisarDesbloqueio" /><% } %>
								<% if (	item.Situacao == (int)eSolicitarPTVSituacao.Rejeitado ||
										item.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao ||
										item.Situacao == (int)eSolicitarPTVSituacao.Bloqueado)
					{ %><input type="button" title="Comunicador" class="icone comunicador btnComunicador" /><% } %>

                    <input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.ID, NumeroTipo = item.NumeroTipo , Numero = item.Numero, CulturaCultivar = item.CulturaCultivar, SituacaoTexto = item.SituacaoTexto, Situacao = item.Situacao } ) %>" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>