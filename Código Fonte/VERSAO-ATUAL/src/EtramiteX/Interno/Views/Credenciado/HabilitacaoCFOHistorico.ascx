<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HistoricoEmissaoCFOCFOCVM>" %>

<%--<script>
	$.extend(HabilitacaoCFOAlterarSituacao.settings, {
		urls: {
			alterarSituacao: '<%= Url.Action("AlterarSituacaoHabilitacaoCFO", "Credenciado") %>'
		},
	    situacaoMotivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Inativo %>',
	    situacaoMotivoAtivo: '<%= (int)eHabilitacaoCFOCFOCSituacao.Ativo %>',
	    motivoSuspenso: '<%= (int)eHabilitacaoCFOCFOCMotivo.Suspensao %>',
	    motivoDescredenciado: '<%= (int)eHabilitacaoCFOCFOCMotivo.Descredenciamento %>'
	});
</script>--%>

<h1 class="titTela">Histórico de Habilitações para Emissão de CFO e CFOC</h1>
<br />

<%= Html.Hidden("HabilitarEmissao.Id", Model.Id, new { @class = "hdnHabilitacaoId" })%>
<fieldset class="block box fsResponsavelTecnico">
    <legend>Responsável Técnico</legend>
	<div class="coluna30">
		<label for="HistoricoEmissao.Nome">Nome</label>
		<%= Html.TextBox("HistoricoEmissao.Nome", Model.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
	</div>

    <div class="coluna30">
		<label for="HistoricoEmissao.Nome">Nº da habilitação</label>
		<%= Html.TextBox("HistoricoEmissao.NumeroHabilitacao", Model.NumeroHabilitacao, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
	</div>

</fieldset>

<fieldset class="block box fsHistorico">
    <legend>Histórico de Habilitações</legend>

    <%--<input type="hidden" class="paginaAtual" value="" />
    <input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

    <div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	    <% Html.RenderPartial("Paginacao", Model.Paginacao); %>--%>

	    <table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
		    <thead>
			    <tr>
				    <th width="20%">Situação</th>
				    <th>Motivo</th>
				    <th width="20%">Data da Situação</th>
                    <th width="20%">Nº do Processo</th>
				    <th width="15%">Data do Histórico</th>
			    </tr>
		    </thead>
		    <tbody>
		    <% foreach (var item in Model.ListaHistoricoHabilitacao) { %>
			    <tr>
				    <td class="situacao" title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				    <td class="motivo" title="<%= Html.Encode(item.MotivoTexto)%>"><%= Html.Encode(item.MotivoTexto)%></td>
				    <td class="dataPenalidade" title="<%= item.SituacaoData%>"><%= item.SituacaoData%></td>
                    <td class="numeroProcesso" title="<%= item.NumeroProcesso%>"><%= item.NumeroProcesso%></td>
                    <td class="dataHistorico" title="<%= item.HistoricoData%>"><%= item.HistoricoData%></td>
			    </tr>
		    <% } %>
		    </tbody>
	    </table>
    </div>
</fieldset>