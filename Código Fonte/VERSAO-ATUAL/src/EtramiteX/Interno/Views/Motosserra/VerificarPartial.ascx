<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraVM>" %>

<div class="block box">
	<div class="block">
		<div class="coluna28 append2">
			<label for="SerieNumero">Nº Fabricação/ Série *</label>
			<%= Html.TextBox("SerieNumero", Model.Motosserra.SerieNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtSerieNumero setarFoco", @maxlength = "80" }))%>
		</div>

		<%if(Model.Motosserra.Id <= 0) {%>
		<div class="coluna10">
			<button class="inlineBotao btnVerificar <%=Model.Motosserra.Id > 0 ? "hide" : "" %>">Verificar</button>
			<button class="inlineBotao btnLimpar  <%=Model.Motosserra.Id > 0 ? "" : "hide" %>">Limpar</button>
		</div>
		<%} %>
	</div>
</div>

<div class="containerMotosserra">
	<%if(Model.Motosserra.Id > 0) {%>
		<% Html.RenderPartial("MotosserraPartial"); %>
	<%} %>
</div>


<fieldset class="box fsMotosserras hide">
	<legend>Motosserras</legend>

	<div class="dataGridView"></div>
</fieldset>