<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteSituacaoAtenderVM>" %>


<script type="text/javascript">
	CondicionanteAtender.settings.urls = {
		atender: '<%= Url.Action("CondicionanteAtenderSalvar", "Titulo") %>'
	}
</script>

<h2 class="titTela">Atender Condicionante</h2>

<%= Html.Hidden("Condicionante.Id", null, new { @class = "hdnCondicionanteId" })%>
<%= Html.Hidden("PeriodicidadeId", null, new { @class = "hdnCondicionantePeriodicidadeId" })%>

<div class="block box">
	<div class="coluna90">
		Tem certeza que deseja definir a condicionante "<%= Model.Condicionante.Descricao %>" como atendida?
	</div>
</div>