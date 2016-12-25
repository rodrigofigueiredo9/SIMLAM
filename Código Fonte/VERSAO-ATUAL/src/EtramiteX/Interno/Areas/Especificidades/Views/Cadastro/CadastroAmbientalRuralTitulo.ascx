<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Cadastro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CadastroAmbientalRuralTituloVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>


<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Cadastro/cadastroAmbientalRuralTitulo.js") %>"></script>
<script>
	CadastroAmbientalRuralTitulo.urlObterDadosCadastroAmbientalRuralTitulo = '<%= Url.Action("ObterDadosCadastro", "Cadastro", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="CadastroAmbientalRuralTitulo_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("CadastroAmbientalRuralTitulo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="CadastroAmbientalRuralTitulo_Matricula">Nº matrícula do empreendimento</label>
			<%= Html.TextBox("CadastroAmbientalRuralTitulo.Matricula", Model.Entidade.Matricula, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtMatricula", @maxlength = "200" }))%>
		</div>
	</div>
</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>