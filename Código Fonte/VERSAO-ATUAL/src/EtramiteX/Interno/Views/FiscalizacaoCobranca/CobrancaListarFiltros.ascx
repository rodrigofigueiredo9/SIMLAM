<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCobrancasVM>" %>

<h1 class="titTela">Controle de Cobrança</h1>
<br />

<%= Html.Hidden("UrlFiltrar", Url.Action("CobrancaFiltrar"), new { @class = "urlFiltrar" })%>

<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
    <div class="filtroCorpo filtroSerializarAjax block">

        <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
        <%= Html.Hidden("UrlFiltrar", Url.Action("CobrancaFiltrar"), new { @class = "urlFiltrar" })%>
        <%= Html.Hidden("UrlVisualizar", Url.Action("CobrancaVisualizar"), new { @class = "urlVisualizar" })%>
        <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
        <%= Html.Hidden("Paginacao.OrdenarPor", "0", new { @class = "ordenarPor" })%>

        <div class="coluna98">
            <div class="block fixado">
                <div class="coluna15 append1">
                    <label for="Filtros_CodigoReceitaTexto">Nº de Registro do Processo</label>
                    <%= Html.TextBox("Filtros.NumeroRegistroProcesso", Model.Filtros.NumeroRegistroProcesso, new { @class = "text txtNumeroRegistroProcesso setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <label for="Filtros_CodigoReceitaTexto">Nº da Autuação (SEP)</label>
                    <%= Html.TextBox("Filtros.NumeroAutuacao", Model.Filtros.NumeroAutuacao, new { @class = "text txtNumeroAutuacao maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <label for="Filtros_NumeroFiscalizacao">Nº Fiscalização</label>
                    <%= Html.TextBox("Filtros.NumeroFiscalizacao", Model.Filtros.NumeroFiscalizacao, new { @class = "text txtNumeroFiscalizacao maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <label for="Filtros_NumeroAIIUF">Nº AI/IUF</label>
                    <%= Html.TextBox("Filtros.NumeroAIIUF", Model.Filtros.NumeroAIIUF, new { @class = "text txtNumeroAIIUF maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <label for="Filtros_SituacaoFiscalizacao">Situacao da Fiscalizacao</label>
                    <%= Html.DropDownList("Filtros.SituacaoFiscalizacao", Model.SituacaoFiscalizacao, new { @class = "text ddlSituacaoFiscalizacao" })%>
                </div>

                <div class="coluna10">
                    <button class="inlineBotao btnBuscar">Buscar</button>
                </div>
            </div>

            <div class="block hide">

                <div class="coluna15 append1">
                    <label for="Filtros_NumeroDUA">Numero DUA</label>
                    <%= Html.TextBox("Filtros.NumeroDUA", Model.Filtros.NumeroDUA, new { @class = "text txtNumeroDUA maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <label for="Filtros_SituacaoCobranca">Situacao Cobrança</label>
                    <%= Html.DropDownList("Filtros.SituacaoCobranca", Model.SituacaoCobranca, new { @class = "text ddlSituacaoCobranca" })%>
                </div>

                <div class="coluna28 append1">
					<label for="Filtros_DataPagamentoDe_DataTexto">Data de Pagamento (de-até)</label>
                </div>
                <div class="coluna23 append1">
					<label for="Filtros_DataVencimentoDe_DataTexto">Data de Vencimento (de-ate)</label>
                </div><br />
                <div class="coluna13 append1">
                    <%= Html.TextBox("Filtros.DataPagamentoDe.DataTexto", Model.Filtros.DataPagamentoDe.DataTexto, new { @class = "text txtDataPagamentoDe maskData setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <%= Html.TextBox("Filtros.DataPagamentoAte.DataTexto", Model.Filtros.DataPagamentoAte.DataTexto, new { @class = "text txtDataPagamentoAte maskData setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <%= Html.TextBox("Filtros.DataVencimentoDe.DataTexto", Model.Filtros.DataVencimentoDe.DataTexto, new { @class = "text txtDataVencimentoDe maskData setarFoco" })%>
                </div>

                <div class="coluna13 append1">
                    <%= Html.TextBox("Filtros.DataVencimentoAte.DataTexto", Model.Filtros.DataVencimentoAte.DataTexto, new { @class = "text txtDataVencimentoAte maskData setarFoco" })%>
                </div>
            </div>

            <div class="block hide">
                <div class="coluna61">
                    <label for="Filtros_NomeRazaoSocial">Nome/Razão social do autuado:</label>
                    <%= Html.TextBox("Filtros.NomeRazaoSocial", Model.Filtros.NomeRazaoSocial, new { @class = "text txtNomeRazaoSocial setarFoco", @maxlength = "100" })%>
                </div>

                <div class="coluna27">
                    <label for="Filtros_CPFCNPJ">CPF/CNPJ:</label>
					<label for="Filtros_CPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioAutuadoCpfCnpj radioCPF" })%>CPF autuado</label>
					<label for="Filtros_CPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioAutuadoCpfCnpj" })%>CNPJ autuado</label>
                    <%= Html.TextBox("Filtros.CPFCNPJ", Model.Filtros.CPFCNPJ, new { @class = "text txtCpfCnpj maskCpfParcial setarFoco" })%>
                </div>
            </div>
        </div>

    </div>
</div>

<div class="gridContainer"></div>
