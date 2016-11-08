<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DominialidadeVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript">
	Dominialidade.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	Dominialidade.settings.textoMerge = '<%= Model.TextoMerge %>';
	Dominialidade.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<h1 class="titTela">Caracterização do Empreendimento -  Dominialidade</h1>
<br />

<input type="hidden" class="hdnEmpreendimentoId" value="<%:Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnATP" value="<%: Model.Caracterizacao.ATPCroqui %>" />

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Dominialidades</legend>

	<div class="block filtroCorpo">
		<%Html.RenderPartial("DominiosPartial", Model); %>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Total de áreas das Matriculas / Posses</legend>

	<div class="block filtroCorpo">
		<div class="block">
			<div class="coluna25 append2">
				<label for="Dominialidade_AreaCroqui">Área croqui (m²)</label>
				<%= Html.TextBox("DominialidadeAreaCroqui", Model.Caracterizacao.AreaCroquiString, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 append2">
				<label for="Dominialidade_AreaDocumento">Área documento (m²)</label>
				<%= Html.TextBox("DominialidadeAreaDocumento", Model.Caracterizacao.AreaDocumentoString, new { @class = "text disabled txtAreaDocumento", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 append2">
				<label for="Dominialidade_AreaCCRI">Área no CCIR (m²)</label>
				<%= Html.TextBox("DominialidadeAreaCCRI", Model.Caracterizacao.AreaCCRIString, new { @class = "text disabled txtAreaCCRI", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25 append2">
				<label for="Dominialidade_ARLCroqui">ARL croqui (m²)</label>
				<%= Html.TextBox("DominialidadeARLCroqui", Model.Caracterizacao.ARLCroquiString, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 append2">
				<label for="Dominialidade_ARLDocumento">ARL documento (m²)</label>
				<%= Html.TextBox("DominialidadeARLDocumento", Model.Caracterizacao.ARLDocumentoString, new { @class = "text disabled txtARLDocumento", @disabled = "disabled" })%>
			</div>

			<div class="coluna25">
				<label for="Dominialidade_APPCroqui">APP (m²)</label>
				<%= Html.TextBox("DominialidadeAPPCroqui", Model.Caracterizacao.APPCroquiString, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25 append2">
				<label for="Dominialidade_AreaVegetacaoNativa">Área de vegetação nativa (m²)</label>
				<%= Html.TextBox("DominialidadeAreaVegetacaoNativa", Model.Caracterizacao.AreaVegetacaoNativaString, new { @class = "text disabled txtAreaVegetacaoNativa", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 append2">
				<label for="Dominialidade_AreaFlorestaPlantada">Área de floresta plantada (m²)</label>
				<%= Html.TextBox("DominialidadeAreaFlorestaPlantada", Model.Caracterizacao.AreaFlorestaPlantadaString, new { @class = "text disabled txtAreaFlorestaPlantada", @disabled = "disabled" })%>
			</div>

			<div class="coluna25">
				<label for="Dominialidade_TotalFloresta">Total de floresta (m²)</label>
				<%= Html.TextBox("DominialidadeTotalFloresta", Model.Caracterizacao.TotalFlorestaString, new { @class = "text disabled txtTotalFloresta", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna26">
				<label for="PossuiAreaExcedenteMatricula">Imóvel possui excedente de área em relação a matrícula? *</label>
				<%= Html.DropDownList("PossuiAreaExcedenteMatricula", Model.BooleanLista, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlPossuiAreaExcedenteMatricula setarFoco" }))%>
			</div>
		</div>
	</div>

	<fieldset class="block box filtroExpansivoAberto boxBranca">
		<legend class="titFiltros">Áreas de vegetação nativa por estágio de regeneração</legend>
		<div class="block filtroCorpo">
			<% var area = Model.ObterArea(eDominialidadeArea.AVN_I); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Inicial (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo,  area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.AVN_M); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Médio (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.AVN_A); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Avançado (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.AVN_D); %>
			<div class="coluna20 append2 divArea">
				<label for="<%= "Area" + area.Tipo %>">Não caracterizado  (m²)</label>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
				<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
			</div>

			<% area = Model.ObterArea(eDominialidadeArea.APP_APMP); %>
			<div class="hide">
				<div class="coluna20 append2 divArea">
					<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea hide disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box filtroExpansivoAberto boxBranca">
		<legend class="titFiltros">Área de vegetação da APP</legend>
		<div class="block filtroCorpo">
			<div class="block">
				<%area = Model.ObterArea(eDominialidadeArea.APP_AVN); %>
				<div class="coluna25 append2 divArea">
					<label for="<%= "Area" + area.Tipo %>">Com vegetação nativa (m²)</label>
					<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
				</div>

				<%area = Model.ObterArea(eDominialidadeArea.APP_AA_USO); %>
				<div class="coluna25 append2 divArea">
					<label for="<%= "Area" + area.Tipo %>">Sem vegetação nativa (m²)</label>
					<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<%area = Model.ObterArea(eDominialidadeArea.APP_AA_REC); %>
				<div class="coluna25 append2 divArea">
					<label for="<%= "Area" + area.Tipo %>">Em recuperação (m²)</label>
					<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea disabled", @disabled = "disabled" })%>
				</div>

				<%area = Model.ObterArea(eDominialidadeArea.AA_USO_FLORES_PLANT); %>
				<div class="coluna25 append2 divArea">
					<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					<%= Html.TextBox("Area" + area.Tipo, area.ValorTexto, new { @class = "text txtArea hide disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna25">
					<label for="Dominialidade_AreaAPPNaoCaracterizada">Não caracterizada (m²)</label>
					<%= Html.TextBox("Dominialidade.AreaAPPNaoCaracterizada", Model.Caracterizacao.AreaAPPNaoCaracterizadaString, new { @class = "text disabled txtAPPNaoCaracterizada", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
	</fieldset>

	<div class="block divARL <%=Model.Caracterizacao.Dominios.SelectMany(dominio => dominio.ReservasLegais).Count(reserva=> reserva.CompensacaoTipo != eCompensacaoTipo.Nulo) > 0? "": "hide" %> ">
		<%Html.RenderPartial("DominialidadeARLPartial", Model); %>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Confrontações do empreendimento</legend>

	<div class="block filtroCorpo">
		<div class="block">
			<label for="Dominialidade_ConfrontacaoNorte">Norte *</label>
			<%= Html.TextArea("Dominialidade.ConfrontacaoNorte", Model.Caracterizacao.ConfrontacaoNorte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoNorte", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominialidade_ConfrontacaoSul">Sul *</label>
			<%= Html.TextArea("Dominialidade.ConfrontacaoSul", Model.Caracterizacao.ConfrontacaoSul, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoSul", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominialidade_ConfrontacaoLeste">Leste *</label>
			<%= Html.TextArea("Dominialidade.ConfrontacaoLeste", Model.Caracterizacao.ConfrontacaoLeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoLeste", @maxlength = "250" }))%>
		</div>

		<div class="block">
			<label for="Dominialidade_ConfrontacaoOeste">Oeste *</label>
			<%= Html.TextArea("Dominialidade.ConfrontacaoOeste", Model.Caracterizacao.ConfrontacaoOeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacaoOeste", @maxlength = "250" }))%>
		</div>
	</div>
</fieldset>