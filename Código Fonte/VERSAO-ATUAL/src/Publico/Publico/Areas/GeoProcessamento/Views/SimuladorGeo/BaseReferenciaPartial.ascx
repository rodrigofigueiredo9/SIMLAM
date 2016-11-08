<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div class="containerBaseReferencia">
	<fieldset class="block box" >
		<legend>Arquivos de Referência para Download</legend>
		
		<div class="block dataGrid">
			<table class="dataGridTable gridArquivosVetoriais">
				<thead>
					<tr>
						<th>Arquivos vetoriais</th>
						<th>Situação de processamento</th>
						<th width="12%">Ação</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.ArquivosVetoriais) { %>
						<tr>
							<td class="txtNome"><%: item.Texto %></td>
							<td  class="txtSituacao"><%: item.SituacaoTexto %></td>
							<td>
								<input type="hidden" class="hdnSituacaoId" value="<%: item.Situacao %>"/>
								<input type="hidden" class="hdnArquivoId" value="<%: item.Id %>"/>
								<input type="hidden" class="hdnArquivoIdRelacionamento" value="<%: item.IdRelacionamento %>"/>
								<input type="hidden" class="hdnArquivoTipo" value="<%: item.Tipo %>"/>
								<span class="StatusFinalizado">
									<span class="btnGerar <%= item.MostrarGerar ? "" : "hide" %>"><button class="icone opcoes " title="Gerar"></button></span>
								</span>
								<span class="btnDownload <%= item.MostrarBaixar ? "" : "hide" %>"><button class="icone download" title="Baixar"></button></span>
								<span class="StatusFinalizado">
									<span class="btnRegerar <%= item.MostrarRegerar ? "" : "hide" %>"><button class="icone refresh" title="Regerar"></button></span>
								</span>
							</td>
						</tr>
					<%  } %>
					<tr>
						<td class="txtNome">Arquivo Modelo</td>
						<td  class="txtSituacao"> - </td>
						<td>
							<span class="btnDownloadModelo"><button class="icone download" title="Baixar"></button></span>								
						</td>
					</tr>
					<tr>
						<td class="txtNome">Manual de Elaboração</td>
						<td  class="txtSituacao"> - </td>
						<td>
							<span class="btnDownloadManual"><button class="icone download" title="Baixar"></button></span>								
						</td>
					</tr>
				</tbody>
			</table>
		</div>

	</fieldset>
</div>