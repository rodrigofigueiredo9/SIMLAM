<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UnidadeProducaoItemVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/listar.js") %>"></script>

<script type="text/javascript">
    
    UnidadeProducaoItem.idsTelaTipoProducao = <%= Model.IdsTela %>;
    UnidadeProducaoItem.mensagens = <%= Model.Mensagens%>;
    
    UnidadeProducaoItem.settings.urls.coordenadaGeo = '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento" })%>';
    UnidadeProducaoItem.settings.urls.listarCulturas = '<%= Url.Action("AssociarCultura", "ConfiguracaoVegetal")%>';
    UnidadeProducaoItem.settings.urls.listarCredenciados = '<%= Url.Action("CredenciadoAssociar", "Credenciado" )%>';
    UnidadeProducaoItem.settings.urls.obterResponsavelTecnico = '<%= Url.Action("ObterResponsavelTecnico", "UnidadeProducao" )%>';
    UnidadeProducaoItem.settings.urls.validarUnidadeProducaoItem = '<%= Url.Action("ValidarUnidadeProducaoItem", "UnidadeProducao" )%>';
    UnidadeProducaoItem.settings.urls.obterLstCultivares = '<%= Url.Action("ObterLstCultivares", "UnidadeProducao" )%>';
</script>

<h1 class="titTela">Adicionar Unidade de Produção</h1>
<br />

<fieldset class="box">
	<legend>Identificação da UP</legend>
	<%=Html.Hidden("Id", Model.UnidadeProducaoItem.Id, new {@class="hdnId"}) %>

	<div class="block">
		<div class="coluna20">
			<label for="Possui_Codigo_Propriedade">Já possui código da UP ?</label><br />
			<label><%=Html.RadioButton("UnidadeProducaoItem.PossuiCodigoPropriedade", true, Model.UnidadeProducaoItem.PossuiCodigoUP, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeProducaoItem.Id > 0), new { @class = "radio RadioPossuiCodigoUP rbCodigoSim" }))%>Sim</label>
			<label><%=Html.RadioButton("UnidadeProducaoItem.PossuiCodigoPropriedade", false, !Model.UnidadeProducaoItem.PossuiCodigoUP, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeProducaoItem.Id > 0), new { @class = "radio RadioPossuiCodigoUP rbCodigoNao" }))%>Não</label>
		</div>
		<div class="coluna20 prepend1">
			<label for="Codigo_Propriedade">Código da UP *</label>
			<%=Html.TextBox("UnidadeProducaoItem.CodigoUP", Model.UnidadeProducaoItem.CodigoUP > 0 ? Model.UnidadeProducaoItem.CodigoUP.ToString() : "Gerado automaticamente",  ViewModelHelper.SetaDisabled((Model.IsVisualizar || !Model.UnidadeProducaoItem.PossuiCodigoUP), new { @class = "text txtCodigoUP maskNumInt", @maxlength = "15"}))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna38">
			<label for="Possui_Codigo_Propriedade">Tipo de produção *</label><br />
			<label><%=Html.RadioButton("UnidadeProducaoItem.TipoProducao", (int)eUnidadeProducaoTipoProducao.Frutos, Model.UnidadeProducaoItem.TipoProducao == (int)eUnidadeProducaoTipoProducao.Frutos || Model.UnidadeProducaoItem.TipoProducao == 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioTipoProducao rbTPFrutos" }))%>Frutos</label>
			<label><%=Html.RadioButton("UnidadeProducaoItem.TipoProducao", (int)eUnidadeProducaoTipoProducao.MaterialPropagacao, Model.UnidadeProducaoItem.TipoProducao == (int)eUnidadeProducaoTipoProducao.MaterialPropagacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioTipoProducao rbTPMudas" }))%>Material de Propagação</label>
			<label><%=Html.RadioButton("UnidadeProducaoItem.TipoProducao", (int)eUnidadeProducaoTipoProducao.Madeira, Model.UnidadeProducaoItem.TipoProducao == (int)eUnidadeProducaoTipoProducao.Madeira, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioTipoProducao rbTPMadeira" }))%>Madeiras</label>
		</div>

		<div class='prepend1 divMatProp <%=Model.UnidadeProducaoItem.TipoProducao == (int)eUnidadeProducaoTipoProducao.MaterialPropagacao? "": "hide" %> '>
			<div class="coluna15">
				<label for="RENASEM_NUMERO">RENASEM Nº *</label>
				<%=Html.TextBox("UnidadeProducaoItem.RenasemNumero", Model.UnidadeProducaoItem.RenasemNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.UnidadeProducaoItem.TipoProducao != (int)eUnidadeProducaoTipoProducao.MaterialPropagacao, new { @class = "text txtRenasemNumero", @maxlength = "30"}))%>
			</div>

			<div class="coluna15 prepend1">
				<label for="Data_Validade">Data de validade *</label>
				<%=Html.TextBox("UnidadeProducaoItem.DataValidadeRenasem", Model.UnidadeProducaoItem.DataValidadeRenasem, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.UnidadeProducaoItem.TipoProducao != (int)eUnidadeProducaoTipoProducao.MaterialPropagacao, new { @class = "text txtDataValidadeRenasem maskData", @maxlength = "11"}))%>
			</div>
		</div>
	</div>

	<div class="block">
		<div class="coluna15">
			<label for="Area_HA">Área (ha) *</label>
			<%=Html.TextBox("UnidadeProducaoItem.AreaHA", Model.UnidadeProducaoItem.AreaHA <= Decimal.Zero? "" : Model.UnidadeProducaoItem.AreaHA.ToStringTrunc(4), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaHA maskDecimalPonto4", @maxlength = "13" }))%>
		</div>
	</div>

	<div class="block">
		<input type="hidden" value='<%=Model.UnidadeProducaoItem.Coordenada.Id %>' class="hdnCoordenadaId" />
		<div class="coluna23">
			<label for="Coordenada_Tipo_Id">Sistema de coordenada *</label>
			<%=Html.DropDownList("TiposCoordenada", Model.TiposCoordenada, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%>
		</div>

		<div class="coluna22 prepend1">
			<label for="Coordenada_Datum_Id">Datum *</label>
			<%=Html.DropDownList("Datuns", Model.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%>
		</div>

		<div class="coluna22 prepend1">
			<label for="Coordenada_FusoUtm">Fuso *</label>
			<%=Html.DropDownList("Coordenada.FusoUtm", Model.Fusos, new { @class = "text disabled ddlFuso", @disabled = "disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna23">
			<label for="EastingUtmTexto">Easting *</label>
			<%=Html.TextBox("UnidadeProducaoItem.Coordenada.EastingUtmTexto", Model.UnidadeProducaoItem.Coordenada.EastingUtmTexto, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
		</div>

		<div class="coluna22 prepend1">
			<label for="Coordenada_NorthingUtmTexto">Northing *</label>
			<%=Html.TextBox("UnidadeProducaoItem.Coordenada.NorthingUtmTexto", Model.UnidadeProducaoItem.Coordenada.NorthingUtmTexto, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
		</div>

		<div class="coluna22 prepend1">
			<label for="Coordenada_HemisferioUtm">Hemisfério *</label>
			<%=Html.DropDownList("Coordenada.HemisferioUtm", Model.Hemisferios, new { @class = "text disabled ddlHemisferio", @disabled = "disabled" })%>
		</div>
		<%if (!Model.IsVisualizar)
			{ %>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
		</div>
		<%} %>
	</div>

	<div class="block">
		<div class="coluna71">
			<label for="Cultivar">Cultura *</label>
			<%=Html.TextBox("UnidadeProducaoItem.CulturaTexto", Model.UnidadeProducaoItem.CulturaTexto, new { @class = "text disabled txtCulturaTexto", @disabled = "disabled" })%>
			<%=Html.Hidden("UnidadeProducaoItem.CulturaId", Model.UnidadeProducaoItem.CulturaId, new { @class="hdnCulturaId"})%>
		</div>
		<%if (!Model.IsVisualizar) { %>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnBuscarCultura">Buscar</button>
		</div>
		<%} %>
	</div>

	<div class="block">
		<div class="coluna45">
			<label for="Cultivar_Id">Cultivar *</label>
			<%=Html.DropDownList("UnidadeProducaoItem.CultivarId", Model.LstCultivar, ViewModelHelper.SetaDisabled(Model.LstCultivar.Count <= 1 || Model.IsVisualizar , new { @class = "text ddlCultivar" })) %>
		</div>
		<div class="coluna24 prepend1">
			<label for="Data_Plantio">Data de plantio (mês/ano) *</label>
			<%=Html.TextBox("UnidadeProducaoItem.DataPlantioAnoProducao", Model.UnidadeProducaoItem.DataPlantioAnoProducao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataPlantioAnoProducao maskMesAno" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Produtor</legend>
	<% if(!Model.IsVisualizar) { %>
	<div class="block">
		<div class="coluna83">
			<label for="Produtor">Produtor *</label>
			<%=Html.DropDownList("LstProdutores", Model.LstProdutores, new { @class="texto ddlProdutores"})%>
		</div>
		<div class="coluna10">
			<button class="inlineBotao btnAdicionarProdutor" type="button" value="Adicionar"><span>Adicionar</span></button>
		</div>
	</div>
	<% } %>
	<div class="block">
		<div class="coluna83">
			<table class="dataGridTable gridProdutores" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Produtor</th>
						<% if(!Model.IsVisualizar){ %><th width="9%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.UnidadeProducaoItem.Produtores) { %>
					<tr>
						<td>
							<label class="lblNomeProdutor" title="<%=item.NomeRazao %>"><%=item.NomeRazao %> </label>
						</td>
						<%if(!Model.IsVisualizar) { %>
						<td>
							<a class="icone excluir btnExcluirProdutor"></a>
							<input type="hidden" value="<%=item.Id %>" class="hdnProdutorItemId" />
							<input type="hidden" value="<%=item.IdRelacionamento%>" class="hdnProdutorRelacionamentoId" />
						</td>
						<% } %>
					</tr>
					<% } %>
					<tr class="trTemplate hide">
						<td>
							<label class="lblNomeProdutor" title=""></label>
						</td>
						<td>
							<a class="icone excluir btnExcluirProdutor"></a>
							<input type="hidden" value="0" class="hdnProdutorItemId" />
							<input type="hidden" value="0" class="hdnProdutorRelacionamentoId" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>
<br />

<fieldset class="box">
	<legend>Responsável Técnico</legend>
	<%if (!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna71">
			<label for="Responsavel_Tecnico">Responsável técnico *</label>
			<%=Html.TextBox("UnidadeProducaoItem.ResponsavelTecnico.NomeRazao", Model.UnidadeProducaoItem.ResponsavelTecnico.NomeRazao, new { @class = "text txtResponsavelNome disabled responsavelClear", @disabled="disabled" })%>
			<%=Html.Hidden("Responsavel_Id", 0, new {@class="hdnResponsavelId responsavelClear"}) %>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnBuscarRespTec <%=Model.UnidadeProducaoItem.CultivarId > 0? "" : "hide" %>">Buscar</button>
		</div>
	</div>

	<div class="block">
		<div class="coluna26">
			<label for="Numero_Habilitacao_CFO">Nº da habilitação CFO/CFOC *</label>
			<%=Html.TextBox("UnidadeProducaoItem.ResponsavelTecnico.CFONumero", Model.UnidadeProducaoItem.ResponsavelTecnico.CFONumero, new { @class = "text txtResponsavelNumCFOCFOC disabled responsavelClear", @disabled="disabled" })%>
		</div>
		<div class="coluna21 prepend1">
			<label for="Numero_ART">Nº da ART *</label>
			<%=Html.TextBox("UnidadeProducaoItem.ResponsavelTecnico.NumeroArt", Model.UnidadeProducaoItem.ResponsavelTecnico.NumeroArt, new { @class = "text txtResponsavelNumART responsavelClear", @maxlength=20 })%>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnVerificarNumero hide">Verificar</button>
		</div>
	</div>
	<div class="block">
		<div class="coluna50">
			<div class="coluna57">
				<label for="ART_Cargo_Funcao">ART de cargo e função ?</label><br />
				<label><%=Html.RadioButton("UnidadeProducaoItem.ResponsavelTecnico.ArtCargoFuncao", true, Model.UnidadeProducaoItem.ResponsavelTecnico.ArtCargoFuncao, new { @class = "radio RadioARTCargoFuncao rbARTCargoFuncaoSim" })%>Sim</label>
				<label><%=Html.RadioButton("UnidadeProducaoItem.ResponsavelTecnico.ArtCargoFuncao", false, !Model.UnidadeProducaoItem.ResponsavelTecnico.ArtCargoFuncao, new { @class = "radio RadioARTCargoFuncao rbARTCargoFuncaoNão" })%>Não</label>
			</div>
			<div class="coluna41 divDataValidadeART hide">
				<label for="Data_Validade_ART">Data de validade da ART *</label>
				<%=Html.TextBox("UnidadeProducaoItem.ResponsavelTecnico.DataValidadeART", Model.UnidadeProducaoItem.ResponsavelTecnico.DataValidadeART, new { @class = "text txtResponsavelDataValidadeART maskData disabled responsavelClear", @disabled="disabled" })%>
			</div>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnAddRespTec">Adicionar</button>
		</div>
	</div>
	<%} %>
	<div class="block">
		<table class="dataGridTable gridResponsaveis" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Responsável Técnico</th>
					<th width="21%">Nº da Habilitação CFO / CFOC</th>
                    <th width="21%">Data de validade da ART</th>
					<%if (!Model.IsVisualizar) { %><th width="9%">Ações</th><%} %>
				</tr>
			</thead>
			<tbody>
				<%foreach (var item in Model.UnidadeProducaoItem.ResponsaveisTecnicos) { %>
				<tr>
					<td>
						<label class="lblNome" title="<%=item.NomeRazao %>"><%=item.NomeRazao%> </label>
					</td>
					<td>
						<label class="lblNumeroHabilitacao" title="<%=item.CFONumero %>"><%=item.CFONumero%> </label>
					</td>
                    <td>
						<label class="lblDataValidadeART" title="<%=item.DataValidadeART %>"><%=item.DataValidadeART%> </label>
					</td>
					<%if (!Model.IsVisualizar) { %>
					<td>
						<a class="icone excluir btnExcluirResponsavelTecnico"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item) %>' class="hdnItemJson" />
					</td>
					<%} %>
				</tr>
				<% } %>
				<tr class="trTemplate hide">
					<td>
						<label class="lblNome" title=""></label>
					</td>
					<td>
						<label class="lblNumeroHabilitacao" title=""></label>
					</td>
                    <td>
						<label class="lblDataValidadeART" title=""></label>
					</td>
					<td>
						<a class="icone excluir btnExcluirResponsavelTecnico"></a>
						<input type="hidden" value="0" class="hdnItemJson" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Estimativa de Produção</legend>
	<div class="block">
		<div class="coluna26">
			<label for="Quantidade_Ano">Quantidade / Ano *</label>
			<%=Html.TextBox("UnidadeProducaoItem.EstimativaProducaoQuantidadeAno", Model.UnidadeProducaoItem.EstimativaProducaoQuantidadeAno > 0 ? Model.UnidadeProducaoItem.EstimativaProducaoQuantidadeAno.ToStringTrunc(4) : "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEstimativaProducaoQuantAno maskDecimalPonto4", @maxlength = "13" }))%>
		</div>
		<div class="coluna21 prepend1">
			<label for="Quantidade_Ano">Unidade de medida *</label>
			<%=Html.TextBox("UnidadeProducaoItem.EstimativaProducaoUnidadeMedida", Model.UnidadeProducaoItem.EstimativaProducaoUnidadeMedida, new { @class = "text txtEstimativaProducaoUnidadeMedida disabled", @disabled="disabled" })%>
		</div>
	</div>
</fieldset>