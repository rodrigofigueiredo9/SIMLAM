<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TituloAdicionarVM>" %>

<% var indice = Model.Indice; %>
<fieldset class="block box divTituloAdicionar <%= string.IsNullOrEmpty(Model.Finalidade.TituloNumero) ? "hide" : "" %>">
	<legend>Licença Ambiental</legend>
	<input type="hidden" class="hdnFinalidadeID" value="<%= Model.Finalidade.Id %>" />
	<input type="hidden" class="hdnFinalidadeIndex" value="<%= indice %>" />

	<div class="block">
		<label ><%= Html.RadioButton("OrgaoEmissor" + indice, ConfiguracaoSistema.EmitidoIDAF, (Model.Finalidade.EmitidoPorInterno.HasValue ? Model.Finalidade.EmitidoPorInterno.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbOrgaoEmissor rbEmitidoIDAF" }))%>Emitido pelo <%= Model.SiglaOrgao %></label>
		<label ><%= Html.RadioButton("OrgaoEmissor" + indice, ConfiguracaoSistema.EmitidoOutroOrgao, (Model.Finalidade.EmitidoPorInterno.HasValue ? !Model.Finalidade.EmitidoPorInterno.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbOrgaoEmissor rbEmitidoOutroOrgao" }))%>Emitido por outro órgão</label>
	</div>

	<div class="divOrgao <%= (Model.Finalidade.EmitidoPorInterno.HasValue && Model.Finalidade.EmitidoPorInterno.Value) ? "" : "hide" %>">
		<div class="block">
			<div class="coluna61 append1">
				<label for="TituloModelo">Licença *</label>
				<%= Html.DropDownList("TituloModelo" + indice, Model.Titulos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTituloModelo" }))%>
			</div>
			<div class="coluna22 append1">
				<label for="TituloNumero">Número *</label>
				<%= Html.TextBox("TituloNumero" + indice, Model.Finalidade.TituloNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "12", @class = "text txtTituloNumero" }))%>
				<input type="hidden" class="hdnTituloID" value="<%= Model.Finalidade.TituloId.GetValueOrDefault() %>" />
			</div>
			<% if (!Model.IsVisualizar) { %>
			<div class="block ultima">
				<button type="button" class="inlineBotao btnBuscarNumero" title="Buscar">Buscar</button>
			</div>
			<% } %>
		</div>

		<div class="block">
			<% if (Model.Finalidade.Id < 1 || Model.Finalidade.TituloId.GetValueOrDefault() > 0) { %>
			<div class="coluna19 append1">
				<label for="TituloValidadeData_DataTexto">Data de validade *</label>
				<%= Html.TextBox("TituloValidadeData.DataTexto" + indice, Model.Finalidade.TituloValidadeData.DataTexto, new { @class = "text maskData disabled txtTituloValidadeData", @disabled = "disabled" })%>
			</div>
			<div class="coluna19 append1">
				<label for="ProtocoloNumero">Nº protocolo *</label>
				<%= Html.TextBox("ProtocoloNumero" + indice, Model.Finalidade.ProtocoloNumero, new { @maxlength = "12", @class = "text disabled txtProtocoloNumero", @disabled = "disabled" })%>
			</div>
			<% } else { %>
			<div class="coluna19 append1">
				<label for="TituloValidadeData_DataTexto">Data de validade *</label>
				<%= Html.TextBox("TituloValidadeData.DataTexto" + indice, Model.Finalidade.TituloValidadeData.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtTituloValidadeData" }))%>
			</div>
			<div class="coluna19 append1">
				<label for="ProtocoloNumero">Nº protocolo *</label>
				<%= Html.TextBox("ProtocoloNumero" + indice, Model.Finalidade.ProtocoloNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "12", @class = "text txtProtocoloNumero" }))%>
			</div>
			<% } %>

			<div class="coluna19 append1">
				<label for="ProtocoloRenovacaoData_DataTexto">Data da renovação</label>
				<%= Html.TextBox("ProtocoloRenovacaoData.DataTexto" + indice, Model.Finalidade.ProtocoloRenovacaoData.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtProtocoloRenovacaoData" }))%>
			</div>
			<div class="coluna22">
				<label for="ProtocoloRenovacaoNumero">Nº protocolo renovação</label>
				<%= Html.TextBox("ProtocoloRenovacaoNumero" + indice, Model.Finalidade.ProtocoloRenovacaoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "12", @class = "text txtProtocoloRenovacaoNumero" }))%>
			</div>
		</div>
	</div>

	<div class="divOrgaoExterno <%= (Model.Finalidade.EmitidoPorInterno.HasValue && !Model.Finalidade.EmitidoPorInterno.Value) ? "" : "hide" %>">
		<div class="block">
			<div class="coluna61 append1">
				<label for="Licenca">Licença *</label>
				<%= Html.TextBox("TituloModelo" + indice, Model.Finalidade.TituloModeloTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "100", @class = "text txtTituloModelo" }))%>
			</div>
			<div class="coluna22">
				<label for="Numero">Número *</label>
				<%= Html.TextBox("TituloNumero" + indice, Model.Finalidade.TituloNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "11", @class = "text txtTituloNumero" }))%>
			</div>
		</div>
		
		<div class="block">
			<div class="coluna19 append1">
				<label for="TituloValidadeData_DataTexto">Data de validade *</label>
				<%= Html.TextBox("TituloValidadeData.DataTexto" + indice, Model.Finalidade.TituloValidadeData.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtTituloValidadeData" }))%>
			</div>
			<div class="coluna19 append1">
				<label for="ProtocoloNumero">Nº protocolo *</label>
				<%= Html.TextBox("ProtocoloNumero" + indice, Model.Finalidade.ProtocoloNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "12", @class = "text txtProtocoloNumero" }))%>
			</div>
			<div class="coluna19 append1">
				<label for="ProtocoloRenovacaoData_DataTexto">Data da renovação</label>
				<%= Html.TextBox("ProtocoloRenovacaoData.DataTexto" + indice, Model.Finalidade.ProtocoloRenovacaoData.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtProtocoloRenovacaoData" }))%>
			</div>
			<div class="coluna22">
				<label for="ProtocoloRenovacaoNumero">Nº protocolo renovação</label>
				<%= Html.TextBox("ProtocoloRenovacaoNumero" + indice, Model.Finalidade.ProtocoloRenovacaoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "12", @class = "text txtProtocoloRenovacaoNumero" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna86">
				<label for="OrgaoExpedidor">Órgão expedidor *</label>
				<%= Html.TextBox("OrgaoExpedidor" + indice, Model.Finalidade.OrgaoExpedidor, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "100", @class = "text txtOrgaoExpedidor" }))%>
			</div>
		</div>
	</div>
</fieldset>