<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AcompanhamentosVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Acompanhamentos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/acompanhamentos.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script>
		$(function () {
			Acompanhamentos.load($('#central'), {
				urls: {
					novo: '<%= Url.Action("AcompanhamentoCriar", "Fiscalizacao", new {fiscalizacaoId = Model.fiscalizacaoId })%>',
					visualizar: '<%= Url.Action("AcompanhamentoVisualizar", "Fiscalizacao") %>',
					editar: '<%= Url.Action("AcompanhamentoEditar", "Fiscalizacao") %>',
					excluir: '<%= Url.Action("AcompanhamentoExcluir", "Fiscalizacao") %>',
					excluirConfirm: '<%= Url.Action("AcompanhamentoExcluirConfirm", "Fiscalizacao") %>',
					alterarSituacao: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao") %>',
					pdf: '<%= Url.Action("LaudoAcompanhamentoFiscalizacaoPdf", "Fiscalizacao") %>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["editar"])) { %>
				<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
					ContainerAcoes.load($(".containerAcoes"), {
						urls: {
							urlGerarPdf: '<%= Url.Action("LaudoAcompanhamentoFiscalizacaoPdf", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
							urlEditar: '<%= Url.Action("AcompanhamentoEditar", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>',
							urlAlterarSituacao: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>'
						}
					});
				<%}%>
			<% } else { %>
				<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
					ContainerAcoes.load($(".containerAcoes"), {
						urls: {
							urlGerarPdf: '<%= Url.Action("LaudoAcompanhamentoFiscalizacaoPdf", "Fiscalizacao", new {id = Request.Params["acaoId"].ToString() }) %>'
						}
					});
				<%}%>
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Acompanhamentos</h2><br />

		<% if(Model.PodeCriar) { %>
		<div class="block margem0 negtBot">
			<button class="btnNovoAcompanhamento direita" title="Novo Acompanhamento">Novo</button>
		</div>
		<br />
		<% } %>

		<div class="block dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Nº do Acompanhamento</th>
						<th>Data da Vistoria</th>
						<th>Situação</th>
						<th width="20%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var item in Model.Resultados) { %>
					<tr>
						<td title="<%= Html.Encode(item.Numero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Numero))%></td>
						<td title="<%= Html.Encode(item.DataVistoria.DataTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataVistoria.DataTexto))%></td>
						<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
						<td>
							<input type="hidden" value="<%= item.Id %>" class="itemId" />
							<input type="hidden" value="<%= item.SituacaoId %>" class="itemSituacao" />

							<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
							<%if (Model.PodeEditar) {%><button type="button"title="Editar" class="icone editar btnEditar"></button><% } %>
							<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
							<%if (Model.PodeAlterarSituacao){%><button type="button" title="Alterar Situação" class="icone sitTitulo btnAlterarSituacao"></button><% } %>
							<%if (Model.PodeVisualizar) {%><button type="button" title="PDF Acompanhamento" class="icone pdf btnPDFAcompanhamento"></button><% } %>
						</td>
					</tr>
				<% } %>
				</tbody>
			</table>
		</div>

		<div class="block box">
			<a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a>
		</div>
	</div>
</asp:Content>