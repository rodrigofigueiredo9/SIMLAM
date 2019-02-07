<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CaracterizacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/caracterizacao.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			Caracterizacao.load($('#central'), {
				urls: {
					CriarProjetoGeo: '<%= Url.Action("Criar", "ProjetoGeografico") %>',
					EditarProjetoGeo: '<%= Url.Action("Editar", "ProjetoGeografico") %>',
					VisualizarProjetoGeo: '<%= Url.Action("Visualizar", "ProjetoGeografico") %>',
					AssociarCaracterizacao: '<%= Url.Action("AssociarCaracterizacaoProjetoDigital", "Caracterizacao") %>',
					DesassociarCaracterizacao: '<%= Url.Action("DesassociarCaracterizacaoProjetoDigital", "Caracterizacao") %>',
					CopiarDadosInstitucional: '<%= Url.Action("CopiarDadosInstitucional", "Caracterizacao") %>',
					FinalizarPasso: '<%= Url.Action("FinalizarPasso", "Caracterizacao") %>'
				},
				empreendimentoID: '<%= Model.EmpreendimentoId %>',
				projetoDigitalID: '<%= Model.ProjetoDigitalId %>',
				caracterizacoesPossivelCopiar: <%: ViewModelHelper.Json(Model.CaracterizacoesPossivelCopiar) %>,
				dependenciaTipos: <%= Model.DependenciaTipos %>,
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Caracterização do Empreendimento</h2>
		<br />

		<fieldset class="block box">
			<input type="hidden" class="hdnIsVisualizar" value="<%= Model.IsVisualizar %>" />
			<legend>Empreendimento</legend>
			<div class="block">
				<div class="coluna20 append1">
					<label>Código</label>
					<%= Html.TextBox("Codigo", Model.CodigoTexto, new { @maxlength = "10", @class = "text disabled", @disabled = "disabled" })%>
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
				<div class="coluna20">
					<label>CNPJ</label>
					<%= Html.TextBox("CNPJ", Model.CNPJ, new { @maxlength = "100", @class = "text cnpj disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>

		<% foreach (string mensagem in Model.MensagensNotificacoes) { %>
		<div class="block msgInline erro">
			<p><label><%= mensagem %></label></p>
		</div>
		<% } %>

		<fieldset class="block box fsAssociadas">
			<legend>Associadas ao Projeto Digital</legend>
			<% if (Model.CaracterizacoesAssociadas.Count > 0) { %>
			<div class="divAssociadas">
				<table class="gridCaractAssociadas" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<% foreach (var item in Model.CaracterizacoesAssociadas) { %>
						<tr>
							<td>
								<span class="tipoTexto"  title="<%: item.Nome %>"><%: item.Nome %></span>
							</td>
							<td width="25%">
								<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)item.Tipo %>" />
								<input type="hidden" class="hdnUrlVisualizar" value="<%= Html.Encode(item.UrlVisualizar) %>" />
								<input type="hidden" class="hdnProjetoGeograficoId" value="<%= Html.Encode(item.ProjetoGeograficoId) %>" />
								<input type="hidden" class="hdnProjetoGeograficoVisualizar" value="<%= Html.Encode(item.ProjetoGeograficoVisualizar) %>" />
								<input type="hidden" class="hdnUrlListar" value="<%= Html.Encode(item.UrlListar) %>" />
								<% if (!Model.IsVisualizar && Model.CaracterizacoesAssociadas.Any(x => x.Tipo == eCaracterizacao.BarragemDispensaLicenca)) { %><input title="Cancelar associação ao projeto digital" class="icone dispensado btnDesassociar" type="hidden" /><%} %>
								<% else { %> <% if (!Model.IsVisualizar) { %><input title="Cancelar associação ao projeto digital" class="icone dispensado btnDesassociar" type="button" /><%} %>
								<%		} %>
								<% if (Model.CaracterizacoesAssociadas.Any(x => x.Tipo == eCaracterizacao.BarragemDispensaLicenca)) {%><input title="Operar" class="icone opcoes btnListar" type="button"/><% } %>
								<% else{%><input title="Visualizar" class="icone visualizar btnVisualizar" type="button"/>  <%} %>
								<% if (item.ProjetoGeografico && item.ProjetoGeograficoId > 0) { %><input title="Projeto geográfico" class="icone projetoGeografico btnProjetoGeografico" type="button"/><% } %>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
			<% } else { %>
				<span title="Não existem caracterizações concluídas."><label>Não existem caracterizações associadas ao projeto digital.</label></span>
			<% } %>

			<% if (Model.MostrarFinalizar) { %>
			<br /><button class="btnFinalizarPasso direita">Finalizar passo 2</button>
			<% } %>
		</fieldset>

		<%if (!Model.IsVisualizar) {%>
		<fieldset class="block box fsCadastradas">
			<legend>Cadastradas</legend>
			<% if (Model.CaracterizacoesCadastradas.Count > 0) { %>
			<div class="divCadastradas">
				<table class="gridCaractCadastrada" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<% foreach (var item in Model.CaracterizacoesCadastradas) { %>
						<tr>
							<td>
								<span class="tipoTexto" title="<%: item.Nome %>"><%: item.Nome %></span>
							</td>
							<td width="25%">
								<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)item.Tipo %>" />
								<input type="hidden" class="hdnUrlVisualizar" value="<%= Html.Encode(item.UrlVisualizar) %>" />
								<input type="hidden" class="hdnUrlEditar" value="<%= Html.Encode(item.UrlEditar) %>" />
								<input type="hidden" class="hdnUrlExcluirConfirm" value="<%= Html.Encode(item.UrlExcluirConfirm) %>" />
								<input type="hidden" class="hdnUrlExcluir" value="<%= Html.Encode(item.UrlExcluir) %>" />
								<input type="hidden" class="hdnProjetoGeograficoId" value="<%= Html.Encode(item.ProjetoGeograficoId) %>" />
								<input type="hidden" class="hdnProjetoGeograficoVisualizar" value="<%= Html.Encode(item.ProjetoGeograficoVisualizar) %>" />
								<input type="hidden" class="hdnUrlListar" value="<%= Html.Encode(item.UrlListar) %>" />
								<% if (item.PodeAssociar && !Model.CaracterizacoesCadastradas.Any(x => x.Tipo == eCaracterizacao.BarragemDispensaLicenca)) { %><input title="Associar ao projeto digital" class="icone associar btnAssociar" type="button" /><% } %>
								<% if (item.PodeCopiar) { %><input title="Copiar do institucional" class="icone comparar btnCopiar" type="button" /><% } %>
								<% if (item.ProjetoGeografico && item.ProjetoGeograficoId > 0) { %><input title="Projeto geográfico" class="icone projetoGeografico btnProjetoGeografico" type="button"/><% } %>
								<% if (item.PodeVisualizar && item.PodeEditar && Model.CaracterizacoesCadastradas.Any(x => x.Tipo == eCaracterizacao.BarragemDispensaLicenca)) { %> <input title = "Operar" class="icone opcoes btnListar" type="button"/> <% } %>
								<% else { %>
								<% if (item.PodeVisualizar) { %><input title="Visualizar" class="icone visualizar btnVisualizar" type="button"/><% } %>
								<% if (item.PodeEditar) { %><input title="Editar" class="icone editar btnEditar" type="button"/><% } %>
								<% } %>
								<% if (item.PodeExcluir) { %><input title="Excluir" class="icone excluir btnExcluir" type="button"/><% } %>
								
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
		<%} %>

		<% if(!Model.IsVisualizar) {%>
		<fieldset class="block box">
			<legend>Não Cadastradas</legend>
			<% if (Model.CaracterizacoesNaoCadastradas.Count > 0) { %>
			<div class="dataGrid">
				<table class="gridCaracterizacao" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<% foreach (var item in Model.CaracterizacoesNaoCadastradas) { %>
						<tr>
							<td>
								<span class="tipoTexto"  title="<%: item.Nome %>"><%: item.Nome %></span>
							</td>
							<td width="13%">
								<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)item.Tipo %>" />
								<input type="hidden" class="hdnUrlCriar" value="<%= Html.Encode(item.UrlCriar) %>" />
								<input type="hidden" class="hdnPossuiProjetoGeo" value="<%= item.ProjetoGeoObrigatorio.ToString().ToLower() %>" />
								<input type="hidden" class="hdnProjetoGeograficoId" value="<%= Html.Encode(item.ProjetoGeograficoId) %>" />
								<input type="hidden" class="hdnUrlListar" value="<%= Html.Encode(item.UrlListar) %>" />

								<% var temModeloCarac = (item.ProjetoGeograficoId > 0); %>
								<% if (item.PodeCopiar) { %><input title="Copiar do Institucional" class="icone comparar btnCopiar" type="button" /><% } %>
								<% if (item.ProjetoGeografico && item.ProjetoGeograficoId > 0 && !item.PodeCopiar) { %><input title="Projeto geográfico" class="icone projetoGeografico btnAdicionar" type="button"/><% } %>
								<% if (item.PodeCadastrar && !temModeloCarac && !item.PodeCopiar) { %><input title="Cadastrar" class="icone criarNovo btnAdicionar" type="button" /><% } %>
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
		<%} %>
		<div class="block box">
			<span class="cancelarCaixa"><span class="btnModalOu"></span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Operar", "ProjetoDigital", new { area = "", Id = Model.ProjetoDigitalId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>