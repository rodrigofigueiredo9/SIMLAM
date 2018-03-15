<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">CAR</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlVisualizarMotivo", Url.Action("VisualizarMotivo"), new { @class = "urlVisualizarMotivo" })%>
		<%= Html.Hidden("UrlPdfTitulo", Url.Action("GerarTituloPdf"), new { @class = "urlPdfTitulo" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="ultima">
			<div class="block fixado">
				<div class="coluna28 append1">
					<label for="Filtros_SolicitacaoNumero">Nº de controle da solicitação</label>
					<%= Html.TextBox("Filtros.SolicitacaoNumero", null, new { @class = "text setarFoco maskIntegerObrigatorio", @maxlength="10" })%>
				</div>

				<div class="coluna28 append1">
					<label for="Filtros_EmpreendimentoCodigo">Código do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCodigo", null, new { @class = "text txtCodigo maskIntegerObrigatorio", @maxlength="13"})%>
				</div>

				<div class="coluna28">
					<label for="Filtros_Protocolo_Numero">Nº de registro do protocolo</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", null, new { @class = "text", @maxlength="15"})%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna58 append1">
					<label for="Filtros_DeclaranteNomeRazao">Nome/ Razão Social do declarante</label>
					<%= Html.TextBox("Filtros.DeclaranteNomeRazao", null, new { @class = "text"})%>
				</div>

				<div class="coluna28">
					<label for="Filtros_DeclaranteCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioDeclaranteCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_DeclaranteCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioDeclaranteCpfCnpj" })%>CNPJ do declarante</label>
					<%= Html.TextBox("Filtros.DeclaranteCpfCnpj", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna58 append1">
					<label for="Filtros_EmpreendimentoDenominador">Nome/ Razão Social/ Denominação/ Imóvel</label>
					<%= Html.TextBox("Filtros.EmpreendimentoDenominador", null, new { @class = "text"})%>
				</div>

				<div class="coluna28">
					<label for="Filtros_EmpreendimentoMunicipio">Município</label>
					<%= Html.DropDownList("Filtros.Municipio", Model.ListaMunicipios, new { @class = "text" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna11 append2">
					<label for="Filtros_Titulo_Texto">Nº do título</label>
					<%= Html.TextBox("Filtros.Titulo.Texto", Model.Filtros.Titulo.Texto, new { @class = "text txtTituloTexto maskNumAno" })%>
				</div>

				<div class="coluna28 append2">
					<label for="Filtros_RequerimentoNumero">Nº do Requerimento/ Projeto digital</label>
					<%= Html.TextBox("Filtros.Requerimento", null, new { @class = "text maskNumInt", @maxlength="15"})%>
				</div>

				<div class="coluna20 append2">
					<label for="Filtros_Situacao">Situação da solicitação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.SituacaoLst, new { @class = "text" })%>
				</div>

				<div class="coluna20">
					<label for="Filtros_Origem">Origem da solicitação</label>
					<%= Html.DropDownList("Filtros.Origem", Model.OrigemLst, new { @class = "text" })%>
				</div>
			</div>
            <div class="block hide">
				<div class="coluna58 append1">
					<label for="Filtros_EmpreendimentoDenominador">N° SICAR</label>
					<%= Html.TextBox("Filtros.codigoImovelSicar", null, new { @class = "text"})%>
				</div>

				<div class="coluna28">
					<label for="Filtros_EmpreendimentoMunicipio">Situação Arquivo SICAR</label>
					<%= Html.DropDownList("Filtros.SituacaoSicar", Model.SituacoesSicar, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>