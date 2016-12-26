<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.Blocos.Entities.Interno.ModuloTitulo.TituloCondicionanteDescricao>" %>

<script>
	CondicionanteDescricaoSalvar.settings.urls = {
		salvar: '<%= Url.Action("CondicionanteDescricaoSalvar", "Titulo") %>'
	}
</script>

<h2 class="titTela">Cadastrar Descrição de Condicionante</h2>

<%= Html.Hidden("Id", null, new { @class = "hdnItemId" })%>

<div class="block box">
	<div class="coluna95">
		<label for="DataEnvio">Descrição*</label>
		<%= Html.TextArea("Texto", null, new { @maxlength = "300", @class = "text textareaPequeno txtDescricao" })%>
	</div>
</div>