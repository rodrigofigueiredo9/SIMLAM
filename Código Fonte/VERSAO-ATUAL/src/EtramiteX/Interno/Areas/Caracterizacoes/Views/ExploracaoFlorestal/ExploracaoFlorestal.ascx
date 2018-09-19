<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ExploracaoFlorestalListVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	ExploracaoFlorestal.settings.dependencias = '<%= ViewModelHelper.Json(Model.Dependencias) %>';
	ExploracaoFlorestal.settings.textoAbrirModal = '<%= Model.TextoAbrirModal %>';
	ExploracaoFlorestal.settings.textoMerge = '<%= Model.TextoMerge %>';
	ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
	ExploracaoFlorestal.settings.mensagens = <%=Model.Mensagens%>;
</script>

<%foreach (var exploracao in Model.ExploracaoFlorestalVM) { %>
<div class="block box expp<%= exploracao.CodigoExploracao %>">
    <input type="hidden" class="hdnEmpreendimentoId" value="<%: exploracao.Caracterizacao.EmpreendimentoId%>" />
    <input type="hidden" class="hdnCaracterizacaoId" value="<%: exploracao.Caracterizacao.Id %>" />
    <input type="hidden" class="hdnCodigoExploracao" value="<%: exploracao.Caracterizacao.CodigoExploracao %>" />

    <input type="hidden" class="hdnCodigoExploracaoAnterior" value="<%: exploracao.Caracterizacao.CodigoExploracao %>" />
    <input type="hidden" class="hdnTipoExploracaoAnterior" value="<%: exploracao.Caracterizacao.TipoAtividade %>" />

    <fieldset class="block boxBranca localizador">
        <legend class="titLocalizador">Localizador</legend>
        <div class="block">
            <div class="coluna22 append2">
                <label for="CodigoExploracao">Código Exploração</label>
                <%= Html.TextBox("CodigoExploracao", exploracao.Caracterizacao.CodigoExploracaoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCodigoExploracao" }))%>
            </div>
            <div class="coluna22 append2">
                <label for="TipoExploracao">Tipo de Exploração *</label>
                <%= Html.DropDownList("TipoExploracao1", exploracao.TipoExploracao, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTipoExploracao" }))%>
            </div>
            <div class="coluna22">
                <label>Data Cadastro</label>
                <%= Html.TextBox("DataCadastro", exploracao.Caracterizacao.DataCadastro.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataCadastro maskData" }))%>
            </div>
        </div>

        <%foreach (var item in exploracao.ExploracaoFlorestalExploracaoVM) { %>
			<fieldset class="block box exploracoesFlorestais" id="exploracao<%: item.ExploracaoFlorestal.Identificacao%>">
				<legend class="titFiltros">Exploração Florestal</legend>
				<%Html.RenderPartial("ExploracaoFlorestalExploracao", item); %>
			</fieldset>
		<%} %>
    </fieldset>
</div>
<%} %>
