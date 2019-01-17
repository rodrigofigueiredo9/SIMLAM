<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.listar-grid.js") %>"></script>
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
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/requerimentoVisualizar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

<script type="text/javascript">
		RequerimentoVis.urlIndex = '<%= Url.Action("Index", "Requerimento") %>';
		RequerimentoObjetivoPedido.visualizarRoteiroModalLink = '<%= Url.Action("Visualizar", "Roteiro") %>';	
		RequerimentoObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedidoVisualizar", "Requerimento") %>';	
		RequerimentoObjetivoPedido.urlBaixarPdf = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';

		RequerimentoResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "Requerimento") %>';
		RequerimentoResponsavel.urlAssociarResponsavelEditarModal = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';

		RequerimentoInteressado.urlObterInteressado = '<%= Url.Action("PessoaInline", "Pessoa") %>';
		RequerimentoInteressado.urlAssociarInteressado = '<%= Url.Action("AssociarInteressado", "Requerimento") %>';	

		RequerimentoEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("EmpreendimentoInline", "Empreendimento") %>';
		RequerimentoEmpreendimento.urlAssociarEmpreendimento = '<%= Url.Action("AssociarEmpreendimento", "Requerimento") %>';
	
		RequerimentoEmpreendimento.urlObterEmpreendimentosInteressado = '<%= Url.Action("EmpreendimentoInlineInteressado", "Empreendimento") %>';
		RequerimentoEmpreendimento.urlIsAtividadeCorte = '<%= Url.Action("IsAtividadeCorte", "Requerimento") %>';

		RequerimentoFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "Requerimento") %>';
		RequerimentoFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "Requerimento") %>';

		RequerimentoVis.urlObterReqInterEmp = '<%= Url.Action("ObterReqInterEmp", "Requerimento") %>';
		RequerimentoVis.urlPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
		RequerimentoVis.Mensagens = <%= Model.Mensagens %>;

		$(function () {
			RequerimentoVis.load($('.RequerimentoCriar'));
			AtividadeSolicitadaAssociar.load($('.RequerimentoCriar'));
			RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
			RequerimentoObjetivoPedido.configurarNumeroAnterior();
		});
</script>


<div class="RequerimentoCriar">
    <br />
    <div class="requerimentoPartial">
        <h1 class="titTela">Visualizar Requerimento Digital</h1>
        <br />

        <% Html.RenderPartial("RequerimentoPartial"); %>

        <div class="divMensagemTemplate hide">
            <div class="block box">
                <div class="block">
                    <div class="coluna100">
                        <label class="lblMensagem"></label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
