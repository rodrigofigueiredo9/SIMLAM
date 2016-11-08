<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertidaoDispensaLicenciamentoAmbientalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certidao/certidaoDispensaLicenciamentoAmbiental.js") %>"></script>

<script type="text/javascript">
	$(function () {
		CertidaoDispensaLicenciamentoAmbiental.urlObterDadosRequerimento = '<%= Url.Action("ObterDadosRequerimeto", "TituloDeclaratorio", new {area=""}) %>';
	});
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>

	<div class="block">
		<div class="coluna75">
			<label for="Atividade">Atividade Dispensada *</label>
			<%= Html.DropDownList("Certidao.Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Atividade.Count == 1, new { @class = "text ddlAtividades" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Interessado">Interessado *</label>
			<%= Html.TextBox("Interessado", Model.Certidao.Interessado, ViewModelHelper.SetaDisabledReadOnly(true, new { @class = "text txtInteressado" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="Vinculo">Vinculo com a propriedade *</label>
			<%= Html.DropDownList("Vinculo", Model.VinculoPropriedade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlVinculo" }))%>
		</div>

		<div class="coluna43 prepend1 <%= Model.Certidao.VinculoPropriedade == (int)eCertidaoDispLicAmbVinculoProp.Outros ? "" : "hide" %>  divOutros">
			<label for="VinculoPropOutro">Especificar *</label>
			<%= Html.TextBox("VinculoPropOutro", Model.Certidao.VinculoPropriedadeOutro, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtVinculoPropOutro", @maxlength="50" }))%>
		</div>
	</div>
</fieldset>