<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFomentoFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Laudo/laudoVistoriaFomentoFlorestal.js") %>"></script>

<script>
	LaudoVistoriaFomentoFlorestal.urlEspecificidade = '<%= Url.Action("LaudoVistoriaFomentoFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoVistoriaFomentoFlorestal.urlObterDadosLaudoVistoriaFomentoFlorestal = '<%= Url.Action("ObterDadosLaudoVistoriaFomentoFlorestal", "Laudo", new {area="Especificidades"}) %>';
	LaudoVistoriaFomentoFlorestal.idsTela = <%= Model.IdsTela %>;
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
			<%= Html.TextArea("Laudo.Objetivo", Model.Laudo.Objetivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtObjetivo", @maxlength = "1000" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Consideracoes">Considerações *</label><br />
			<%= Html.TextArea("Laudo.Consideracoes", Model.Laudo.Consideracoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtConsideracoes", @maxlength = "1000" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_DescricaoParecer">Parecer técnico *</label><br />
			<%= Html.TextArea("Laudo.DescricaoParecer", Model.Laudo.DescricaoParecer, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtDescricaoParecer", @maxlength = "1000" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna25">
			<label for="Laudo_Conclusao">Conclusão *</label><br />
			<%= Html.DropDownList("Laudo.Conclusao", Model.Conclusoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlConclusoes" }))%>
		</div>
	</div>

	<div class="block divRestricoes <%:(Model.IsVisualizar && Model.Laudo.ConclusaoTipo == (int)eEspecificidadeConclusao.Favoravel) ? "hide" : "" %>">
		<div class="coluna70">
			<label for="Laudo_Restricoes">Restrições</label><br />
			<%= Html.TextArea("Laudo.Restricoes", Model.Laudo.Restricoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtRestricoes", @maxlength = "1000" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Observacoes">Observações</label><br />
			<%= Html.TextArea("Laudo.Observacoes", Model.Laudo.Observacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media text txtObservacoes", @maxlength = "1000" }))%>
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