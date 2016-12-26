<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PulverizacaoProdutoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.PulverizacaoProduto%>" />
<input type="hidden" class="hdnIsEditar" value="<%:Model.IsEditar.ToString().ToLower()%>" />

<script>
	PulverizacaoProduto.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	PulverizacaoProduto.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	PulverizacaoProduto.settings.textoMerge = '<%= Model.TextoMerge %>';
	PulverizacaoProduto.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	PulverizacaoProduto.settings.temARL = <%= Model.TemARL.ToString().ToLower() %>;
	PulverizacaoProduto.settings.temARLDesconhecida = <%= Model.TemARLDesconhecida.ToString().ToLower() %>;
</script>

<fieldset class="box">
	<div class="block">
		<div class="coluna70">
			<label for="PulverizacaoProduto_Atividade">Atividade *</label>
			<%= Html.DropDownList("PulverizacaoProduto.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count <= 2, new { @class = "text ddlAtividade " }))%>
		</div>
	</div>
	<br />

	<div class="divCulturas">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna23 append2">
				<label for="PulverizacaoProdutoCultura_Tipo">Tipo de cultura *</label>
				<%= Html.DropDownList("PulverizacaoProdutoCultura.Tipo", Model.Culturas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCulturaTipo" }))%>
			</div>

			<div class="coluna23 append2 divEspecificar hide">
				<label for="PulverizacaoProdutoCultura_EspecificarTexto">Especificar *</label>
				<%= Html.TextBox("PulverizacaoProdutoCultura.EspecificarTexto", String.Empty, new { @class = "text txtCulturaEspecificarTexto", @maxlength = "120" })%>
			</div>

			<div class="coluna25 append2">
				<label for="PulverizacaoProdutoCultura_Area">Área de cultura (ha) *</label>
				<%= Html.TextBox("PulverizacaoProdutoCultura.Area", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCulturaArea maskDecimalPonto4", @maxlength = "13" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarCultura btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna70">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="30%">Tipo de cultura</th>
							<th width="30%">Área da cultura (ha)</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var cultura in Model.Caracterizacao.Culturas){ %>
					<tbody>
						<tr>
							<td>
								<span class="tipo" title="<%:cultura.TipoTexto%>"><%:cultura.TipoTexto%></span>
							</td>
							<td>
								<span class="area" title="<%:cultura.Area%>"><%:cultura.Area%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(cultura)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirCultura" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="tipo"></span></td>
								<td><span class="area"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirCultura" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</div>
	<br />

	<div class="block">
		<div class="coluna26">
			<label for="PulverizacaoProduto_AreaTotal">Área total de pulverização (ha) *</label>
			<%= Html.TextBox("PulverizacaoProduto.AreaTotal", Model.AreaTotal, new { @class = "text txtAreaTotalCultura disabled", @disabled = "disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna70">
			<label for="PulverizacaoProduto_EmpresaPrestadora">Empresa prestadora de serviço de aplicação aérea de agrotóxicos *</label>
			<%= Html.TextBox("PulverizacaoProduto.EmpresaPrestadora", Model.Caracterizacao.EmpresaPrestadora, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEmpresaPrestadora", @maxlength="120"}))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna18">
			<label for="PulverizacaoProduto_CNPJ">CNPJ *</label>
			<%= Html.TextBox("PulverizacaoProduto.CNPJ", Model.Caracterizacao.CNPJ, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskCnpjParcial txtCNPJ", @maxlength="18" }))%>
		</div>
	</div>
</fieldset>

<% Html.RenderPartial("CoordenadaAtividade", Model.CoodernadaAtividade); %>

