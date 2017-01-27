<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<FiscalizacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Fiscalização
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

<!-- DEPENDENCIAS DE PESSOA -->
<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/inline.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE PESSOA -->

<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/inline.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/listar.js") %>"></script>
<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

<!-- DEPENDENCIAS DE PROJETO GEOGRAFICO -->
<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/navegador.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Fiscalizacao/projetoGeografico.js") %>"></script>
<!-- DEPENDENCIAS DE PROJETO GEOGRAFICO -->

<%--<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>--%>
<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacao.js") %>"></script>



<script>

	Fiscalizacao.urls.localInfracaoVisualizar = '<%= Url.Action("LocalInfracaoVisualizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.complementacaoDados = '<%= Url.Action("ComplementacaoDados", "Fiscalizacao") %>';
	Fiscalizacao.urls.enquadramento = '<%= Url.Action("Enquadramento", "Fiscalizacao") %>';
	Fiscalizacao.urls.objetoInfracao = '<%= Url.Action("ObjetoInfracao", "Fiscalizacao") %>';
	Fiscalizacao.urls.autuado = '<%= Url.Action("Autuado", "Fiscalizacao") %>';
	Fiscalizacao.urls.projetoGeografico = '<%= Url.Action("ProjetoGeograficoVisualizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.diagnostico = '<%= Url.Action("Diagnostico", "Fiscalizacao") %>';
	Fiscalizacao.urls.consideracaoFinalVisualizar = '<%= Url.Action("ConsideracaoFinalVisualizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.finalizar = '<%= Url.Action("Finalizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.infracao = '<%= Url.Action("InfracaoVisualizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.materialApreendido = '<%= Url.Action("MaterialApreendidoVisualizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.finalizar = '<%= Url.Action("Finalizar", "Fiscalizacao") %>';
	Fiscalizacao.urls.documentosGerados = '<%= Url.Action("DownloadDocumentosGerados", "Fiscalizacao") %>';

	FiscalizacaoLocalInfracao.urlCoordenadaGeo = '<%= Url.Action("AreaAbrangenciaPartial", "Mapa", new {area="GeoProcessamento"}) %>';
	FiscalizacaoLocalInfracao.urlsObterEstadosMunicipiosPorCoordenada = '<%= Url.Action("obterEstadosMunicipiosPorCoordenada", "Mapa", new {area="GeoProcessamento"}) %>';

	//Complementacao de dados do autuado
	FiscalizacaoComplementacaoDados.settings.idsTela = <%=Model.ComplementacaoDadosVM.IdsTela%>;
	FiscalizacaoComplementacaoDados.settings.urls.salvar = '<%= Url.Action("CriarComplementacaoDados", "Fiscalizacao") %>';
	FiscalizacaoComplementacaoDados.settings.urls.visualizar = '<%= Url.Action("ComplementacaoDadosVisualizar", "Fiscalizacao") %>';
	FiscalizacaoComplementacaoDados.settings.mensagens = <%=Model.ComplementacaoDadosVM.Mensagens%>;
	FiscalizacaoComplementacaoDados.settings.urls.urlObterVinculoPropriedade = '<%= Url.Action("ObterVinculoPropriedade", "Fiscalizacao") %>';

	//Enquadramento
	FiscalizacaoEnquadramento.settings.urls.salvar = '<%= Url.Action("CriarEnquadramento", "Fiscalizacao") %>';
	FiscalizacaoEnquadramento.settings.urls.visualizar = '<%= Url.Action("EnquadramentoVisualizar", "Fiscalizacao") %>';
	FiscalizacaoEnquadramento.settings.mensagens = <%=Model.EnquadramentoVM.Mensagens%>;

	//Objeto Infração
	FiscalizacaoObjetoInfracao.settings.urls.salvar = '<%= Url.Action("CriarObjetoInfracao", "Fiscalizacao") %>';
	FiscalizacaoObjetoInfracao.settings.urls.visualizar = '<%= Url.Action("ObjetoInfracaoVisualizar", "Fiscalizacao") %>';
	FiscalizacaoObjetoInfracao.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoObjetoInfracao.TiposArquivo = <%= Model.ObjetoInfracaoVM.TiposArquivoValido %>;
	FiscalizacaoObjetoInfracao.settings.mensagens = <%=Model.ObjetoInfracaoVM.Mensagens%>;

	Fiscalizacao.salvarEdicao = true;

	$(function () {
		Fiscalizacao.load($('#central'));
	});

	Fiscalizacao.modo = 1;//Cadastrar | Editar
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="FiscalizarCriar">


		<h1 class="titTela">Fiscalização</h1><br />
		<div class="fiscalizacaoPartial">
			<% Html.RenderPartial("FiscalizacaoPartial", Model); %>
		</div>
	</div>
</asp:Content>

