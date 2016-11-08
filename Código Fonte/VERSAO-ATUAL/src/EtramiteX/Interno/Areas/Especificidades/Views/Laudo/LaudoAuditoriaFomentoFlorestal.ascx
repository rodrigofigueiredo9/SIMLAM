<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoAuditoriaFomentoFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Laudo/laudoAuditoriaFomentoFlorestal.js") %>"></script>

<script type="text/javascript">
	LaudoAuditoriaFomentoFlorestal.urlEspecificidade = '<%= Url.Action("LaudoAuditoriaFomentoFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoAuditoriaFomentoFlorestal.urlObterDadosLaudoAuditoriaFomentoFlorestal = '<%= Url.Action("ObterDadosLaudoAuditoriaFomentoFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoAuditoriaFomentoFlorestal.idsTela = <%= Model.IdsTela %>;
</script>

<fieldset class="block box divEspecificidade">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="Laudo_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Laudo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
		<div class="coluna15 prepend2">
			<label for="Laudo_DataVistoria_DataTexto">Data da Vistoria *</label><br />
			<%= Html.TextBox("Laudo.DataVistoria.DataTexto", Model.Laudo.DataVistoria.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text maskData txtDataVistoria" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Objetivo">Objetivo *</label><br />
			<%= Html.TextArea("Laudo.Objetivo", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtObjetivo", @maxlength = "1000" }))%>
		</div>
	</div>

	<fieldset class="box" id="fsConstatacoes">
		<legend>Constatações</legend>

		<div class="block">
			<div class="coluna48">
				<label for="Laudo_PlantioAPP">Plantio em APP *</label><br />
				<label><%= Html.RadioButton("Laudo.PlantioAPP", 0, (Model.Laudo.PlantioAPP == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPlantioAPP" }))%>Não</label>
				<label><%= Html.RadioButton("Laudo.PlantioAPP", 1, (Model.Laudo.PlantioAPP == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPlantioAPP" }))%>Sim</label>
			</div>

			<div class="coluna18 prepend2 divPlantioAPPArea <%:(Model.IsVisualizar && Model.Laudo.PlantioAPP == 1) ? "hide" : "" %>">
				<label for="Laudo_PlantioAPPArea">Área Plantada(ha) *</label><br />
				<%= Html.TextBox("Laudo.PlantioAPPArea", Model.Laudo.PlantioAPPArea, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPlantioAPPArea maskDecimal", @maxlength = "10" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna48">
				<label for="Laudo_PlantioMudasEspeciesFlorestNativas">Plantio de mudas de espécies florestais nativas *</label><br />
				<label><%= Html.RadioButton("Laudo.PlantioMudasEspeciesFlorestNativas", 0, (Model.Laudo.PlantioMudasEspeciesFlorestNativas == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPlantioMudasEspeciesFlorestNativas" }))%>Não</label>
				<label><%= Html.RadioButton("Laudo.PlantioMudasEspeciesFlorestNativas", 1, (Model.Laudo.PlantioMudasEspeciesFlorestNativas == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPlantioMudasEspeciesFlorestNativas" }))%>Sim</label>
			</div>

			<div class="coluna18 prepend2 divPlantioMudasEspeciesFlorestNativasQtd <%:(Model.IsVisualizar && Model.Laudo.PlantioMudasEspeciesFlorestNativas == 1) ? "hide" : "" %>">
				<label for="Laudo_PlantioMudasEspeciesFlorestNativasQtd">Quantidade de planta *</label><br />
				<%= Html.TextBox("Laudo.PlantioMudasEspeciesFlorestNativasQtd", Model.Laudo.PlantioMudasEspeciesFlorestNativasQtd, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPlantioMudasEspeciesFlorestNativasQtd", @maxlength = "5" }))%>
			</div>

			<div class="coluna18 prepend2 divPlantioMudasEspeciesFlorestNativasArea <%:(Model.IsVisualizar && Model.Laudo.PlantioMudasEspeciesFlorestNativas == 1) ? "hide" : "" %>">
				<label for="Laudo_PlantioMudasEspeciesFlorestNativasArea">Área Plantada(ha) *</label><br />
				<%= Html.TextBox("Laudo.PlantioMudasEspeciesFlorestNativasArea", Model.Laudo.PlantioMudasEspeciesFlorestNativasArea, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPlantioMudasEspeciesFlorestNativasArea maskDecimal", @maxlength = "10" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna48">
				<label for="Laudo_PreparoSolo">Preparo do solo no sentido da pendente topográfica (>24º) *</label><br />
				<label><%= Html.RadioButton("Laudo.PreparoSolo", 0, (Model.Laudo.PreparoSolo == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPreparoSolo" }))%>Não</label>
				<label><%= Html.RadioButton("Laudo.PreparoSolo", 1, (Model.Laudo.PreparoSolo == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbPreparoSolo" }))%>Sim</label>
			</div>

			<div class="coluna18 prepend2 divPreparoSoloArea <%:(Model.IsVisualizar && Model.Laudo.PreparoSolo == 1) ? "hide" : "" %>">
				<label for="Laudo_PreparoSoloArea">Área (ha) *</label><br />
				<%= Html.TextBox("Laudo.PreparoSoloArea", Model.Laudo.PreparoSoloArea, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPreparoSoloArea maskDecimal", @maxlength = "10" }))%>
			</div>
		</div>
	</fieldset>

	<div class="block">
		<div class="coluna25">
			<label for="Laudo_Resultados">Resultado *</label><br />
			<%= Html.DropDownList("Laudo.Resultados", Model.Resultados, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlResultados" }))%>
		</div>
	</div>

	<div class="block divQuais <%:(Model.IsVisualizar && Model.Laudo.ResultadoTipo == (int)eEspecificidadeResultado.NaoConforme) ? "hide" : "" %>">
		<div class="coluna70">
			<label for="Laudo_ResultadoQuais">Qual(is) *</label><br />
			<%= Html.TextArea("Laudo.ResultadoQuais", Model.Laudo.ResultadoQuais, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtResultadoQuais" }))%>
		</div>
	</div>
	
	<div class="block">
		<div class="ultima">
			<label for="Laudo_ParecerDescricao">Parecer Técnico *</label><br />
			<%= Html.TextArea("Laudo.ParecerDescricao", Model.Laudo.ParecerDescricao, ViewModelHelper.SetaDisabled(true, new { @class = "textarea media text txtParecerDescricao" }))%>
		</div>
	</div>
</fieldset>

<% if (Model.IsCondicionantes) { %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes</legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<legend>Arquivo</legend>
	<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>