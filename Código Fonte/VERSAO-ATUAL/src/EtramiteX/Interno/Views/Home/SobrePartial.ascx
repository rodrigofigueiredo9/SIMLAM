<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMSobre" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SobreVM>" %>

<div class="modalContent boxModal borders">
	<div class="sobreHeader">
		<div class="versaoSobreCaixa">
			<p class="sistema">SIMLAM IDAF</p>
			<p class="txtInfo">Versão <%: Model.Sobre.Versao %></p>
			<p class="txtInfo"><%: Model.Sobre.Data%></p>
		</div>
	</div>
	<div class="block box margem0">
		<div class="coluna98 prepend3">
			<span class="small quiet">Licenciado para:</span>
			<p><strong><%: Model.Sobre.Licenciado%></strong></p>
			<span class="small quiet">Desenvolvido por:</span>
			<p><a href="http://www.tecnomapas.com.br/" title="Tecnomapas - Exelência em Geotecnologia" target="_blank"><img src="<%= Url.Content("~/Content/_imgLogo/tecnomapas_sobre.png") %>" width="99" height="27" alt="Tecnomapas - Exelência em Geotecnologia" /></a></p>
			<p class="floatRight append3 negtTop">
				<button class="btnHistorico" title="Histórico de Versões">Histórico de Versões</button>
				<button class="fMdl" title="OK">OK</button>
			</p>
		</div>
	</div>
</div>