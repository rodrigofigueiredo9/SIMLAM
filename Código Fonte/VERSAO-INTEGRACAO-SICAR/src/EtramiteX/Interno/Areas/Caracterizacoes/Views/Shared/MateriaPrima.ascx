<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MateriaPrimaFlorestalConsumidaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divMateriasPrima">
	<fieldset class="box">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna30 append2">
				<label for="MateriaPrima_MateriaPrimaFlorestalConsumida">Matéria-prima florestal consumida *</label>
				<%= Html.DropDownList("MateriaPrima.MateriaPrimaFlorestalConsumida", Model.MateriaPrimaFlorestalConsumidaLst, new { @class = "text ddlMateriaPrima " })%>
			</div>

			<div class="coluna10 append2">
				<label for="MateriaPrima_Unidade">Unidade *</label>
				<%= Html.DropDownList("MateriaPrima.Unidade", Model.Unidade, new { @class = "text ddlUnidade" })%>
			</div>

			<div class="coluna16 divEspecificar hide append2">
				<label for="MateriaPrima_Especificar">Especificar *</label>
				<%= Html.TextBox("MateriaPrima.Especificar", String.Empty, new { @class = "text txtEspecificar", @maxlength = "30" })%>
			</div>

			<div class="coluna16 append2">
				<label for="MateriaPrima_Quantidade">Quantidade (mês) *</label>
				<%= Html.TextBox("MateriaPrima.Quantidade", String.Empty, new { @class = "text txtQuantidade maskDecimal", @maxlength = "10" })%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarMateria btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna69">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="45%">Matéria-prima florestal consumida </th>
							<th width="10%">Unidade</th>
							<th width="25%">Quantidade (mês)</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var materia in Model.MateriasPrimasFlorestais){ %>
					<tbody>
						<tr>
							<td>
								<span class="materiaPrimaConsumida" title="<%:materia.MateriaPrimaConsumidaTexto%>"><%:materia.MateriaPrimaConsumidaTexto%></span>
							</td>
							<td>
								<span class="unidade" title="<%:materia.UnidadeTexto%>"><%:materia.UnidadeTexto%></span>
							</td>
							<td>
								<span class="quantidade" title="<%:materia.Quantidade%>"><%:materia.Quantidade%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="<%: ViewModelHelper.Json(materia)%>" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMateria" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="materiaPrimaConsumida"></span></td>
								<td><span class="unidade"></span></td>
								<td><span class="quantidade"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMateria" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>