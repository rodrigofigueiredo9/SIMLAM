<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<BeneficiamentoMadeiraVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Beneficiamento e Tratamento de Madeira</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/beneficiamentoMadeira.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/beneficiamentoMadeiraBeneficiamento.js") %>"></script>
	<script type="text/javascript">
	    $(function () {
	        BeneficiamentoMadeira.load($('#central'), {
	            urls: {
				    mergiar: '<%= Url.Action("GeoMergiar", "BeneficiamentoMadeira") %>',
				    obterTemplate: '<%= Url.Action("ObterTemplateBeneficiamento", "BeneficiamentoMadeira") %>'
				},
			    idsTela: <%=Model.IdsTela %>
			    });
		    CoordenadaAtividade.settings.mensagens = <%= Model.Mensagens%>;
		    CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "BeneficiamentoMadeira") %>';
		    CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "BeneficiamentoMadeira") %>';
		    MateriaPrimaFlorestalConsumida.settings.mensagens = <%= Model.Mensagens%>;
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Beneficiamento e Tratamento de Madeira</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("BeneficiamentoMadeira", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>