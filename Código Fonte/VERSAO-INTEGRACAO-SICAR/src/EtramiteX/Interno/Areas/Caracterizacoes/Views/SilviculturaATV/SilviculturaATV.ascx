<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaATVVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<script type="text/javascript">
	SilviculturaATV.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	SilviculturaATV.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	SilviculturaATV.settings.textoMerge = '<%= Model.TextoMerge %>';
	SilviculturaATV.settings.mensagens = <%=Model.Mensagens%>;
	SilviculturaATV.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	SilviculturaATV.settings.temARL = <%= Model.TemARL.ToString().ToLower() %>;
	SilviculturaATV.settings.temARLDesconhecida = <%= Model.TemARLDesconhecida.ToString().ToLower() %>;
	SilviculturaATV.abrirModalARL();
</script>

<fieldset class="box">
	<legend class="titFiltros">Total de área das Matriculas/Posses do Croqui</legend>
	<div class="block">
		<% var area = Model.ObterArea(eSilviculturaAreaATV.ATP_TOTAL); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
            <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaAreaATV.AVN); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
            <input type="hidden" class="hdnAreaValor hdnNativa" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled txtNativa", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaAreaATV.DECLIVIDADE); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %> *</label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.Valor == 0 ? "" : area.ValorTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArea maskDecimal", @maxlength = "5", @id = "txtAreaDeclividade" }))%>
		</div>
	</div>

	<div class="block">
		<% area = Model.ObterArea(eSilviculturaAreaATV.AA_FLORESTA_PLANTADA); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
            <input type="hidden" class="hdnAreaValor hdnPlantada" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled txtPlantada", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaAreaATV.AA_FOMENTO); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.Valor == 0 ? "" : area.ValorTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArea maskDecimal", @maxlength = "10", @id = "txtAreaFomento" }))%>
		</div>

		<% area = Model.ObterArea(eSilviculturaAreaATV.AA_PLANTIO); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.Valor == 0 ? "" : area.ValorTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArea maskDecimal keyCalcular", @maxlength = "10", @id = "txtAreaPlantio" }))%>
		</div>
	</div>

	<div class="block">
		<% area = Model.ObterArea(eSilviculturaAreaATV.APP); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
            <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
		</div>

		<% area = Model.ObterArea(eSilviculturaAreaATV.AA_TOTAL_FLORESTA); %>
		<div class="coluna30 append2 divArea">
			<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
			<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
			<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
            <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
			<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled txtTotalFloresta", @disabled = "disabled" })%>
		</div>	
	</div>	

	<div class="block">
		<fieldset class="box boxBranca">
			<legend class="titFiltros">Área de reserva legal</legend>

			<% area = Model.ObterArea(eSilviculturaAreaATV.ARL_NATIVA); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
				<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
				<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
                <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eSilviculturaAreaATV.ARL_USO); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
				<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
				<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
                <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eSilviculturaAreaATV.ARL_RECUPERAR); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
				<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
				<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
                <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eSilviculturaAreaATV.ARL_N); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>"><%= area.Descricao %></label>
				<input type="hidden" class="hdnAreaId" value="<%= area.Id %>" />
				<input type="hidden" class="hdnAreaTipo" value="<%= area.Tipo %>" />
                <input type="hidden" class="hdnAreaValor" value="<%= area.Valor %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>
		</fieldset>
	</div>
</fieldset>

<%foreach (var item in Model.SilviculturaCaracteristicaVM){ %>
<fieldset class="block box fsSilvicultura" id="silvicultura<%=item.Caracterizacao.Identificacao%>">
	<legend class="titFiltros">Caracterização da área a ser reflorestada</legend>
	<%Html.RenderPartial("SilviculturaCarateristicaATV", item); %>
</fieldset>
<%} %>