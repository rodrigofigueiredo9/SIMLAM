<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Barragem>" %> 


<fieldset class="block box">

	<div class="block" id="checkboxes">
        <%--<input type="hidden" class="ItemID" value="<%:Model.ID%>" />--%>
		<label>Selecione as finalidades*</label> 
         
        <br /> 
 
    <%--<%= Html.TextBox("BlocoTipoDocumento", Model.TipoDocumentoTexto, new { @class = "text ddlBloco ddlTipoDocumento", @readonly="readonly" })%>--%> 
             
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Reservação") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="Reservacao" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="Reservacao" /> 
        <% } %> Reservação 
 
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para Irrigação") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="CaptacaoIrrigacao" class="CaptacaoIrrigacao" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="CaptacaoIrrigacao" class="CaptacaoIrrigacao" /> 
        <% } %> Captação para Irrigação 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Ecoturismo/Turismo Rural") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="Turismo" class="Turismo" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="Turismo" class="Turismo" /> 
        <% } %> Ecoturismo/Turismo Rural 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Dessedentação de Animais") > 0) 
            { %> 
            <input type="checkbox" name="Barragens" value="Dessedentacao" class="Dessedentacao" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="Dessedentacao" class="Dessedentacao" /> 
        <% } %> Dessedentação de Animais 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Aquicultura") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="Aquicultura" class="Aquicultura" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="Aquicultura" class="Aquicultura" /> 
        <% } %> Aquicultura 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para Abastecimento Industrial") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="CaptacaoAbastecimentoIndustrial" class="CaptacaoAbastecimentoIndustrial" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="CaptacaoAbastecimentoIndustrial" class="CaptacaoAbastecimentoIndustrial" /> 
        <% } %> Captação para Abastecimento Industrial 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para abastecimento Público") > 0) 
            { %> 
            <input type="checkbox" name="Barragens" value="CaptacaoAbastecimentoPublico" class="CaptacaoAbastecimentoPublico" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="CaptacaoAbastecimentoPublico" class="CaptacaoAbastecimentoPublico" /> 
        <% } %> Captação para abastecimento Público 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Outros") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="Outros" class="Outros" checked="checked" /> Outros 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="Outros" class="Outros" hidden /> 
        <% } %> 
             
        <%--<%= Html.CheckBox("Reservação", false)%>--%> 
        <%--<%= Html.CheckBox("Captação para Irrigação", false) %> 
        <%= Html.CheckBox("Outros", false) %> 
        <%= Html.CheckBox("Ecoturismo/Turismo Rural", false) %> 
        <%= Html.CheckBox("Dessedentação de Animais", false) %> 
        <%= Html.CheckBox("Aquicultura", false) %> 
        <%= Html.CheckBox("Captação para Abastecimento Industrial", false) %> 
        <%= Html.CheckBox("Captação para abastecimento Público", false) %>--%> 
	</div>
</fieldset>