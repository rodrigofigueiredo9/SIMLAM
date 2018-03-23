<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCobrancasVM>" %>

<h1 class="titTela">Controle de Cobrança</h1>
<br />

<%= Html.Hidden("UrlFiltrar", Url.Action("CobrancaFiltrar"), new { @class = "urlFiltrar" })%>

<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
    <div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "4", new { @class = "ordenarPor" })%>

        <div class="coluna98">
            <div class="block fixado">
                <div class="coluna15 append2">
                    <label for="Filtros_CodigoReceitaTexto">Nº de Registro do Processo</label>
                    <%= Html.TextBox("Filtros.NumeroRegistroProcesso", Model.Filtros.NumeroRegistroProcesso, new { @class = "text txtNumeroConfig setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="Filtros_CodigoReceitaTexto">Nº da Autuação (SEP)</label>
                    <%= Html.TextBox("Filtros.NumeroAutuacao", Model.Filtros.NumeroAutuacao, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Nº Fiscalização</label>
                    <%= Html.TextBox("Filtros.NumeroFiscalizacao", Model.Filtros.NumeroFiscalizacao, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Nº AI/IUF</label>
                    <%= Html.TextBox("Filtros.NumeroAIIUF", Model.Filtros.NumeroAIIUF, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="Filtros_SituacaoFiscalizacao">Situacao da Fiscalizacao</label>
                    <%= Html.DropDownList("Filtros.SituacaoFiscalizacao", Model.SituacaoFiscalizacao, new { @class = "text ddlClassificacao" })%>
                </div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
            </div>

            <div class="block hide">

                <div class="coluna15 append2">
                    <label for="NumeroAIIUF">Numero DUA</label>
                    <%= Html.TextBox("Filtros.NumeroDUA", Model.Filtros.NumeroDUA, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

				 <div class="coluna13 append2">
                    <label for="Filtros_SituacaoDUA">Situacao da DUA</label>
                    <%= Html.DropDownList("Filtros.SituacaoDUA", Model.SituacaoDUA, new { @class = "text ddlClassificacao" })%>
                </div>

               <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Data de Pagamento de:</label>
                    <%= Html.TextBox("Filtros.DataPagamentoDe", Model.Filtros.DataPagamentoDe, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Ata de Pagamento até:</label>
                    <%= Html.TextBox("Filtros.DataPagamentoAte", Model.Filtros.DataPagamentoAte, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Data de Vencimento de:</label>
                    <%= Html.TextBox("Filtros.DataVencimentoDe", Model.Filtros.DataVencimentoDe, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna13 append2">
                    <label for="NumeroAIIUF">Data de Vencimento até:</label>
                    <%= Html.TextBox("Filtros.DataVencimentoAte", Model.Filtros.DataVencimentoAte, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>
            </div>

            <div class="block hide">
                <div class="coluna23 append2">
                    <label for="NumeroAIIUF">Nome/Razão social do autuado:</label>
                    <%= Html.TextBox("Filtros.DataVencimentoAte", Model.Filtros.DataVencimentoAte, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>

                <div class="coluna23 append2">
                    <label for="NumeroAIIUF">CPF/CNPJ:</label>
                    <%= Html.TextBox("Filtros.DataVencimentoAte", Model.Filtros.DataVencimentoAte, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
                </div>
            </div>
        </div>

    </div>
</div>

<div class="gridContainer"></div>
