<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DescricaoLicenciamentoAtividadeVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Descrição de Atividade</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/descricaoLicenciamentoAtividade.js") %>"></script>
	<script>
		$(function () {
			DescricaoLicenciamentoAtividade.load($('#central'), {
				urls: {
					Salvar: '<%= Url.Action("Salvar", "DescricaoLicenciamentoAtividade") %>',
					Visualizar: '<%= Url.Action("Visualizar", "DescricaoLicenciamentoAtividade") %>',
					Redirecionar:  '<%= Model.UrlAvancar  %>'
				},
				Mensagens: <%= Model.Mensagens() %>,
				textoMerge: '<%= Model.TextoMerge %>',
				atualizarDependenciasModalTitulo: '<%= Model.AtualizarDependenciasModalTitulo %>'
			});

			DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua = <%= Model.FontesAbastecimentoAgua  %>;
			DescricaoLicenciamentoAtividade.PontosLancamentoEfluente = <%= Model.PontosLancamentoEfluente  %>;
			DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId = <%= Model.FontesGeracaoOutrosId  %>;

			DescricaoLicenciamentoAtividade.TratamentoOutrasFormasId = <%= Model.TratamentoOutrasFormasId  %>;
			DescricaoLicenciamentoAtividade.DestinoFinalOutrosId = <%= Model.DestinoFinalOutrosId  %>;

			$('.ddlFontesAbastecimentoAguaTipo', DescricaoLicenciamentoAtividade.container).change();
			$('.ddlPontosLancamentoEfluenteTipo', DescricaoLicenciamentoAtividade.container).change();
		});
	</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Descrição de Atividade - <%= Model.CaracterizacaoTipoTexto %></h2>
		<br />
		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Model.DscLicAtividade.EmpreendimentoId %>" />
		<input type="hidden" class="hdnDscLicAtividadeId" value="<%= Model.DscLicAtividade.Id %>" />
		<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= (int)Model.DscLicAtividade.Tipo %>" />
		<input type="hidden" class="hdnIsCadastrarCaracterizacao" value="<%= Model.IsCadastrarCaracterizacao %>" />		

		<div class="divCaracterizacao">
			<%Html.RenderPartial("DescricaoLicenciamentoAtividadePartial", Model);%>
		</div>

		<div class="block box">
			<% if (!Model.IsVisualizar){ %><input class="floatLeft btnSalvar" type="button" value="Salvar" /><% } %>
			<span class="cancelarCaixa"><span class="btnModalOu <%: (Model.IsVisualizar)? "hide" : "" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.DscLicAtividade.EmpreendimentoId }) %>">Cancelar</a></span>
			<span class="spanBotoes floatRight spanAvancar <%= Model.DscLicAtividade.Id > 0 ? "" : "hide" %>"><input class="btnAvancar" type="button" value="Avançar" /></span>
			
		</div>
	</div>
</asp:Content>

