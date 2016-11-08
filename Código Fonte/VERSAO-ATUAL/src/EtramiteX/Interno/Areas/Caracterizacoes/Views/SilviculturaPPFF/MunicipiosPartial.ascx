<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaPPFFVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divMunicipios">
	<fieldset class="box">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna18 append2">
				<label for="Silvicultura_Municipios">Municipios *</label>
				<%= Html.DropDownList("Silvicultura.Municipio", Model.Municipios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlMunicipio" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarMunicipio btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna70">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="91%">Município de abrangência</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var item in Model.Caracterizacao.Itens){ %>
					<tbody>
						<tr>
							<td>
								<span class="municipio" title="<%:item.Municipio.Texto%>"><%:item.Municipio.Texto%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMunicipio" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="municipio"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMunicipio" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>
