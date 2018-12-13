<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RetificacaoNFCaixaVM>" %>


<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
    <div class="filtroCorpo filtroSerializarAjax block">
        <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
        <input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />

        <%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarNFCaixa"), new { @class = "urlFiltrar" })%>
        <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
        <%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

        <div class="ultima">
            <div class="block fixado">
				<div class="coluna50">
					<label for="Filtros_NumeroNFCaixa">N° nota fiscal de caixa</label>
					<%= Html.TextBox("Filtros.NumeroNFCaixa", string.Empty, new { @class = "text setarFoco" })%>
				</div>
				<div class="coluna20">
					<label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.NFCaixaIsCPF", true, true, new { @class = "radio radioCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.NFCaixaIsCPF", false, false, new { @class = "radio radioCpfCnpj" })%>CNPJ</label>
					<%= Html.TextBox("Filtros.NFCaixaCPFCNPJ", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
            </div>
            <div class="block hide">
                <div class="coluna50">
                    <label for="Filtros_NumeroPTV">Número PTV</label>
                    <%= Html.TextBox("Filtros.NumeroPTV", string.Empty, new { @class = "text maskNum15", @maxlength="100" })%>
                </div>
				<div class="coluna40 isPossuiNFCaixa">
					<label for="NotaFiscalApresentacao">Tipo da caixa</label><br />
					<label>
						<%=Html.RadioButton("Filtros.tipoDeCaixa", (int)eTipoNotaFiscalDeCaixa.Madeira, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="1" }))%>
						Madeira
					</label>
					<label>
						<%=Html.RadioButton("Filtros.tipoDeCaixa", (int)eTipoNotaFiscalDeCaixa.Plastico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="2" }))%>
						Plástico
					</label>
					<label>
						<%=Html.RadioButton("Filtros.tipoDeCaixa", (int)eTipoNotaFiscalDeCaixa.Papelao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="3" }))%>
						Papelão
					</label>
				</div>	
            </div>
        </div>
    </div>
</div>

<div class="gridContainer"></div>
