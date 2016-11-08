<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LicencaPorteUsoMotosserraVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Licenca/licencaPorteUsoMotosserra.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Motosserra/motosserraListar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Motosserra/motosserra.js") %>"></script>

<script type="text/javascript">
	LicencaPorteUsoMotosserra.settings.urls.obterDadosLicencaPorteUsoMotosserra = '<%= Url.Action("ObterDadosLicencaPorteUsoMotosserra", "Licenca", new {area="Especificidades"}) %>';
	LicencaPorteUsoMotosserra.settings.urls.associarMotosserra = '<%= Url.Action("Associar", "../Motosserra") %>';
	LicencaPorteUsoMotosserra.settings.urls.obterMotosserra = '<%= Url.Action("ObterMotosserra", "Licenca", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Licenca.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna15 append2">
			<label for="Licenca_Via">Vias *</label>
			<%= Html.DropDownList("Licenca.Via", Model.Vias, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Vias.Count <= 1, new { @class = "text ddlVias" }))%>
		</div>

		<div class="coluna15 divViasOutra <%= ((Model.Licenca.Vias > 5)? "" : "hide" ) %> append2">
			<label ></label><br />
			<%= Html.TextBox("Licenca.ViasOutra", Model.Licenca.Vias, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtViasOutra maskNumInt", @maxlength = 2 }))%>
		</div>

		<div class="coluna20 append2">
			<label for="">Exercício *</label><br />
			<%= Html.TextBox("Licenca.AnoExercicio", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtExercicio maskNumInt", @maxlength = 4 }))%>
		</div>
	</div>
	<br />

	<fieldset id="fsMotosserra" class="boxBranca coluna72">
		<legend>Motosserra</legend>

		<input type="hidden" class="hdnMotosserraId" value="<%=Model.Licenca.Motosserra.Id %>" />
		<input type="hidden" class="hdnMotosserraProprietarioId" value="<%=Model.Licenca.Motosserra.ProprietarioId %>" />
		<input type="hidden" class="hdnMotosserraTid" value="<%=Model.Licenca.Motosserra.Tid %>" />

		<div class="block">
			<div class="coluna40 append2">
				<label >Nº Registro *</label><br />
				<%= Html.TextBox("Licenca.NumeroRegistro", Model.Licenca.Motosserra.NumeroRegistro, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumeroRegistro", @maxlength = 80 }))%>
			</div>

			<div class="coluna40">
				<label for="">Nº Fabricação/Série *</label><br />
				<%= Html.TextBox("Licenca.NumeroFabricacao", Model.Licenca.Motosserra.NumeroFabricacao, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumeroFabricacao", @maxlength = 80 }))%>
			</div>

			<div class="ultima prepend2 divBotoesMotosserra">
			<% if (!Model.IsVisualizar){ %>
				<button type="button" class="floatLeft inlineBotao <%= Model.Licenca.Motosserra.Id <= 0? "" : "hide" %> btnBuscarMotosserra">Buscar</button>
				<span class="prepend1 btnLimparContainerMotosserra <%= Model.Licenca.Motosserra.Id > 0 ? "" : "hide" %>"><button type="button" class="inlineBotao btnLimparMotosserra">Limpar</button></span>
			<% } %>
			</div>
		</div>

		<div class="block">
			<div class="coluna40 append2">
				<label >Marca/Modelo *</label><br />
				<%= Html.TextBox("Licenca.Marca", Model.Licenca.Motosserra.Marca, ViewModelHelper.SetaDisabled(true, new { @class = "text txtMarcaModelo", @maxlength = 80 }))%>
			</div>

			<div class="coluna40">
				<label >Nº Nota Fiscal *</label><br />
				<%= Html.TextBox("Licenca.NotaFiscal", Model.Licenca.Motosserra.NotaFiscal, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNotaFiscal", @maxlength = 80 }))%>
			</div>
		</div>
	</fieldset>

</fieldset>