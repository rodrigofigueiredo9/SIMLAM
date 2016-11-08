<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th>Interessado</th>
				<th>Empreendimento</th>
				<th width="15%">Situação</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= item.Numero %>"><%= item.Numero %></td>
				<td title="<%= Html.Encode(item.Interessado.NomeRazaoSocial)%>"><%= Html.Encode(item.Interessado.NomeRazaoSocial)%></td>
				<td title="<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Empreendimento.Denominador))%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Empreendimento.Denominador))%></td>
				<td title="<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id }) %>" />

					<%if (Model.PodeVisualizar) { %>
					<button type="button" title="PDF do requerimento" class="icone pdf btnPdfRequerimento"></button>
					<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
					<%}%>
					<%if (item.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao) {%><button type="button" title="Pendências" class="icone pendencias btnPendencia"></button><% } %>
					<%if (Model.PodeImportar && item.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoImportacao) {%><input type="button" title="Importar" class="icone importDigital btnImportar" /><% } %>

				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>