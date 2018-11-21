<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EmpreendimentoVM>" %>

<div class="modalLocalizarEmpreendimento">
<h1 class="titTela">Identificação do Empreendimento</h1>
<br />

<div class="block filtroSerializarAjax">
	<%= Html.Hidden("UrlFiltrar", Url.Action("Localizar", "Empreendimento"), new { @class = "urlFiltrar" })%>
	<%= Html.Hidden("Paginacao.OrdenarPor", 0, new { @class = "ordenarPor" })%>
	<%= Html.Hidden("ModoEmpreendimento", false, new { @class = "hdnModoEmp" })%>
	<%= Html.Hidden("EstadoDefault", Model.LocalizarVM.EstadoDefault, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.LocalizarVM.EstadoDefaultSigla, new { @class = "hdnEstadoDefaultSigla" })%>
	<%= Html.Hidden("CidadeDefault", Model.LocalizarVM.MunicipioDefault, new { @class = "hdnCidadeDefault" })%>

	<div class="divCod box">
		<div class="block">
			<div class="coluna37">
				<input type="hidden" name ="Filtros_IsInformacaoCorte" value="<%= Model.LocalizarVM.IsInformacaoCorte %>" />
				<label> Empreendimento com código identificador?</label><br />
				<label><%= Html.RadioButton("Filtros.PossuiCodigo", true, Model.LocalizarVM.Filtros.PossuiCodigo, new { @class = "radio RadioEmpreendimentoCodigo rbCodigoSim" })%>Sim</label>
				<label><%= Html.RadioButton("Filtros.PossuiCodigo", false, !Model.LocalizarVM.Filtros.PossuiCodigo, new { @class = "radio RadioEmpreendimentoCodigo rbCodigoNao" })%>Não</label>
			</div>
			
			<div class="coluna30 prepend1 divCodigoEmp <%= (Model.LocalizarVM.Filtros.PossuiCodigo) ? "" : "hide" %>">
				<label for="Filtros_Codigo">Código do empreendimento</label>
				<%= Html.TextBox("Filtros.Codigo", Model.LocalizarVM.Filtros.Codigo , new { @class = "txtCodigo text maskIntegerObrigatorio", @maxlength = "13" })%> 
			</div>

			<div class="coluna8 prepend1 divCodigoEmp <%= (Model.LocalizarVM.Filtros.PossuiCodigo) ? "" : "hide" %>">
				<button type="button" title="verificar" id="btnVerificarCodigo" class="inlineBotao btnVerificarCodigo">Verificar</button>
			</div>
		</div>
	</div>

	<div class="divFiltros box <%= (Model.LocalizarVM.Filtros.PossuiCodigo) ? "hide" : "" %>">
		<div class="block">
			<div class="coluna21">
				<label for="Filtros_Coordenada_Tipo_Id">Sistema de coordenada *</label>
				<%= Html.DropDownList("Filtros.Coordenada.Tipo.Id", Model.LocalizarVM.TiposCoordenada, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%> 
			</div>

			<div class="coluna19 prepend1">
				<label for="Filtros_Coordenada_Datum_Id">Datum *</label>
				<%= Html.DropDownList("Filtros.Coordenada.Datum.Id", Model.LocalizarVM.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%> 
			</div>

			<div class="coluna19 prepend1">
				<label for="Filtros_Coordenada_FusoUtm">Fuso *</label>
				<%= Html.DropDownList("Filtros.Coordenada.FusoUtm", Model.LocalizarVM.Fusos, new { @class = "text disabled ddlFuso", @disabled = "disabled" })%> 
			</div>

			<div class="coluna21 prepend1">
				<label for="Filtros_AreaAbrangencia">Área de abrangência (m) *</label>
				<%= Html.TextBox("Filtros.AreaAbrangencia", Model.LocalizarVM.Filtros.AreaAbrangencia, new { @class = "text maskNumInt", @maxlength = "5" })%> 
			</div>
		</div>

		<div class="block">
			<div class="coluna21">
				<label for="Filtros_Coordenada_EastingUtmTexto">Easting *</label>
				<%= Html.TextBox("Filtros.Coordenada.EastingUtmTexto", Model.LocalizarVM.Filtros.Coordenada.EastingUtmTexto, new { @class = "text disabled txtEasting", @disabled = "disabled" })%> 
			</div>

			<div class="coluna19 prepend1">
				<label for="Filtros_Coordenada_NorthingUtmTexto">Northing *</label>
				<%= Html.TextBox("Filtros.Coordenada.NorthingUtmTexto", Model.LocalizarVM.Filtros.Coordenada.NorthingUtmTexto, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%> 
			</div>

			<div class="coluna19 prepend1">
				<label for="Filtros_Coordenada_HemisferioUtm">Hemisfério *</label>
				<%= Html.DropDownList("Filtros.Coordenada.HemisferioUtm", Model.LocalizarVM.Hemisferios, new { @class = "text disabled ddlHemisferio", @disabled = "disabled" })%> 
			</div>

			<div class="coluna20 prepend1">
				<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna65">
				<label for="Filtros_Responsavel_NomeRazao">Responsável do empreendimento</label>
				<%= Html.Hidden("Filtros.Responsavel.Id", Model.LocalizarVM.Filtros.Responsavel.Id, new { @class = "hdnResponsavelId" })%>
				<%= Html.TextBox("Filtros.Responsavel.NomeRazao", Model.LocalizarVM.Filtros.Responsavel.NomeRazao, new { @class = "text disabled txtNomeResponsavel", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 prepend1">
				<label for="Filtros_Responsavel_CpfCnpj">CPF/CNPJ</label>
				<%= Html.TextBox("Filtros.Responsavel.CpfCnpj", Model.LocalizarVM.Filtros.Responsavel.CpfCnpj, new { @class = "text disabled txtCnpjResponsavel", @disabled = "disabled" })%> 
			</div>
			<div class="coluna8 prepend1 divBtnBuscarResponsavel <%= Model.LocalizarVM.Filtros.Responsavel.Id > 0 ? "hide" : "" %>">
				<button type="button" title="Buscar responsável" id="btnAssociarResponsavel" class="inlineBotao btnAssociarResponsavel">Buscar</button>
			</div>
			<div class="coluna8 prepend1 divBtnLimparResponsavel <%= Model.LocalizarVM.Filtros.Responsavel.Id > 0 ? "" : "hide" %>">
				<button type="button" title="Limpar responsável" id="btnLimparResponsavel" class="inlineBotao btnLimparResponsavel">Limpar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna21">
				<label for="Filtros_Empreendimento_SegmentoId">Segmento</label>
				<%= Html.DropDownList("Filtros.Segmento", Model.LocalizarVM.Segmentos, new { @class = "text ddlSegmento" })%> 
			</div>
			<div class="coluna75 prepend1">
				<label for="Filtros_Denominador" class="lblDenominador">Denominação</label>
				<%= Html.TextBox("Filtros.Denominador", Model.LocalizarVM.Filtros.Denominador, new { @class = "text txtDenominador", @maxlength = "100" })%>
			</div>
		</div>

		<div class="block divEstiloCnpj">
			<div class="coluna21 divCnpjEmp divCnpjEmpMostrar">
				<label for="Filtros_CnpjEmpreemdimento">CNPJ</label>
				<%= Html.TextBox("Filtros.CnpjEmpreemdimento", Model.LocalizarVM.Filtros.CnpjEmpreemdimento, new { @class = "text txtCnpjEmp maskCnpj" })%>
			</div>
		</div>

		<div class="block">
			<button style="float:right" class="inlineBotao btnBuscar">Verificar Empreendimento</button>
		</div>
	</div>
</div>

<div class="gridContainer">
	<% if (Model.ListarVM.Resultados.Count > 0 && Model.LocalizarVM.IsInformacaoCorte) {%>
		<fieldset class="block box">
			<legend>Empreendimentos</legend>
			<% Html.RenderPartial("ListarResultadosCorte", Model.ListarVM); %>
		</fieldset>
	<%} %>
</div>
</div>