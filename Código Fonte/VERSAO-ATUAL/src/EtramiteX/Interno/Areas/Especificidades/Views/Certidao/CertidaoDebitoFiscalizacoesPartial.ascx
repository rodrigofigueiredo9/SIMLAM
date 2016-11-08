<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertidaoDebitoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%if(Model.Certidao.Fiscalizacoes.Count > 0){ %>
	<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="25%">Nº Fiscalização</th>
						<th width="25%">Data da Vistoria</th>
						<th width="15%">Nº Processo</th>
						<th width="25%">Situação</th>
						<th width="10%">Ação</th>
					</tr>
				</thead>
				<% foreach (var fiscalizacao in Model.Certidao.Fiscalizacoes){ %>
				<tbody>
					<tr>
						<td>
							<span class="numero" title="<%:fiscalizacao.NumeroFiscalizacao%>"><%:fiscalizacao.NumeroFiscalizacao%></span>
						</td>
						<td>
							<span class="data" title="<%:fiscalizacao.DataFiscalizacao%>"><%:fiscalizacao.DataFiscalizacao%></span>
						</td>
						<td>
							<span class="numeroProcesso" title="<%:fiscalizacao.NumeroProcesso%>"><%:fiscalizacao.NumeroProcesso%></span>
						</td>
						<td>
							<span class="situacao" title="<%:fiscalizacao.SituacaoTexto%>"><%:fiscalizacao.SituacaoTexto%></span>
						</td>
						<td class="tdAcoes">
							<input type="hidden" class="itemId" value='<%: fiscalizacao.Id%>' />
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(fiscalizacao)%>' />
							<input title="Visualizar" type="button" class="icone visualizar btnVisualizarFiscalizacao" value="" />
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
<%}else{ %>
	<p style="font-weight: normal">Nenhuma fiscalização encontrada.</p>
<%} %>