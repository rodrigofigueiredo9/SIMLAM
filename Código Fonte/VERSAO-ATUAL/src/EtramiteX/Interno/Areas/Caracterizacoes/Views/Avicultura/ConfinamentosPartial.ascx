<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AviculturaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divConfinamentos">
	<fieldset class="box">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna18 append2">
				<label for="Avicultura_ConfinamentoFinalidade">Finalidade *</label>
				<%= Html.DropDownList("Avicultura.ConfinamentoFinalidade", Model.Finalidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlConfinamentoFinalidades" }))%>
			</div>

			<div class="coluna40 append2">
				<label for="Avicultura_ConfinamentoArea">Área de confinamento de aves (galpões em m²) *</label>
				<%= Html.TextBox("Avicultura.ConfinamentoArea", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtConfinamentoArea maskDecimal", @maxlength = "10" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarConfinamento btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna70">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="22%">Finalidade</th>
							<th width="30%">Área de confinamento (m²)</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var confinamento in Model.Caracterizacao.Confinamentos){ %>
					<tbody>
						<tr>
							<td>
								<span class="finalidade" title="<%:confinamento.FinalidadeTexto%>"><%:confinamento.FinalidadeTexto%></span>
							</td>
							<td>
								<span class="area" title="<%:confinamento.Area%>"><%:confinamento.Area%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(confinamento)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirConfinamento" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="finalidade"></span></td>
								<td><span class="area"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirConfinamento" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>

	<fieldset class="box">
		<div class="block">
			<div class="coluna35">
				<label for="Avicultura_AreaTotalConfinamento">Área de confinamento total (galpões em m²) *</label>
				<%= Html.TextBox("Avicultura.AreaTotalConfinamento", Model.AreaTotalConfinamento, new { @class = "text txtAreaTotalConfinamento disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>
</div>
