<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RetificacaoNFCaixaEditarVM>" %>

<h1 class="titTela">Editar Nota Fiscal de Caixa</h1>
<br />

<input type="hidden" class="hdnNFCaixaId" value="<%= Model.NotaFiscalDeCaixa.id %>" />
<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
<input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />
<%= Html.Hidden("urlPTVNFCaixaPag", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

<div class="block box">
	<fieldset>
		<legend>Nota Fiscal de Caixa</legend>
        <div class="block">
			<div class="coluna24">
				<label for="Filtros_Numero">N° nota fiscal de caixa</label>
				<%=Html.TextBox("NotaFiscalDeCaixa.notaFiscalCaixaNumero", Model.NotaFiscalDeCaixa.notaFiscalCaixaNumero , ViewModelHelper.SetaDisabled(true , new { @class="text"}))%>
			</div>
			<div class="coluna24">
				<label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.DUAIsCPF", true, (int)Model.NotaFiscalDeCaixa.PessoaAssociadaTipo == 1, ViewModelHelper.SetaDisabled(true,  new { @class = "radio radioCpfCnpj radioCPF" }))%>CPF</label>
				<label for="Filtros_DUACPFCNPJ"><%= Html.RadioButton("Filtros.DUAIsCPF", true, (int)Model.NotaFiscalDeCaixa.PessoaAssociadaTipo == 2, ViewModelHelper.SetaDisabled(true,  new { @class = "radio radioCpfCnpj" }))%>CNPJ</label>
				<%= Html.TextBox("NotaFiscalDeCaixa.PessoaAssociadaCpfCnpj", Model.NotaFiscalDeCaixa.PessoaAssociadaCpfCnpj, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCpfCnpj maskCpfParcial" }))%>
			</div>
			<div class="coluna40 isPossuiNFCaixa">
				<label for="NotaFiscalApresentacao">Tipo da caixa</label><br />
				<label>
					<%=Html.RadioButton("NotaFiscalDeCaixa.tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Madeira, Model.NotaFiscalDeCaixa.tipoCaixaId == 1, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="1" }))%>
					Madeira
				</label>
				<label>
					<%=Html.RadioButton("NotaFiscalDeCaixa.tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Plastico, Model.NotaFiscalDeCaixa.tipoCaixaId == 2, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="2" }))%>
					Plástico
				</label>
				<label>
					<%=Html.RadioButton("NotaFiscalDeCaixa.tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Papelao,Model.NotaFiscalDeCaixa.tipoCaixaId == 3, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="3" }))%>
					Papelão
				</label>
			</div>	
        </div>
        <div class="block">
            <div class="coluna24">
                <label for="Filtros_Nome">Saldo inicial</label>
				<%=Html.TextBox("NotaFiscalDeCaixa.saldoInicial", Model.NotaFiscalDeCaixa.saldoInicial , ViewModelHelper.SetaDisabled(true , new { @class="text"}))%>
            </div>
            <div class="coluna24">
                <label for="Filtros_Nome">Saldo atual</label>
				<%=Html.TextBox("NotaFiscalDeCaixa.saldoAtual", Model.NotaFiscalDeCaixa.saldoAtual , ViewModelHelper.SetaDisabled(true , new { @class="text"}))%>
            </div>
            <div class="coluna40">
                <label for="Filtros_Nome">Valor a retificar (<i>para diminuir o saldo, use número negativos</i>)</label>
				<%= Html.TextBox("SaldoRetificado", null, new { @class = "text novoSaldo setarFoco maskNumNegative" , @maxlength = "8" })%>
            </div>
			
        </div>
	</fieldset>

	<div class="block box">
		<fieldset>
			<legend>PTVs</legend>
			<% Html.RenderPartial("Paginacao", Model.Paginacao); %>
			<%Html.RenderPartial("RetificacaoNFCaixaEditarListar", Model.ResultadosPTV); %>
		</fieldset>
	</div>
</div>
