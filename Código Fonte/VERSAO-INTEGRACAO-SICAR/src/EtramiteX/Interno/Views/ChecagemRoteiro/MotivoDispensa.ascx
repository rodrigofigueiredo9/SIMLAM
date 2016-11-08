<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<h1 class="titTela">Cadastrar Checagem de Itens de Roteiro</h1>
<br />
<div>
	<div>
		<label>Motivo *</label>
		<%= Html.TextArea("Motivo", null, new { @class = "textarea text txtMotivo", @maxlength = "500", @style = "height: 140px;"})%>
	</div>
</div>