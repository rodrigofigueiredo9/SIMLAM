<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DominialidadeVM>" %>

<fieldset class="block box filtroExpansivoAberto boxBranca">
	<legend class="titFiltros">Áreas de reserva legal compensada</legend>
	<div class="block filtroCorpo">
		<div class="block">
			<% var area = Model.ObterArea(eDominialidadeArea.ARL_CEDENTE); %>
			<div class="coluna25 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Total de ARL cedente (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.ARL_RECEPTOR); %>
			<div class="coluna25 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Total de ARL receptor (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<% area = Model.ObterArea(eDominialidadeArea.ARL_PRESERVADA); %>
			<div class="coluna25 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Preservada (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.ARL_RECUPERACAO); %>
			<div class="coluna25 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Em recuperação (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<% area = Model.ObterArea(eDominialidadeArea.ARL_USO); %>
			<div class="coluna25 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">A recuperar (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25">
				<label for="Dominialidade_AreaARLNaoCaracterizada">Não caracterizada (m²)</label>
				<%= Html.TextBox("Dominialidade.AreaARLNaoCaracterizada", Model.Caracterizacao.ARLNaoCaracterizadaCompensada.ToStringTrunc(), new { @class = "text disabled txtARLNaoCaracterizada", @disabled = "disabled" })%>
			</div>
		</div>
		<%bool mostrar = Model.Caracterizacao.PossuiPorcentagemMinimaARL;%>

		<div class="block msgInline erro divAlertaARL <%= mostrar ? "" : "hide" %>">
			<p><label><strong>Alerta!</strong> O empreendimento cedente não possui os 20% de ARL. Isso irá inviabilizar o Cadastro Ambiental Rural.</label></p>
		</div>
	</div>
</fieldset>