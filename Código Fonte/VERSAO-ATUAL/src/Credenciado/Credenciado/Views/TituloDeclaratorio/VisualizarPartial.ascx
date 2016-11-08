﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/TituloDeclaratorio/tituloDeclaratorio.js") %>"></script>
<script type="text/javascript">
	$(function () {
		TituloDeclaratorio.load($('#central'), {
			urls: {
				modelosCadastrados: '<%= Url.Action("ObterModelosCadastradosSetor", "Titulo") %>',
				associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
				validarAssociarRequerimento: '<%= Url.Action("ValidarAssociarRequerimento", "TituloDeclaratorio") %>',
				tituloCamposModelo: '<%= Url.Action("TituloCamposModelo", "TituloDeclaratorio") %>',
				obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "TituloDeclaratorio") %>',
				obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "TituloDeclaratorio") %>',
				especificidade: '<%= Url.Action("Salvar", "Especificidade", new {area="Especificidades"}) %>',

				redirecionar: '<%= Url.Action("Criar", "TituloDeclaratorio") %>',
				salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
			},
			Mensagens: <%= Model.Mensagens %>,
			carregarEspecificidade: <%= Model.Titulo.Modelo.PossuiEspecificidade().ToString().ToLower() %>,
			isVisualizar: true
		});
	});
</script>

<h1 class="titTela">Visualizar Título Declaratório</h1>
<br />

<div class="divVisualizarTitulo">
	<%= Html.Hidden("TituloId", Model.Titulo.Id, new { @class = "hdnTituloId" })%>

	<div class="block box">
		<div class="block">
			<div class="coluna75">
				<label for="AutorNome">Autor *</label>
				<%= Html.TextBox("AutorNome", Model.Titulo.Autor.Nome, new { @maxlength = "80", @class = "disabled text txtAutor", @disabled = "disabled" })%>
			</div>
			<div class="coluna18 prepend2">
				<label for="DataCriacao">Data de criação *</label>
				<%= Html.TextBox("DataCriacao", Model.Titulo.DataCriacao.DataTexto, new { @maxlength = "80", @class = "disabled text txtDataCriacao", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna75">
				<label for="LocalEmissao">Local da emissão *</label>
				<%= Html.DropDownList("LocalEmissao", Model.LstLocalEmissao, new { @class = "disabled text ddlLocal", @disabled = "disabled" })%>
			</div>
			<div class="coluna18 prepend2">
				<label for="SituacaoTexto">Situação *</label>
				<%= Html.TextBox("SituacaoTexto", Model.Titulo.Situacao.Texto, new { @maxlength = "80", @class = "disabled text txtSituacao", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna75">
				<label for="Modelos">Modelo *</label>
				<%= Html.DropDownList("Modelos", Model.LstModelos, new { @class = "text ddlModelos disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna18 prepend2">
				<label for="SituacaoTexto">Origem *</label>
				<%= Html.TextBox("OrigemTexto", Model.Titulo.CredenciadoId.GetValueOrDefault() > 0 ? "Credenciado" : "Institucional", new { @class = "disabled text txtOrigem", @disabled = "disabled" })%>
			</div>
		</div>
	</div>

	<div class="block box tituloRequerimento">
		<% Html.RenderPartial("TituloDeclaratorioRequerimento"); %>
	</div>

	<% if ((Model.Titulo.Prazo > 0 || Model.Titulo.DataEmissao.Data != null || Model.Titulo.DataAssinatura.Data != null ||
			Model.Titulo.DataEntrega.Data != null || Model.Titulo.DataVencimento.Data != null || Model.Titulo.DiasProrrogados > 0 || Model.Titulo.DataEncerramento.Data != null)) { %>
	<div class="block box">
		<% if (!string.IsNullOrEmpty(Model.LabelTipoPrazo)) { %>
		<div class="coluna25 append2">
			<label for="Prazo">Prazo em <%= Model.LabelTipoPrazo %> *</label>
			<%= Html.TextBox("Prazo", Model.Titulo.Prazo, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DataEmissao.Data != null) { %>
		<div class="coluna25 append2">
			<label for="DataEmissao">Data de emissão *</label>
			<%= Html.TextBox("DataEmissao", Model.Titulo.DataEmissao.DataTexto, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DataAssinatura.Data != null) { %>
		<div class="coluna25 append2">
			<label for="DataAssinatura">Data de assinatura *</label>
			<%= Html.TextBox("DataAssinatura", Model.Titulo.DataAssinatura.DataTexto, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DataEntrega.Data != null) { %>
		<div class="coluna25 append2">
			<label for="DataEntrega">Data de entrega *</label>
			<%= Html.TextBox("DataEntrega", Model.Titulo.DataEntrega.DataTexto, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DataVencimento.Data != null) { %>
		<div class="coluna25 append2">
			<label for="DataVencimento">Data de vencimento *</label>
			<%= Html.TextBox("DataVencimento", Model.Titulo.DataVencimento.DataTexto, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DiasProrrogados > 0) { %>
		<div class="coluna25 append2">
			<label for="DiasProrrogados">Dias prorrogados *</label>
			<%= Html.TextBox("DiasProrrogados", Model.Titulo.DiasProrrogados, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>

		<% if (Model.Titulo.DataEncerramento.Data != null) { %>
		<div class="coluna25">
			<label for="DataEncerramento">Data de encerramento *</label>
			<%= Html.TextBox("DataEncerramento", Model.Titulo.DataEncerramento.DataTexto, new { @class = "disabled text", @disabled = "disabled" })%>
		</div>
		<% } %>
	</div>
	<% } %>

	<div class="divTituloEspContent">
	</div>

	<div class="tituloValoresModelo">
		<% Html.RenderPartial("TituloCamposModelo"); %>
	</div>
</div>