<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CulturaVM>" %>

<fieldset class="block box">
	<div class="block">
		<input type="hidden" value="<%=Model.Cultura.Id %>" class="hdnId" />

		<div class="block coluna95 ultima">
			<label>Cultura *</label>
			<%=Html.TextBox("Cultura.Cultura", Model.Cultura.Nome, new { @class="txtCultura text setarFoco", @maxlength="100" }) %>
		</div>

		<div class="block coluna88 ultima">
			<label>Cultivar *</label>
			<%=Html.TextBox("Cultura.Cultivar", string.Empty, new { @class="txtCultivar text", @maxlength="250" }) %>
		</div>

		<div class="coluna10">
			<button class="inlineBotao btnAdicionar">Adicionar</button>
			<button class="inlineBotao btnEditar hide">Editar</button>
		</div>
	</div>

	<div class="block">
		<div class="gridContainer">
			<table class="dataGridTable gridCultivar" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Cultivar</th>
						<th class="semOrdenacao" width="10%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura.Cultivar item = null;
						for (int i = 0; i < Model.Cultura.LstCultivar.Count; i++)  {
							item = Model.Cultura.LstCultivar[i]; 
					%>
						<tr>
							<td>
								<label class="lblNome" title="<%= item.Nome %>"><%= item.Nome %></label>
							</td>
							<td>
								<a class="icone editar btnItemEditar" title="Editar"></a>
								<a class="icone opcoes btnConfigurar" title="Configurar declaração adicional"></a>
								<input type="hidden" value="<%= item.Id %>" class="hdnItemId" />
								<input type="hidden" value="<%= i %>" class="hdnItemIndex" />							
								<input type="hidden" class="hdnItemJson" value='<%= ViewModelHelper.Json(item.LsCultivarConfiguracao) %>' />
							</td>
						</tr>
					<% } %>
					<tr class="hide tr_template">
						<td>
							<label class="lblNome" id="cultivar"></label>
						</td>
						<td>
							<a class="icone editar btnItemEditar" title="Editar"></a>
							<a class="icone opcoes btnConfigurar" title="Configurar declaração adicional"></a>
							<input type="hidden" value="" class="hdnItemId" />
							<input type="hidden" value="" class="hdnItemIndex" />
							<input type="hidden" value="" class="hdnItemJson" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>