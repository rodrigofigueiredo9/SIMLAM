<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProcesso" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JuntarApensarVM>" %>

<!-- PROCESSO -->
<%= Html.Hidden("Processo.Id", null, new { @class = "hdnProcessoId" })%>

<fieldset class="block box">
	<legend>Processo</legend>
		<div class="block">
			<div class="coluna20">
				<label for="Processo.Numero">Nº de registro *</label>
				<% if (Model.IsProcessoValido) { %>
					<%= Html.TextBox("Processo.Numero", null, new { @Id = "Processo_Numero", @disabled = "disabled", @maxlength = 12, @class = "text txtProcessoNumero disabled" })%>
				<% } else { %>
					<%= Html.TextBox("Processo.Numero", null, new { @Id = "Processo_Numero", @maxlength = 12, @class = "text txtProcessoNumero" })%>
				<% } %>
			</div>

			<div class="coluna15 prepend2">
				<% if (Model.IsProcessoValido) { %><button type="button" class="inlineBotao btnLimpar" title="">Limpar</button>
				<% } else { %><button type="button" class="inlineBotao btnVerificar" title="">Verificar</button> <% } %>
			</div>
		</div>
</fieldset>

<% if (Model.IsProcessoValido) { %>
	<!-- DOCUMENTOS -->
	<fieldset class="block box">
		<legend>Documentos Juntados</legend>
		
		<div class="block">
			<div class="coluna20">
				<label for="NovoDocumentoNumero">Nº de registro *</label>
				<%= Html.TextBox("DocumentoJuntarNumero", null, new { @maxlength = 12, @class = "text txtNovoDocumentoNumero" })%>
			</div>

			<div class="coluna10 prepend2">
				<button type="button" class="inlineBotao botaoAdicionar btnAdicionarDocumento" title="Juntar documento" style="width: 32px">&nbsp;</button>
			</div>
		</div>

		<table class="dataGridTable tabDocumentosJuntados" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="20%">Número de registro</th>
					<th width="20%">Tipo</th>
					<th>Nome</th>
					<th width="10%">Ação</th>
				</tr>
			</thead>
			<tbody>
				<tr class="trSemItens <%= (Model.Processo.Documentos.Count > 0 ? "hide" : "") %>">
					<td colspan="4">
						<label>Não há documentos juntados</label>
					</td>
				</tr>
				<tr class="hide trTemplate trDocumento">
					<td class="primeiro"><span class="docNum"></span></td>
					<td><span class="docTipo"></span></td>
					<td><span class="docNome"></span></td>
					<td>
						<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
						<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>

						<input type="hidden" class="hdnItemId" value="" />
						<input type="hidden" class="hdnItemNumero" value="" />
					</td>
				</tr>
				<% foreach (Documento doc in Model.Processo.Documentos) { %>
					<tr class="trDocumento">
						<td class="primeiro"><span class="docNum" title="<%= Html.Encode(doc.Numero)%>"><%= Html.Encode(doc.Numero)%></span></td>
						<td><span class="docTipo" title="<%= Html.Encode(doc.Tipo.Texto)%>"><%= Html.Encode(doc.Tipo.Texto)%></span></td>
						<td><span class="docNome" title="<%= Html.Encode(doc.Nome)%>"><%= Html.Encode(doc.Nome)%></span></td>
						<td>
							<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
							<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>

							<input type="hidden" class="hdnItemId" value="<%= doc.Id %>" />
							<input type="hidden" class="hdnItemNumero" value="<%= doc.Numero %>" />
						</td>
					</tr>
				<% } %>
			</tbody>
		</table>
	</fieldset>

	<!-- PROCESSOS -->
	<fieldset class="block box">
		<legend>Processos Apensados</legend>
		<div class="block">
			<div class="coluna20">
				<label for="NovoProcessoNumero">Nº de registro *</label>
				<%= Html.TextBox("ProcessoApensarNumero", null, new { @maxlength = 12, @class = "text txtNovoProcessoNumero" })%>
			</div>

			<div class="coluna10 prepend2">
				<button type="button" class="inlineBotao botaoAdicionar btnAdicionarProcesso" title="Apensar processo" style="width: 32px">&nbsp;</button>
			</div>
		</div>

		<table class="dataGridTable tabProcessosApensados" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="20%">Número de registro</th>
					<th width="20%">Tipo</th>
					<th>Empreendimento</th>
					<th width="10%">Ação</th>
				</tr>
			</thead>
			<tbody>
				<tr class="trSemItens <%= (Model.Processo.Processos.Count > 0 ? "hide" : "") %>">
					<td colspan="4">
						<label>Não há processos apensados</label>
					</td>
				</tr>

				<tr class="hide trTemplate trProcesso">
					<td class="primeiro"><span class="procNum"></span></td>
					<td><span class="procTipo"></span></td>
					<td><span class="procEmp"></span></td>
					<td>
						<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
						<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>
					
						<input type="hidden" class="hdnItemId" value="" />
						<input type="hidden" class="hdnItemNumero" value="" />
					</td>
				</tr>
				<% foreach (Processo proc in Model.Processo.Processos) { %>
					<tr class="trProcesso">
						<td class="primeiro"><span class="procNum" title="<%= Html.Encode(proc.Numero)%>"><%= Html.Encode(proc.Numero)%></span></td>
						<td><span class="procTipo" title="<%= Html.Encode(proc.Tipo.Texto)%>"><%= Html.Encode(proc.Tipo.Texto)%></span></td>
						<td><span class="procEmp" title="<%= Html.Encode(proc.Empreendimento.Denominador)%>"><%= Html.Encode(proc.Empreendimento.Denominador)%></span></td>
						<td>
							<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
							<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>
						
							<input type="hidden" class="hdnItemId" value="<%= proc.Id %>" />
							<input type="hidden" class="hdnItemNumero" value="<%= proc.Numero %>" />
						</td>
					</tr>
				<% } %>
			</tbody>
		</table>
	</fieldset>
<% } %>