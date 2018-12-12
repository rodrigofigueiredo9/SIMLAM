<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Autorizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AutorizacaoExploracaoFlorestalVM>" %>

<div class="block">
    <div class="coluna40">
        <label for="Autorizacao_Exploracao">Código da Exploração *</label><br />
        <%= Html.DropDownList("Autorizacao.Exploracao", Model.Exploracoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Exploracoes.Count <= 1, new { @class = "text ddlExploracoes" }))%>
    </div>
</div>

<div class="block">
    <div class="coluna40">
        <div class="dataGrid">
            <table class="dataGridTable ordenavel tabExploracoes" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                        <th>Caracterização favorável da Exploração</th>
                        <% if (!Model.IsVisualizar) { %><th class="semOrdenacao" width="12%">Ações</th><%} %>
                    </tr>
                </thead>
                <tbody>
                    <tr class="trTemplateRow hide">
                        <td><span class="descricao" title=""></span>
                            <input type="hidden" class="autorizacaoSinaflorId" name="autorizacaoSinaflorId" value="" />
                            <input type="hidden" class="hdnId" name="hdnId" value="" />
							<input type="hidden" class="exploracaoId" name="exploracaoId" value="" />
						</td>
						<% if (!Model.IsVisualizar) { %><td><input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao" /></td> <%} %>
                    </tr>
					 <% foreach (var exploracao in Model.TituloExploracaoDetalhes) { %>
                    <tr>
                        <td>
                            <span class="descricao" title="<%:exploracao.ExploracaoFlorestalExploracaoTexto%>"><%:exploracao.ExploracaoFlorestalExploracaoTexto%></span>
                            <input type="hidden" class="exploracaoId" name="exploracaoId" value="<%= exploracao.ExploracaoFlorestalExploracaoId %>" />
                            <input type="hidden" class="autorizacaoSinaflorId" name="autorizacaoSinaflorId" value="<%= exploracao.AutorizacaoSinaflorId %>" />
                            <input type="hidden" class="hdnId" name="hdnId" value="<%= exploracao.Id %>" />
                        </td>
                        <% if (!Model.IsVisualizar) { %><td><input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao" /></td><%} %>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        </div>
    </div>
</div>
