<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFlorestalVM>" %>

<div class="dataGrid">
	<table class="dataGridTable ordenavel tabCaracterizacao" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Caracterização selecionada</th>
				<th class="semOrdenacao" width="12%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var exploracao in Model.ExploracaoFlorestal){ %>
            <tr>
                <td>
                    <span class="descricao" title="<%:exploracao.CodigoExploracaoTexto%>"><%:exploracao.CodigoExploracaoTexto%></span>
                </td>
                <td>
					<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(exploracao)%>' /> 
					<input type="hidden" value="<%= exploracao.Id %>" class="exploracaoId" /> 
					<input type="hidden" class="parecerFavoravel" value="" /> 
					<input type="hidden" class="parecerDesfavoravel" value="" /> 
                    <input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao" />
                </td>
            </tr>
			<% } %>
			<tr class="trTemplateRow hide"> 
                <td><span class="descricao" title=""></span></td>
                <td>
					<input type="hidden" class="hdnItemJSon" value="" /> 
					<input type="hidden" class="exploracaoId" value="" /> 
					<input type="hidden" class="parecerFavoravel" value="" /> 
					<input type="hidden" class="parecerDesfavoravel" value="" /> 
                    <input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao" />
                </td>
			</tr>
		</tbody>
	</table>
</div>