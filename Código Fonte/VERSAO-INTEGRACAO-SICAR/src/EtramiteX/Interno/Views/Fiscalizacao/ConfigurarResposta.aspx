﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RespostaInfracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Resposta</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			ConfigurarRespostaInfracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ConfigurarResposta", "Fiscalizacao") %>',
					podeDesativar: '<%= Url.Action("ConfiguracaoIsAssociadoRespostaInfracao", "Fiscalizacao") %>',
					podeEditar: '<%= Url.Action("PodeEditarRespostaInfracao", "Fiscalizacao") %>',
					Excluir: '<%= Url.Action("ExcluirRespostaInfracao", "Fiscalizacao") %>',
					ExcluirConfirm: '<%= Url.Action("ExcluirRespostaInfracaoConfirm", "Fiscalizacao") %>',
					alterarSituacao: '<%= Url.Action("AlterarSituacaoRespostaInfracao", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		
		<h1 class="titTela">Resposta</h1><br />

		<fieldset class="box">
			<div class="block">
				<div class="coluna45 append1">
					<label for="Item_NomeCampo">Nome da resposta *</label>
					<%= Html.TextBox("Item.NomeCampo", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeCampo", @maxlength = "100" }))%>
				</div>

				<div class="coluna7">
					<button type="button" style="width:60px" class="inlineBotao btnSalvar" title="Adicionar">Salvar</button>
				</div>

				<input type="hidden" class="hdnItemId" value='0' />
				<input type="hidden" class="hdnItemIsAtivo" value='1' />
			</div>

			<div class="DivItens">
				<% Html.RenderPartial("InfracaoItens"); %>
			</div>
		</fieldset>
	</div>
</asp:Content>
