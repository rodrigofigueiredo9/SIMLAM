<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SecagemMecanicaGraosVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divSecadores">
	<fieldset class="box">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna14 append2">
				<label for="SecagemMecanicaGraos_Identificador">Identificador</label>
				<%= Html.TextBox("SecagemMecanicaGraos.Identificador", (Model.Caracterizacao.Secadores.Count > 0) ? (Model.Caracterizacao.Secadores.Count + 1).ToString() : String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtIdentificador disabled", @disabled = "disabled" }))%>
			</div>

			<div class="coluna25 append2">
				<label for="SecagemMecanicaGraos_Capacidade">Capacidade p/ secador (L) *</label>
				<%= Html.TextBox("SecagemMecanicaGraos.Capacidade", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCapacidade", @maxlength = "7" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarSecador btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna50">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="15%">Identificador </th>
							<th width="30%">Capacidade por secador (L)</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var secador in Model.Caracterizacao.Secadores){ %>
					<tbody>
						<tr>
							<td>
								<span class="identificador" title="<%:secador.Identificador%>"><%:secador.Identificador%></span>
							</td>
							<td>
								<span class="capacidadeSecador" title="<%:secador.Capacidade%>"><%:secador.Capacidade%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(secador)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirSecador" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="identificador"></span></td>
								<td><span class="capacidadeSecador"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirSecador" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
		<br />
		<div class="block">
			<div class="coluna25">
				<label for="SecagemMecanicaGraos_CapacidadeTotal">Capacidade total instalada (L) *</label>
				<%= Html.TextBox("SecagemMecanicaGraos_CapacidadeTotal", Model.CapacidadeTotalSecadores, new { @class = "text txtCapacidadeTotalSecadores disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>
</div>
