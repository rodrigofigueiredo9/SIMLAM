<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CobrancaVM>" %>

<input type="hidden" class="hdnCobrancaId" value="<%:Model.Entidade.Id %>" />
<input type="hidden" class="hdnVisualizar" value="<%:Model.IsVisualizar %>" />

<fieldset class="block box">
    <div class="block">
        <div class="coluna15">
            <label>N° de Registro do Processo</label><br />
            <%= Html.TextBox("Cobranca.ProcessoNumero", Model.Entidade.ProcessoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0, new { @class = "text txtProcessoNumero setarFoco" }))%>
        </div>
        <div class="coluna15">
            <label>N° Autuação (SEP)</label><br />
            <%= Html.TextBox("Cobranca.NumeroAutuacao", Model.Entidade.NumeroAutuacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0, new { @class = "text txtNumeroAutuacao" }))%>
        </div>
        <div class="coluna15">
            <label>N° Fiscalização</label><br />
            <%= Html.TextBox("Cobranca.NumeroFiscalizacao", Model.Entidade.NumeroFiscalizacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0, new { @class = "text txtFiscalizacao" }))%>
        </div>
        <div class="coluna15">
            <label>N° AI / IUF *</label><br />
            <%= Html.TextBox("Cobranca.NumeroIUF", Model.Entidade.NumeroIUF, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0, new { @class = "text txtNumeroIUF" }))%>
        </div>
        <div class="coluna15">
            <label>Série</label><br />
			<%= Html.DropDownList("Cobranca.Serie", Model.Series, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0), new { @class = "text ddlSeries" }))%>
        </div>
        <div class="coluna15">
            <label>Data Emissão AI / IUF *</label><br />
            <%= Html.TextBox("Cobranca.DataEmissaoIUF", Model.Entidade.DataEmissaoIUF.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.NumeroFiscalizacao > 0, new { @class = "text txtDataEmissaoIUF maskData" }))%>
        </div>
        <div class="coluna31">
            <label>Nome / Razão Social autuado *</label><br />
            <%= Html.TextBox("Cobranca.AutuadoPessoa.NomeRazaoSocial", Model.Entidade.AutuadoPessoa != null ? Model.Entidade.AutuadoPessoa.NomeRazaoSocial : "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtAutuadoNome" }))%>
            <%= Html.Hidden("hdnAutuadoPessoaId", Model.Entidade.AutuadoPessoaId, new { @class = "hdnAutuadoPessoaId" })%>
        </div>
        <div class="coluna15">
            <label>CPF/CNPJ autuado *</label><br />
            <%= Html.TextBox("Cobranca.AutuadoPessoa.CPFCNPJ", Model.Entidade.AutuadoPessoa != null ? Model.Entidade.AutuadoPessoa.CPFCNPJ : "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtAutuadoCpfCnpj" }))%>
        </div>
        <div class="prepend2">
            <% if (!Model.IsVisualizar && Model.Entidade.NumeroFiscalizacao == 0)
				{ %>
            <button type="button" title="Buscar" class="floatLeft inlineBotao botaoBuscar btnAssociarAutuado">Buscar</button>
            <% } %>
            <span class="spanVisualizarAutuado <%= (Model.Entidade.AutuadoPessoaId > 0) ? "" : "hide" %>">
                <button type="button" class="icone visualizar esquerda inlineBotao btnEditarAutuado" title="Visualizar autuado"></button>
            </span>
        </div>
    </div>
    <div class="block">
        <div class="coluna31">
            <label for="Cobranca_CodigoReceita">Código Receita *</label>
            <%= Html.DropDownList("Cobranca.CodigoReceita", Model.CodigoReceita, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.CodigoReceitaId > 0, new { @class = "text ddlCodigoReceita", @style = "height:21px;" }))%>
        </div>

        <div class="coluna15">
            <label for="Cobranca_ValorMulta">Valor Multa (R$) *</label>
            <%= Html.TextBox("Cobranca.ValorMulta", String.Format("{0:N2}",  Model.Parcelamento.ValorMulta), ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Parcelamento.ValorMulta > 0, new { @class = "text maskDecimalPonto2 txtValorMulta", @maxlength = "17"}))%>
        </div>

        <div class="coluna31">
            <label for="Cobranca_SituacaoFiscalizacao">Situação Fiscalização</label>
            <%= Html.DropDownList("Cobranca.SituacaoFiscalizacao", Model.SituacaoFiscalizacao, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSituacaoFiscalizacao", @style = "height:21px;" }))%>
        </div>
    </div>

    <div class="block">
        <div class="coluna15">
            <label for="Cobranca_DataIUF">Data da Notificação IUF *</label><br />
            <%= Html.TextBox("Cobranca.DataIUF", Model.Entidade.DataIUF.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || (Model.Entidade.Notificacao == null ? false : Model.Entidade.Notificacao.Id > 0), new { @class = "text maskData txtDataIUF", @maxlength = "100" }))%>
        </div>
        <div class="coluna15">
            <label for="Cobranca_DataJIAPI">Data da Notificação JIAPI</label><br />
            <%= Html.TextBox("Cobranca.DataJIAPI", Model.Entidade.DataJIAPI.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || (Model.Entidade.Notificacao == null ? false : Model.Entidade.Notificacao.Id > 0), new { @class = "text maskData txtDataJIAPI", @maxlength = "100" }))%>
        </div>
        <div class="coluna15">
            <label for="Cobranca_DataCORE">Data da Notificação CORE</label><br />
            <%= Html.TextBox("Cobranca.DataCORE", Model.Entidade.DataCORE.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || (Model.Entidade.Notificacao == null ? false : Model.Entidade.Notificacao.Id > 0), new { @class = "text maskData txtDataCORE", @maxlength = "100" }))%>
        </div>
    </div>

    <div class="block">

        <div class="coluna15">
            <label for="Cobranca_Data1Vencimento">Data 1º Vencimento</label><br />
            <%= Html.TextBox("Cobranca.Data1Vencimento", Model.Parcelamento.Data1Vencimento.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Parcelamento.DUAS.FindAll(x => x.Id > 0).Count > 0, new { @class = "text maskData txtData1Vencimento", @maxlength = "100" }))%>
        </div>

        <div class="coluna15">
            <label for="Cobranca_Parcelas">Parcelas *</label><br />
            <div class="coluna20 append1" >
                <input class="icone refresh btnAtualizar"  type="button" <%=  Model.IsVisualizar || Model.Parcelamento.DUAS.FindAll(x => x.Id > 0).Count > 0 ? "disabled" : ""%>/>
            </div>
            <div class="coluna77">
                <%= Html.DropDownList("Cobranca.Parcelas", Model.Parcelas, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Parcelamento.DUAS.FindAll(x => x.Id > 0).Count > 0, new { @class = "text ddlParcelas", @style = "height:21px;" }))%>
            </div>
        </div>

        <div class="coluna15">
            <label for="Cobranca_ValorMultaAtualizado"><%= Model.Parcelamento.ValorMultaAtualizado < Model.Parcelamento.ValorMulta && Model.Entidade.Parcelamentos.Count == 1 ? "Valor Multa Com Desconto (R$)" : "Valor Multa Atualizado (R$)"%></label>
            <%= Html.TextBox("Cobranca.ValorMultaAtualizado", String.Format("{0:N2}", Model.Parcelamento.ValorMultaAtualizado), ViewModelHelper.SetaDisabled(true, new { @class = "text maskDecimalPonto2 txtValorMultaAtualizado", @maxlength = "17"}))%>
        </div>

        <div class="coluna15">
            <label for="Cobranca_DataEmissao">Data Emissão</label><br />
            <%= Html.TextBox("Cobranca.DataEmissao", Model.Parcelamento.DataEmissao.IsValido ? Model.Parcelamento.DataEmissao.DataTexto : DateTime.Now.ToString("dd/MM/yyyy"), ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Entidade.Id > 0, new { @class = "text maskData txtDataEmissao", @maxlength = "100" }))%>
        </div>
    </div>

    <div class="block DivParcelas">
        <% Html.RenderPartial("CobrancaParcelamento", Model); %>
        <input type="hidden" class="hdnParcelamento" value='<%: ViewModelHelper.Json(Model.Parcelamento)%>' />
    </div>

    <%if (Model.IsVisualizar)
		{%>
    <div class="block">
        <% var indexParcelamento = Model.Entidade.Parcelamentos.IndexOf(Model.Parcelamento); %>
        <div class="coluna2">
            <input class="icone floatLeft setaEsquerda btnParcelamentoAnterior" type="button" value="" <%= indexParcelamento <= 0 ? "disabled" : "" %> />
        </div>
        <div class="coluna9">
            Parcelamento <%= indexParcelamento + 1 %>/<%= Model.Entidade.Parcelamentos.Count%>
        </div>
        <input class="icone floatLeft setaDireita btnParcelamentoPosterior" type="button" value="" <%= indexParcelamento == (Model.Entidade.Parcelamentos.Count - 1) || indexParcelamento < 0 ? "disabled" : "" %> />
        <input type="hidden" class="hdnIndexParcelamento" value="<%= indexParcelamento %>" />
    </div>
    <%} %>
</fieldset>

<fieldset class="block box fsArquivos">
    <legend style="color: #000000"><b>Cópia de Notificação</b></legend>
    <% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>

<div class="block box">
    <%if (Model.IsVisualizar)
		{%>
    <input class="floatLeft btnEditar" type="button" value="Editar Cobrança" />
    <%}
		else
		{ %>
    <div class="coluna7 append1">
        <input class="floatLeft btnRecalcular" type="button" value="Recalcular" />
    </div>
    <div class="coluna5 append1">
        <input class="floatLeft btnSalvar" type="button" value="Salvar" />
    </div>
    <input class="floatLeft btnNovoParcelamento" type="button" value="Novo Parcelamento" />
    <%} %>
    <span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="#">Cancelar</a></span>
</div>
