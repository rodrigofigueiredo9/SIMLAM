<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>


<script>
	$(function () {
		ProjetoGeografico.mensagens = <%= Model.ProjetoGeoVM.Mensagens %>;
		EnviarProjeto.mensagens =  <%= Model.ProjetoGeoVM.MensagensImportador %>;
	    Desenhador.mensagens = <%= Model.ProjetoGeoVM.MensagensDesenhador %>;

	    ProjetoGeografico.settings.isVisualizar = <%= Model.ProjetoGeoVM.IsVisualizar.ToString().ToLower() %>;
	    ProjetoGeografico.settings.isFinalizado = <%= Model.ProjetoGeoVM.IsFinalizado.ToString().ToLower() %>;

		ProjetoGeografico.settings.urls.avancar = '<%= Model.ProjetoGeoVM.UrlAvancar %>';
		ProjetoGeografico.settings.urls.coordenadaGeo = '<%= Url.Action("AreaAbrangenciaFiscPartial", "Mapa", new {area="GeoProcessamento"}) %>';
		ProjetoGeografico.settings.urls.criarParcial = '<%= Url.Action("CriarParcial", "Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.confirmarAlterarArea = '<%= Url.Action("ConfirmarAlteracaoCoordenadas", "Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.cancelarProcessamentoArquivosVet = '<%: Url.Action("CancelarProcessamento","Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.alterarAreaAbrangencia = '<%: Url.Action("AlterarBaseAbrangencia", "Fiscalizacao") %>';

		ProjetoGeografico.settings.urls.excluir = '<%: Url.Action("ExcluirRascunho","Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.refazer = '<%: Url.Action("Refazer","Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.recarregar = '<%: Url.Action("Recarregar","Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.finalizar = '<%: Url.Action("Finalizar", "Fiscalizacao") %>';
		ProjetoGeografico.settings.urls.salvar = '<%: Url.Action("ProjetoGeograficoSalvar", "Fiscalizacao") %>';

		BaseReferencia.settings.urls.atualizarArqVetoriais = '<%= Url.Action("ObterSituacao", "Fiscalizacao") %>';
		BaseReferencia.settings.urls.gerarArquivoVetorial = '<%= Url.Action("GerarArquivoVetorial", "Fiscalizacao") %>';
		BaseReferencia.settings.urls.gerarArquivoMosaio = '<%= Url.Action("ObterOrtoFotoMosaico", "Fiscalizacao") %>';
		BaseReferencia.settings.urls.baixarArquivoModelo = '<%= Url.Action("BaixarArquivoModelo", "Fiscalizacao") %>';
		BaseReferencia.settings.urls.baixarArquivoVetorial = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';
		BaseReferencia.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';

	    BaseReferencia.settings.urls.baixarArquivoOrtoMosaico = '<%= Model.ProjetoGeoVM.UrlBaixarOrtofoto %>';
	    BaseReferencia.settings.urls.salvarArquivoOrtoMosaico = '<%: Url.Action("SalvarOrtofoto","Fiscalizacao") %>';

		EnviarProjeto.settings.urls.EnviarProjeto = '<%: Url.Action("arquivo","arquivo", new { area = ""}) %>';
		EnviarProjeto.settings.urls.EnviarParaProcessar = '<%: Url.Action("Importar","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.ArquivosProcessados = '<%: Url.Action("baixarArquivoProcessado","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.cancelarProcessamento = '<%= Url.Action("CancelarProcessamento","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.reenviarArquivo = '<%= Url.Action("ReenviarArquivoImportador","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.confirmarReenviarArquivo = '<%= Url.Action("ConfirmarReenviarArquivoImportador","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.obterSituacao = '<%= Url.Action("ObterSituacao","Fiscalizacao") %>';
		EnviarProjeto.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';

		Desenhador.settings.urls.confirmarReenviarArquivo = '<%= Url.Action("ConfirmarReenviarArquivoImportador","Fiscalizacao") %>';
		Desenhador.settings.urls.processarArquivoDesenhador='<%= Url.Action("ProcessarDesenhador","Fiscalizacao") %>';
		Desenhador.settings.urls.obterSituacao = '<%= Url.Action("ObterSituacaoDesenhador", "Fiscalizacao") %>';
		Desenhador.settings.urls.reprocessar = '<%= Url.Action("ReprocessarDesenhador","Fiscalizacao") %>';
		Desenhador.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>';
		Desenhador.settings.urls.desenhador	= '<%= Url.Action("DesenhadorPartial", "Mapa", new {area="GeoProcessamento"}) %>';
		Desenhador.settings.urls.cancelarProcessamento = '<%= Url.Action("CancelarProcessamento","Fiscalizacao") %>';


		BaseReferencia.settings.situacoesValidas = <%= Model.ProjetoGeoVM.SituacoesValidasJson %>;
		EnviarProjeto.settings.situacoesValidas = <%= Model.ProjetoGeoVM.SituacoesValidasJson %>;
		Desenhador.settings.situacoesValidas = <%= Model.ProjetoGeoVM.SituacoesValidasJson %>;

		ProjetoGeografico.settings.urls.obterMerge = '<%: Url.Action("ObterMerge", "Fiscalizacao") %>';

		ProjetoGeografico.settings.fiscalizacaoEstaDentroAreaAbrangencia = <%= Model.ProjetoGeoVM.Projeto.FiscalizacaoEstaDentroAreaAbrangencia.ToString().ToLower() %>;

		FiscalizacaoProjetoGeografico.urlObter = '<%= Url.Action("ProjetoGeografico","Fiscalizacao") %>';

	});
</script>

<div class="projetoGeograficoContainer">

<div class="box block">
	<div class="coluna25">
		<label>Sistema de Coordenada *</label>
		<%= Html.TextBox("SistemaCoordenada", "SIRGAS 2000 - UTM zone - 24S", new { @disabled = "disabled", @class = "text disabled txtSistemaCoordenada" })%>
	</div>
	<div class="coluna22 prepend1">
		<label>Nível de precisão</label>
		<%= Html.DropDownList("Projeto.NivelPrecisaoId", Model.ProjetoGeoVM.NiveisPrecisao, ViewModelHelper.SetaDisabled(Model.ProjetoGeoVM.IsFinalizado || Model.ProjetoGeoVM.IsVisualizar, new { @class = "text ddlNivel" }))%>
	</div>
</div>

<fieldset class="block box" id="AreaAbrangencia_Container">
	<legend>Área de abrangência</legend>

	<% if (!Model.ProjetoGeoVM.IsVisualizar) { %>
	<div class="block divBotoesAreaAbrangencia <%= Model.ProjetoGeoVM.IsFinalizado ? "hide" : ""%>">
		<button class="btnSelecionarCoordenada  <%= (Model.ProjetoGeoVM.IsEditar) ? "hide" : "" %>" >Selecionar</button>
		<button class="btnAlterarCoordenada  <%= !(Model.ProjetoGeoVM.IsEditar) ? "hide" : "" %>"> Alterar a área</button>
		<button class="btnObterCoordenadaAuto" title=" Gerar área de abrangência a partir do ponto da fiscalização"> Automático</button>
	</div>
	<% } %>

	<% Model.ProjetoGeoVM.Projeto.CorrigirMbr(); %>

	<div class="divCoordenada <%= Model.ProjetoGeoVM.IsFinalizado ? "hide" : "" %>">
		<fieldset class="boxBranca coluna44 prepend2 ">
			<legend>Coordenada 1</legend>
			<div class="coluna45 prepend3">
				<label>Mínimo X *</label>
				<%= Html.TextBox("txtMenorX", Model.ProjetoGeoVM.MenorX, new { @disabled = "disabled", @class = "text txtMenorX disabled" })%>
			</div>
			<div class="coluna45 prepend3">
				<label>Máximo Y*</label>
				<%= Html.TextBox("txtMaiorY", Model.ProjetoGeoVM.MaiorY, new { @disabled = "disabled", @class = "text txtMaiorY disabled" })%>
			</div>
		</fieldset>

		<fieldset class="boxBranca coluna43 prepend2">
			<legend>Coordenada 2</legend>
			<div class="coluna45 prepend3">
				<label>Máximo X*</label>
				<%= Html.TextBox("txtMaiorX", Model.ProjetoGeoVM.MaiorX, new { @disabled = "disabled", @class = "text txtMaiorX disabled" })%>
			</div>
			<div class="coluna45 prepend3">
				<label>Mínimo Y*</label>
				<%= Html.TextBox("txtMenorY", Model.ProjetoGeoVM.MenorY, new { @disabled = "disabled", @class = "text txtMenorY disabled" })%>
			</div>
		</fieldset>
	</div>
</fieldset>

<fieldset class="block box fsMecanismo <%= Model.ProjetoGeoVM.IsFinalizado ? "hide" : "" %>">
	<legend>Mecanismo de elaboração</legend>
	<label><input type="radio" class="radioTiPoMecanismo" name="mecanismo" value="1" <%= Model.ProjetoGeoVM.IsFinalizado || Model.ProjetoGeoVM.IsVisualizar ? "disabled=\"disabled\"" : ""%> <%= Model.ProjetoGeoVM.IsImportadorShape ? "checked=\"checked\"" : ""%>/>Importador de shapes</label>
	<label><input type="radio" class="radioTiPoMecanismo" name="mecanismo" value="2" <%= Model.ProjetoGeoVM.IsFinalizado || Model.ProjetoGeoVM.IsVisualizar ? "disabled=\"disabled\"" : ""%>  <%= Model.ProjetoGeoVM.IsDesenhador ? "checked=\"checked\"" : ""%>/>Desenhador</label>

	<input type="hidden" class="hdnProjetoId" value="<%= Model.ProjetoGeoVM.Projeto.Id %>" />
	<input type="hidden" class="hdnFiscNorthing" value="<%= Model.ProjetoGeoVM.Projeto.FiscalizacaoNorthing%>" />
	<input type="hidden" class="hdnFiscEasting" value="<%= Model.ProjetoGeoVM.Projeto.FiscalizacaoEasting %>" />
	<input type="hidden" class="hdnArquivoEnviadoTipo" value="<%= Model.ProjetoGeoVM.ArquivoEnviadoTipo%>" />
	<input type="hidden" class="hdnProjetoSituacaoId" value="<%= Model.ProjetoGeoVM.Projeto.SituacaoId%>" />
</fieldset>

<div class="divImportador <%= Model.ProjetoGeoVM.IsImportadorShape ? "" : "hide"%>">
	<fieldset class="block box">
		<legend>Importador de Shapes</legend>

		<fieldset class="block boxBranca">
			<legend>Base de referência para download</legend>
				<% Html.RenderPartial("ProjetoGeoBaseReferenciaPartial", Model.ProjetoGeoVM.BaseReferencia); %>
		</fieldset>

		<fieldset class="block boxBranca fsEnviarProjeto <%= Model.ProjetoGeoVM.Projeto.Arquivos.Count > 0? "" : "hide"%>">
			<legend>Enviar projeto geográfico</legend>
			<% Html.RenderPartial("ProjetoGeoEnviarProjetoPartial", Model.ProjetoGeoVM.EnviarProjeto); %>
		</fieldset>
	</fieldset>
</div>

<div class="divDesenhador <%= Model.ProjetoGeoVM.IsDesenhador ? "" : "hide"%>">
	<fieldset class="block box">
		<legend>Desenhador</legend>
		<% Html.RenderPartial("ProjetoGeoDesenhadorPartial", Model.ProjetoGeoVM.Desenhador); %>
	</fieldset>

</div>

</div>