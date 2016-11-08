<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<div class="block">
	<h5>Largura das colunas</h5>
	<p>Escolha a melhor configuração das colunas para o relatório que está sendo criado.</p>
</div>

<div class="block">
	<h5>Orientação</h5>
	<div class="coluna30">
			<label><%= Html.RadioButton("Model.ConfiguracaoRelatorio.OrientacaoRetrato", true, Model.ConfiguracaoRelatorio.OrientacaoRetrato, new { @class = "radio  radioRetrato" })%>Retrato</label>
			<label><%= Html.RadioButton("Model.ConfiguracaoRelatorio.OrientacaoRetrato", false, !Model.ConfiguracaoRelatorio.OrientacaoRetrato, new { @class = "radio radioPaisagem" })%>Paisagem</label>
	</div>
</div>
<br />
<% foreach (var item in Model.ConfiguracaoRelatorio.CamposSelecionados) { %>
	<div class="block margem0 divColuna">
		<input type="hidden" class="hdnCampoId" value="<%= item.Campo.Id %>" />
		<div class="coluna40">
			<p>
				<strong><%: item.Campo.Alias %></strong>
			</p>
		</div>
		<div class="coluna10">
			<p>
				<input id="col<%= item.Posicao %>" type="text" class="title campoColuna maskNumInt" maxlength="3" value="<%= item.Tamanho %>" />
				<strong class="large">%</strong>
			</p>
		</div>
	</div>
<% } %>
<div class="block margem0">
	<div class="coluna38">
		<strong class="somatoria direita">100</strong>
	</div>

	<div class="coluna30">
		<span class="dica quiet">Sempre deixe a somatória das colunas em 100</span>
	</div>
</div>
<div class="block margemDTop">
	<h5>Prévia da configuração das colunas</h5>
	<table width="100%" border="0" cellspacing="0" cellpadding="0" class="dataGridTable previaColunas">
		<thead>
			<tr>
			<% foreach (var item in Model.ConfiguracaoRelatorio.CamposSelecionados) { %>
				<th width="<%= item.Tamanho %>%" title="#col<%= item.Posicao %>"><%: item.Campo.Alias %></th>
			<% } %>
			</tr>
		</thead>
	</table>
</div>