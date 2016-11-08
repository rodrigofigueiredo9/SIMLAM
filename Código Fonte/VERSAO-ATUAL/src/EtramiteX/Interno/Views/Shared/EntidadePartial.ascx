<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Tecnomapas.Blocos.Etx.ModuloCore.View.Sessao>>" %>
<% foreach (var item in Model)
   {
	   Html.RenderPartial("SessaoPartial", item);
   } %>