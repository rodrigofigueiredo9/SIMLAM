<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertidaoCartaAnuenciaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>


<script src="<%= Url.Content("~/Scripts/Titulo/destinatarioEspecificidade.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certidao/certidaoCartaAnuencia.js") %>"></script>

<script>
	CertidaoCartaAnuencia.urlObterDadosCertidaoCartaAnuencia = '<%= Url.Action("ObterDadosCertidaoDestinatarios", "Certidao", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>


	<% Html.RenderPartial("DestinatarioEspecificidade", Model.DestinatariosVM); %>


	<div class="block">
		<div class="coluna75">
			<label for="Certidao_Dominio">Matrícula *</label>
			<%= Html.DropDownList("Certidao.Dominio", Model.Dominios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Dominios.Count == 1, new { @class = "text ddlDominios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certidao_Descricao">Descrição *</label>
			<%= Html.TextArea("Certidao.Descricao", Model.Certidao.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtDescricao", @maxlength = "1500" }))%>
		</div>
	</div>


</fieldset>