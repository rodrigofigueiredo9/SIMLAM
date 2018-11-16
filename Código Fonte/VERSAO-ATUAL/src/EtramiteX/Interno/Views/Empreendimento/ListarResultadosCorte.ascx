<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<fieldset class="block box">
	<div class="divCod box">
		<div class="block">
			<div class="coluna37">
				<label>Empreendimento com código identificador?</label><br />
				<label><%= Html.RadioButton("Filtros.PossuiCodigo", true, Model.Filtros.PossuiCodigo, new { @class = "radio RadioEmpreendimentoCodigo rbCodigoSim disabled", disabled=true })%>Sim</label>
				<label><%= Html.RadioButton("Filtros.PossuiCodigo", false, !Model.Filtros.PossuiCodigo, new { @class = "radio RadioEmpreendimentoCodigo rbCodigoNao disabled", disabled=true })%>Não</label>
			</div>

			<div class="coluna30 prepend1 divCodigoEmp <%= (Model.Filtros.PossuiCodigo) ? "" : "hide" %>">
				<label for="Filtros_Codigo">Código do empreendimento</label>
				<%= Html.TextBox("Filtros.Codigo", Model.Filtros.Codigo , new { @class = "txtCodigo text maskIntegerObrigatorio disabled", disabled=true, @maxlength = "13" })%>
			</div>

			<div class="coluna8 prepend1 divCodigoEmp" >
				<button type="button" title="verificar" id="btnVerificarCodigo" class="inlineBotao btnVerificarCodigo disabled" disabled>Verificar</button>
			</div>
		</div>
	</div>
</fieldset>


<fieldset class="block box">
	<legend>Empreendimentos</legend>
	<div class="dataGrid <%= ((Model.Resultados.Count > 0) ? string.Empty : "hide") %> ">

		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="17%">Código</th>
					<th width="17%">Segmento</th>
					<th>Razão social/Denominação/Nome</th>
					<th>Município</th>
					<th width="15%">CNPJ</th>
					<th class="semOrdenacao" width=" <%= (Model.PodeAssociar) ? "9%" : "17%" %>">Ações</th>
				</tr>
			</thead>

			<tbody class="trCorpo">
			<% foreach (var item in Model.Resultados) { %>
				<tr>
					<td class="itemCodigo" title="<%= Html.Encode(item.Codigo.GetValueOrDefault().ToString("N0"))%>"> <%= Html.Encode(item.Codigo.GetValueOrDefault().ToString("N0"))%></td>
					<td title="<%= Html.Encode(item.SegmentoTexto)%>"><%= Html.Encode(item.SegmentoTexto)%></td>
					<td title="<%= Html.Encode(item.Denominador)%>" ><%= Html.Encode(item.Denominador)%></td>
					<td title="<%= Html.Encode(item.Denominador)%>" ><%= Html.Encode(item.Enderecos[0].MunicipioTexto)%></td>
					<td title="<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CNPJ))%>" ><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CNPJ))%></td>
					<td>
						<input type="hidden" value="<%= item.Id %>" class="itemId" />
						<input type="hidden" value="<%= item.Denominador %>" class="itemDenominador" />
						<input type="hidden" value="<%= item.CNPJ %>" class="itemCnpj" />
						<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(item.Id)%>" />
						<input type="hidden" class="hdnEmpreendimentoInternoId" value="<%= Html.Encode(item.InternoId)%>" />
						<input type="button" title="Visualizar" class="icone visualizar btnVisualizarEmpreendimento" />
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>