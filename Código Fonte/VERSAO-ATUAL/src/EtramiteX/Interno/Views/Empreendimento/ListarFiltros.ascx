<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Empreendimentos</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlValidarPosse", Url.Action("ValidarPosseEmpreendimento"), new { @class = "urlValidarPosse" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", null, new { @class = "ordenarPor" })%>
		<%= Html.Hidden("Filtros.EmpreendimentoCompensacao", Model.Filtros.EmpreendimentoCompensacao ) %>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna28">
					<label for="Filtros_Codigo">Código do empreendimento</label>
					<%= Html.TextBox("Filtros.Codigo", null, new { @class = "text txtCodigo setarFoco maskIntegerObrigatorio", @maxlength = "13" })%>
				</div>
				<div class="coluna58 prepend1">
					<label for="Filtros_Denominador">Razão social/Denominação/Nome</label>
					<%= Html.TextBox("Filtros.Denominador", null, new { @class = "text txtDenominador", @maxlength = "80" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna28">
					<label for="Filtros_CNPJ">CNPJ do empreendimento</label>
					<%= Html.TextBox("Filtros.CNPJ", null, new { @class = "text maskCnpjParcial" })%>
				</div>

				<div class="coluna28 prepend1">
					<label for="Filtros_Segmento">Segmento</label>
					<%= Html.DropDownList("Filtros.Segmento", Model.SelListSegmentos, new { @class = "text" })%>
				</div>

				<div class="coluna28 prepend1">
					<label for="Filtros_Protocolo_NumeroTexto">Nº registro</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", Model.Filtros.Protocolo.NumeroTexto, new { @class = "text txtProtocoloNumeroTexto" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna28">
					<label for="Filtros_Responsavel_CpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioResponsavelCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_Responsavel_CpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioResponsavelCpfCnpj" })%>CNPJ do responsável</label>
					<%= Html.TextBox("Filtros.Responsavel.CpfCnpj", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
				<div class="coluna58 prepend1">
					<label for="Filtros_Responsavel_NomeRazao">Razão social/Nome do responsável</label>
					<%= Html.TextBox("Filtros.Responsavel.NomeRazao", null, new { @class = "text", @maxlength = "80" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna88">
					<label for="Filtros_Atividade_Atividade">Atividade principal</label>
					<%= Html.DropDownList("Filtros.Atividade.Id", Model.SelListAtividades, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>