<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<RequerimentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Requerimento Digital</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script> 

	<!-- DEPENDENCIAS DE PESSOA -->
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/inline.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/inline.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/listar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/requerimento.js") %>"></script>

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script type="text/javascript">
		Requerimento.urlIndex = '<%= Url.Action("Operar", "ProjetoDigital", (Model.ProjetoDigitalId > 0) ? new { id = Model.ProjetoDigitalId } : null) %>';
		Requerimento.urlAvancar = '<%= Url.Action("Salvar", "Requerimento") %>';

		AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';
		AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;

		FinalidadeAssociar.urlValidarNumeroProcesso = '<%= Url.Action("ValidarNumeroProcesso", "Requerimento") %>';
		FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
		FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
		FinalidadeAssociar.urlObterNumerosTitulos = '<%= Url.Action("ObterNumerosTitulos", "Requerimento") %>';
		FinalidadeAssociar.urlValidarNumeroModeloAnterior = '<%= Url.Action("ValidarNumeroModeloAnterior", "Requerimento") %>';
		FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;

		RequerimentoObjetivoPedido.urlAtividadeSolicitada = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
		RequerimentoObjetivoPedido.atividadeSolicitadaLink = '<%= Url.Action("CriarAtividadeSolicitada", "Requerimento") %>';
		RequerimentoObjetivoPedido.urlCriarObjetivoPedido = '<%= Url.Action("CriarObjetivoPedido", "Requerimento") %>';
		RequerimentoObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedido", "Requerimento") %>';
		RequerimentoObjetivoPedido.urlObterObjetivoPedidoVisualizar = '<%= Url.Action("ObterObjetivoPedidoVisualizar", "Requerimento") %>';
		RequerimentoObjetivoPedido.urlObterRoteirosAtividade = '<%= Url.Action("ObterRoteirosAtividade", "Requerimento") %>';
		RequerimentoObjetivoPedido.urlBaixarPdf = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';
		RequerimentoObjetivoPedido.urlVerificarPassoDois = '<%= Url.Action("VerificarPassoDois", "Requerimento") %>';

		RequerimentoInteressado.urlObterInteressado = '<%= Url.Action("PessoaInline", "Pessoa") %>';
		RequerimentoInteressado.urlAssociarInteressado = '<%= Url.Action("AssociarInteressado", "Requerimento") %>';
		RequerimentoInteressado.urlLimparInteressado = '<%= Url.Action("LimparInteressado", "Requerimento") %>';
		RequerimentoInteressado.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "Requerimento") %>';

		RequerimentoResponsavel.urlCriarResponsavel = '<%= Url.Action("CriarResponsavel", "Requerimento") %>';	
		RequerimentoResponsavel.urlExcluirResponsavel = '<%= Url.Action("ExcluirResponsaveis", "Requerimento") %>';
		RequerimentoResponsavel.urlObterResponsavelVisualizar = '<%= Url.Action("ObterResponsavelVisualizar", "Requerimento") %>';
		RequerimentoResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "Requerimento") %>';
		RequerimentoResponsavel.urlAssociarResponsavelModal = '<%= Url.Action("PessoaModal", "Pessoa") %>';
		RequerimentoResponsavel.urlAssociarResponsavelEditarModal = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';

		RequerimentoEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("EmpreendimentoInline", "Empreendimento") %>';
		RequerimentoEmpreendimento.urlAssociarEmpreendimento = '<%= Url.Action("AssociarEmpreendimento", "Requerimento") %>';

		RequerimentoFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "Requerimento") %>';	
		RequerimentoFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "Requerimento") %>';

		Requerimento.urlObterReqInterEmp = '<%= Url.Action("ObterReqInterEmp", "Requerimento") %>';
		Requerimento.urlPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
		Requerimento.Mensagens = <%= Model.Mensagens %>;
		Requerimento.ProjetoDigitalId = '<%: Model.ProjetoDigitalId %>';

		$(function () {
			Requerimento.load($('#central'));
			AtividadeSolicitadaAssociar.load($('#central'), RequerimentoObjetivoPedido.onCallBackTituloAssociado);
			RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="RequerimentoCriar">
		<h1 class="titTela">Cadastrar Requerimento Digital</h1>
		<br />
		<div class="requerimentoPartial">
			<% Html.RenderPartial("RequerimentoPartial"); %>
		</div>
	</div>
</asp:Content>