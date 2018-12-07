<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RequerimentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editar Requerimento Padrão</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="RequerimentoCriar">
		<h1 class="titTela">Editar Requerimento Padrão</h1>
		<br />
		<div class="requerimentoPartial">
			<% Html.RenderPartial("RequerimentoPartialVisualizar"); %>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

<script src="<%= Url.Content("~/Scripts/jquery.listar-grid.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>

<!-- DEPENDENCIAS DE PESSOA -->
<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/inline.js") %>" ></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>" ></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE PESSOA -->
<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/inline.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/listar.js") %>"></script>
<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Requerimento/requerimento.js") %>"></script>

<script>

	Requerimento.urlIndex = '<%= Url.Action("Index", "Requerimento") %>';
	Requerimento.urlAvancar = '<%= Url.Action("Salvar", "Requerimento") %>';
	Requerimento.urlVoltar = '<%= Url.Action("LocalizarMontar", "Requerimento") %>';

	RequerimentoObjetivoPedido.AddRoteiroLink = '<%= Url.Action("AssociarRoteiro", "Roteiro") %>';
	RequerimentoObjetivoPedido.visualizarRoteiroModalLink = '<%= Url.Action("Visualizar", "Roteiro") %>';
	RequerimentoObjetivoPedido.urlAtividadeSolicitada = '<%= Url.Action("AssociarAtividade", "Atividade") %>';
	RequerimentoObjetivoPedido.atividadeSolicitadaLink = '<%= Url.Action("CriarAtividadeSolicitada", "Requerimento") %>';
	RequerimentoObjetivoPedido.urlCriarObjetivoPedido = '<%= Url.Action("CriarObjetivoPedido", "Requerimento") %>';

	RequerimentoObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedido", "Requerimento") %>';
	RequerimentoObjetivoPedido.urlObterObjetivoPedidoVisualizar = '<%= Url.Action("ObterObjetivoPedidoVisualizar", "Requerimento") %>';

	RequerimentoObjetivoPedido.urlObterRoteirosAtividade = '<%= Url.Action("ObterRoteirosAtividade", "Requerimento") %>';
	RequerimentoObjetivoPedido.urlBaixarPdf = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';

	RequerimentoResponsavel.urlCriarResponsavel = '<%= Url.Action("CriarResponsavel", "Requerimento") %>';

	RequerimentoResponsavel.urlExcluirResponsavel = '<%= Url.Action("ExcluirResponsaveis", "Requerimento") %>';
	RequerimentoResponsavel.urlObterResponsavelVisualizar = '<%= Url.Action("ObterResponsavelVisualizar", "Requerimento") %>';
	RequerimentoResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "Requerimento") %>';

	RequerimentoResponsavel.urlAssociarResponsavelModal = '<%= Url.Action("PessoaModal", "Pessoa") %>';
	RequerimentoResponsavel.urlAssociarResponsavelEditarModal = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';

	RequerimentoInteressado.urlObterInteressado = '<%= Url.Action("PessoaInline", "Pessoa") %>';
	RequerimentoInteressado.urlAssociarInteressado = '<%= Url.Action("AssociarInteressado", "Requerimento") %>';
	RequerimentoInteressado.urlLimparInteressado = '<%= Url.Action("LimparInteressado", "Requerimento") %>';
	RequerimentoInteressado.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "Requerimento") %>';

	Requerimento.urlObterReqInterEmp = '<%= Url.Action("ObterReqInterEmp", "Requerimento") %>';

	RequerimentoEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("EmpreendimentoInline", "Empreendimento") %>';
	RequerimentoEmpreendimento.urlAssociarEmpreendimento = '<%= Url.Action("AssociarEmpreendimento", "Requerimento") %>';

	AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';

	RequerimentoEmpreendimento.urlObterEmpreendimentosInteressado = '<%= Url.Action("EmpreendimentoInlineInteressado", "Empreendimento") %>';
	RequerimentoEmpreendimento.urlIsAtividadeCorte = '<%= Url.Action("IsAtividadeCorte", "Requerimento") %>';

	RequerimentoFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "Requerimento") %>';
	RequerimentoFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "Requerimento") %>';

	FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
	FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
	FinalidadeAssociar.urlObterNumerosTitulos = '<%= Url.Action("ObterNumerosTitulos", "Requerimento") %>';
	FinalidadeAssociar.urlValidarNumeroModeloAnterior = '<%= Url.Action("ValidarNumeroModeloAnterior", "Requerimento") %>';

	Requerimento.urlPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
	Requerimento.Mensagens = <%= Model.Mensagens %>;
	AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;
	FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;

	$(function () {
		Requerimento.load($('#central'));
		AtividadeSolicitadaAssociar.load($('#central'), RequerimentoObjetivoPedido.onCallBackTituloAssociado);
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
	});
	</script>
</asp:Content>