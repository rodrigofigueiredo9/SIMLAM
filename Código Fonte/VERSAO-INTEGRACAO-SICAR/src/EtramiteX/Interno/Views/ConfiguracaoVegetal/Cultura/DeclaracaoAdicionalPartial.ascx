<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CulturaVM>" %>

<h1 class="titTela"></h1>
<br />
<script type="text/javascript">
	DeclaracaoAdicional.settings.urls.validarDeclaracaoAdicional = '<%= Url.Action("ValidarDeclaracaoAdicional", "ConfiguracaoVegetal") %>';
</script>

<fieldset ID="configuracaoAdicional" class="block box">
	<div class="block">
		<div class="block coluna40">
			<label>Cultura</label>
			<%=Html.TextBox("DeclaracaoAdicionalCultura", Model.Cultura.Nome, ViewModelHelper.SetaDisabled(true, new { @class="txtCultura text", @maxlength="100" })) %>
		</div>
		<div class="block coluna88 ultima">
			<label>Cultivar *</label>
			<%=Html.TextBox("DeclaracaoAdicionalCultivar", Model.Cultura.NomeCultivar, ViewModelHelper.SetaDisabled(true,  new { @class="txtCultivar text", @maxlength="100" })) %>
			<input type="hidden" value="<%=Model.Cultura.Id %>" class="hdnItemId"/>
		</div>
	</div>		
		
	<div class="block">
		<div class="block coluna40">
			<label>Praga *</label>
			<%= Html.DropDownList("Pragas", Model.LsPragas, new { @class="ddlPragas text"}) %>
		</div>
		<div class="block coluna87">
			<label>Tipo de Produção *</label>
			<%= Html.DropDownList("TipoProducao", Model.TipoProducao, new { @class="ddlTipoProducao text"}) %>
		</div>
	</div>
	<div class="block">
		<div class="block coluna80">
			<label>Declaração Adicional *</label>
			<%= Html.DropDownList("DeclaracaoAdicional", Model.DeclaracaoAdicional, new { @class="ddlDeclaracaoAdicional text"}) %>
		</div>
		<div class="block coluna10">			
			<button class="inlineBotao btnAdicionar"> <span class="ui-button-icon-primary ui-icon ui-icon-plusthick"></span></button>
		</div>
	</div>
	
	<div class="block">
		<div class="gridContainer">
			<table class="dataGridTable gridDeclaracaoAdicional" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>						
						<th width="18%">Pragas</th>
						<th width="24%">Tipo de Produção</th>
						<th>Declaração adicional</th>
						<th class="semOrdenacao" width="10%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var item in Model.Cultura.Cultivar.LsCultivarConfiguracao) { %>
					<tr>
						<td>
							<label class="lblPragas" title="<%= item.PragaTexto %>"><%= item.PragaTexto %> </label>
						</td>
						<td>
							<label class="lblTipoProducao" title="<%= item.TipoProducaoTexto %>"><%= item.TipoProducaoTexto %> </label>
						</td>
						<td>
							<label class="lblDeclaracaoAdicional" title="<%= item.DeclaracaoAdicionalTexto %>"><%= item.DeclaracaoAdicionalTexto %> </label>
						</td>
						<td>
							<a class="icone excluir btnItemExcluir" title="Excluir"></a>
							<input type="hidden" class="hdnItemJSON" value="<%: ViewModelHelper.Json(item) %>" />
						</td>
					</tr>
					<% } %>
					<tr class="hide trTemplate">
						<td>
							<label class="lblPragas"></label>
						</td>
						<td>
							<label class="lblTipoProducao"></label>
						</td>
						<td>
							<label class="lblDeclaracaoAdicional"></label>
						</td>
						<td>
							<a class="icone excluir btnItemExcluir" title="Excluir"></a>
							<input type="hidden" value="" class="hdnItemJSON" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>