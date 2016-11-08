<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SobreposicoesVM>" %>

<div class="containerSobreposicao">

	<div class="block">
		<div class="dataHoraVer coluna50">
			Data e hora da ultima verificação: <label class="lblDataVerificacao"><%= (Model.UltimaVerificacao != null && !Model.UltimaVerificacao.IsEmpty)? Model.UltimaVerificacao.DataHoraTexto : "" %></label>
		</div>

		<div class="hide verificando coluna50">
			<img src="<%= Url.Content("~/Content/_img/loader_pequeno.gif") %>" alt="Carregando" />
			<label style="vertical-align:middle; ">Verificando sobreposição...</label>
		</div>

		<div class="coluna30 divVerificarSobreposicao <%= Model.MostarVerificar ? "" : "hide" %>">
			<span class="">
				<button class="btnVerificarSobreposicao">Verificar</button>
			</span>
		</div>
	</div>

	<div class="block dataGrid <%= Model.SobreposicaoGeoBases.Count > 0 ? "" : "hide" %>">
		<table class="dataGridTable sobreposicaoGeoBasesGrid">
			<thead>
				<tr>
					<th width="35%">Sobreposições verificadas no GEOBASES</th>
					<th>Identificação</th>
				</tr>
			</thead>
			<tbody>
				<%foreach (Sobreposicao sobreposicao in Model.SobreposicaoGeoBases) { %>
					<tr>
						<td><%: sobreposicao.TipoTexto  %></td>
						<td><%: sobreposicao.Identificacao  %></td>
					</tr>
				<% } %>
			</tbody>
		</table>
	</div>


	<div class="block dataGrid <%= Model.SobreposicaoIDAF.Count > 0 ? "" : "hide" %>">
		<table class="dataGridTable sobreposicaoIdafGrid">
			<thead>
				<tr>
					<th width="35%">Sobreposições verificadas na base de dados do IDAF</th>
					<th>Identificação</th>
				</tr>
			</thead>
			<tbody>
				<%foreach (Sobreposicao sobreposicao in Model.SobreposicaoIDAF) { %>
					<tr>
						<td><%: sobreposicao.TipoTexto  %></td>
						<td><%: sobreposicao.Identificacao  %></td>
					</tr>
				<% } %>
			</tbody>
		</table>
	</div>

	<table class="hide">
		<tbody>
			<tr class="trTemplateSobreposicao">
				<td class="tipoTexto"></td>
				<td class="identificacao"></td>
			</tr>
		</tbody>
	</table>

</div>