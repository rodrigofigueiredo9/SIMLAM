<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Protocolos</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlValidarPosse", Url.Action("ValidarProcessoPosse", "Processo"), new { @class = "urlValidarPosse" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", null, new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna42">
					<label for="Filtros_Numero">Número de registro</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", null, new { @class = "text txtNumero setarFoco" })%>
				</div>
				<div class="coluna40 prepend2">
					<label for="Filtros_ProcessoNumeroAutuacao">Número de autuação (SEP)</label>
					<%= Html.TextBox("Filtros.NumeroAutuacao", null, new { @class = "text txtNumero" })%>
				</div>
				
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">	
				<div class="coluna42">
					<label for="Filtros_Municipio">Município de origem do protocolo</label>
					<%= Html.DropDownList("Filtros.Municipio", Model.ListaMunicipios, new { @class = "text ddlMunicipio" })%> 
				</div>
			</div>

			<div class="block hide">
				<div class="coluna65">
					<label for="Filtros_InteressadoNomeRazao"> Nome/Razão social do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoNomeRazao", null, new { @class = "text" })%>
				</div>
				<div class="coluna29 prepend2">
					<label><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioInteressadoCpfCnpj radioCPF" })%>CPF</label>
					<label><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioInteressadoCpfCnpj" })%>CNPJ do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoCpfCnpj", Model.Filtros.InteressadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna65">
					<label for="Filtros_EmpreendimentoRazaoDenominacao">Razão social/ Denominação/Nome da propriedade/Imóvel</label>
					<%= Html.TextBox("Filtros.EmpreendimentoRazaoDenominacao", null, new { @class = "text" })%>
				</div>
				<div class="coluna29 prepend2">
					<label for="Filtros_EmpreendimentoCnpj">CNPJ do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCnpj", null, new { @class = "text maskCnpjParcial" })%>
				</div>
			</div>
		</div>

	</div>
</div>

<div class="gridContainer"></div>