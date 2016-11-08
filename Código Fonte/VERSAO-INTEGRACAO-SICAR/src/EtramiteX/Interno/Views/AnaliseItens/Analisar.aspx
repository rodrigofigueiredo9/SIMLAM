<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AnaliseItemVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Análise de Itens de Processo/Documento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/AnaliseItens/analiseItem.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/AnaliseItens/analise.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ChecagemRoteiro/salvarChecagemRoteiro.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Roteiro/listarItem.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Roteiro/salvarItem.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/atividadesSolicitadas.js") %>"></script>
	<script type="text/javascript">

		$(function () {
			Analise.load($('#central'), {
				urls: {
					pdfRequerimento : '<%= Url.Action("GerarPdf", "Requerimento") %>',
					pdfRoteiro : '<%= Url.Action("RelatorioRoteiro","Roteiro") %>',
					obterRequerimento : '<%= Url.Action("ObterRequerimentos", "AnaliseItens") %>',
					obterAnalisePartial : '<%= Url.Action("ObterAnalisePartial", "AnaliseItens") %>',
					analiseItem : '<%= Url.Action("ObterAnaliseItem", "AnaliseItens") %>',
					atividadesSolicitadas : '<%= Url.Action("AtividadesSolicitadas", "AnaliseItens") %>',
					versaoRoteiros : '<%= Url.Action("ObterVersaoRoteiros", "AnaliseItens") %>',
					modalAtualizarRoteiro : '<%= Url.Action("CriarModalAtualizarRoteiro", "AnaliseItens") %>',
					visualizarHistorico : '<%= Url.Action("ObterHistoricoAnaliseItem", "AnaliseItens") %>',
					salvar : '<%= Url.Action("Salvar", "AnaliseItens") %>',
					pdfAnalise : '<%= Url.Action("GerarPdf", "AnaliseItens") %>',
					criarItem : '<%= Url.Action("ListarItem", "Roteiro") %>',
					visualizarChecagem : '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
					validarPossuiTitPendencia : '<%= Url.Action("ValidarGerarTituloPendencia", "Titulo") %>',
					criarTitPendencia : '<%= Url.Action("CriarDeModelo", "Titulo") %>',
					excluirItem : '<%= Url.Action("ExcluirItem", "AnaliseItens") %>',
					salvarAnaliseItem : '<%= Url.Action("SalvarAnaliseItem", "AnaliseItens") %>',
					analisar: '<%= Url.Action("Analisar", "AnaliseItens") %>',
					projetoGeografico: '<%= Url.Action("VisualizarProjetoGeoCredenciado", "ProjetoGeografico", new { Area = "Caracterizacoes"})%>',
					caracterizacoes: <%= !string.IsNullOrEmpty(Model.UrlsCaracterizacoes) ? Model.UrlsCaracterizacoes : "[]" %>,
					importarProjetoDigital: '<%= Url.Action("ImportarProjetoDigital", "AnaliseItens")%>'
				},
				isPendente: eval('<%= Model.IsPendente.ToString().ToLower() %>')
			});

			Analise.mensagens = <%= Model.Mensagens %>;
			Analise.tipos = <%= Model.Tipos %>;
			Analise.situacoes = <%= Model.SituacoesItem %>;

			AnaliseItem.mensagens = <%= Model.Mensagens %>;
			AnaliseItem.situacoes = <%= Model.SituacoesItem %>;

			AtividadesSolicitadas.urlVisualizarPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
		    ChecagemRoteiroSalvar.visualizarRoteiroModalLink = '<%= Url.Action("Visualizar", "Roteiro") %>';

            <% 
			
			if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
			{%>
			
		    ContainerAcoes.load($(".containerAcoes"), {
		        botoes: [{label: 'Gerar PDF da análise', url:'<%= Url.Action("GerarPdf", "AnaliseItens", new {id = Request.Params["acaoId"].ToString() })%>'},
		                 {label: 'Listar Empreendimentos', url:'<%= Url.Action("Index", "Empreendimento")%>'}],
				});

			<%}%>

		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Análise de Itens de Processo/Documento</h1>
		<br />

		<div class="block">

			<input type="hidden" class="hdnProcDocId" value="<%= Model.ProtocoloId %>" />
			<input type="hidden" class="hdnIsProcesso" value="" />
			<input type="hidden" class="hdnAnaliseId" value="<%= Model.AnaliseId %>" />
			<input type="hidden" class="hdnRequerimentoSelecionadoId" value="<%= Model.RequerimentoSelecionado %>" />
			<input type="hidden" class="hdnIsAtualizarRoteiro" value="false" />

			<%bool possuiNumeroProtocolo =  !String.IsNullOrWhiteSpace(Model.ProtocoloNumero); %>
			<fieldset class="block box">
				<legend>Processo/ Documento</legend>
				<div class="block">
					<div class="coluna20">
						<label>Número de registro *</label>
						<%= Html.TextBox("NumeroProcDoc", Model.ProtocoloNumero, ViewModelHelper.SetaDisabled(possuiNumeroProtocolo, new { @maxlength = "12", @class = "text txtNumeroProtocolo" }))%>
					</div>
					<span>
						<button type="button" title="Verificar" class="inlineBotao <%= !possuiNumeroProtocolo ? "" : "hide" %> botaoBuscar btnVerificar">Verificar</button>
						<button type="button" title="Limpar" class="inlineBotao btnLimpar <%= possuiNumeroProtocolo ? "" : "hide" %>">Limpar</button>
					</span>
				</div>
			</fieldset>

			<fieldset class="block box fsRequerimento <%=Model.Requerimentos.Count > 0 ? "" : "hide" %>">
				<legend>Requerimento</legend>
				<div class="divRequerimentoContainer coluna90">
					<%if (Model.Requerimentos.Count > 0) { Html.RenderPartial("Requerimentos", Model); }%>
				</div>
			</fieldset>

			<div class="divConteudoAnalisePartial">
				<%if (Model.Requerimentos.Count > 0) { Html.RenderPartial("AnalisarPartial", Model); }%>
			</div>
		</div>

		<div class="block box">

			<input class="btnSalvarAnalise floatLeft <%=Model.Requerimentos.Count > 0 && !Model.IsPendente ? "" : "hide" %>" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu <%=Model.Requerimentos.Count > 0 ? "" : "hide" %>">ou </span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
			
			<div class="floatRight">
				<span class="cancelarCaixa <%=Model.AnaliseId > 0 ? "" : "hide" %> spanGerarPdfAnalise">
					<input type="button" value="Gerar PDF da análise" class="btnGerarPdfAnalise" />
				</span>

				<span class="cancelarCaixa <%=Model.ExisteItensPendentes ? "" : "hide" %> spanGerarTituloAnalise">
					<input type="button" value="Emitir título de pendência" class="btnEmitirTitPen" />
				</span>
			</div>
		</div>

	</div>
</asp:Content>