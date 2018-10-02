<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFlorestalVM>" %>

<div class="block">
    <div class="coluna40">
        <label for="Laudo_Caracterizacao">Caracterização *</label><br />
        <%= Html.DropDownList("Laudo.Caracterizacao", Model.Caracterizacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Caracterizacoes.Count <= 1, new { @class = "text ddlCaracterizacoes" }))%>
    </div>
    <div class="coluna20 prepend2">
        <button type="button" class="inlineBotao btnAddCaracterizacao botaoAdicionarIcone" title="Adicionar caracterização">Adicionar</button>
    </div>
</div>

<div class="block">
    <div class="coluna40">
        <div class="dataGrid">
            <table class="dataGridTable ordenavel tabCaracterizacao" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                        <th>Caracterização selecionada</th>
                        <th class="semOrdenacao" width="12%">Ações</th>
                    </tr>
                </thead>
                <tbody>
                    <% foreach (var exploracao in Model.Exploracoes){ %>
                    <tr>
                        <td>
                            <span class="descricao" title="<%:exploracao.ExploracaoFlorestalTexto%>"><%:exploracao.ExploracaoFlorestalTexto%></span>
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
    </div>
</div>
