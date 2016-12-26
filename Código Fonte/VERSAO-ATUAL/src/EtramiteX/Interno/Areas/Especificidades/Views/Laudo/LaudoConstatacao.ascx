<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoConstatacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Laudo/laudoConstatacao.js") %>"></script>

<script>
	LaudoConstatacao.urlObterDadosLaudoConstatacao = '<%= Url.Action("ObterDadosLaudoConstatacao", "Laudo", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
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
			<%= Html.TextBox("Laudo.DataVistoria.DataTexto", Model.Laudo.DataVistoria.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea text maskData txtDataVistoria" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Objetivo">Objetivo *</label><br /><!-- br previne que firefox bagunçe com label/textarea -->
			<%= Html.TextArea("Laudo.Objetivo", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "media text txtObjetivo", @maxlength = "500" }))%>
		</div>
	</div>

	<div class="block">
		<div class="ultima">
			<label for="Laudo_Constatacao">Constatação *</label><br /><!-- br previne que firefox bagunçe com label/textarea -->
			<%= Html.TextArea("Laudo.Constatacao", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConstatacao"}))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<legend>Arquivo</legend>
	<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>