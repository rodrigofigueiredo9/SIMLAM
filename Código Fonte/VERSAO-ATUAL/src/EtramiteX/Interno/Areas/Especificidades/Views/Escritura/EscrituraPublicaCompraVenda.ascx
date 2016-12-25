<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Escritura" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EscrituraPublicaCompraVendaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Escritura/escrituraPublicaCompraVenda.js") %>"></script>

<script>
	EscrituraPublicaCompraVenda.urlEspecificidade = '<%= Url.Action("EscrituraPublicaCompraVenda", "Escritura", new {area="Especificidades"}) %>';
	EscrituraPublicaCompraVenda.urlObterDadosEscrituraPublicaCompraVenda = '<%= Url.Action("ObterDadosEscrituraPublicaCompraVenda", "Escritura", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box divEspecificidade">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna15 append2">
			<label for="Escritura_Livro,">Livro *</label><br />
			<%= Html.TextBox("Escritura.Livro", Model.Escritura.Livro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLivro", @maxlength = "10" }))%>
		</div>

		<div class="coluna15">
			<label for="Escritura_Folhas">Folhas *</label><br />
			<%= Html.TextBox("Escritura.Folhas", Model.Escritura.Folhas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFolhas", @maxlength = "10" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Escritura_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Escritura.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>
</fieldset>