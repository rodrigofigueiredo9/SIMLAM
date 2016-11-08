<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMSobre" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SobreVM>" %>

<div class="modalContent ">

	<div class="block borderB margem0">
		<img src="<%= Url.Content("~/Content/_imgLogo/logo_idaf_versoes.jpg") %>" alt="SIMLAM IDAF" class="floatLeft" />
		<h2 class="titVersoes floatLeft">Histórico de Versões</h2>
	</div>
			
	<div class="block box boxModal">

		<div class="itemVersao borderB">
			<p class="aberto"><%: Model.Sobre.Versao %></p>
			<input type="hidden" class="versaoId" value="<%: Model.Sobre.Id %>" />
			<ul class="">
			<% foreach (var item in Model.Sobre.Itens) { %>
				<li><span class="hide"><%:item.Tipo %></span><span><%: "#"+ item.NumeroTP + " - " + item.Descricao %></span></li>
			<% } %>		
			<%if (Model.Sobre.Itens.Count == 0) { %>
				<li><span class="hide"></span><span>Não existe itens para esta versão</span></li>		 
			<% } %>		
			</ul>
		</div>				
		<%foreach (var item in Model.VersoesAntigas) { %>
		<div class="itemVersao borderB">
			<p><%: item.Versao%></p>
			<input type="hidden" class="versaoId" value="<%: item.Id %>" />
			<ul class="hide">
			</ul>
		</div>
		<% } %>
	</div>

	<li style="display: none; visibility: hidden;" class="liTemplate"><span class="hide"></span><span></span></li>

</div>
	