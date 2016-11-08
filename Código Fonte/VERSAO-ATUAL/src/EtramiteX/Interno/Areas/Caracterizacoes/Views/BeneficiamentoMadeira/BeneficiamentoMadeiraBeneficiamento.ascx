<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BeneficiamentoMadeiraBeneficiamentoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<fieldset id="fsBeneficiamentoMadeira<%:Model.Caracterizacao.Identificador%>" class="boxBranca fsBeneficiamento">
	
	<input type="hidden" class="hdnIdentificador" value="<%=Model.Caracterizacao.Identificador%>" />
		
	<p class="block">
		<%if (!Model.IsVisualizar) { %>
			<a class="btnAsmExcluir btnExluirAtividade" title="Excluir beneficiamento e tratamento de madeira">Excluir</a>
		<%} %>
	</p>

	<input type="hidden" class="hdnBeneficiamentoId" value="<%:Model.Caracterizacao.Id %>" />
	<input type="hidden" class="hdnIdentificador" value="<%:Model.Caracterizacao.Identificador %>" />

	<fieldset class="box">
		<div class="block">
			<div class="coluna80">
				<label for="BeneficiamentoMadeira_Atividade<%:Model.Caracterizacao.Identificador%>">Atividade *</label>
				<%= Html.DropDownList("BeneficiamentoMadeira.Atividade"+Model.Caracterizacao.Identificador, Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAtividade " }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna40 <%:(Model.IsVisualizar && Model.MostrarVolumeMadeiraSerrar) ? "" : "hide"%> divVolumeMadeiraSerrar">
				<label for="BeneficiamentoMadeira_VolumeMadeiraSerrar<%:Model.Caracterizacao.Identificador%>">Volume de madeira a ser serrada (m³/mês) *</label>
				<%= Html.TextBox("BeneficiamentoMadeira.VolumeMadeiraSerrar" + Model.Caracterizacao.Identificador, Model.Caracterizacao.VolumeMadeiraSerrar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVolumeMadeiraSerrar maskDecimal", @maxlength = "10" }))%>
			</div>

			<div class="coluna40 <%:(Model.IsVisualizar && Model.MostrarVolumeMadeiraProcessar) ? "" : "hide"%> divVolumeMadeiraProcessar">
				<label for="BeneficiamentoMadeira_VolumeMadeiraProcessar<%:Model.Caracterizacao.Identificador%>">Volume de madeira a ser processada (m³/mês) *</label>
				<%= Html.TextBox("BeneficiamentoMadeira.VolumeMadeiraProcessar" + Model.Caracterizacao.Identificador, Model.Caracterizacao.VolumeMadeiraProcessar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVolumeMadeiraProcessar maskDecimal", @maxlength = "10" }))%>
			</div>
		</div>
	</fieldset>

	<div class="divMateriasPrima">
		<fieldset class="box">
			<%if(!Model.IsVisualizar) {%>
			<div class="block">
				<div class="coluna30 append2">
					<label for="MateriaPrima_MateriaPrimaFlorestalConsumida<%:Model.Caracterizacao.Identificador%>">Matéria-prima florestal consumida *</label>
					<%= Html.DropDownList("MateriaPrima.MateriaPrimaFlorestalConsumida" + Model.Caracterizacao.Identificador, Model.MateriaPrimaFlorestalConsumida.MateriaPrimaFlorestalConsumidaLst, new { @class = "text ddlMateriaPrima " })%>
				</div>

				<div class="coluna10 append2">
					<label for="MateriaPrima_Unidade<%:Model.Caracterizacao.Identificador%>">Unidade *</label>
					<%= Html.DropDownList("MateriaPrima.Unidade" + Model.Caracterizacao.Identificador, Model.MateriaPrimaFlorestalConsumida.Unidade, new { @class = "text ddlUnidade" })%>
				</div>

				<div class="coluna16 divEspecificar hide append2">
					<label for="MateriaPrima_Especificar<%:Model.Caracterizacao.Identificador%>">Especificar *</label>
					<%= Html.TextBox("MateriaPrima.Especificar" + Model.Caracterizacao.Identificador, String.Empty, new { @class = "text txtEspecificar", @maxlength = "30" })%>
				</div>

				<div class="coluna16 append2">
					<label for="MateriaPrima_Quantidade<%:Model.Caracterizacao.Identificador%>">Quantidade (mês) *</label>
					<%= Html.TextBox("MateriaPrima.Quantidade" + Model.Caracterizacao.Identificador, String.Empty, new { @class = "text txtQuantidade maskDecimal", @maxlength = "10" })%>
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
						<% foreach (var materia in Model.MateriaPrimaFlorestalConsumida.MateriasPrimasFlorestais){ %>
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

	<fieldset class="box">
		<div class="block">
			<div class="coluna85">
				<label for="BeneficiamentoMadeira_EquipControlePoluicaoSonora<%:Model.Caracterizacao.Identificador%>">Equipamentos de controle de poluição sonora (se fora das normas) e atmosférica *</label>
				<%= Html.TextArea("BeneficiamentoMadeira.EquipControlePoluicaoSonora" + Model.Caracterizacao.Identificador, Model.Caracterizacao.EquipControlePoluicaoSonora, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtEquipControlePoluicaoSonora", @maxlength = "500" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="box">
	<legend class="titFiltros">Coordenada da atividade para a licença</legend>

	<div class="block">
		<div class="coluna30 append2">
			<label for="CoordenadaAtividade_Tipo<%:Model.Caracterizacao.Identificador%>">Tipo geométrico *</label>
			<%= Html.DropDownList("CoordenadaAtividade.Tipo" + Model.Caracterizacao.Identificador, Model.CoodernadaAtividade.TipoGeometrico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaTipoGeometria " }))%>
		</div>

		<div class="coluna40">
			<label for="CoordenadaAtividade_CoordenadaAtividade<%:Model.Caracterizacao.Identificador%>">Coordenada da atividade *</label>
			<%= Html.DropDownList("CoordenadaAtividade.CoordenadaAtividade" + Model.Caracterizacao.Identificador, Model.CoodernadaAtividade.CoordenadasAtividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCoordenadaAtividade " }))%>
		</div>
	</div>
</fieldset>

</fieldset>

