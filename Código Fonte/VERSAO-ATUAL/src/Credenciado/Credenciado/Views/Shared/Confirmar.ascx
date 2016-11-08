<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfirmarVM>" %>

<h2 class="titTela"><%=Model.Titulo %></h2>
<br />

<input type="hidden" class="hdConfirmarId" value="<%= Model.Id%>" />
<input type="hidden" class="hdAuxiliarId" value="<%= Model.AuxiliarID%>" />
<div class="block box">
	<label><%=Model.Mensagem.Texto %></label>
</div>