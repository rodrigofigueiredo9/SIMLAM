<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CertificadoCadastroProdutoVegetalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Certificado/certificadoCadastroProdutoVegetal.js") %>"></script>
<script>
	CertificadoCadastroProdutoVegetal.settings.urls.urlObterDadosCertificado = '<%= Url.Action("ObterDadosCertificadoCadastroProdutoVegetal", "Certificado", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Certificado.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios", @id = "ddlDestinatarios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Nome">Nome do Produto *</label>
			<%= Html.TextBox("Certificado.Nome", Model.Certificado.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNome", @id = "txtNome" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Fabricante">Fabricante do produto *</label>
			<%= Html.TextBox("Certificado.Fabricante", Model.Certificado.Fabricante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFabricante", @id = "txtFabricante" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_ClasseToxicologica">Classe toxilógica *</label>
			<%= Html.TextBox("Certificado.ClasseToxicologica", Model.Certificado.ClasseToxicologica, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtClasseToxicologica", @id = "txtClasseToxicologica" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Classe">Classe *</label>
			<%= Html.TextBox("Certificado.Classe", Model.Certificado.Classe, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtClasse", @id = "txtClasse" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Ingrediente">Ingrediente Ativo *</label>
			<%= Html.TextBox("Certificado.Ingrediente", Model.Certificado.Ingrediente, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtIngrediente", @id = "txtIngrediente" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Classificacao">Classificação Qto. Pot. Peric. Ambiental *</label>
			<%= Html.TextBox("Certificado.Classificacao", Model.Certificado.Classificacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCultura", @id = "txtCultura" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Certificado_Cultura">Cultura(s) indicada(s) *</label>
			<%= Html.TextBox("Certificado.Cultura", Model.Certificado.Cultura, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtClassificacao", @id = "txtClassificacao" }))%>
		</div>
	</div>

</fieldset>