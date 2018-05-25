<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>


<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
	<thead>
		<tr>
			<th width="65%">Documentos cancelados</th>
			<th width="30%">Data de cancelamento</th>
			<th width="8%">Ação</th>
		</tr>
	</thead>
	<tbody>

	<%foreach (var item in Model.DocumentosCancelados){%>

		<%if (item.Croqui.Id.GetValueOrDefault() > 0){%>
		<tr>
			<td>
				<label title="Croqui da fiscalização (PDF)">Croqui da fiscalização (PDF)</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoCroqui", item.Croqui.Id, new { @class = "hdnArquivoId" })%>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<button type="button" class="icone pdf btnAnexoCroquiCancelado" title="Croqui da fiscalização (PDF)">Croqui da fiscalização (PDF)</button>
			</td>
		</tr>
		<% } %>


		<%if (item.PdfGeradoAutoTermo.Id.GetValueOrDefault() > 0){%>
		<tr>		
			<td>
				<label title="<%= item.PdfGeradoAutoTermo.Nome%>"><%= item.PdfGeradoAutoTermo.Nome%></label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfAutoCancelado icone pdf" title="PDF do Auto Termo Fiscalização"></span>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<%= Html.Hidden("hdnArquivoId", item.PdfGeradoAutoTermo.Id, new { @class = "hdnArquivoId" })%>
			</td>
		</tr>
		<% } %>

		<%if (item.PdfGeradoLaudo.Id.GetValueOrDefault() > 0){%>
		<tr>
			<td>
				<label title="Laudo de fiscalização">Laudo de fiscalização</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfLaudoCancelado icone pdf" title="PDF do Laudo de fiscalização"></span>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<%= Html.Hidden("hdnArquivoId", item.PdfGeradoLaudo.Id, new { @class = "hdnArquivoId" })%>
			</td>
		</tr>
		<% } %>
        <% if (item.PdfGeradoIUF.Id.GetValueOrDefault() > 0) { %>
        <tr>
			<td>
				<%if (!string.IsNullOrWhiteSpace(item.NomeArquivo)) { %>
					<label title="<%=item.NomeArquivo %>"><%=item.NomeArquivo %></label>
				<%} else { %>
					<label title="Instrumento Único de Fiscalização">IUF</label>
				<%} %>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfIUFCancelado icone pdf" title="PDF do IUF de fiscalização"></span>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<%= Html.Hidden("hdnArquivoId", item.PdfGeradoIUF.Id, new { @class = "hdnArquivoId" })%>
			</td>
		</tr>
        <% } %>

		<%if (item.PdfAutoInfracao.Id.GetValueOrDefault() > 0){%>
		<tr>
			<td>
				<label title="Auto de infração - <%= item.PdfAutoInfracao.Nome%>">
					Auto de infração - <span style="font-style: italic; font-weight: normal;"><%= item.PdfAutoInfracao.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoInfracao", item.PdfAutoInfracao.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<button type="button" class="icone pdf btnAnexoCancelado" title="Anexo ao Auto de infração">Anexo ao Auto de infração</button>
			</td>
		</tr>
		<% } %>


		<% if (item.PdfTermoEmbargoInter.Id.GetValueOrDefault() > 0){ %>
		<tr>
			<td>
				<label title="Termo de embargo e interdição - <%= item.PdfTermoEmbargoInter.Nome%>">
					Termo de embargo e interdição - <span style="font-style: italic; font-weight: normal;"><%= item.PdfTermoEmbargoInter.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoApreensao", item.PdfTermoEmbargoInter.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<button type="button" class="icone pdf btnAnexoCancelado" title="Anexo ao Termo de apreensão e depósito">Anexo ao Termo de apreensão e depósito</button>
			</td>
		</tr>
		<% } %>


		<% if (item.PdfTermoApreensaoDep.Id.GetValueOrDefault() > 0){ %>
		<tr>
			<td>
				<label title="Termo de apreensão e depósito - <%= item.PdfTermoApreensaoDep.Nome%>">
					Termo de apreensão e depósito - <span style="font-style: italic; font-weight: normal;"><%= item.PdfTermoApreensaoDep.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoEmbargo", item.PdfTermoApreensaoDep.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				
				<button type="button" class="icone pdf btnAnexoCancelado" title="Anexo ao Termo de embargo e interdição">Anexo ao Termo de embargo e interdição</button>
			</td>
		</tr>
		<% } 

		if (item.PdfTermoCompromisso.Id.GetValueOrDefault() > 0){ %>
		<tr>
			<td>
				<label title="Termo de compromisso - <%= item.PdfTermoCompromisso.Nome%>">
					Termo de compromisso - <span style="font-style: italic; font-weight: normal;"><%= item.PdfTermoCompromisso.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%=item.SituacaoData.DataTexto%>"><%=item.SituacaoData.DataTexto%></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoTermoCompromisso", item.PdfTermoCompromisso.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<%= Html.Hidden("hdnHistoricoFisc", item.HistoricoId, new { @class = "hdnHistoricoId" })%>
				<button type="button" class="icone pdf btnAnexoCancelado" title="Anexo a considerações finais">Anexo a considerações finais</button>
			</td>
		</tr>
		<% }
	} %>

	<% foreach (var item in Model.Acompanhamentos.Where(x => x.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado)) { %>
	<tr>
		<td>
			<label title="Laudo de Acompanhamento de Fiscalização – <%= item.Numero %>">
				Laudo de Acompanhamento de Fiscalização – <span style="font-style: italic; font-weight: normal;"><%= item.Numero %></span>
			</label>
		</td>
		<td>
			<label title="<%=item.DataSituacao.DataTexto%>"><%=item.DataSituacao.DataTexto%></label>
		</td>
		<td class="tdAcoes">
			<%= Html.Hidden("Acompanhamento_Id", item.Id, new { @class = "hdnAcompanhamentoId" })%>
			<button type="button" class="icone pdf btnAcompanhamento" title="Laudo de Acompanhamento de Fiscalização">Laudo de Acompanhamento de Fiscalização</button>
		</td>
	</tr>

	<% if (item.Arquivo.Id.GetValueOrDefault() > 0) { %>
	<tr>
		<td>
			<label title="Termo de compromisso do acompanhamento <%= item.Numero +" - "+ item.Arquivo.Nome%>">
				Termo de compromisso do acompanhamento  <%= item.Numero %> - <span style="font-style: italic; font-weight: normal;"><%= item.Arquivo.Nome%></span>
			</label>
		</td>
		<td>
			<label title="<%= item.DataSituacao.DataTexto%>"><%= item.DataSituacao.DataTexto %></label>
		</td>
		<td class="tdAcoes">
			<%= Html.Hidden("hdnArquivoTermoCompromissoAcompanhamento", item.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
			<button type="button" class="icone pdf btnAnexo" title="Anexo a acompanhamento">Anexo a acompanhamento</button>
		</td>
	</tr>
	<% } %>
	<% } %>
	</tbody>
</table>