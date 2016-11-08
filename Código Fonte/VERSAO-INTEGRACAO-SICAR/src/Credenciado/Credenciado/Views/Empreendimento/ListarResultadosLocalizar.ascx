<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LocalizarVM>" %>

<fieldset class="block box">
	<legend>Empreendimentos</legend>
	<% if( Model.Resultados.Count > 0){ %>
		<div class="habilitarAvançar">
			<% if( Model.Resultados.Count > 1){ %>
			<div class="block">
				<label>Algum dos empreendimentos listados é aquele que você procura? Caso contrário clique em novo para continuar o cadastro.</label>
			</div>
			<% } %>
			<div class="dataGrid">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th width="30%">Segmento</th>
							<th>Razão social/Denominação/Nome</th>
							<th width="20%">CNPJ</th>
							<th width="10%">Ações</th>
						</tr>
					</thead>
					<tbody class="trCorpo">
					<% for (int i = 0; i < Model.Resultados.Count; i++) { %>
						<tr>
							<td title="<%= Html.Encode(Model.Resultados[i].SegmentoTexto)%>"><%= Html.Encode(Model.Resultados[i].SegmentoTexto)%></td>
							<td title="<%= Html.Encode(Model.Resultados[i].Denominador)%>"><%= Html.Encode(Model.Resultados[i].Denominador)%></td>
							<td title="<%= Html.Encode(Model.Resultados[i].Cnpj)%>"><%= Html.Encode(Model.Resultados[i].Cnpj)%></td>
							<td>
								<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(Model.Resultados[i].Id)%>" />
								<input type="hidden" class="hdnEmpreendimentoInternoId" value="<%= Html.Encode(Model.Resultados[i].InternoId)%>" />
								<input title="Visualizar" type="button" class="icone visualizar btnEditarEmpreendimento" value="" />
							</td>
						</tr>
					<% } %>
					</tbody>
				</table>
			</div>
		</div>
	<% } else { %>
	<label class="habilitarAvançar">Não foi encontrado nenhum empreendimento com a identificação informada. Clique em novo para continuar o cadastro.</label>
	<% } %>
</fieldset>