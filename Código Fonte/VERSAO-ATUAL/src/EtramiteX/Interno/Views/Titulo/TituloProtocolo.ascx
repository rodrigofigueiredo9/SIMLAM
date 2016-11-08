<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="block">
	<input type="hidden" class="hdnAtividadeEspecificidadeCaracterizacao" value='<%= Model.AtividadeEspecificidadeCaracterizacaoJSON %>' />
	<input type="hidden" class="hdnAtividadesID" value='<%= Model.AtividadesIDJSON %>' />
	<div class="coluna22">
		<label for="Numero">Número *</label>
		<% if (Model.IsNumeracaoAutomatica || Model.IsVisualizar) {%>
			<%= Html.TextBox("Numero", (Model.IsNumeracaoAutomatica && Model.Titulo.Numero.IsEmpty) ? "Gerado Automaticamente" : Model.Titulo.Numero.Texto, new { @class = "disabled text txtNumero", @disabled = "disabled" })%>
		<% } else { %>
			<%= Html.TextBox("Numero", Model.Titulo.Numero.Texto, new { @maxlength = "12", @class = "text txtNumero" })%>
		<% } %>
	</div>
	<div class="coluna51 prepend2">
		<% if ((!Model.IsVisualizar && Model.IsProtocoloProcesso) || Model.Titulo.Protocolo.IsProcesso) { %>
			<label class="append5"><% if (!Model.IsVisualizar){ %><%= Html.RadioButton("OpcaoBusca", 1, Model.Titulo.Protocolo.IsProcesso, new { @class = "radio rdbOpcaoBuscaProtocolo" })%><% } %>Nº de registro processo</label> 
		<% } %>
		<% if ((!Model.IsVisualizar && Model.IsProtocoloDocumento) || !Model.Titulo.Protocolo.IsProcesso) { %>
			<label class="append5"><% if (!Model.IsVisualizar){ %><%= Html.RadioButton("OpcaoBusca", 2, !Model.Titulo.Protocolo.IsProcesso, new { @class = "radio rdbOpcaoBuscaProtocolo" })%><% } %>N° de registro documento</label>
		<% } %>
		<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(Model.Titulo.Protocolo.Id) %>" />
		<%= Html.TextBox("ProtocoloNumero", Model.Titulo.Protocolo.Numero, new { @class = "text disabled txtProtocoloNumero", @disabled = "disabled" })%>
	</div>
	<div class="ultima prepend2 divBotoesProtocolo">
	<% if (!Model.IsVisualizar){ %>
		<button type="button" class="floatLeft inlineBotao botaoBuscar btnBuscarProtocolo">Buscar</button>
		<span class="prepend1 btnLimparContainer <%= (Model.Titulo.Protocolo.Id > 0)?"":"hide" %>"><button type="button" class="inlineBotao btnLimparNumero">Limpar</button></span>
	<% } %>
	</div>
</div>

<div class="block divEmpTitulo">
	<div class="coluna76">
		<label class="lblDenominador">Empreendimento</label>
		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(Model.Titulo.EmpreendimentoId) %>" />
		<%= Html.TextBox("Titulo.Empreendimento.Denominador", Model.Titulo.EmpreendimentoTexto, new { @class = "text disabled txtEmpDenominador", @disabled = "disabled" })%>
	</div>	
	<div class="ultima prepend2 divBotoesEmp <%= (Model.Titulo.Protocolo.Empreendimento.Id>0) ? "hide" : "" %>">	
	<% if (!Model.IsVisualizar && !Model.IsProtocoloObrigatorio){ %>
		<button type="button" class="floatLeft inlineBotao botaoBuscar btnBuscarEmpreendimento">Buscar</button>
		<span class="prepend1 btnLimparContainer <%= (Model.Titulo.EmpreendimentoId > 0 || (Model.Titulo.Protocolo.Empreendimento.Id>0)) ? "" : "hide" %>"><button type="button" class="inlineBotao btnLimparNumero">Limpar</button></span>
	<% } %>
	</div>
</div>