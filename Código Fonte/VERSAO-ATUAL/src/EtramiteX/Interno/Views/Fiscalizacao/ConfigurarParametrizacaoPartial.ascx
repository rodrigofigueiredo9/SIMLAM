<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ParametrizacaoVM>" %>

<input type="hidden" class="hdnParametrizacaoId" value="<%:Model.Entidade.Id %>" />

<fieldset class="box">
    <div class="block">
        <div class="coluna29">
            <label for="Parametrizacao_CodigoReceita">Código Receita *</label>
            <%= Html.DropDownList("Parametrizacao.CodigoReceitaId", Model.CodigoReceita, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCodigoReceita", @style = "height:21px;" }))%>
        </div>
        <div class="coluna44">
            <label for="Parametrizacao_InicioVigencia">Período Vigência (de - até) *</label><br />
            <div class="coluna25 append1">
                <%= Html.TextBox("Parametrizacao.InicioVigencia", Model.Entidade.InicioVigencia.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtInicioVigencia", @maxlength = "100" }))%>
            </div>
            <div class="coluna25">
                <%= Html.TextBox("Parametrizacao.FimVigencia", Model.Entidade.FimVigencia.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtFimVigencia", @maxlength = "100" }))%>
            </div>
        </div>
    </div>

    <div class="block">
        <div class="coluna14">
            <label for="Parametrizacao_ValorMinimoPF">Valor Mínimo PF (VRTE) *</label>
            <%= Html.TextBox("Parametrizacao.ValorMinimoPF", Model.Entidade.ValorMinimoPF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum5 txtValorMinimoPF" }))%>
        </div>

        <div class="coluna14">
            <label for="Parametrizacao_ValorMinimoPJ">Valor Mínimo PJ (VRTE) *</label>
            <%= Html.TextBox("Parametrizacao.ValorMinimoPJ", Model.Entidade.ValorMinimoPJ, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum5 txtValorMinimoPJ" }))%>
        </div>

        <div class="coluna11">
            <label for="Parametrizacao_MultaPercentual">Multa (%)</label>
            <%= Html.TextBox("Parametrizacao.MultaPercentual", Model.Entidade.MultaPercentual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtMulta" }))%>
        </div>

        <div class="coluna11">
            <label for="Parametrizacao_JurosPercentual">Juros a.m (%)</label>
            <%= Html.TextBox("Parametrizacao.JurosPercentual", Model.Entidade.JurosPercentual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNum3 txtJuros" }))%>
        </div>
    </div>
    <div class="block">
        <div class="coluna45 append3">
            <div class="coluna31 append1">
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
    <fieldset class="block box">
        <legend><b>Detalhe</b></legend>
        <div class="block">
			<% if (!Model.IsVisualizar) {%>
				<div class="coluna12 append1">
					<label for="Parametrizacao_ValorInicial">Valor Inicial (R$)</label>
					<%= Html.TextBox("ValorInicial", String.Empty, new { @class = "text maskDecimalPonto2 txtValorInicial", @maxlength = "17" })%>
				</div>

				<div class="coluna12 append1">
					<label for="Parametrizacao_ValorFinal">Valor Final (R$)</label>
					<%= Html.TextBox("ValorFinal", String.Empty, new { @class = "text maskDecimalPonto2 txtValorFinal", @maxlength = "17" })%>
				</div>

				<div class="coluna12 append1">
					<label for="Parametrizacao_MaximoParcelas">N° Máx. Parcelas</label>
					<%= Html.TextBox("MaximoParcelas", String.Empty, new { @class = "text maskInteger txtMaximoParcelas", @maxlength = "3" })%>
				</div>

				<div class="coluna7">
					<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAdicionarDetalhe" title="Adicionar Detalhe">+</button>
				</div>
			<%} %>
            <input type="hidden" class="hdnDetalheId" value='0' />
        </div>

        <div class="DivItens">
            <% Html.RenderPartial("ConfigurarParametrizacaoDetalhe"); %>
        </div>
    </fieldset>
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
