<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CampoInfracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Campos da fiscalização</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>" ></script>
	<script>
		$(function () {
			ConfigurarCampoInfracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ConfigurarCampo", "Fiscalizacao") %>',
					podeDesativar: '<%= Url.Action("ConfiguracaoIsAssociadoCampoInfracao", "Fiscalizacao") %>',
					podeEditar: '<%= Url.Action("PodeEditarCampoInfracao", "Fiscalizacao") %>',
					Excluir: '<%= Url.Action("ExcluirCampoInfracao", "Fiscalizacao") %>',
					ExcluirConfirm: '<%= Url.Action("ExcluirCampoInfracaoConfirm", "Fiscalizacao") %>',
					alterarSituacao: '<%= Url.Action("AlterarSituacaoCampoInfracao", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Campos da fiscalização</h1><br />

		<fieldset class="box">
			<div class="block">
				<div class="coluna35 append2">
					<label for="Item_NomeCampo">Nome do campo *</label>
					<%= Html.TextBox("Item.NomeCampo", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeCampo", @maxlength = "100" }))%>
				</div>

				<div class="coluna15 append2">
					<label for="Item_TipoCampo">Tipo do campo *</label>
					<%= Html.DropDownList("Item.TipoCampo", Model.Tipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoCampo"}))%>
				</div>

				<div class="coluna15 append1 divUnidadeMedida hide">
					<label for="Item_UnidadeMedida">Unidade de medida</label>
					<%= Html.DropDownList("Item.UnidadeMedida", Model.Unidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlUnidadeMedida"}))%>
				</div>

				<div class="coluna7">
					<button type="button" style="width:60px" class="inlineBotao btnSalvar" title="Adicionar">Salvar</button>
				</div>

				<input type="hidden" class="hdnItemId" value='0' />
				<input type="hidden" class="hdnItemIsAtivo" value='1' />
			</div>

			<div class="DivItens">
				<% Html.RenderPartial("FiscalizacaoCampos"); %>
			</div>
		</fieldset>
	</div>
</asp:Content>
