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
                        <th class="semOrdenacao" width="12%">Ações</th>
                    </tr>
                </thead>
                <tbody>
                    <tr class="trTemplateRow hide">
                        <td><span class="descricao" title=""></span></td>
                        <td>
                            <input type="hidden" class="exploracaoId" value="" />
                            <input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>
