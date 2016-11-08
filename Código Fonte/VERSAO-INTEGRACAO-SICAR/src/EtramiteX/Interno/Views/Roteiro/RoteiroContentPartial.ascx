<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloArquivo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<% if (!Model.Roteiro.Padrao) { %>
<fieldset class="block box">
	<legend>Atividades Solicitadas</legend>
	<div class="block dataGrid">
		<div class="block">
			<label class="lblNenhumaAtividade <%= Model.Roteiro.Atividades.Count > 0 ? "hide" : "" %>">Não existe atividade solicitada associada.</label>
			<button type="button" title="Buscar atividade" class="floatRight btnAssociarAtividade">Buscar</button>
		</div>

		<table class="dataGridTable tabAtividades <%= Model.Roteiro.Atividades.Count > 0 ? "" : "hide" %>" width="50%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Nome</th>
					<th width="10%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<%for (int i = 0; i < Model.Roteiro.Atividades.Count; i++) { %>
				<tr>
					<td>
						<input type="hidden" class="hdnAtividadeIdRelacionamento" value="<%= Model.Roteiro.Atividades[i].IdRelacionamento %>" />
						<input type="hidden" class="hdnAtividadeId" value="<%= Model.Roteiro.Atividades[i].Id %>" />
						<span title="<%: Model.Roteiro.Atividades[i].Texto%>" class="AtividadeTexto"><%: Model.Roteiro.Atividades[i].Texto%></span>
					</td>
					<td>
						<button title="Excluir" class="icone excluir btnExcluirAtividade" type="button"></button>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>

		<table style="display: none">
			<tbody>
				<tr class="trItemTemplateAtividade">
					<td>
						<input type="hidden" class="hdnAtividadeIdRelacionamento" value="0" />
						<input type="hidden" class="hdnAtividadeId" value="0" />
						<span class="AtividadeTexto"></span>
					</td>
					<td width="10%">
						<button title="Excluir" class="icone excluir btnExcluirAtividade" value="" type="button"></button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>

	<div class="block divFinalidade <%= Model.Roteiro.Atividades.Count >  0 ? "" : "hide" %>">
		<div class="block">
			<label>Finalidade *</label>
		</div>
		<div class="block finalidades">
			<%for (int i = 0; i < Model.Finalidades.Count; i++) { %>
			<div class="finalidade coluna<%= i == 0 ? 10 : 15 %> append1">
				<label class="labelCheckBox">
				<input class="checkboxFinalidade" type="checkbox" title="<%= Model.Finalidades[i].Texto%>" value="<%= Model.Finalidades[i].Codigo %>" <%= (Model.Roteiro.Finalidade != null) && (Model.Roteiro.Finalidade & Model.Finalidades[i].Codigo) != 0  ? "checked=\"checked\"" : ""  %> />
				<%: Model.Finalidades[i].Texto%></label>
			</div>
			<% } %>
		</div>
	</div>
	<div class="block linhaModeloTitulos <%= Model.Roteiro.ModelosAtuais.Count > 0 ? "" : "hide" %>">
		<div class="block"><label>Titulo *</label></div>
		<div class="modelosConteudo">
		<%foreach (var item in Model.Roteiro.ModelosAtuais) { %>
		<div class="coluna32 modeloTitulo">
			<input type="hidden" class="hdnModeloIdRelacionamento" value="<%= item.IdRelacionamento %>" />
			<label class="labelCheck" title="<%= item.Texto %>"><input type="checkbox" <%= (item.IsAtivo ? "checked=\"checked\"" : "") %>  class="chkModelos" value="<%= item.Id %>"/><%= (item.Texto)%></label>
		</div>
		<%} %>
		</div>
	</div>
	<div class="coluna32 hide modeloTitulo templateModeloTitulo">
		<input type="hidden" class="hdnModeloIdRelacionamento" value="0" />
		<input type="checkbox" class="chkModelos" value="0" />
		<label class="labelCheck" title=""></label>
	</div>
</fieldset>
<% }%>

<fieldset id="Container_ItemRoteiro" class="block box">
	<legend>Itens de Roteiro</legend>
	<div class="block dataGrid">
		<div class="block ">
			<button type="button" title="Buscar item" class="floatRight btnAssociarItem">Buscar</button>
		</div>

		<table class="dataGridTable tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="14%">Tipo</th>
					<th>Nome</th>
					<th>Condicionante</th>
					<th width="15%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<% for (int i = 0; i < Model.Roteiro.Itens.Count; i++) { %>
				<tr>
					<td>
						<span title="<%= (Model.Roteiro.Itens[i].Tipo == 1) ? "Técnico" : "Administrativo"%>" class="ItemTipo"><%= (Model.Roteiro.Itens[i].Tipo == 1) ? "Técnico" : "Administrativo"%></span>
						<input type="hidden" class="hdnItemId" name="Roteiro.Itens[<%= i %>].Id" value="<%= Html.Encode(Model.Roteiro.Itens[i].Id) %>" />
						<input type="hidden" class="hdnItemIndex" name="Roteiro.Itens.Index" value="<%= i %>" />
						<input type="hidden" class="hdnItemOrdem" name="Roteiro.Itens[<%= i %>].Ordem" value="<%= Html.Encode(Model.Roteiro.Itens[i].Ordem) %>" />
						<input type="hidden" class="hdnItemTipo" name="Roteiro.Itens[<%= i %>].Tipo" value="<%= Html.Encode(Model.Roteiro.Itens[i].Tipo) %>" />
						<input type="hidden" class="hdnItemProcedimento" name="Roteiro.Itens[<%= i %>].ProcedimentoAnalise" />
					</td>
					<td>
						<span title="<%= Html.Encode(Model.Roteiro.Itens[i].Nome)%>" class="ItemNome"><%= Html.Encode(Model.Roteiro.Itens[i].Nome)%></span>
						<input type="hidden" class="hdnItemNome" name="<%= i %>Roteiro.Itens[<%= i %>].Nome" value="<%= Html.Encode(Model.Roteiro.Itens[i].Nome) %>" />
					</td>
					<td>
						<span title="<%= Html.Encode(Model.Roteiro.Itens[i].Condicionante)%>" class="ItemCondicionante"><%= Html.Encode(Model.Roteiro.Itens[i].Condicionante)%></span>
						<input type="hidden" class="hdnItemCondicionante" name="<%= i %>Roteiro.Itens[<%= i %>].Condicionante" value="<%= Html.Encode(Model.Roteiro.Itens[i].Condicionante) %>" />
					</td>
					<td>
						<button title="Descer" class="icone abaixo btnDescerLinhaTab" type="button"></button>
						<button title="Subir" class="icone acima  btnSubirLinhaTab" type="button"></button>
						<button title="Excluir" class="icone excluir btnExcluirLinha" type="button"></button>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>

		<table style="display: none">
			<tbody>
				<tr class="trItemTemplate">
					<td>
						<span class="ItemTipo">TIPO</span>
						<input type="hidden" class="hdnItemIndex" name="templateRoteiro.Itens.Index" value="#Index" />
						<input type="hidden" class="hdnItemId" name="templateRoteiro.Itens.[#Index].Id" value="#ID" />
						<input type="hidden" class="hdnItemOrdem" name="templateRoteiro.Itens.[#Index].Ordem" value="#ORDEM" />
						<input type="hidden" class="hdnItemTipo" name="templateRoteiro.Itens[#Index].Tipo" value="#TIPO" />
						<input type="hidden" class="hdnItemProcedimento" name="templateRoteiro.Itens[#Index].ProcedimentoAnalise" value="#PROCEDIMENTO" />
					</td>
					<td>
						<span class="ItemNome">NOME</span>
						<input type="hidden" class="hdnItemNome" name="templateRoteiro.Itens[#Index].Nome" value="#NOME" />
					</td>
					<td>
						<span class="ItemCondicionante">CONDICIONANTE</span>
						<input type="hidden" class="hdnItemCondicionante" name="templateRoteiro.Itens[#Index].Condicionante" value="#CONDICIONANTE" />
					</td>
					<td>
						<button title="Descer" class="icone abaixo btnDescerLinhaTab" type="button"></button>
						<button title="Subir" class="icone acima btnSubirLinhaTab" type="button"></button>
						<button title="Excluir" class="icone excluir btnExcluirLinha" type="button"></button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Anexos</legend>
	<div class="block">
		<div class="coluna60 inputFileDiv">
			<label for="ArquivoTexto">Arquivo *</label>
			<%= Html.TextBox("Roteiro.Arquivo.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome", @style = "display: none;" })%>
			<input type="hidden" class="hdnArquivo" name="hdnArquivo" />
			<div class="anexoArquivos">
			</div>
			<input type="file" id="ArquivoId_" class="inputFile" style="display: block; width: 100%" name="file" />
		</div>
	</div>
	<div class="block">
		<div class="coluna60">
			<label for="Descricao">
				Descrição *</label>
			<%= Html.TextBox("Descricao", Model.Descricao, new { @maxlength = "100", @class = "text txtAnexoDescricao" })%>
		</div>
		<div class="coluna10 botoesAnexoDiv">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddAnexoArquivo" title="Adicionar anexo"
				onclick="RoteiroSalvar.onEnviarAnexoArquivoClick('<%= Url.Action("arquivo", "arquivo") %>');">Adicionar</button>
		</div>
	</div>

	<div class="block dataGrid">
		<label class="lblGridVazio <%= Model.Roteiro.Anexos.Count > 0 ? "hide" : "" %>">Não existe anexo adicionado.</label>

		<table class="dataGridTable tabAnexos <%= Model.Roteiro.Anexos.Count > 0 ? "" : "hide" %>" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Arquivo</th>
					<th>Descrição</th>
					<th width="15%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<%
					int y = 0;
					foreach (Anexo anexo in Model.Roteiro.Anexos) { y++; %>
				<tr>
					<td>
						<span class="ArquivoNome" title="<%= Html.Encode(anexo.Arquivo.Nome) %>"><%= Html.ActionLink(anexo.Arquivo.Nome, "Baixar", "Roteiro", new { @id = anexo.Arquivo.Id }, new { @Style = "display: block" })%></span>
						<input type="hidden" class="hdnAnexoIndex" name="Roteiro.Anexos.Index" value="<%= y %>" />
						<input type="hidden" class="hdnAnexoOrdem" name="Roteiro.Anexos[<%= y %>].Ordem" value="<%= Html.Encode(anexo.Ordem) %>" />
						<input type="hidden" class="hdnArquivoNome" name="Roteiro.Anexos[<%= y %>].Arquivo.Nome" value="<%= Html.Encode(anexo.Arquivo.Nome) %>" />
						<input type="hidden" class="hdnArquivoExtensao" name="Roteiro.Anexos[<%= y %>].Arquivo.Extensao" value="<%= Html.Encode(anexo.Arquivo.Extensao) %>" />
					</td>
					<td>
						<span title="<%= Html.Encode(anexo.Descricao) %>" class="AnexoDescricao"><%= Html.Encode(anexo.Descricao) %></span>
						<input type="hidden" class="hdnAnexoDescricao" name="Roteiro.Anexos[<%= y %>].Descricao" value="<%= Html.Encode(anexo.Descricao) %>" />
					</td>
					<td>
						<input type="hidden" class="hdnAnexoArquivoJson" name="Roteiro.Anexos[<%= y %>].ArquivoJson" value="<%= Html.Encode(anexo.ArquivoJson) %>" />
						<button title="Descer" class="icone abaixo btnDescerLinha" type="button"></button>
						<button title="Subir" class="icone acima btnSubirLinha" type="button"></button>
						<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
		<table style="display: none">
			<tbody>
				<tr class="trAnexoTemplate">
					<td>
						<span class="ArquivoNome">NOME</span>
						<input type="hidden" class="hdnAnexoIndex" name="templateRoteiro.Anexos.Index" value="#Index" />
						<input type="hidden" class="hdnAnexoOrdem" name="templateRoteiro.Anexos[#Index].Ordem" value="#ORDEM" />
						<input type="hidden" class="hdnArquivoNome" name="templateRoteiro.Anexos[#Index].Arquivo.Nome" value="#ARQUIVONOME" />
						<input type="hidden" class="hdnArquivoExtensao" name="Roteiro.Anexos[#Index].Arquivo.Extensao" value="#EXTENSAONOME" />
					</td>
					<td>
						<span class="AnexoDescricao">DESCRICAO</span>
						<input type="hidden" class="hdnAnexoDescricao" name="templateRoteiro.Anexos[#Index].Descricao"
							value="#DESCRICAO" />
					</td>
					<td>
						<input type="hidden" class="hdnAnexoArquivoJson" name="templateRoteiro.Anexos[#Index].ArquivoJson" value="#ARQUIVOJSON" />
						<button title="Descer" class="icone abaixo btnDescerLinha" type="button"></button>
						<button title="Subir" class="icone acima btnSubirLinha" type="button"></button>
						<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>




<fieldset class="block box">
	
	<div class="block">
		<div class="coluna70">
		<label for="Roteiro.Observacoes">Observações</label>
		<%= Html.TextArea("Roteiro.Observacoes", Model.Roteiro.Observacoes, new { @class = "textarea media txtObservacao tinymce" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna50">
			<label for="Roteiro.PalavraChave">Palavra-chave</label>
			<%= Html.TextBox("Roteiro.PalavraChave", Model.PalavraChave, new { @class = "text txtPalavraChaveNome", @maxlength = "50" })%>
		</div>
		<div class="coluna15 botoesPalavraChavesDiv">
			<button type="button" style="width:35px" class="btnAddPalavraChave inlineBotao botaoAdicionarIcone" title="Adicionar palavra-chave">Adicionar</button>
		</div>
	</div>

	<div class="block dataGrid">
		<label class="lblGridVazio <%= Model.Roteiro.PalavraChaves.Count > 0 ? "hide" : "" %>">Não existe palavra-chave adicionada.</label>

		<table class="dataGridTable tabPalavraChaves <%= Model.Roteiro.PalavraChaves.Count > 0 ? "" : "hide" %>" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Palvra-chave</th>
					<th width="7%">Ações</th>
				</tr>
			</thead>
			<tbody>
			<% int j = 0;
				foreach (PalavraChave chave in Model.Roteiro.PalavraChaves) { j++; %>
					<tr>
						<td>
							<span class="PalavraChaveNome" title="<%= Html.Encode(chave.Nome) %>"><%= Html.Encode(chave.Nome) %></span>
							<input type="hidden" class="hdnPalavraChaveIndex" name="Roteiro.PalavraChaves.Index" value="<%= j %>" />
							<input type="hidden" class="hdnPalavraChaveId" name="Roteiro.PalavraChaves[<%= j %>].Id" value="<%= chave.Id %>" />
							<input type="hidden" class="hdnPalavraChaveNome" name="Roteiro.PalavraChaves[<%= j %>].Nome" value="<%= Html.Encode(chave.Nome) %>" />
						</td>
						<td>
							<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
						</td>
					</tr>
			<% } %>
			</tbody>
		</table>
		<table style="display:none">
			<tbody>
				<tr class="trPalavraChavesTemplate">
					<td><span class="PalavraChaveNome">PALAVRACHAVE</span>
						<input type="hidden" class="hdnPalavraChaveIndex" name="templatePalavraChaves.PalavraChaves.Index" value="#Index" />
						<input type="hidden" class="hdnPalavraChaveId" name="templatePalavraChaves.PalavraChaves[#Index].Id" value="0" />
						<input type="hidden" class="hdnPalavraChaveNome" name="templatePalavraChaves.PalavraChaves[#Index].Nome" value="#PALAVRACHAVE" />
					</td>
					<td>
						<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>