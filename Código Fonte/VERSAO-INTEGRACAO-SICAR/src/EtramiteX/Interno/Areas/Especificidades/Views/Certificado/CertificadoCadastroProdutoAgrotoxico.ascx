<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertificadoCadastroProdutoAgrotoxicoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certificado/certificadoCadastroProdutoAgrotoxico.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>

<script type="text/javascript">
	CertificadoCadastroProdutoAgrotoxico.settings.urls.obterDadosCertificadoCadastroProdutoAgrotoxico = '<%= Url.Action("ObterDadosCertificadoCadastroProdutoAgrotoxico", "Certificado", new {area="Especificidades"}) %>';
	CertificadoCadastroProdutoAgrotoxico.settings.urls.obterDadosAgrotoxico = '<%= Url.Action("ObterDadosAgrotoxico", "Certificado", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box fdsEspecificidade">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_AgrotoxicoNome">Agrotóxico *</label>
			<%= Html.TextBox("Certificado.AgrotoxicoNome", Model.Certificado.AgrotoxicoNome,  ViewModelHelper.SetaDisabled(true, new { @class = "text txtAgrotoxicoNome" }))%>
			<%= Html.Hidden("Certificado.AgrotoxicoId", Model.Certificado.AgrotoxicoId, new { @class = "text hdnAgrotoxicoId" })%>
			<%= Html.Hidden("Certificado.AgrotoxicoTid", Model.Certificado.AgrotoxicoTid, new { @class = "text hdnAgrotoxicoTid" })%>
		</div>
		<div class="coluna10">
			<button type="button" class="inlineBotao btnAtualizarAgrotoxico hide">Atualizar</button>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Certificado.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>
</fieldset>