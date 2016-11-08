<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtividadeConfiguracaoVM>" %>

<div class="divConfiguracaoAtividade">
	<input type="hidden" class="hdnConfiguracaoId" value="<%: Model.Configuracao.Id %>" />
	<div class="block box">
		<div class="coluna95">
			<label>Nome do grupo *</label>
			<%= Html.TextBox("Configuracao.NomeGrupo", null, new { @class = "text textNome", @maxlength = "100" })%>
		</div>
	</div>
	<fieldset id="Container_Atividade" class="block box">	
		<legend>Atividade Solicitada</legend>
		<div class="block dataGrid">
			<div class="block ">
				<button type="button" title="Buscar item" class="floatRight btnBuscarAtividade">Buscar</button>
			</div>
			<table class="dataGridTable tabAtividades" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Nome</th>
						<th width="6%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.Configuracao.Atividades) {%>
					<tr>
						<td>
							<span class="ItemNome" title="<%: item.Texto %>"><%: item.Texto %></span>
							<input type="hidden" class="hdnItemId" value="<%: item.Id %>" />
							<input type="hidden" class="hdnItemSetorId" value="<%: item.SetorId %>" />
							<input type="hidden" class="hdnAtividadeIdRelacionamento" value="<%: item.IdRelacionamento %>" />
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
					<tr class="templateAtividade">
						<td>
							<span class="ItemNome"></span>
							<input type="hidden" class="hdnItemId" value="0" />
							<input type="hidden" class="hdnItemSetorId" value="0" />
							<input type="hidden" class="hdnAtividadeIdRelacionamento" value="0" />
						</td>
						<td>
							<button title="Excluir" class="icone excluir btnExcluirAtividade" type="button">
							</button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset id="Container_Titulo" class="block box">
		<legend>Título Emitido</legend>

		<div class="block">
			<div class="coluna85">
				<label>Modelo de título *</label>
				<%= Html.DropDownList("Modelos", Model.Modelos, new { @class = "text ddlModelos" })%>
			</div>
			<div>
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAssociarItem" title="Adicionar Modelo">Adicionar</button>
			</div>
		</div>
			
		<div class="block dataGrid divTabModelos">
			<table class="dataGridTable tabModelos" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Modelo de título</th>
						<th width="6%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<%	foreach (var item in Model.Configuracao.Modelos) { %>
					<tr class="">
						<td>
							<span class="ItemModeloNome" title="<%: item.Texto%>"><%: item.Texto%></span>
							<input type="hidden" class="hdnItemIdRelacionamento" value="<%: item.IdRelacionamento %>"/>
							<input type="hidden" class="hdnItemModeloId" value="<%: item.Id %>"/>
						</td>
						<td width="6%">
							<button title="Excluir" class="icone excluir btnExcluirModelo"type="button"></button>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
			<table style="display: none">
				<tbody>
					<tr class="templateModelo">
						<td>
							<span class="ItemModeloNome"></span>
							<input type="hidden" class="hdnItemIdRelacionamento" value="0"/>
							<input type="hidden" class="hdnItemModeloId" value="0" />
						</td>
						<td width="6%">
							<button title="Excluir" class="icone excluir btnExcluirModelo" type="button"></button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>
</div>