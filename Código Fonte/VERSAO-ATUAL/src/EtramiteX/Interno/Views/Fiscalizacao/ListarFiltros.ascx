<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Fiscalizações</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlDocumentosGerados", Url.Action("DocumentosGeradosPartial"), new { @class = "urlDocumentosGerados" })%>
		<%= Html.Hidden("UrlVisualizarPdf", Url.Action("GerarPdf"), new { @class = "urlVisualizarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna16 append2">
					<label for="Filtros_NumeroFiscalizacao">Nº da fiscalização</label>
					<%= Html.TextBox("Filtros.NumeroFiscalizacao", Model.Filtros.NumeroFiscalizacao, new { @class = "text txtNumeroFiscalizacao maskNum15 setarFoco" })%>
				</div>

				<div class="coluna15 append2">
					<label for="Filtros_NumeroAIBloco">Nº do AI</label>
					<%= Html.TextBox("Filtros.NumeroAIBloco", Model.Filtros.NumeroAIBloco, new { @class = "text txtNumeroAIBloco maskNum15" })%>
				</div>

				<div class="coluna15 append2">
					<label for="Filtros_NumeroTEIBloco">Nº do TEI</label>
					<%= Html.TextBox("Filtros.NumeroTEIBloco", Model.Filtros.NumeroTEIBloco, new { @class = "text txtNumeroTEIBloco maskNum15" })%>
				</div>

				<div class="coluna16 append2">
					<label for="Filtros_NumeroTADBloco">Nº do TAD</label>
					<%= Html.TextBox("Filtros.NumeroTADBloco", Model.Filtros.NumeroTADBloco, new { @class = "text txtNumeroTADBloco maskNum15" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna52 append2">
					<label for="Filtros_AutuadoNomeRazao">Nome/Razão social do autuado</label>
					<%= Html.TextBox("Filtros.AutuadoNomeRazao", Model.Filtros.AutuadoNomeRazao, new { @class = "text txtAutuadoNomeRazao" })%>
				</div>

				<div class="coluna27">
					<label for="Filtros_AutuadoCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioAutuadoCpfCnpj radioCPF" })%>CPF autuado</label>
					<label for="Filtros_AutuadoCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioAutuadoCpfCnpj" })%>CNPJ autuado</label>
					<%= Html.TextBox("Filtros.AutuadoCpfCnpj", Model.Filtros.AutuadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna52 append2">
					<label for="Filtros_EmpreendimentoDenominador">Razão social/Denominação/Nome do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoDenominador", Model.Filtros.EmpreendimentoDenominador, new { @class = "text txtEmpreendimentoDenominador" })%>
				</div> 

				<div class="coluna27">
					<label for="Filtros_EmpreendimentoCnpj">CNPJ do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCnpj", Model.Filtros.EmpreendimentoCnpj, new { @class = "text maskCnpjParcial txtEmpreendimentoCnpj" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna20 append2">
					<label for="Filtros_DataFiscalizacao_DataTexto">Data da vistoria</label>
					<%= Html.TextBox("Filtros.DataFiscalizacao.DataTexto", Model.Filtros.DataFiscalizacao.DataTexto, new { @class = "text txtDataFiscalizacao maskData" })%>
				</div>

				<div class="coluna29 append2">
					<label for="Filtros_SituacaoTipo">Situação</label>
					<%= Html.DropDownList("Filtros.SituacaoTipo", Model.SituacaoTipo, new { @class = "text ddlSituacaoTipo" })%>
				</div>

				<div class="coluna27 append2">
					<label for="Filtros_Protocolo_NumeroTexto">Nº Registro do processo</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", Model.Filtros.Protocolo.NumeroTexto, new { @class = "text txtProtocoloNumeroTexto" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna39 append2">
					<label for="Filtros_InfracaoTipo">Tipo infração</label>
					<%= Html.DropDownList("Filtros.InfracaoTipo", Model.InfracaoTipo, new { @class = "text ddlInfracaoTipo" })%>
				</div>

				<div class="coluna40">
					<label for="Filtros_ItemTipo">item</label>
					<%= Html.DropDownList("Filtros.ItemTipo", Model.Itens, new { @class = "text ddlItens" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna82">
					<label for="Filtros_AgenteFiscal">Agente fiscal</label>
					<%= Html.TextBox("Filtros.AgenteFiscal", Model.Filtros.AgenteFiscal, new { @class = "text txtAgenteFiscal" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna82">
					<label for="Filtros_SetorTipo">Setor</label>
					<%= Html.DropDownList("Filtros.SetorTipo", Model.Setores, new { @class = "text ddlSetores" })%>
				</div>
			</div>
		</div>
	</div>
</div>



<div class="gridContainer"></div>