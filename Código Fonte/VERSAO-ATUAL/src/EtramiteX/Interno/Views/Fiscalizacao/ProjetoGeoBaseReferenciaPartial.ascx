<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BaseReferenciaVM>" %>

<div class="containerBaseReferencia">
		<div class="block dataGrid">
			<input type="hidden" class="hdnEstadoPadrao" value='<%= ViewModelHelper.JsSerializer.Serialize(Model.ArquivosDefault) %>'/>
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
								<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">
									<span class="btnGerar <%= item.MostarGerar ? "" : "hide" %>"><button class="icone opcoes " title="Gerar"></button></span>
								</span>
								<span class="btnDownload <%= item.MostarBaixar ? "" : "hide" %>"><button class="icone download" title="Baixar"></button></span>
								<% if (!Model.IsVisualizar) { %>
								<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">
									<span class="btnRegerar <%= item.MostarRegerar ? "" : "hide" %>"><button class="icone refresh" title="Regerar"></button></span>
								</span>
								<% } %>
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
				</tbody>
			</table>
			<table class="hide">
				<tbody>
					<tr class="trTempletaArqVetorial">
						<td class="txtNome"></td>
						<td  class="txtSituacao"></td>
						<td>
							<input type="hidden" class="hdnSituacaoId" />
							<input type="hidden" class="hdnArquivoId" />
							<input type="hidden" class="hdnArquivoIdRelacionamento" value=""/>
							<input type="hidden" class="hdnArquivoTipo" value=""/>
							<span class="btnGerar"><button class="icone opcoes " title="Gerar"></button></span>
							<span class="btnDownload"><button class="icone download" title="Baixar"></button></span>
							<span class="btnRegerar"><button class="icone refresh" title="Regerar"></button></span>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<div class="block dataGrid">
		<br />
			<table class="dataGridTable gridArquivosRaster <%= Model.ArquivosOrtoFotoMosaico.Count > 0 ? "" : "hide" %>">
				<thead>
					<tr>
						<th>Arquivos ortofotomosaico 2007/2008</th>
						<th width="12%">Ação</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.ArquivosOrtoFotoMosaico) { %>
						<tr>
							<td class="txtNome" title="<%: item.Texto %>"><%: item.Texto %></td>
							<td>
								<%--<input type="hidden" class="hdnOrtoFotoMosaicoId" value="<%: item.Id%>"/>--%>
								<input type="hidden" class="hdnCaminho" value="<%: item.Caminho%>"/>
								<input type="hidden" class="hdnChave" value="<%: item.Chave%>"/>
                                <input type="hidden" class="hdnChaveData" value="<%: item.ChaveData%>"/>
								<span class="btnDownload"><button class="icone download"></button></span>
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
		<table class="hide">
			<tbody>
				<tr class="templateMosaico">
					<td class="txtNome"></td>
					<td>
						<input type="hidden" class="hdnOrtoFotoMosaicoId" value=""/>
						<input type="hidden" class="hdnCaminho" value=""/>
						<input type="hidden" class="hdnChave" value=""/>
						<span class="btnDownload"><button class="icone download"></button></span>
					</td>
				</tr>
			</tbody>
		</table>

		<div class="block dataGrid divDadosDominio <%= Model.DadosDominio.Count == 0 ? "hide" : "" %>">
		<br />
			<table  class="dataGridTable gridDadosDominio">
				<thead>
					<tr>
						<th>Dados do domínio</th>
						<th width="12%">Ação</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.DadosDominio) {%>
						<tr>
							<td class="txtNome" title="<%: item.Texto %>"><%: item.Texto %></td>
							<td>
								<input type="hidden" class="hdnArquivoId" value="<%: item.Id%>"/>
								<span class="btnDownload"><button class="icone download"></button></span>
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
		<table class="hide">
			<tbody>
				<tr class="templateDadosDominio">
					<td class="txtNome"></td>
						<td>
							<input type="hidden" class="hdnArquivoId" value=""/>
						<span class="btnDownload"><button class="icone download"></button></span>
					</td>
				</tr>
			</tbody>
		</table>
</div>