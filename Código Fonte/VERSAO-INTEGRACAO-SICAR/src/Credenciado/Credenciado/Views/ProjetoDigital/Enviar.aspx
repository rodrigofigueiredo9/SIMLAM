<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalEnviarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Enviar Projeto Digital</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/enviar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			AtividadeSolicitadaAssociar.load($('#central'));
			ProjetoDigitalEnviar.load($('#central'), {
				projetoDigitalId: '<%: Model.ProjetoDigital.Id %>',
				urls: {
					enviar: '<%= Url.Action("Enviar", "ProjetoDigital") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Enviar Projeto Digital</h1>
		<br />

		<fieldset class="block box">
			<legend>Projeto Digital</legend>
			<div class="block">
				<div class="coluna22">
					<label for="DataCriacao">Data de criação</label>
					<%= Html.TextBox("DataCriacao", Model.ProjetoDigital.DataCriacao.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
				<div class="coluna22 prepend2">
					<label for="RequerimentoId">Nº do Projeto Digital</label>
					<%= Html.TextBox("RequerimentoId", Model.ProjetoDigital.RequerimentoId, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
				<div class="coluna22 prepend2">
					<label for="SituacaoTexto">Situação do Projeto Digital</label>
					<%= Html.TextBox("SituacaoTexto", Model.ProjetoDigital.SituacaoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>1 - Requerimento Digital</legend>
			<div class="block box">
				<div class="coluna22">
					<label>Data de criação</label>
					<%= Html.TextBox("Requerimento.DataCadastro", Model.RequerimentoVM.DataCriacao, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna22 prepend2">
					<label>Nº do Requerimento</label>
					<%= Html.TextBox("Requerimento.Numero", Model.RequerimentoVM.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna22 prepend2">
					<label>Situação do Requerimento</label>
					<%= Html.TextBox("Requerimento.SituacaoTexto", Model.RequerimentoVM.SituacaoTexto, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="divConteudoAtividadeSolicitada associarMultiplo">
				<fieldset class="block boxBranca">
					<legend>Atividade Solicitada</legend>

					<div class="asmItens">
						<% foreach (var atividade in Model.RequerimentoVM.AtividadesSolicitadasVM) { %>
							<div class="asmItemContainer" style="border:0px">
								<% Html.RenderPartial("AtividadeSolicitadaVisualizar", atividade); %>
							</div>
						<% } %>
					</div>
				</fieldset>
			</div>

			<fieldset class="block boxBranca">
				<legend>Interessado</legend>
				<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.RequerimentoVM.Interessado.Id) %>" />
				<div class="coluna75">
					<label>Nome/Razão Social</label>
					<%= Html.TextBox("Requerimento.Pessoa.NomeRazaoSocial", Model.RequerimentoVM.Interessado.NomeRazaoSocial, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20 prepend2">
					<label>CPF/CNPJ</label>
					<%= Html.TextBox("Requerimento.Pessoa.CPFCNPJ", Model.RequerimentoVM.Interessado.CPFCNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</fieldset>

			<fieldset class="block boxBranca">
				<legend>Responsável Técnico</legend>
				<% Html.RenderPartial("~/Views/Requerimento/CriarRespTecnico.ascx", Model.RequerimentoVM); %>
			</fieldset>

			<fieldset class="block boxBranca">
				<legend>Empreendimento</legend>

				<% if(Model.RequerimentoVM.Empreendimento.Id > 0) { %>
				<div class="coluna75">
					<label class="lblDenominador">Denominação</label>
					<%= Html.TextBox("Requerimento.Empreendimento.Denominador", Model.RequerimentoVM.Empreendimento.Denominador, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20 prepend2">
					<label>CNPJ</label>
					<%= Html.TextBox("Requerimento.Empreendimento.CNPJ", Model.RequerimentoVM.Empreendimento.CNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<% } else { %>
				<label>Não existe empreendimento associado a este requerimento.</label>
				<% } %>
			</fieldset>
		</fieldset>

		<% if (Model.ProjetoDigital.Dependencias != null && Model.ProjetoDigital.Dependencias.Count > 0) { %>
		<fieldset class="block box">
			<legend>2 - Caracterizações</legend>
			<div class="dataGrid">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Caracterização do empreendimento</th>
						</tr>
					</thead>
					<tbody>
					<% foreach (var item in Model.ProjetoDigital.Dependencias.Where(x=> x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao)) { %>
						<tr>
							<td title="<%: item.DependenciaCaracterizacaoTexto %>"><%: item.DependenciaCaracterizacaoTexto %></td>
						</tr>
					<% } %>
					</tbody>
				</table>
			</div>
		</fieldset>
		<% } %>

		<% Html.RenderPartial("DocumentosGerados", Model.DocumentosGeradosVM); %>

		<div class="block box">
			<% if(Model.ModoVisualizar) { %>
				<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("Operar", "ProjetoDigital", new { id = Model.ProjetoDigital.Id }) %>">Cancelar</a></span>
			<% } else { %>
				<input class="btnConcluir floatLeft" type="button" value="Concluir" />
				<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Operar", "ProjetoDigital", new { id = Model.ProjetoDigital.Id }) %>">Cancelar</a></span>
			<% } %>
		</div>
	</div>
</asp:Content>