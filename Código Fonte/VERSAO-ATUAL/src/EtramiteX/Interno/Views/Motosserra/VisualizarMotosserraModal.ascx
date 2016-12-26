<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraVM>" %>

<!-- DEPENDENCIAS DE PESSOA -->
<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE PESSOA -->

<div class="motosserraModalContainer">
	<h1 class="titTela">Visualizar Motosserra</h1>
	<br />

	<%  Html.RenderPartial("VerificarPartial", Model); %>
</div>