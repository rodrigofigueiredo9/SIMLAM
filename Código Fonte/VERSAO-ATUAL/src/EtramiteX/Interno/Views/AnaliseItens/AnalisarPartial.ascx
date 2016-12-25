<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnaliseItemVM>" %>

<input type="hidden" class="hdnItemAtualizado" value="false" />
<% Requerimento requerimento = Model.Requerimentos.FirstOrDefault(x => x.Id == Model.RequerimentoSelecionado); %>

<script>
	Analise.settings.urls.caracterizacoes = <%=Model.UrlsCaracterizacoes%>
</script>

<%if(Model.Roteiros.Count > 0) {%>
<fieldset class="block box fsRoteiro">
	<legend>Roteiro Orientativo</legend>
	<div class="divRoteiroContainer">
		<div class="dataGrid">
			<table class="tabRoteiros dataGridTable" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="10%">Número</th>
						<th width="10%">Versão</th>
						<th width="15%">Nome</th>
						<th width="14%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.Roteiros) { %>
					<tr>
						<td>
							<span class="trRoteiroNumero" title="<%= Html.Encode(item.Numero) %>">
								<%= Html.Encode(item.Numero) %></span>
						</td>
						<td>
							<span class="trRoteiroVersao" title="<%= Html.Encode(Model.Atualizado ? item.Versao : item.VersaoAtual) %>">
								<%= Html.Encode(Model.Atualizado ? item.Versao : item.VersaoAtual) %></span>
						</td>
						<td>
							<span class="trRoteiroNome" title="<%= Html.Encode(Model.Atualizado ? item.NomeAtual : item.Nome) %>">
								<%= Html.Encode(Model.Atualizado ? item.NomeAtual : item.Nome)%></span>
						</td>
						<td><% %>
							<input type="hidden" class="hdnRoteiroTid" value="<%= Html.Encode(item.Tid) %>" />
							<input type="hidden" class="hdnIdRelacionamento" value="<%= Html.Encode(item.IdRelacionamento) %>" />
							<input type="hidden" class="hdnRoteiroNumero" value="<%= Html.Encode(item.Id) %>" />
							<input title="PDF do roteiro" type="button" class="icone pdf btnVisualizarPDF" />
						</td>
					</tr>
					<%} %>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>
<%} %>

<%if (Model.CheckListId > 0) { %>
<fieldset class="block box">
	<legend>Checagem de Itens de Roteiro</legend>

	<div class="block">
		<div class="coluna15">
			<label>Número *</label>
			<%= Html.TextBox("txtNumeroChecagem", Model.CheckListId, new { @class = "text disabled txtNumeroChecagem", @disabled = "disabled" })%>
		</div>

		<div class="coluna20 ultima">
			<button type="button" class="icone visualizar inlineBotao btnVisChecagem" title="Visualizar checagem"></button>
		</div>
	</div>
</fieldset>
<%} %>

<%if (Model.ItensAdmin.Count > 0){ %>
<fieldset class="block box">
	<legend>Itens Administrativo</legend>
	<div class="divItensAdmin divItens">

		<%Html.RenderPartial("Item", Model.ItensAdmin); %>

		<div>
			<input type="hidden" class="hdnTipoItem" value="<%=(int)eRoteiroItemTipo.Administrativo%>" />
			<button type="button" class="inlineBotao botaoBuscar floatRight btnAddItem">+ item</button>
		</div>
	</div>
</fieldset>
<%} %>

<%if (Model.ItensTecnico.Count > 0){ %>
<fieldset class="block box">
	<legend>Itens Técnicos</legend>
	<div class="divItensTecnico divItens">

		<%Html.RenderPartial("Item", Model.ItensTecnico); %>

		<div>
			<input type="hidden" class="hdnTipoItem" value="<%=(int)eRoteiroItemTipo.Tecnico%>" />
			<button type="button" class="inlineBotao botaoBuscar floatRight btnAddItem">+ item</button>
		</div>
	</div>
</fieldset>
<%} %>

<%if (Model.ItensProjetoDigital.Count > 0){ %>
<fieldset class="block box">
	<legend>Itens do Projeto Digital</legend>
	<div class="divItensProjetoDigital divItens">

		<%Html.RenderPartial("ItemProjetoDigital", Model.ItensProjetoDigital); %>

		<div>
			<input type="hidden" class="hdnTipoItem" value="<%=(int)eRoteiroItemTipo.ProjetoDigital%>" />
			<button type="button" class="inlineBotao botaoBuscar <%=Model.ImportarDados ? "" : "hide" %> floatRight btnImportarProjetoDigital">Importar projeto digital</button>
		</div>
	</div>
</fieldset>
<%} %>

<div class="hide">
	<table >
		<tbody>
			<tr class="trItemTemplate">
				<td>
					<input type="checkbox" disabled="disabled"/>
				</td>
				<td>
					<span class="itemNome" title=""></span>
				</td>
				<td>
					<span class="itemCondicionante" title=""></span>
				</td>
				<td>
					<span class="itemSituacaoTexto" title=""></span>
				</td>
				<td>
					<input type="hidden" class="itemJSON" value="" />
					<input title="Análise" type="button" class="icone analisarItem btnAnalisarItem" />
					<input title="Histórico" type="button" class="icone historico btnHistoricoItem" />
					<input title="Excluir" type="button" class="icone excluir btnExcluirItem" />
				</td>
			</tr>
		</tbody>
	</table>
</div>