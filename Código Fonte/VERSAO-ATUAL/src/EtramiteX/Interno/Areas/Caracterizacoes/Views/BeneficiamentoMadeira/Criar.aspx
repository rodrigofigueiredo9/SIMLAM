<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<BeneficiamentoMadeiraVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar Beneficiamento e Tratamento de Madeira</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/beneficiamentoMadeira.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/beneficiamentoMadeiraBeneficiamento.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			BeneficiamentoMadeira.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "BeneficiamentoMadeira") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "BeneficiamentoMadeira") %>',
					obterTemplate: '<%= Url.Action("ObterTemplateBeneficiamento", "BeneficiamentoMadeira") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Salvar Beneficiamento e Tratamento de Madeira</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("BeneficiamentoMadeira", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>