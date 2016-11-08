<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TipoInfracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Tipo de infração</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			ConfigurarTipoInfracao .load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ConfigurarTipoInfracao", "Fiscalizacao") %>',
					podeDesativar: '<%= Url.Action("ConfiguracaoIsAssociadoTipoInfracao", "Fiscalizacao") %>',
					podeEditar: '<%= Url.Action("PodeEditarTipoInfracao", "Fiscalizacao") %>',
					Excluir: '<%= Url.Action("ExcluirTipoInfracao", "Fiscalizacao") %>',
					ExcluirConfirm: '<%= Url.Action("ExcluirTipoInfracaoConfirm", "Fiscalizacao") %>',
					alterarSituacao: '<%= Url.Action("AlterarSituacaoTipoInfracao", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		
		<h1 class="titTela">Tipo de infração</h1><br />

		<fieldset class="box">
			<div class="block">
				<div class="coluna45 append1">
					<label for="Item_NomeCampo">Tipo de infração *</label>
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
