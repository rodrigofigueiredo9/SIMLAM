<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PragaVM>" %>

<fieldset class="block box">
    <div class="block">
        <input type="hidden" value="<%=Model.Praga.Id %>" class="hdnId" />
        <div class="coluna98">
            <label>Nome Científico*</label>
            <%=Html.TextBox("Praga.NomeCientifico", Model.Praga.NomeCientifico, new { @class="txtNomeCientifico text limpar", @maxlength="100" }) %>
        </div>
        <div class="coluna98">
            <label>Nome Comum</label>
            <%=Html.TextBox("Praga.NomeComum", Model.Praga.NomeComum, new { @class="txtNomeComum text limpar", @maxlength="100" }) %>
        </div>
    </div>

</fieldset>
