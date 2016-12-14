<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DeclaracaoAdicionalVM>" %>

<fieldset class="block box">
    <div class="block">
        <input type="hidden" value="<%=Model.DeclaracaoAdicional.Id %>" class="hdnId" />
        <div class="coluna98">
            <label>Texto</label>
            <%=Html.TextArea("DeclaracaoAdicional.Texto", Model.DeclaracaoAdicional.Texto, new { @class="txtTexto text limpar", @maxlength="200" }) %>
        </div>
        <div class="coluna98">
		    <label for="OutroEstado">Outro Estado? </label><br />
		    <label>
			    <%=Html.RadioButton("DeclaracaoAdicional.OutroEstado", (int)ConfiguracaoSistema.NAO,  Model.DeclaracaoAdicional.OutroEstado==0 , new { @class = "rdbOutroEstado radio"})%>
			    Não
		    </label>
		    <label>
			    <%=Html.RadioButton("DeclaracaoAdicional.OutroEstado", (int)ConfiguracaoSistema.SIM,  Model.DeclaracaoAdicional.OutroEstado==1  , new { @class = "rdbOutroEstado radio"})%>
			    Sim
		    </label>
	    </div>
    </div>

</fieldset>
