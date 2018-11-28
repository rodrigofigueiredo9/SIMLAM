<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<div class="containerDesenhador">

    <fieldset class="block box">
        <legend>Empreendimento</legend>
        <div class="block">
            <div class="coluna20 append1">
                <label>Código</label>
                <%= Html.TextBox("EmpreendimentoId", Model.EmpreendimentoCodigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
            </div>
            <div class="coluna77">
                <label><%=Model.DenominadorTexto%> *</label>
                <%= Html.TextBox("DenominadorValor", Model.DenominadorValor, new { @maxlength = "100", @class = "text denominador disabled", @disabled = "disabled" })%>
            </div>
        </div>

        <div class="block">
            <div class="coluna20 append1">
                <label>Zona de localização *</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.EmpreendimentoZonaLocalizacao, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna7 append1">
                <label>UF</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.EmpreendimentoUf, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna45 append1">
                <label>Município</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.EmpreendimentoMunicipio, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna20 ">
                <label>CNPJ</label>
                <%= Html.TextBox("CNPJ", Model.EmpreendimentoCNPJ, new { @maxlength = "100", @class = "text cnpj disabled", @disabled = "disabled" })%>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box">
        <legend>Licença</legend>
        <div class="block">
            <div class="coluna10">
                <label>N.º Licença</label>
                <%= Html.TextBox("NumeroLicenca", Model.NumeroLicena, new { @class = "text numeroLicenca" })%>
            </div>

            <div class="coluna10">
                <label>Atividade</label>
                <%= Html.TextBox("Atividade", Model.Atividade, ViewModelHelper.SetaDisabled(false, new { @class = "text atividade"}))%>
            </div>

            <div class="coluna15">
                <label>Área Licenciada / Plantada (ha)</label>
                <%= Html.TextBox("AreaLicenciada", Model.Atividade, ViewModelHelper.SetaDisabled(false, new { @class = "text areaLicenciada"}))%>
            </div>

            <div class="coluna10">
                <label>Data Vencimento</label>
                <%= Html.TextBox("DataVencimento", Model.Atividade, ViewModelHelper.SetaDisabled(false, new { @class = "text maskData dataVencimento"}))%>
            </div>

            <div class="coluna10">
                <br />
                <input class="btnAdicioinar" type="button" value="Adicionar" />
            </div>
        </div>

        <div class="dataGrid">
            <table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                        <th width="9%">Nº Linceça</th>
                        <th width="9%">Atividade</th>
                        <th width="19%">Área Licenciada/Plantada (ha)</th>
                        <th width="12%">Vencimento</th>
                        <th class="semOrdenacao" width="9%">Ações</th>
                    </tr>
                </thead>

                <tbody>
                    <% foreach (var item in Model.Resultados)
						{ %>
                    <tr>
                        <td title="<%= Html.Encode(item.Numero.Texto)%>"><%= Html.Encode(item.Numero.Texto)%></td>
                        <td title="<%= Html.Encode(item.Modelo.Nome)%>"><%= Html.Encode(item.Modelo.Sigla)%></td>
                        <td title="<%= Html.Encode(item.Protocolo.Numero)%>"><%= Html.Encode(item.Protocolo.Numero)%></td>
                        <td title="<%= Html.Encode(item.EmpreendimentoTexto)%>"><%= Html.Encode(item.EmpreendimentoTexto)%></td>
                        <td title="<%= Html.Encode(item.DataVencimento.DataTexto)%>"><%= Html.Encode(item.DataVencimento.DataTexto)%></td>
                        <td>
                            <input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />


                            <%if (Model.IsPodeExcluir)
								{%><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
                            
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        </div>

    </fieldset>

</div>
