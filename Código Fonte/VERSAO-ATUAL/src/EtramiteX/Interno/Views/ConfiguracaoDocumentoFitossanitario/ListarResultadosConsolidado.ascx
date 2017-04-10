<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoNumeracaoListarVM>" %>

<fieldset class="block box">
    <div class="block dataGrid">
	    <table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
		    
		    <tbody>
                <tr>
				    <th rowspan="2" width="25%"></th>
				    <th colspan="3" >Bloco</th>
				    <th colspan="3" >Digital</th>
			    </tr>
                 <tr>
				    <th>CFO</th>
				    <th>CFOC</th>
                    <th>PTV</th>
                    <th>CFO</th>
				    <th>CFOC</th>
                    <th>PTV</th>
			    </tr>
			    <% foreach (var item in Model.ResultadosConsolidados) { %>
				    <tr class="Linha">
					    <td>
						    <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						    <span><%:item.Texto%></span>
					    </td>
					    <td>
                            <% if (!string.IsNullOrWhiteSpace(item.QtdBlocoCFO)) { %>
						        <span><%:item.QtdBlocoCFO.ToString() %></span>
                            <% } else if (item.QtdBlocoCFO == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
					    <td>
                            <% if (!string.IsNullOrWhiteSpace(item.QtdBlocoCFOC)) { %>
						        <span><%:item.QtdBlocoCFOC.ToString() %></span>
                            <% } else if (item.QtdBlocoCFOC == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
                        <td>
                            <% if (!string.IsNullOrWhiteSpace(item.QtdBlocoPTV)) { %>
						        <span><%:item.QtdBlocoPTV.ToString() %></span>
                            <% } else if (item.QtdBlocoPTV == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
                        <td>
                            <% if (!string.IsNullOrWhiteSpace(item.QtdDigitalCFO)) { %>
						        <span><%:item.QtdDigitalCFO.ToString() %></span>
                            <% } else if (item.QtdDigitalCFO == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
                        <td>
                            <% if (!string.IsNullOrWhiteSpace(item.QtdDigitalCFOC)) { %>
						        <span><%:item.QtdDigitalCFOC.ToString() %></span>
                            <% } else if (item.QtdDigitalCFOC == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
                        <td>
                           <% if (!string.IsNullOrWhiteSpace(item.QtdDigitalPTV)) { %>
						        <span><%:item.QtdDigitalPTV.ToString() %></span>
                            <% } else if (item.QtdDigitalPTV == string.Empty){ %>
                                <span>0</span>
                            <% } else{ %>
                                <span>-</span>
                            <% } %>
					    </td>
				    </tr>
			    <% } %>
		    </tbody>
	    </table>
    </div>
</fieldset>