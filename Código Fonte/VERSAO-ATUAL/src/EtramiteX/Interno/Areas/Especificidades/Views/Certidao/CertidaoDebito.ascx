<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertidaoDebitoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>


<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certidao/certidaoDebito.js") %>"></script>
<script>
	CertidaoDebito.urlObterDadosCertidaoDebito = '<%= Url.Action("ObterDestinatarios", "Certidao", new {area="Especificidades"}) %>';
	CertidaoDebito.urlObterFiscalizacoesPorAutuado = '<%= Url.Action("ObterFiscalizacoesPorAutuado", "Certidao", new {area="Especificidades"}) %>';
	CertidaoDebito.urlVisualizarFiscalizacao = '<%= Url.Action("VisualizarFiscalizacaoModal", "../Fiscalizacao") %>';
</script>

<input type="hidden" class="hdnIsVisualizar" value='<%:Model.IsVisualizar %>' />

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna75">
			<label for="CertidaoDebito_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("CertidaoDebito.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<fieldset class="coluna72 boxBranca">
			<legend>Fiscalizações encontradas</legend>

			<div class="divFiscalizacoes">
				<% Html.RenderPartial("CertidaoDebitoFiscalizacoesPartial", Model); %>
			</div>
		</fieldset>
	</div>

	<div class="block">
		<fieldset class="coluna72 boxBranca">
			<legend>Resultado</legend>

			<div class="coluna65 append2">
				<label for="CertidaoDebito_TipoCertidao">Tipo da certidão</label>
				<%= Html.TextBox("CertidaoDebito.TipoCertidao", Model.TipoCertidao, new { @class = "text txtTipoCertidao disabled", @disabled = "disabled" })%>
			</div>
		</fieldset>
	</div>

</fieldset>