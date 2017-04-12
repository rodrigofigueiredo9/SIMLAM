<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemItem>" %>


<fieldset class="block box">

	<div class="block">
        <%--<input type="hidden" class="ItemID" value="<%:Model.ID%>" />--%>
		<div class="coluna20">
			<label>Selecione as finalidades*</label>
			<%--<%= Html.TextBox("BlocoTipoDocumento", Model.TipoDocumentoTexto, new { @class = "text ddlBloco ddlTipoDocumento", @readonly="readonly" })%>--%>
            <%= Html.CheckBox("Reservação", false) %>
            <%= Html.CheckBox("Captação para Irrigação", false) %>
            <%= Html.CheckBox("Outros", false) %>
            <%= Html.CheckBox("Ecoturismo/Turismo Rural", false) %>
            <%= Html.CheckBox("Dessedentação de Animais", false) %>
            <%= Html.CheckBox("Aquicultura", false) %>
            <%= Html.CheckBox("Captação para Abastecimento Industrial", false) %>
            <%= Html.CheckBox("Captação para abastecimento Público", false) %>
		</div>
	</div>
</fieldset>