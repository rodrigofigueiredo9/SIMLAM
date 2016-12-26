<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<script>
	Silvicultura.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Silvicultura.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	Silvicultura.settings.textoMerge = '<%= Model.TextoMerge %>';
	Silvicultura.settings.mensagens = <%=Model.Mensagens%>;
	Silvicultura.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	Silvicultura.settings.temARL = <%= Model.TemARL.ToString().ToLower() %>;
	Silvicultura.settings.temARLDesconhecida = <%= Model.TemARLDesconhecida.ToString().ToLower() %>;
</script>

<fieldset class="box">
	<legend class="titFiltros">Total de áreas das matrículas/posses </legend>

	<div class="block">
		<% SilviculturaArea area = Model.ObterArea(eSilviculturaArea.ATP_CROQUI); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Área croqui (m²) ATP</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.AVN); %>
		<div class="coluna23 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Área vegetação nativa (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.AA_FLORESTA_PLANTADA); %>
		<div class="coluna25 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Área de floresta plantada (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna20 append2">
			<label for="TotalFloresta">Total de floresta (m²)</label>
			<%= Html.TextBox("TotalFloresta" + area.Tipo, Model.TotalFloresta.ToStringTrunc(), new { @class = "text txtTotalFloresta disabled", @disabled = "disabled" })%>
		</div>
	</div>

	<fieldset class="box">
		<legend class="titFiltros">Área de vegetação da APP</legend>

		<% area = Model.ObterArea(eSilviculturaArea.APP_AVN); %>
		<div class="coluna23 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Com vegetação nativa (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.APP_AA_USO); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Em uso (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.APP_AA_REC); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">A recuperação (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.APP); %>
		<div class="coluna20 append2 hide divArea">
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna20 append2">
			<label for="AreaAPPNaoCaracterizada">Não caracterizada (m²)</label>
			<%= Html.TextBox("AreaAPPNaoCaracterizada" + area.Tipo, Model.Caracterizacao.AreaAPPNaoCaracterizada, new { @class = "text txtTotalFloresta disabled", @disabled = "disabled" })%>
		</div>
	</fieldset>

	<fieldset class="box">
		<legend class="titFiltros">Área de reserva legal</legend>

		<% area = Model.ObterArea(eSilviculturaArea.ARL_AVN); %>
		<div class="coluna23 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Com vegetação nativa (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.ARL_USO); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Em uso (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.ARL_REC); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">A recuperação (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaArea.ARL_D); %>
		<div class="coluna20 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>">Não caracterizada (m²)</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>
	</fieldset>
</fieldset>

<%foreach (var item in Model.SilviculturaSilvicultVM){ %>
<fieldset class="block box fsSilvicultura" id="silvicultura<%=item.Caracterizacao.Identificacao%>">
	<legend class="titFiltros">Silvicultura</legend>
	<%Html.RenderPartial("SilviculturaSilvicult", item); %>
</fieldset>
<%} %>

<fieldset class="box">
	<legend class="titFiltros">Total da silvicultura</legend>

	<div class="block">
		<div class="coluna20 append2">
			<label for="Silvicultura_TotalAreaCroqui">Área total do croqui (ha)</label>
			<%= Html.TextBox("Silvicultura.TotalAreaCroqui", Model.TotalAreaCroqui.ToStringTrunc(), new { @class = "text txtTotalAreaCroqui disabled", disabled = "disabled" })%>
		</div>
	</div>

	<div class="block coluna65 dataGrid">
		<table class="dataGridTable tableCulturas" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Cultura florestal</th>
					<th width="40%">Área total por cultura (ha)</th>
				</tr>
			</thead>
			<% if (Model.IsVisualizar){foreach (CulturaFlorestal cultura in Model.CulturasGroup){ %>
			<tbody>
				<tr>
					<td>
						<span class="CulturaTipo" title="<%:cultura.CulturaTipoTexto%>"><%:cultura.CulturaTipoTexto%></span>
					</td>
					<td>
						<span class="AreaCultura" title="<%:cultura.AreaCulturaHa%>"><%:cultura.AreaCulturaHa%></span>
					</td>
				</tr>
				</tbody>
			<% }} %>
			<tr class="trTemplateRowCulturas hide">
				<td><span class="CulturaTipo"></span></td>
				<td><span class="AreaCultura"></span></td>
			</tr>
		</table>
	</div>
</fieldset>