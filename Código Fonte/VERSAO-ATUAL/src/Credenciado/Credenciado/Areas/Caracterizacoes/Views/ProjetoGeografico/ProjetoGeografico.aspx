<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoGeograficoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Projeto Geográfico</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/navegador.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/projetoGeografico.js") %>"></script>
	<script type="text/javascript">
		$(function () {

			/*BaseReferencia*/
			BaseReferencia.settings.urls.obterSituacao = '<%= Url.Action("ObterSituacao", "ProjetoGeografico") %>';
			BaseReferencia.settings.urls.gerarArquivoVetorial = '<%= Url.Action("Processar", "ProjetoGeografico") %>';
			BaseReferencia.settings.urls.gerarArquivoOrtoFotoMosaico = '<%= Url.Action("ObterOrtoFotoMosaico", "ProjetoGeografico") %>';			
			BaseReferencia.settings.urls.baixarArquivoModelo = '<%= Url.Action("BaixarArquivoModelo", "ProjetoGeografico") %>';
			BaseReferencia.settings.urls.baixarArquivoVetorial = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';			
			BaseReferencia.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';			
			BaseReferencia.settings.urls.baixarArquivoOrtoMosaico = '<%= Model.UrlBaixarOrtofoto %>';
			BaseReferencia.settings.urls.salvarArquivoOrtoMosaico = '<%: Url.Action("SalvarOrtofoto","ProjetoGeografico") %>';
			BaseReferencia.settings.situacoesValidas = <%= Model.SituacoesValidasJson %>;

			/*ImportadorShape*/
			ImportadorShape.settings.urls.ImportadorShape = '<%: Url.Action("arquivo","arquivo", new { area = ""}) %>';
			ImportadorShape.settings.urls.EnviarParaProcessar = '<%: Url.Action("EnviarArquivo","ProjetoGeografico") %>';
			ImportadorShape.settings.urls.ArquivosProcessados = '<%: Url.Action("baixarArquivoProcessado","ProjetoGeografico") %>';
			ImportadorShape.settings.urls.reenviarArquivo = '<%= Url.Action("ReenviarArquivoImportador","ProjetoGeografico") %>';
			ImportadorShape.settings.urls.confirmarReenviarArquivo = '<%= Url.Action("ConfirmarReenviarArquivoImportador","ProjetoGeografico") %>';
			ImportadorShape.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';
			ImportadorShape.settings.situacoesValidas = <%= Model.SituacoesValidasJson %>;
			ImportadorShape.mensagens =  <%= Model.MensagensImportador %>;

			/*Desenhador*/
			Desenhador.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';
			Desenhador.settings.urls.desenhador	= '<%= Url.Action("DesenhadorPartial", "Mapa", new {area="GeoProcessamento"}) %>';
			Desenhador.settings.situacoesValidas = <%= Model.SituacoesValidasJson %>;
			Desenhador.settings.isVisualizar = <%= (Model.IsVisualizar || Model.IsFinalizado).ToString().ToLower() %>;
			Desenhador.mensagens = <%= Model.MensagensDesenhador %>;

			/*Sobreposicao*/
			Sobreposicao.settings.urls.verificarSobreposicao = '<%= Url.Action("VerificarSobreposicao","ProjetoGeografico") %>';
			Sobreposicao.settings.sobreposicoesObjJon = <%= ViewModelHelper.Json(Model.Projeto.Sobreposicoes) %>;

			/*ProjetoGeografico*/
			ProjetoGeografico.settings.EmpreendimentoEstaDentroAreaAbrangencia = <%= Model.Projeto.EmpreendimentoEstaDentroAreaAbrangencia.ToString().ToLower() %>;
			ProjetoGeografico.settings.textoMerge = '<%= Model.TextoMerge %>';
			ProjetoGeografico.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
			ProjetoGeografico.settings.dependencias = '<%= ViewModelHelper.Json(Model.Projeto.Dependencias) %>';
			ProjetoGeografico.settings.possuiAPPNaoCaracterizada = <%= Model.PossuiAPPNaoCaracterizada.ToString().ToLower() %>;
			ProjetoGeografico.settings.possuiARLNaoCaracterizada = <%= Model.PossuiARLNaoCaracterizada.ToString().ToLower() %>;
			ProjetoGeografico.settings.idsTelaSitacaoProcessamento = <%= Model.SituacoesProcessamento %>;
			ProjetoGeografico.settings.idsTelaArquivoTipo = <%= Model.ArquivoTipo %>;
			ProjetoGeografico.settings.idsTelaEtapaProcessamento = <%= Model.EtapasProcessamento %>;
			ProjetoGeografico.settings.idsTelaMecanismo = <%= Model.MecanismosProcessamento %>;
			ProjetoGeografico.isCadastrarCaracterizacao = '<%= Model.isCadastrarCaracterizacao %>';
			ProjetoGeografico.settings.isDominialidade = <%= Model.IsDominialidade.ToString().ToLower() %>;
			ProjetoGeografico.settings.isVisualizar = <%= Model.IsVisualizar.ToString().ToLower() %>;
			ProjetoGeografico.settings.isFinalizado = <%= Model.IsFinalizado.ToString().ToLower() %>;
			ProjetoGeografico.settings.isProcessado = <%= Model.IsProcessado.ToString().ToLower() %>;
			ProjetoGeografico.settings.urls.verificarAreaNaoCaracterizada = '<%= Url.Action("VerificarAreaNaoCaracterizada", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.avancar = '<%= Model.UrlAvancar %>';
			ProjetoGeografico.settings.urls.coordenadaGeo = '<%= Url.Action("AreaAbrangenciaPartial", "Mapa", new {area="GeoProcessamento"}) %>';
			ProjetoGeografico.settings.urls.criarParcial = '<%= Url.Action("CriarParcial", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.confirmarAlterarArea = '<%= Url.Action("ConfirmarAlteracaoCoordenadas", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.cancelarProcessamentoArquivosVet = '<%: Url.Action("CancelarProcessamento","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.alterarAreaAbrangencia = '<%: Url.Action("AlterarAreaAbrangencia", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.excluir = '<%: Url.Action("ExcluirRascunho","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.refazer = '<%: Url.Action("Refazer","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.recarregar = '<%: Url.Action("Recarregar","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.finalizar = '<%: Url.Action("Finalizar", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.salvar = '<%: Url.Action("Salvar", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.obterMerge = '<%: Url.Action("ObterMerge", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.processar ='<%= Url.Action("Processar","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.cancelarProcessamento = '<%= Url.Action("CancelarProcessamento","ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.obterSituacao = '<%= Url.Action("ObterSituacao", "ProjetoGeografico") %>';
			ProjetoGeografico.settings.urls.obterArquivos = '<%= Url.Action("ObterArquivos","ProjetoGeografico") %>';
			ProjetoGeografico.mensagens = <%= Model.Mensagens %>;

			ProjetoGeografico.load($('#central'), 
			{
				projetoId: $('.hdnProjetoId', $('#central')).val() 
			});
		});
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Projeto Geográfico - <%= Model.Projeto.CaracterizacaoTexto %></h1>
		<br />
		<div class="box block">
			<div class="coluna25">
				<label>Sistema de Coordenada *</label>
				<%= Html.TextBox("Projeto.SistemaCoordenada", null, new { @disabled = "disabled", @class = "text disabled txtSistemaCoordenada" })%>
			</div>
			<div class="coluna22 prepend1">
				<label>Nível de precisão<%= Model.ArquivoEnviadoTipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado ? " - APMP *" : " *"%></label>
				<%= Html.DropDownList("Projeto.NivelPrecisaoId", Model.NiveisPrecisao, ViewModelHelper.SetaDisabled(Model.IsFinalizado || Model.IsVisualizar, new { @class = "text ddlNivel" }))%>
			</div>
			<div class="coluna30 prepend1">
				<label>Situação do projeto geográfico</label>
				<%= Html.TextBox("SituacaoProjetoTexto", Model.Projeto.SituacaoTexto, new { @disabled = "disabled", @class = "text disabled" })%>
			</div>
		</div>

		<fieldset class="block box" id="AreaAbrangencia_Container">
			<legend>Área de abrangência</legend>
			<div class="block divBotoesAreaAbrangencia <%= Model.IsFinalizado || Model.IsVisualizar ? "hide" : ""%>">
				<button class="btnSelecionarCoordenada  <%= Model.IsEditar || !Model.IsDominialidade ? "hide" : "" %>">Selecionar</button>
				<button class="btnAlterarCoordenada  <%= !Model.IsEditar || !Model.IsDominialidade ? "hide" : "" %>">Alterar a área</button>
				<button class="btnObterCoordenadaAuto  <%= !Model.IsDominialidade ? "hide" : "" %>" title=" Gerar área de abrangência a partir do ponto do empreendimento">Automático</button>
			</div>

			<% Model.Projeto.CorrigirMbr(); %>

			<div class="divCoordenada <%= Model.Projeto.MenorX == 0 ? "hide" : "" %>">
				<fieldset class="boxBranca coluna44 prepend2 ">
					<legend>Coordenada 1</legend>
					<div class="coluna45 prepend3">
						<label>Mínimo X *</label>
						<%= Html.TextBox("txtMenorX", Model.Projeto.MenorX > 0 ? Model.Projeto.MenorX.ToString() : "0", new { @disabled = "disabled", @class = "text txtMenorX disabled" })%>
					</div>
					<div class="coluna45 prepend3">
						<label>Máximo Y*</label>
						<%= Html.TextBox("txtMaiorY", Model.Projeto.MaiorY > 0 ? Model.Projeto.MaiorY.ToString() : "0", new { @disabled = "disabled", @class = "text txtMaiorY disabled" })%>
					</div>
				</fieldset>

				<fieldset class="boxBranca coluna43 prepend2">
					<legend>Coordenada 2</legend>
					<div class="coluna45 prepend3">
						<label>Máximo X*</label>
						<%= Html.TextBox("txtMaiorX", Model.Projeto.MaiorX > 0 ? Model.Projeto.MaiorX.ToString() : "0", new { @disabled = "disabled", @class = "text txtMaiorX disabled" })%>
					</div>
					<div class="coluna45 prepend3">
						<label>Mínimo Y*</label>
						<%= Html.TextBox("txtMenorY", Model.Projeto.MenorY > 0 ? Model.Projeto.MenorY.ToString() : "0", new { @disabled = "disabled", @class = "text txtMenorY disabled" })%>
					</div>
				</fieldset>
			</div>
		</fieldset>

		<div class="msgInline divExistemProcessamentosAndamento hide">
			<label>Existem processamentos em andamento!</label>
		</div>

		<fieldset class="block box fsMecanismo <%= (Model.Projeto.MecanismoElaboracaoId != 0 || !Model.IsDominialidade) ? "" : "hide" %>">
			<legend>Mecanismo de elaboração</legend>
			<label>
				<input type="radio" class="radioTiPoMecanismo" name="mecanismo" value="1" <%= Model.IsFinalizado || Model.IsVisualizar ? "disabled=\"disabled\"" : ""%> <%= Model.IsImportadorShape ? "checked=\"checked\"" : ""%> />Importador de shapes</label>
			<label>
				<input type="radio" class="radioTiPoMecanismo" name="mecanismo" value="2" <%= Model.IsFinalizado || Model.IsVisualizar ? "disabled=\"disabled\"" : ""%> <%= Model.IsDesenhador ? "checked=\"checked\"" : ""%> />Desenhador</label>

			<input type="hidden" class="hdnProjetoId" value="<%= Model.Projeto.Id %>" />
			<input type="hidden" class="hdnProjetoDigitalId" value="<%= Model.Projeto.ProjetoDigitalId %>" />
			<input type="hidden" class="hdnEmpreendimentoId" value="<%= Model.Projeto.EmpreendimentoId %>" />
			<input type="hidden" class="hdnCaracterizacaoTipo" value="<%= Model.Projeto.CaracterizacaoId %>" />
			<input type="hidden" class="hdnEmpNorthing" value="<%= Model.Projeto.EmpreendimentoNorthing%>" />
			<input type="hidden" class="hdnEmpEasting" value="<%= Model.Projeto.EmpreendimentoEasting %>" />
			<input type="hidden" class="hdnArquivoEnviadoTipo" value="<%= Model.ArquivoEnviadoTipo%>" />
			<input type="hidden" class="hdnArquivoEnviadoFilaTipo" value="<%= Model.ArquivoEnviadoFilaTipo%>" />
			<input type="hidden" class="hdnArquivoEnviadoSituacao" value="<%= Model.Importador.ArquivoEnviado.Situacao%>" />
			<input type="hidden" class="hdnProjetoSituacaoId" value="<%= Model.Projeto.SituacaoId%>" />
			<input type="hidden" class="hdnInternoId" value="<%= Model.Projeto.InternoID%>" />
		</fieldset>

		<div class="divImportador <%= Model.IsImportadorShape ? "" : "hide"%>">
			<fieldset class="block box">
				<legend>Importador de Shapes</legend>

				<fieldset class="block boxBranca">
					<legend>Base de referência para download</legend>
					<% Html.RenderPartial("BaseReferenciaPartial", Model.BaseReferencia); %>
				</fieldset>

				<fieldset class="block boxBranca fsImportadorShape <%= Model.Projeto.Arquivos.Count > 0? "" : "hide"%>">
					<legend>Enviar projeto geográfico</legend>
					<% Html.RenderPartial("ImportadorShapePartial", Model.Importador); %>
				</fieldset>
				
				<div class="block msgInline erro divAlertaAPP <%= Model.IsProcessado && Model.PossuiAPPNaoCaracterizada ? "" : "hide" %>">
					<p><label>O projeto geográfico possui APP não caracterizada. Isso irá inviabilizar o Cadastro Ambiental Rural.</label></p>
				</div>
				<div class="block msgInline erro divAlertaARL <%= Model.IsProcessado && Model.PossuiARLNaoCaracterizada? "" : "hide" %>">
					<p><label>O projeto geográfico possui ARL não caracterizada. Isso irá inviabilizar o Cadastro Ambiental Rural.</label></p>
				</div>
			</fieldset>
		</div>

		<div class="divDesenhador <%= Model.IsDesenhador ? "" : "hide"%>">
			<fieldset class="block box">
				<legend>Desenhador</legend>
				<% Html.RenderPartial("DesenhadorPartial", Model.Desenhador); %>

				<div class="block msgInline erro divAlertaAPP <%= Model.IsProcessado && Model.PossuiAPPNaoCaracterizada ? "" : "hide" %>">
					<p><label>O projeto geográfico possui APP não caracterizada. Isso irá inviabilizar o Cadastro Ambiental Rural.</label></p>
				</div>
				<div class="block msgInline erro divAlertaARL <%= Model.IsProcessado && Model.PossuiARLNaoCaracterizada? "" : "hide" %>">
					<p><label>O projeto geográfico possui ARL não caracterizada. Isso irá inviabilizar o Cadastro Ambiental Rural.</label></p>
				</div>
			</fieldset>
		</div>

		<div class="divSobreposicao <%= Model.IsProcessado ? "" : "hide" %>">
			<fieldset class="block box">
				<legend>Sobreposições da área total da propriedade</legend>
				<% Html.RenderPartial("SobreposicoesPartial", Model.Sobreposicoes); %>
			</fieldset>
		</div>

		<div class="block box ">
			<span class="spnSalvar <%= Model.IsFinalizado|| Model.IsVisualizar ? "hide" : ""%>">
				<input class="btnSalvar floatLeft" type="button" value="Salvar" /></span>
			<span class="cancelarCaixa spanCancelar"><span class="btnModalOu spanOuCancelar <%= Model.IsFinalizado || Model.IsVisualizar ? "hide" : ""%>">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Projeto.EmpreendimentoId, projetoDigitalId = Model.Projeto.ProjetoDigitalId, visualizar = Model.RetornarVisualizar }) %>">Cancelar</a></span>
			<span class="spanBotoes floatRight spanFinalizar <%:( Model.IsEditar && !Model.IsFinalizado) && !Model.IsVisualizar ? "" : "hide" %>">
				<input class="floatLeft btnFinalizar" type="button" value="Finalizar" /></span>
			<span class="spanBotoes floatRight spanAvancar">
				<input class="btnAvancar <%: Model.IsFinalizado && Model.MostrarAvancar ? "" : "hide" %>" type="button" value="Avançar" /></span>
			<span class="spanBotoes floatRight spanRefazer">
				<input class="btnRefazer <%: Model.IsFinalizado && !Model.IsVisualizar ? "" : "hide" %>" type="button" value="Refazer" /></span>
			<span class="spanBotoes floatRight spanRecaregar <%: Model.Projeto.SituacaoId == (int)eProjetoGeograficoSituacao.EmRascunho && !Model.IsVisualizar ? "" : "hide" %>">
				<input class="btnRecarregar" type="button" value="Recarregar" /></span>
			<span class="spanBotoes floatRight spanExcluir <%: Model.Projeto.SituacaoId == (int)eProjetoGeograficoSituacao.EmRascunho && !Model.IsVisualizar ? "" : "hide" %>">
				<input class="btnExluir " type="button" value="Excluir Rascunho" /></span>
		</div>
	</div>
</asp:Content>