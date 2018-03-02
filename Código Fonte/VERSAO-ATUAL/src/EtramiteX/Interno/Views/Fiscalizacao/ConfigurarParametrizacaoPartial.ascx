<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ParametrizacaoVM>" %>

<input type="hidden" class="hdnParametrizacaoId" value="<%:Model.Entidade.Id %>" />

<fieldset class="box">
    <div class="block">
        <div class="coluna45 append2">
            <label for="Parametrizacao_CodigoReceita">Código Receita *</label>
            <%= Html.DropDownList("Parametrizacao.CodigoReceitaId", Model.CodigoReceita, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCodigoReceita", @style = "height:21px;" }))%>
        </div>
        <div class="coluna45 append2">
            <label for="Parametrizacao_InicioVigencia">Período Vigência (de - até) *</label><br />
            <div class="coluna25 append2">
                <%= Html.TextBox("Parametrizacao.InicioVigencia", Model.Entidade.InicioVigencia.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtInicioVigencia", @maxlength = "100" }))%>
            </div>
            <div class="coluna25 append2">
                <%= Html.TextBox("Parametrizacao.FimVigencia", Model.Entidade.FimVigencia.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtFimVigencia", @maxlength = "100" }))%>
            </div>
        </div>
    </div>

    <div class="block">
        <div class="coluna45 append2">
            <div class="coluna31 append2">
                <label for="Parametrizacao_MaximoParcelas">N° Máximo Parcelas *</label>
                <%= Html.TextBox("Parametrizacao.MaximoParcelas", Model.Entidade.MaximoParcelas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum2 txtMaximoParcelas" }))%>
            </div>

            <div class="coluna31 append2">
                <label for="Parametrizacao_ValorMinimoPF">Valo Mínimo PF (VRTE) *</label>
                <%= Html.TextBox("Parametrizacao.ValorMinimoPF", Model.Entidade.ValorMinimoPF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum5 txtValorMinimoPF" }))%>
            </div>

            <div class="coluna31">
                <label for="Parametrizacao_ValorMinimoPJ">Valo Mínimo PJ (VRTE) *</label>
                <%= Html.TextBox("Parametrizacao.ValorMinimoPJ", Model.Entidade.ValorMinimoPJ, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum5 txtValorMinimoPJ" }))%>
            </div>
        </div>

        <div class="coluna45 append2">
            <div class="coluna25 append2">
                <label for="Parametrizacao_MultaPercentual">Multa (%)</label>
                <%= Html.TextBox("Parametrizacao.MultaPercentual", Model.Entidade.MultaPercentual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtMulta" }))%>
            </div>

            <div class="coluna25 append2">
                <label for="Parametrizacao_JurosPercentual">Juros a.m (%)</label>
                <%= Html.TextBox("Parametrizacao.JurosPercentual", Model.Entidade.JurosPercentual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtJuros" }))%>
            </div>
        </div>
    </div>
    <div class="block">

        <div class="coluna45 append3">
            <div class="coluna31 append2">
                <label for="Parametrizacao_DescontoPercentual">Desconto (%)</label>
                <%= Html.TextBox("Parametrizacao.DescontoPercentual", Model.Entidade.DescontoPercentual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtDesconto" }))%>
            </div>

            <div class="coluna45 append2">
                <label for="Parametrizacao_PrazoDescontoUnidade">Prazo p/ Desconto</label><br />
                <div class="coluna25 append2">
                    <%= Html.TextBox("Parametrizacao.PrazoDescontoUnidade", Model.Entidade.PrazoDescontoUnidade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtPrazoDescontoUnidade" }))%>
                </div>
                <div class="coluna42 append2">
                    <%= Html.DropDownList("Parametrizacao.PrazoDescontoDecorrencia", Model.Decorrencia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlPrazoDescontoDecorrencia", @style = "height:21px;" }))%>
                </div>
            </div>
        </div>
    </div>

</fieldset>

<div class="block box">
    <%if (Model.IsVisualizar)
		{%>
    <span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("ConfigurarParametrizacaoListar", "Fiscalizacao") %>">Cancelar</a></span>
    <%}
		else
		{ %>
    <input class="floatLeft btnSalvar" type="button" value="Salvar" />
    <span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("ConfigurarParametrizacaoListar", "Fiscalizacao") %>">Cancelar</a></span>
    <%} %>
</div>
