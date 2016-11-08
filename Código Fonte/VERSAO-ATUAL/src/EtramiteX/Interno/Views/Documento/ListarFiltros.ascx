<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Documentos</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlValidarPosse", Url.Action("ValidarDocumentoPosse", "Documento"), new { @class = "urlValidarPosse" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="ultima">
			<div class="block fixado">
				<div class="coluna23">
					<label for="Filtros_Protocolo_NumeroTexto">Nº de registro do documento</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", null, new { @class = "text txtNumero setarFoco" })%>
				</div>
				<div class="coluna41 prepend1">
					<label for="Filtros_Nome">Nome do documento</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text txtNome" })%>
				</div>
				<div class="coluna20 prepend1">
					<label for="Filtros_EmpreendimentoCodigo">Cód. do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCodigo", null, new { @class = "text maskIntegerObrigatorio txtEmpreendimentoCodigo", @maxlength="13" })%>
				</div>
				<div class="ultima">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna23">
					<label for="Filtros_DataRegistro_DataTexto">Data de registro</label>
					<%= Html.TextBox("Filtros.DataRegistro.DataTexto", null, new { @class = "text maskData setarFoco" })%>
				</div>
				<div class="coluna33 prepend1">
					<label for="Filtros_Tipo">Tipo do documento</label>
					<%= Html.DropDownList("Filtros.Tipo", Model.ListaTiposDocumento, new { @class = "text " })%>
				</div>
				<div class="coluna28 prepend1">
					<label for="Filtros_Municipio">Município de origem do documento</label>
					<%= Html.DropDownList("Filtros.Municipio", Model.ListaMunicipios, new { @class = "text " })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna58">
					<label for="Filtros_AtividadeSolicitada">Atividade solicitada</label>
					<%= Html.DropDownList("Filtros.AtividadeSolicitada", Model.ListaAtividadeSolicitadas, new { @class = "text " })%>
				</div>
				<div class="coluna28 prepend1">
					<label for="Filtros_SituacaoAtividade">Situação da atividade solicitada</label>
					<%= Html.DropDownList("Filtros.SituacaoAtividade", Model.ListaSituacaoAtividades, new { @class = "text " })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna58">
					<label for="Filtros_InteressadoNomeRazao"> Nome/Razão social do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoNomeRazao", null, new { @class = "text" })%>
				</div>
				<div class="coluna28 prepend1">
					<label for="Filtros_InteressadoCpfCnpj"><%= Html.RadioButton("Filtros.InteressadoIsCnpj", true, !Model.Filtros.InteressadoIsCnpj, new { @class = "radio radioInteressadoCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_InteressadoCpfCnpj"><%= Html.RadioButton("Filtros.InteressadoIsCnpj", false, Model.Filtros.InteressadoIsCnpj, new { @class = "radio radioInteressadoCpfCnpj" })%>CNPJ do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoCpfCnpj", Model.Filtros.InteressadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna58">
					<label for="Filtros_EmpreendimentoRazaoDenominacao">Razão social/ Denominação/Nome do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoRazaoDenominacao", null, new { @class = "text" })%>
				</div>
				<div class="coluna28 prepend1">
					<label for="Filtros_EmpreendimentoCnpj">CNPJ do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCnpj", null, new { @class = "text maskCnpjParcial" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>