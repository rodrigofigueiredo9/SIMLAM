<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens"%>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HistoricoAnaliseVME>" %>

<h1 class="titTela">Histórico da Análise</h1>	
<br />

<fieldset class="block box">
	<legend>Item</legend>
		<div class="block">
			<div class="coluna50">
				<p><label for="Item.Tipo">Tipo *</label></p>
				<label><%= Html.RadioButton("TipoHist", (int)eRoteiroItemTipo.Tecnico, Model.Historico.TipoHist == (int)eRoteiroItemTipo.Tecnico, new { @class = "radio rdbItemTipo", @disabled = "disabled" })%> Técnico</label>
				<label><%= Html.RadioButton("TipoHist", (int)eRoteiroItemTipo.Administrativo, Model.Historico.TipoHist == (int)eRoteiroItemTipo.Administrativo, new { @class = "radio rdbItemTipo", @disabled = "disabled" })%> Administrativo</label>
				<label><%= Html.RadioButton("TipoHist", (int)eRoteiroItemTipo.ProjetoDigital, Model.Historico.TipoHist == (int)eRoteiroItemTipo.ProjetoDigital, new { @class = "radio rdbItemTipo", @disabled = "disabled" })%> Projeto Digital</label>
			</div>
		</div>

		<div class="block">
			<div class="coluna70">
				<label>Nome *</label>
				<%= Html.TextBox("Nome", Model.Historico.NomeHist, new { @class = "text Nome disabled", @maxlength = "250", @disabled = "disabled" })%>
			</div>
		</div>		
</fieldset>

<fieldset class="block box">
	<legend>Histórico de análise</legend>

	<div class="block">
		<table id="tabHistorico" class="dataGridTable" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="14%">Data da análise</th>
					<th width="25%">Analista</th>
					<th width="8%">Setor</th>
					<th width="12%">Situação do Item</th>
					<th>Motivo</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.Historico.Analises){ %>
				<tr>
					<td>
						<span title="<%= Html.Encode(item.DataAnalise) %>"><%= Html.Encode(item.DataAnalise)%></span>
					</td>
					<td>
						<span title="<%= Html.Encode(item.Analista) %>"><%= Html.Encode(item.Analista)%></span>
					</td>
					<td>
						<span title="<%= Html.Encode(item.SetorNome) %>"><%= Html.Encode(item.SetorSigla)%></span>
					</td>
					<td>
						<span  title="<%= Html.Encode(item.SituacaoTexto) %>"><%= Html.Encode(item.SituacaoTexto)%></span>
					</td>
					<% if (!string.IsNullOrEmpty(item.Motivo)){ %>
					<td>
						<span title="<%= Html.Encode(item.Motivo) %>"><%= Html.Encode(item.Motivo)%></span>
					</td>
					<% } else { %>
					<td>
						<span></span>
					</td>
					<% }%>
				</tr>
				<%} %>
			</tbody>
		</table>
	</div>
</fieldset>