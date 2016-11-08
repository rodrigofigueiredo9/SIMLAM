<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<h5>Tipo de Relatório</h5>
<div class="block">
	<div class="coluna35 margemDTop">
		<p>
			<label for="Agrupar">Agrupar por:</label>
			<%= Html.DropDownList("Agrupar", Model.CamposLst, new { @class = "text setarFoco ddlAgrupador" })%>
		</p>
	</div>
	<div class="coluna40">
		<img class="imgAgrupar" src="<%= Model.CaminhoImagem %>" alt="Exemplo de Relatórios Agrupado" width="311" height="156" />
	</div>
</div>