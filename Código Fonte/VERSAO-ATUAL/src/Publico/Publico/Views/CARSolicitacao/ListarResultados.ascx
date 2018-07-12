<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMCARSolicitacao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="13%">Número</th>
				<th>Nome/Razão Social/Denominação/Imóvel</th>
				<th width="21%">Município</th>
				<th width="12%">Situação</th>
				<th class="semOrdenacao" width="8%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NumeroTexto)%>"><%= Html.Encode(item.NumeroTexto)%></td>
				<td title="<%= Html.Encode(item.EmpreendimentoDenominador)%>"><%= Html.Encode(item.EmpreendimentoDenominador)%></td>
				<td title="<%= Html.Encode(item.MunicipioTexto)%>"><%= Html.Encode(item.MunicipioTexto)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, InternoId = item.InternoId, SituacaoSolicitacaoId = item.SituacaoID, SituacaoSolicitacaoTexto = item.SituacaoTexto, SituacaoArquivoCarId = item.SituacaoArquivoCarID, SituacaoArquivoCarTexto = item.SituacaoArquivoCarTexto, UrlPdfReciboSICAR = item.UrlPdfReciboSICAR, Origem = item.Origem, ArquivoSICAR=item.ArquivoSICAR }) %>" />
					<input type="hidden" class="itemId" value="<%= item.Id%>" />
					<input type="hidden" class="isCredenciado" value="<%= item.IsCredenciado%>" />
					<%if(item.IsTitulo){ %>
						<input type="button" title="PDF do Título" class="icone pdf btnPDFTitulo" />
					<%} else {%>
						<input type="button" title="PDF da Solicitação" class="icone pdf btnPDF" />
					<%} %>
					<% if (item.SituacaoID == (int)eCARSolicitacaoSituacao.Valido || item.SituacaoID == (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR || item.SituacaoID == 0) 
						{ %>
                            <input type="button" title="Baixar Demonstrativo do CAR" class="icone documento btnDemonstrativoCar" />
                    <% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>