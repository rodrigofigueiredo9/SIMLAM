<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>


<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
	<thead>
		<tr>
			<th width="72%">Documentos gerados</th>
			<th width="20%">Situação</th>
			<th width="8%">Ação</th>
		</tr>
	</thead>
	<tbody>
		<%ArquivoProjeto arquivoProjeto = (Model.ProjetoGeoVM.Projeto != null && Model.ProjetoGeoVM.Projeto.Arquivos.Count > 0) ? Model.ProjetoGeoVM.Projeto.Arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui) : null;
		if (arquivoProjeto != null) {%>
		<tr>
			<td>
				<label title="Croqui da fiscalização (PDF)">Croqui da fiscalização (PDF)</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoCroqui", arquivoProjeto.Id, new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexoCroqui" title="Croqui da fiscalização (PDF)">Croqui da fiscalização (PDF)</button>
			</td>
		</tr>
		<% } %>
		<% if (Model.InfracaoVM.Infracao.IsGeradaSistema.GetValueOrDefault() || Model.ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.GetValueOrDefault() > 0 || Model.MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault()) { %>
		<tr>		
			<td>
				<label title="<%= Model.LabelTrDownload%>"><%= Model.LabelTrDownload%></label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfAuto icone pdf" title="PDF do Auto Termo Fiscalização"></span>
			</td>
		</tr>
		<% } %>
		<tr>
			<td>
				<label title="Laudo de fiscalização">Laudo de fiscalização</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfLaudo icone pdf" title="PDF do Laudo de fiscalização"></span>
			</td>
		</tr>
        <% if (Model.MultaVM.Multa.IsDigital == true
               || Model.MaterialApreendidoVM.MaterialApreendido.IsDigital == true
               || Model.ObjetoInfracaoVM.Entidade.IsDigital == true
               || Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital == true)
           { %>
        <tr>
			<td>
				<label title="Instrumento Único de Fiscalização">IUF</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<span class="btnPdfIUF icone pdf" title="PDF do IUF"></span>
			</td>
		</tr>
        <% } else { %>
            <% foreach (var item in Model.Fiscalizacao.ConsideracaoFinal.AnexosIUF) { %>
                <tr>
		        	<td>
		        		<label title="<%=item.Descricao %>"><%=item.Descricao %></label>
		        	</td>
		        	<td>
		        		<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
		        	</td>
		        	<td class="tdAcoes">
                        <%= Html.Hidden("hdnArquivoIUFBloco", item.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoIUFBlocoId" })%>
		        		<span class="btnPdfIUFBloco icone pdf" title="PDF do <%=item.Descricao %>"></span>
		        	</td>
		        </tr>
            <% } %>
        <% } %>



		<% if (Model.InfracaoVM.Infracao.Arquivo.Id.GetValueOrDefault() > 0) { %>
		<tr>
			<td>
				<label title="Auto de infração - <%= Model.InfracaoVM.Infracao.Arquivo.Nome %>">
					Auto de infração - <span style="font-style: italic; font-weight: normal;"><%= Model.InfracaoVM.Infracao.Arquivo.Nome %></span>
				</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoInfracao", Model.InfracaoVM.Infracao.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexo" title="Anexo ao Auto de infração">Anexo ao Auto de infração</button>
			</td>
		</tr>
		<% } %>
		<% if (Model.ObjetoInfracaoVM.Entidade.Arquivo.Id.GetValueOrDefault() > 0) { %>
		<tr>
			<td>
				<label title="Termo de embargo e interdição - <%= Model.ObjetoInfracaoVM.Entidade.Arquivo.Nome%>">
					Termo de embargo e interdição - <span style="font-style: italic; font-weight: normal;"><%= Model.ObjetoInfracaoVM.Entidade.Arquivo.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoApreensao", Model.ObjetoInfracaoVM.Entidade.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexo" title="Anexo ao Termo de apreensão e depósito">Anexo ao Termo de apreensão e depósito</button>
			</td>
		</tr>
		<% } %>
		<% if (Model.MaterialApreendidoVM.MaterialApreendido.Arquivo.Id.GetValueOrDefault() > 0) { %>
		<tr>
			<td>
				<label title="Termo de apreensão e depósito - <%= Model.MaterialApreendidoVM.MaterialApreendido.Arquivo.Nome%>">
					Termo de apreensão e depósito - <span style="font-style: italic; font-weight: normal;"><%= Model.MaterialApreendidoVM.MaterialApreendido.Arquivo.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoEmbargo", Model.MaterialApreendidoVM.MaterialApreendido.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexo" title="Anexo ao Termo de embargo e interdição">Anexo ao Termo de embargo e interdição</button>
			</td>
		</tr>
		<% } %>
		<% if (Model.ConsideracaoFinalVM.ConsideracaoFinal.Arquivo.Id.GetValueOrDefault() > 0) { %>
		<tr>
			<td>
				<label title="Termo de compromisso - <%= Model.ConsideracaoFinalVM.ConsideracaoFinal.Arquivo.Nome%>">
					Termo de compromisso - <span style="font-style: italic; font-weight: normal;"><%= Model.ConsideracaoFinalVM.ConsideracaoFinal.Arquivo.Nome%></span>
				</label>
			</td>
			<td>
				<label title="<%= Model.Fiscalizacao.SituacaoTexto%>"><%= Model.Fiscalizacao.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoTermoCompromisso", Model.ConsideracaoFinalVM.ConsideracaoFinal.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexo" title="Anexo a considerações finais">Anexo a considerações finais</button>
			</td>
		</tr>
		<% } %>

		<% foreach (var item in Model.Acompanhamentos.Where(x => x.SituacaoId != (int)eAcompanhamentoSituacao.Cancelado)) { %>
		<tr>
			<td>
				<label title="Laudo de Acompanhamento de Fiscalização - <% = item.Numero%>">
					Laudo de Acompanhamento de Fiscalização - <span style="font-style: italic; font-weight: normal;"><%= item.Numero %></span>
				</label>
			</td>
			<td>
				<label title="<%= item.SituacaoTexto %>"><%= item.SituacaoTexto %></label>
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
				<label title="<%= item.SituacaoTexto%>"><%= item.SituacaoTexto %></label>
			</td>
			<td class="tdAcoes">
				<%= Html.Hidden("hdnArquivoTermoCompromissoAcompanhamento", item.Arquivo.Id.GetValueOrDefault(), new { @class = "hdnArquivoId" })%>
				<button type="button" class="icone pdf btnAnexo" title="Anexo a acompanhamento">Anexo a acompanhamento</button>
			</td>
		</tr>
		<% }
		} %>
	</tbody>
</table>