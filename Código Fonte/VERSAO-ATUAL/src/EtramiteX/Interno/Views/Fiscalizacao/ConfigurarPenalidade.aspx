<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PenalidadeVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Penalidades</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>" ></script>
	<script>
	    $(function () {
	        ConfigurarPenalidade.load($('#central'), {
	            urls: {
	                salvar: '<%= Url.Action("ConfigurarPenalidade", "Fiscalizacao") %>',
				    podeEditar: '<%= Url.Action("PodeEditarPenalidade", "Fiscalizacao") %>',
				    Excluir: '<%= Url.Action("ExcluirPenalidade", "Fiscalizacao") %>',
	                ExcluirConfirm: '<%= Url.Action("ExcluirPenalidadeConfirm", "Fiscalizacao") %>',
	                alterarSituacao: '<%= Url.Action("AlterarSituacaoPenalidade", "Fiscalizacao") %>'
				},
			    mensagens: <%= Model.Mensagens %>,
			    idsTela: <%= Model.IdsTela %>
			    });
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Penalidades</h1><br />

		<fieldset class="box">
			<div class="block">
				<div class="coluna10 append2">
					<label for="Item_NomeCampo">Artigo *</label>
					<%= Html.TextBox("Artigo", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArtigo", @maxlength = "10" }))%>
				</div>

				<div class="coluna10 append2">
					<label for="Item_NomeCampo">Item *</label>
					<%= Html.TextBox("Item", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtItem", @maxlength = "10" }))%>
				</div>

                <div class="coluna45 append2">
					<label for="Item_NomeCampo">Descrição *</label>
					<%= Html.TextBox("Descricao", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricao", @maxlength = "200" }))%>
				</div>

				<div class="coluna7">
					<button type="button" style="width:60px" class="inlineBotao btnSalvar" title="Adicionar">Salvar</button>
				</div>

				<input type="hidden" class="hdnItemId" value='0' />
				<input type="hidden" class="hdnItemIsAtivo" value='1' />
			</div>

			<div class="DivPenalidades">
				<% Html.RenderPartial("FiscalizacaoPenalidade"); %>
			</div>
		</fieldset>
	</div>
</asp:Content>
