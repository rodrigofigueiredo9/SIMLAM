<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfirmarVM>" %>

<h2 class="titTela"><%=Model.Titulo %></h2>
<br />

<input type="hidden" class="hdConfirmarId" value="<%= Model.Id%>" />
<div class="block box">
	<label><%=Model.Mensagem.Texto %></label>
</div>