<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMManual" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ManualVM>" %>

<div class="block borderB margem0">
	<img src="<%= Url.Content("~/Content/_imgLogo/logo_idaf_versoes.jpg") %>" alt="SIMLAM IDAF" class="floatLeft" />
	<h2 class="titVersoes floatLeft">Manuais do Sistema</h2>
</div>
			
<div class="block box boxModal">
				
	<ul class="manuais">
		<% foreach (var item in Model.Itens) { %>
		<li class="itemManual">
			<h4><%: item.Titulo %></h4>
			<a href="<%= Url.Content("~/Content/_manuais/"+item.Arquivo) %>" target="_blank" title="<%: item.Titulo %>" class="">Download do Manual em PDF </a>
		</li>
	 <%} %>
	<%= Model.Itens.Count == 0 ? "<h4>Não ha manuais disponiveis.</h4>" : ""%>
	</ul>

</div>