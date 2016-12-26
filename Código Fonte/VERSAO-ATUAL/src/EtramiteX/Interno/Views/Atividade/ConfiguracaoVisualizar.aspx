<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AtividadeConfiguracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Visualizar Configuração de Atividade Solicitada
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Atividade/atividadeConfiguracao.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>

<script>

		$(function () {
			AtividadeConfiguracao.urlObterAtividade = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
			AtividadeConfiguracao.Mensagens = <%= Model.Mensagens %>;

			AtividadeConfiguracao.urlValidarAtividadeConfigurada = '<%= Url.Action("ValidarAtividadeConfigurada", "Atividade") %>';
			AtividadeConfiguracao.urlConfiguracaoSalvar = '<%= Url.Action("ConfigurarCriar", "Atividade") %>';

			AtividadeConfiguracao.load($('#central'));
		});

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Configuração de Atividade Solicitada</h1>
		<br />

		<input type="hidden" class="hdnConfiguracaoId" value="<%: Model.Configuracao.Id %>" />
		<div class="block box">
			<div class="coluna95">
				<label>Nome do grupo *</label>
				<%= Html.TextBox("Configuracao.NomeGrupo", null, new { @class = "text textNome disabled", @disabled = "disabled", @maxlength = "100" })%>
			</div>
		</div>
		<fieldset class="block box">
			<legend>Atividade Solicitada</legend>
			<div class="block dataGrid">
				<table class="dataGridTable tabAtividades" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Nome</th>
						</tr>
					</thead>
					<tbody>
						<%foreach (var item in Model.Configuracao.Atividades) {%>
						<tr>
							<td>
								<span title="<%: item.Texto %>"><%: item.Texto %></span>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Título Emitido</legend>

			<div class="block dataGrid divTabModelos">
				<table class="dataGridTable tabModelos" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Modelo de título</th>
						</tr>
					</thead>
					<tbody>
						<%	foreach (var item in Model.Configuracao.Modelos) { %>
						<tr>
							<td>
								<span title="<%: item.Texto %>"><%: item.Texto%></span>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</fieldset>
		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" tabindex = "36" href="<%= Url.Action("") %>" >Cancelar</a></span>
		</div>
	</div>
</asp:Content>