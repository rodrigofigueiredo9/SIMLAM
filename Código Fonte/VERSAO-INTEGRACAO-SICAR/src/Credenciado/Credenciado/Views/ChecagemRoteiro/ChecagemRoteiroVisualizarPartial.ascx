<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarCheckListRoteiroVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMChecagemRoteiro" %>

<script type="text/javascript">
	function visualizarRoteiro(id, tid) {
		var urlVisualizar = '<%= Url.Action("Visualizar", "Roteiro") %>';
		urlVisualizar += '?id=' + id +'&tid=' + tid;		
		Modal.abrir(urlVisualizar, null, function (container) {Modal.defaultButtons(container);}, Modal.tamanhoModalGrande);
	}
</script>

<h1 class="titTela">Visualizar Checagem de Itens de Roteiro</h1>
<br />
<div class="block box">	
	<div class="block">
		<div class="coluna10">
			<label for="ChecagemRoteiro.Id">Número *</label>
			<%= Html.TextBox("Id", Model.ChecagemRoteiro.Id, new { disabled = "disabled", @class = "text disabled" })%>
		</div>

		<div class="coluna76 prepend2">
			<label for="ChecagemRoteiro_Interessado">Interessado *</label>
			<%= Html.TextBox("vm.ChecagemRoteiro.Interessado", Model.ChecagemRoteiro.Interessado, new { disabled = "disabled", @maxlength = "80", @class = "text txtInteressado disabled" })%>
		</div>
	</div>
</div>

<fieldset class="block box">
	<legend>Roteiro Orientativo</legend>
	<div class="block">
		<div class="dataGrid">
			<table class="tabRoteiros dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="10%">Número</th>
						<th width="10%">Versão</th>
						<th>Nome</th>
						<th width="10%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var item in Model.ChecagemRoteiro.Roteiros) { %>
					<tr>
						<td>
							<span class="trRoteiroNumero" title="<%= Html.Encode(item.Numero) %>"><%= Html.Encode(item.Numero) %></span>
						</td>
						<td>
							<span class="trRoteiroVersao" title="<%= Html.Encode(item.Versao) %>"><%= Html.Encode(item.Versao)%></span>
						</td>
						<td>
							<span class="trRoteiroNome" title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome) %></span>
						</td>
						<td>
							<input title="Visualizar" type="button" onclick="visualizarRoteiro('<%= item.Id %>','<%= item.Tid %>');" class="icone visualizar btnVisualizarRoteiro" value="" />
						</td>
					</tr>
				<%}%>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>
<fieldset class="block box">
	<legend>Itens de Roteiro</legend>
	<div class="block">
		<div class="dataGrid">
			<table class="tabItensRoteiro dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Nome</th>
						<th>Condicionante</th>
						<th width="13%">Situação</th>
					</tr>
				</thead>
				<tbody>
				<%foreach (Item item in Model.ChecagemRoteiro.Itens) {%>
					<tr>
						<td>
							<span class="trItemRoteiroNome" title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome) %></span>
						</td>
						<td>
							<span class="trItemRoteiroCondicionante" title="<%= Html.Encode(item.Condicionante) %>"><%= Html.Encode(item.Condicionante)%></span>
						</td>
						<td>
							<span class="trItemRoteiroSituacaoTexto" title="<%= Html.Encode(item.SituacaoTexto) %>"><%= Html.Encode(item.SituacaoTexto) %></span>
						</td>
					</tr>
				<%}%>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>