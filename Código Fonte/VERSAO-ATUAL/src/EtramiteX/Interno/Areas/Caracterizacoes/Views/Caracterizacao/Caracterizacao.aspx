<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CaracterizacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/caracterizacao.js") %>"></script>
	<script>
		$(function () {
			Caracterizacao.load($('#central'), {
				urls: {
					CriarProjetoGeo: '<%= Url.Action("Criar", "ProjetoGeografico") %>',
					EditarProjetoGeo: '<%= Url.Action("Editar", "ProjetoGeografico") %>',
					VisualizarProjetoGeo: '<%= Url.Action("Visualizar", "ProjetoGeografico") %>',
					CriarDscLicAtividade: '<%= Url.Action("Criar", "DescricaoLicenciamentoAtividade") %>',
					EditarDscLicAtividade: '<%= Url.Action("Editar", "DescricaoLicenciamentoAtividade") %>',
					VisualizarDscLicAtividade: '<%= Url.Action("Visualizar", "DescricaoLicenciamentoAtividade") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Caracterização do Empreendimento</h2>
		<br />

		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Model.EmpreendimentoId %>" />
		<fieldset class="block box">
			<legend>Empreendimento</legend>
			<div class="block">
                <div class="coluna20 append1">
					<label>Código</label>
					<%= Html.TextBox("EmpreendimentoId", Model.Codigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna77">
					<label><%=Model.DenominadorTexto%> *</label>
					<%= Html.TextBox("DenominadorValor", Model.DenominadorValor, new { @maxlength = "100", @class = "text denominador disabled", @disabled = "disabled" })%>
				</div>

			</div>

			<div class="block">
				<div class="coluna20 append1">
					<label>Zona de localização *</label>
					<%= Html.DropDownList("SelecionarPrimeiroItem", Model.ZonaLocalizacao, new { disabled = "disabled", @class = "text disabled" })%>
				</div>
				<div class="coluna7 append1">
					<label>UF</label>
					<%= Html.DropDownList("SelecionarPrimeiroItem", Model.Uf, new { disabled = "disabled", @class = "text disabled" })%>
				</div>
				<div class="coluna45 append1">
					<label>Município</label>
					<%= Html.DropDownList("SelecionarPrimeiroItem", Model.Municipio, new { disabled = "disabled", @class = "text disabled" })%>
				</div>
                <div class="coluna20 ">
					<label>CNPJ</label>
					<%= Html.TextBox("CNPJ", Model.CNPJ, new { @maxlength = "100", @class = "text cnpj disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box fsCadastradas">
			<legend>Cadastradas</legend>
			<% if (Model.CaracterizacoesCadastradas.Count > 0) { %>
			<div class="divCadastradas">
				<table class="gridCaractCadastrada" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<% foreach (var item in Model.CaracterizacoesCadastradas) { %>
						<tr>
							<td>
								<span title="<%: item.Nome %>"><%: item.Nome %></span>
							</td>
							<td width="25%">
								<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)item.Tipo %>" />
								<input type="hidden" class="hdnUrlVisualizar" value="<%= Html.Encode(item.UrlVisualizar) %>" />
								<input type="hidden" class="hdnUrlEditar" value="<%= Html.Encode(item.UrlEditar) %>" />
								<input type="hidden" class="hdnUrlExcluirConfirm" value="<%= Html.Encode(item.UrlExcluirConfirm) %>" />
								<input type="hidden" class="hdnUrlExcluir" value="<%= Html.Encode(item.UrlExcluir) %>" />
								<input type="hidden" class="hdnProjetoGeograficoId" value="<%= Html.Encode(item.ProjetoGeograficoId) %>" />
								<input type="hidden" class="hdnDscLicAtividade" value="<%= Html.Encode(item.DscLicAtividadeId) %>" />
								<input type="hidden" class="hdnProjetoGeograficoVisualizar" value="<%= Html.Encode(item.ProjetoGeograficoVisualizar) %>" />
								<input type="hidden" class="hdnDscLicAtividadeVisualizar" value="<%= Html.Encode(item.DscLicAtividadeVisualizar) %>" />

								<% if (item.PodeVisualizar) { %><button title="Visualizar" class="icone visualizar btnVisualizar" type="button"></button><% } %>
								<% if (item.ProjetoGeografico && item.ProjetoGeograficoId > 0) { %><button title="Projeto geográfico" class="icone projetoGeografico btnProjetoGeografico" type="button"></button><% } %>
								<% if (item.DscLicAtividade && item.DscLicAtividadeId > 0) { %><button title="Descrição de Atividade" class="icone btnDscLicAtividade descricaoAtividades" type="button">ATV</button><% } %>
								<% if (item.PodeEditar) { %><button title="Editar" class="icone editar btnEditar" type="button"></button><% } %>
								<% if (item.PodeExcluir) { %><button title="Excluir" class="icone excluir btnExcluir" type="button"></button><% } %>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
			<% } else { %>
				<span title="Não existem caracterizações cadastradas."><label>Não existem caracterizações cadastradas.</label></span>
			<% } %>
		</fieldset>

		<fieldset class="block box">
			<legend>Não Cadastradas</legend>
			<% if (Model.CaracterizacoesNaoCadastradas.Count > 0) { %>
			<div class="dataGrid">
				<table class="gridCaracterizacao" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<% foreach (var item in Model.CaracterizacoesNaoCadastradas) { %>
						<tr>
							<td>
								<span title="<%: item.Nome %>"><%: item.Nome %></span>
							</td>
							<td width="13%">
								<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)item.Tipo %>" />
								<input type="hidden" class="hdnUrlCriar" value="<%= Html.Encode(item.UrlCriar) %>" />
								<input type="hidden" class="hdnPossuiProjetoGeo" value="<%= item.ProjetoGeoObrigatorio.ToString().ToLower() %>" />
								<input type="hidden" class="hdnProjetoGeograficoId" value="<%= Html.Encode(item.ProjetoGeograficoId) %>" />
								<input type="hidden" class="hdnDscLicAtividade" value="<%= Html.Encode(item.DscLicAtividadeId) %>" />
								<% var temModeloCarac = (item.ProjetoGeograficoId > 0) || (item.DscLicAtividadeId > 0); %>
								<% if (item.ProjetoGeografico && item.ProjetoGeograficoId > 0) { %><button title="Projeto geográfico" class="icone projetoGeografico btnAdicionar" type="button"></button><% } %>
								<% if (item.DscLicAtividade && item.DscLicAtividadeId > 0) { %><button title="Descrição de Atividade" class="icone btnDscLicAtividade descricaoAtividades" type="button">ATV</button><% } %>
								<% if (item.PodeCadastrar && !temModeloCarac) { %><button title="Cadastrar" class="icone criarNovo btnAdicionar" type="button"></button><% } %>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
			<% } else { %>
				<span title="Não existem caracterizações não cadastradas."><label>Não existem caracterizações não cadastradas.</label></span>
			<% } %>
		</fieldset>

		<div class="block box">
			<span class="cancelarCaixa"><span class="btnModalOu"></span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("", "Empreendimento", new { area = ""}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>