<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertificadoRegistroAtividadeFlorestalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certificado/certificadoRegistroAtividadeFlorestal.js") %>"></script>

<script type="text/javascript">
    CertificadoRegistroAtividadeFlorestal.settings.urls.obterDadosCertificadoRegistroAtividadeFlorestal = '<%= Url.Action("ObterDadosCertificadoDestinatarios", "Certificado", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Certificado.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>	
    
    <div class="block">
		<div class="coluna15">
			<label for="">Vias *</label>
			<%= Html.DropDownList("ViaSelecionada", Model.Vias, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Vias.Count <= 1, new { @class = "text ddlVias" }))%>
		</div>
		<div class="coluna15 divViasOutra <%= ((Convert.ToInt16(Model.Certificado.Vias) > 5)? "" : "hide" ) %> ">
			<label ></label><br />
			<%= Html.TextBox("ViasOutra", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtViasOutra maskNumInt", @maxlength = 2 }))%>
		</div>
		<div class="coluna15 prepend2">
			<label for="">Exercício *</label><br />
			<%= Html.TextBox("Certificado.AnoExercicio", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAnoExercicio maskNumInt", @maxlength = 4 }))%>
		</div>
	</div>

</fieldset>