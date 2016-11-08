<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrgaoParceiroConveniadoVM>" %>
<br />

<%=Html.Hidden("Orgao_Id", Model.OrgaoParceiroConveniado.Id) %>
<fieldset class="block box fsParceiroConveniado">
    <legend>Parceiro/ Conveniado</legend>
    <div class="block">
        <div class="coluna20 append1">
            <label>Sigla*</label>
            <%=Html.TextBox("Sigla", Model.OrgaoParceiroConveniado.Sigla, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@maxlength="30", @class="txtSigla text"})) %>
        </div>
        <div class="ultima block">
            <label>Nome do órgão*</label>
            <%=Html.TextBox("NomeOrgao", Model.OrgaoParceiroConveniado.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new {@maxlength="80", @class="txtNomeOrgao text"})) %>
        </div>
    </div>
</fieldset>

<fieldset id="fsUnidade" class="block box fsUnidade">
    <legend>Unidade</legend>
    <div class="block">
        <div class="coluna20 append1">
            <label>Sigla</label>
            <%=Html.TextBox("Unidade_Sigla", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@maxlength="30", @class="txtUnidadeSigla text"})) %>
        </div>
        <div class="coluna60 append1">
            <label>Nome/ Local da unidade</label>
            <%=Html.TextBox("Unidade_Nome_Local", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new {@maxlength="80", @class="txtUnidadeNomeLocal text"}) )%>
        </div>

        <div class="coluna10">
			<%if(!Model.IsVisualizar){ %>
            <button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarUnidade btnAddItem" title="Adicionar">+</button>
		    <%} %>
        </div>
    </div>

    <div class="block">
        <table class="dataGridTable tabUnidades" width="100%" border="0" cellspacing="0" cellpadding="0">
            <thead>
			    <tr>
				    <th width="25%">Sigla</th>
					<th width="65%">Nome/ Local da Unidade</th>
					<%if(!Model.IsVisualizar){ %>
                        <th width="10%">Ação</th>
                    <%} %>
				</tr>
			</thead>
			<tbody>
			    <tr class="trItem trUnidade">
	                <%foreach(var item in Model.OrgaoParceiroConveniado.Unidades) {%>
                    <td>
		                <span class="Sigla" title="<%= Html.Encode(item.Sigla) %>"> <%= Html.Encode(item.Sigla)%></span>
	                </td>
	                <td>
		                <span class="NomeLocal" title="<%= Html.Encode(item.Nome) %>"> <%= Html.Encode(item.Nome)%></span>
	                </td>
	                <%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">    
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirUnidade" value="" />
							</td>
						<%} %>
                    </tr>
          </tbody>
            <%} %>
                    <% if(!Model.IsVisualizar) { %>
					    <tr class="trTemplateRow hide">
						    <td><span class="Sigla"></span></td>
							<td><span class="NomeLocal"></span></td>
							<td class="tdAcoes">
								    <input type="hidden" class="hdnItemJSon" value="" />
								    <input title="Excluir" type="button" class="icone excluir btnExcluirUnidade" value="" />
							    </td>
						    </tr>
				   <% } %>
		</table>
    </div>
</fieldset>

<fieldset class="block box fsTermoCooperacaoAdesao">
    <legend>Termo de Cooperação/ Adesão</legend>
    <div class="block">
        <div class="coluna20 append1">
            <label>Número/ Ano</label>
            <%=Html.TextBox("Termo_Cooperacao_Adesao_Numero_Ano", Model.OrgaoParceiroConveniado.TermoNumeroAno, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@maxlength="13", @class="txtTermoCooperacaoNumeroAno text"})) %>
        </div>
        <div class="coluna20 ">
            <label>Diário oficial (data)</label>
            <%=Html.TextBox("Termo_Cooperacao_Adesao_Diario_Oficial_Data", Model.OrgaoParceiroConveniado.DiarioOficialData.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new {@class="txtTermoCooperacaoData text maskData"})) %>
        </div>

    </div>
</fieldset>

