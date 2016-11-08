<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<h1 class="titTela">Visualizar Roteiro Orientativo</h1>
<br />

<% if (Model.Roteiro.Id > 0) { %>
<div class="block box">
	<div class="block">
		<div class="coluna23">
			<label for="Roteiro.Numero">Número *</label>
			<%= Html.TextBox("Roteiro.Numero", Model.Roteiro.Numero, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
		<div class="coluna23 prepend1">
			<label for="Roteiro.Versao">Versão *</label>
			<%= Html.TextBox("Roteiro.Versao", Model.Roteiro.Versao, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna48">
			<label for="Roteiro.Setor">Setor *</label>
			<%= Html.DropDownList("Roteiro.Setor", Model.Setores, new { @maxlength = "", disabled = "disabled", @class = "text disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna90">
			<label for="Roteiro.Nome">Nome *</label>
			<%= Html.TextBox("Roteiro.Nome", Model.Roteiro.Nome, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
	</div>
</div>

<% if (!Model.Roteiro.Padrao || Model.IsVisualizar) { %>
<fieldset class="block box">
	<legend>Atividades Solicitadas</legend>
	<% if (Model.Roteiro.Atividades.Count <= 0) { %>
	<div class="block">
		<label>Não existe atividade solicitada associada.</label>
	</div>
	<% } else { %>
	<div class="block dataGrid">
		<div class="block">
			<table class="dataGridTable tabAtividades"
			width="50%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Nome</th>
				</tr>
			</thead>
			<tbody>
				<%for (int i = 0; i < Model.Roteiro.Atividades.Count; i++) {%>
				<tr>
					<td>
						<input type="hidden" class="hdnAtividadeIdRelacionamento" value="<%= Model.Roteiro.Atividades[i].IdRelacionamento %>" />
						<input type="hidden" class="hdnAtividadeId" value="<%= Model.Roteiro.Atividades[i].Id %>" />
						<span title="<%: Model.Roteiro.Atividades[i].Texto%>" class="AtividadeTexto"><%: Model.Roteiro.Atividades[i].Texto%></span>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
		</div>
	</div>
	<div class="block divFinalidade <%= Model.Roteiro.Atividades.Count >  0 ? "" : "hide" %>">
		<div class="block"><label>Finalidade *</label></div>
		<div class="block finalidades">
			<%for (int i = 0; i < Model.Finalidades.Count; i++){ %>
			<div class="finalidade coluna24">
				<label class="labelCheckBox">
				<input class="checkboxFinalidade" type="checkbox" title="<%= Model.Finalidades[i].Texto%>" disabled="disabled" value="<%= Model.Finalidades[i].Codigo %>" <%= (Model.Roteiro.Finalidade != null) && (Model.Roteiro.Finalidade & Model.Finalidades[i].Codigo) != 0  ? "checked=\"checked\"" : ""  %> />
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
			<label class="labelCheck" title="<%= item.Texto %>"><input type="checkbox" <%= (item.IsAtivo ? "checked=\"checked\"" : "") %> disabled="disabled"  class="chkModelos" value="<%= item.Id %>"/><%= (item.Texto)%></label>
		</div>
		<%} %>
		</div>
	</div>
	<% } %>
</fieldset>
<% } %>

<fieldset class="block box">
	<legend>Itens de Roteiro</legend>
	<div class="block dataGrid">
		<table class="dataGridTable tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="14%">Tipo</th>
					<th>Nome</th>
					<th>Condicionante</th>
				</tr>
			</thead>
			<tbody>
				<% for (int i = 0; i < Model.Roteiro.Itens.Count; i++) { %>
				<tr>
					<td>
						<span class="ItemTipo" title="<%= Html.Encode(Model.Roteiro.Itens[i].TipoTexto) %>"><%= Html.Encode(Model.Roteiro.Itens[i].TipoTexto) %></span>
					</td>
					<td>
						<span class="ItemNome" title="<%= Html.Encode(Model.Roteiro.Itens[i].Nome) %>"><%= Html.Encode(Model.Roteiro.Itens[i].Nome) %></span>
					</td>
					<td>
						<span class="ItemCondicionante" title="<%= Html.Encode(Model.Roteiro.Itens[i].Condicionante) %>"><%= Html.Encode(Model.Roteiro.Itens[i].Condicionante)%></span>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Anexos</legend>
	<div class="block">
		<div class="dataGrid">
			<% if (Model.Roteiro.Anexos.Count <= 0) { %>
			<div class="block">
				<label>Não existe anexo adicionado.</label>
			</div>
			<% } else { %>
			<table class="dataGridTable tabAnexos" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Arquivo</th>
						<th>Descrição</th>
					</tr>
				</thead>
				<tbody>
					<%	for (int i = 0; i < Model.Roteiro.Anexos.Count; i++) {%>
					<tr>
						<td>
							<span class="ArquivoNome" title="<%= Html.Encode(Model.Roteiro.Anexos[i].Arquivo.Nome) %>"><%= Html.ActionLink(Model.Roteiro.Anexos[i].Arquivo.Nome, "Baixar", "Roteiro", new { @id = Model.Roteiro.Anexos[i].Arquivo.Id }, new { @Style = "display: block" })%></span>
						</td>
						<td>
							<span class="AnexoDescricao" title="<%= Html.Encode(Model.Roteiro.Anexos[i].Descricao) %>"><%= Html.Encode(Model.Roteiro.Anexos[i].Descricao) %></span>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
			<% } %>
		</div>
	</div>
</fieldset>

<div class="block box">
	<div class="block">
		<div class="coluna100">
			<label for="Roteiro.Observacoes">Observações</label>
			<div class="block boxBranca"><%=Model.Roteiro.Observacoes%></div>
		</div>
	</div>
	<div class="block">
		<div class="dataGrid">
			<% if (Model.Roteiro.PalavraChaves.Count <= 0) { %>
			<div class="block">
				<label>Não existe palavra-chave adicionada.</label>
			</div>
			<% } else { %>
			<table class="dataGridTable tabPalavraChaves" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Palvra-Chave</th>
					</tr>
				</thead>
				<tbody>
				
				<% for (int i = 0; i < Model.Roteiro.PalavraChaves.Count; i++) {%>
				<tr>
					<td>
						<span class="PalavraChaveNome" title="<%= Html.Encode(Model.Roteiro.PalavraChaves[i].Nome) %>"><%= Html.Encode(Model.Roteiro.PalavraChaves[i].Nome) %></span>
					</td>
				</tr>
				<% } %>
				</tbody>
			</table>
			<% } %>
		</div>
	</div>
</div>

<% } %>