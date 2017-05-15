<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Barragem>" %> 


<fieldset class="block box">

	<div class="block" id="checkboxes">
        <input type="hidden" class="ItemIDGeral" value="<%:Model.Id%>" />
        <input type="hidden" class="ItemIDBarragem" value="<%:Model.Barragens.First().Id%>" />
		<label>Selecione as finalidades*</label> 
         
        <br /> 
             
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Reservação") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="1" checked="checked" /> 
        <% }else{ %> 
            <input type="checkbox" name="Barragens" value="1" /> 
        <% } %> Reservação 
 
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para Irrigação") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="2" class="CaptacaoIrrigacao" checked="checked" /> 
        <% }else{ %>                                       
            <input type="checkbox" name="Barragens" value="2" class="CaptacaoIrrigacao" /> 
        <% } %> Captação para Irrigação 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Ecoturismo/Turismo Rural") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="4" class="Turismo" checked="checked" /> 
        <% }else{ %>                                       
            <input type="checkbox" name="Barragens" value="4" class="Turismo" /> 
        <% } %> Ecoturismo/Turismo Rural 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Dessedentação de Animais") > 0) 
            { %> 
            <input type="checkbox" name="Barragens" value="5" class="Dessedentacao" checked="checked" /> 
        <% }else{ %>                                       
            <input type="checkbox" name="Barragens" value="5" class="Dessedentacao" /> 
        <% } %> Dessedentação de Animais 
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Aquicultura") > 0 ){ %> 
            <input type="checkbox" name="Barragens" value="6" class="Aquicultura" checked="checked" /> 
        <% }else{ %>                                       
            <input type="checkbox" name="Barragens" value="6" class="Aquicultura" /> 
        <% } %> Aquicultura
             
        <br /> 
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para abastecimento Público") > 0) 
            { %> 
            <input type="checkbox" name="Barragens" value="8" class="CaptacaoAbastecimentoPublico" checked="checked" /> 
        <% }else{ %>                                       
            <input type="checkbox" name="Barragens" value="8" class="CaptacaoAbastecimentoPublico" /> 
        <% } %> Captação para abastecimento Público 
 
        <!-- As atividades abaixo não existem mais, e serão exibidas somente em caracterizações que já as possuíam -->
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Captação para Abastecimento Industrial") > 0 ){ %> 
            <br /> 
            <input type="checkbox" name="Barragens" value="7" class="CaptacaoAbastecimentoIndustrial" checked="checked" /> Captação para Abastecimento Industrial 
        <% } %>
 
        <% if (Model.Barragens.Count(i => i.FinalidadeTexto == "Outros") > 0 ){ %> 
            <br /> 
            <input type="checkbox" name="Barragens" value="3" class="Outros" checked="checked" /> Outros 
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