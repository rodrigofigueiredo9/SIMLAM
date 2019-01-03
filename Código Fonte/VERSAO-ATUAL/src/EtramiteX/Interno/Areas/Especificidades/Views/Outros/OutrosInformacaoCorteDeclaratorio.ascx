<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrosInformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Outros/outrosInformacaoCorteDeclaratorio.js") %>"></script>
<script type="text/javascript">
	$(function () {
		OutrosInformacaoCorte.urlObterDadosRequerimento = '<%= Url.Action("ObterDadosOutrosInformacaoCorteDeclaratorio", "Outros", new {area=""}) %>';
	});
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>

	<div class="block">
		<div class="coluna75">
			<label for="Atividade">Atividade Dispensada *</label>
			<%--<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>--%>
			<%= Html.DropDownList("Certidao.Atividade", Model.AtividadeList, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividades" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Interessado">Interessado *</label>
			<%= Html.TextBox("Interessado", Model.Outros.Interessado, ViewModelHelper.SetaDisabledReadOnly(true, new { @class = "text txtInteressado" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="InformacaoCorte">Informação de corte *</label>
			<%= Html.DropDownList("Certidao.InformacaoCorte", Model.InformacaoCortes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlInformacaoCorte" }))%>
		</div>

		<div class="coluna43 prepend1 divOutros">
			<label for="Validade">Validade de Informação de corte (dias) *</label>
			<%= Html.TextBox("Validade", Model.Outros.Validade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtValidadeInfCorte", @maxlength="3" }))%>
		</div>
	</div>
</fieldset>