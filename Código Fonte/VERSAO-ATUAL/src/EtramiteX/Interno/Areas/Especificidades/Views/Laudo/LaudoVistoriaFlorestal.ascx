<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Laudo/laudoVistoriaFlorestal.js") %>"></script>

<script>
	LaudoVistoriaFlorestal.urlEspecificidade = '<%= Url.Action("LaudoVistoriaFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoVistoriaFlorestal.urlObterDadosLaudoVistoriaFlorestal = '<%= Url.Action("ObterDadosLaudoVistoriaFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoVistoriaFlorestal.idsTela = <%= Model.IdsTela %>;
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
			<%= Html.TextArea("Laudo.Objetivo", null, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "textarea media text txtObjetivo", @maxlength = "500" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Laudo_ResponsaveisTecnico">Responsável Técnico</label><br />
			<%= Html.DropDownList("Laudo.ResponsaveisTecnico", Model.ResponsaveisTecnico, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ResponsaveisTecnico.Count <= 1, new { @class = "text ddlResponsaveisTecnico" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Laudo_Caracterizacao">Caracterização *</label><br />
			<%= Html.DropDownList("Laudo.Caracterizacao", Model.Caracterizacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacoes.Count <= 1, new { @class = "text ddlCaracterizacoes" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Consideracao">Considerações *</label><br />
			<%= Html.TextArea("Laudo.Consideracao", Model.Laudo.Consideracao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "textarea media text txtConsideracao" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_ParecerDescricao">Descrição do Parecer Técnico *</label><br />
			<%= Html.TextArea("Laudo.ParecerDescricao", Model.Laudo.ParecerDescricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "textarea media text txtDescricao" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="Laudo_Conclusao">Conclusão *</label><br />
			<%= Html.DropDownList("Laudo.Conclusao", Model.Conclusoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlEspecificidadeConclusoes" }))%>
		</div>
	</div>

	<div class="block hide divRestricao">
		<div class="ultima">
			<label for="Laudo_Restricao">Restrições</label><br />
			<%= Html.TextArea("Laudo.Restricao", Model.Laudo.Restricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "textarea media text txtRestricao", @maxlength = "1000" }))%>
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