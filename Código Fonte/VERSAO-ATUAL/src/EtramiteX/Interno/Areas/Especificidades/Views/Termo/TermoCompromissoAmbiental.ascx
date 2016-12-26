<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TermoCompromissoAmbientalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/termoCompromissoAmbiental.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>

<script>
	TermoCompromissoAmbiental.settings.urls.obterDadosTermoCompromissoAmbiental = '<%= Url.Action("ObterDadosTermoCompromissoAmbiental", "Termo", new {area="Especificidades"}) %>';
	TermoCompromissoAmbiental.settings.urls.obterDadosTermoCompromissoAmbientalRepresentantes = '<%= Url.Action("ObterDadosTermoCompromissoAmbientalRepresentantes", "Termo", new {area="Especificidades"}) %>';
	TermoCompromissoAmbiental.settings.urls.associarTitulo = '<%= Url.Action("Associar", "Titulo") %>';
	TermoCompromissoAmbiental.settings.urls.validarAssociarTitulo = '<%= Url.Action("ValidarAssociarTitulo", "Termo") %>';
	TermoCompromissoAmbiental.settings.modelosCodigosTitulos = <%=Model.ModelosCodigosTitulo%>
</script>

<fieldset class="block box fdsEspecificidade">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
			<div class="coluna68">
				<label>Licença Ambiental de Regularização - LAR *</label>
				<%=Html.Hidden("Termo.Licenca", Model.Termo.Licenca, new { @class = "hdnLicencaId" })%>
				<%= Html.TextBox("Termo.LicencaNumero", Model.Termo.LicencaNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLicencaNumero" }))%>
			</div>

			<div class="ultima">
			<% if (!Model.IsVisualizar){ %>
				<button type="button" class="floatLeft inlineBotao <%= (Model.Termo.Licenca > 0) ? "hide" : ""%> btnBuscarTitulo">Buscar</button>
				<span class="prepend1 btnLimparTituloContainer <%= (Model.Termo.Licenca > 0) ? "" : "hide"%>">
					<button type="button" class="inlineBotao btnLimparTituloNumero">Limpar</button>
				</span>
			<% } %>
			</div>
		</div>

	<div class="block">
		<div class="coluna75">
			<label for="Termo_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Termo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block divRepresentantes <%=Model.Termo.DestinatarioTipo == PessoaTipo.JURIDICA ? "" : "hide"%> ">
		<div class="coluna75">
			<label for="Termo_Representante">Representante Legal *</label>
			<%= Html.DropDownList("Termo.Representante", Model.Representantes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Representantes.Count <= 2, new { @class = "text ddlRepresentantes" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Termo_Descricao">Descrição de compromisso ambiental *</label>
			<%= Html.TextArea("Termo.Descricao", Model.Termo.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtDescricao", @maxlength="5000" }))%>
		</div>
	</div>
</fieldset>