<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="divRegChave">

<input type="hidden" class="credId" value="<%= Model.Credenciado.Id %>" />

<h2 class="titTela">Regerar Chave</h2>
<br />
Tem certeza que deseja regerar a chave de acesso do <%= Model.Credenciado.Nome%>?
</div>





