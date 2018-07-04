<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVHistoricoVM>" %>

<h2 class="titTela">Histórico da Análise do EPTV</h2>
<br />

<fieldset class="box">
	<div class="block">
		<div class="coluna22">
			<label for="Numero">Número EPTV</label>
			<%=Html.TextBox("NumeroEPTV", Model.NumeroEPTV , ViewModelHelper.SetaDisabled(true , new { @class="text"}))%>
	  </div>
        
        <div class="coluna22">
			<label for="data">Data de emissão</label>
			<%=Html.TextBox("DataEmissao", Convert.ToDateTime(Model.DataEmissao).ToShortDateString(), ViewModelHelper.SetaDisabled(true , new { @class="text"}))%>
	  </div>
        
        <div class="coluna22">
			<label for="SituacaoAtual">Situação Atual</label>
			<%=Html.DropDownList("SituacaoAtual", Model.Situacoes , ViewModelHelper.SetaDisabled(true , new { @class="text ddlSituacoes"}))%>
	  </div>
		</div>
</fieldset>
	
<% if (Model.Resultados.Count > 0) { %>

<fieldset class="box">
	<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="13%">Data da análise</th>
				<th width="25%">Analista</th>
				<th width="15%">Setor</th>
				<th width="10%">Situação</th>
                <th>Motivo</th>
			</tr>
		</thead> 
		<tbody>
        <% for (int i = 0; i < Model.Resultados.Count; i++) {
                %>
               <tr>
				<td title="<%= Html.Encode(Model.Resultados[i].DataAnalise)%>"><%= Html.Encode(Model.Resultados[i].DataAnalise)%></td>
				<td title="<%= Html.Encode(Model.Resultados[i].Analista)%>"><%= Html.Encode(Model.Resultados[i].Analista)%></td>
				<td title="<%= Html.Encode(Model.Resultados[i].SetorTexto)%>"><%= Html.Encode(Model.Resultados[i].SetorTexto)%></td>
                <td title="<%= Html.Encode(Model.Resultados[i].SituacaoTexto)%>"><%= Html.Encode(Model.Resultados[i].SituacaoTexto)%></td>
				<td title="<%= Html.Encode(Model.Resultados[i].MotivoTexto)%>"><%= Html.Encode(Model.Resultados[i].MotivoTexto)%></td>
			  </tr> 
           <%  } %>
		</tbody>
	</table>
</fieldset>

<% } %>
