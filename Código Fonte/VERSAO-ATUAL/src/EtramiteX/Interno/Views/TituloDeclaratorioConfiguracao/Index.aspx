<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfigurarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/TituloDeclaratorioConfiguracao/configurar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			TituloDeclaratorioConfiguracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorioConfiguracao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</h1>
		<br />
		<%= Html.Hidden("Configurar_Id", Model.Configuracao.Id, new { @class = "hdnId"}) %>
		<fieldset class="block box">
			<legend>Área alagada na soleira do vertedouro (ha)</legend>
			<div class="block">
				<div class="coluna20 append2">
					<label for="Configurar_MaximoAreaAlagada">Valor máximo atual:</label>
					<%= Html.TextBox("Configurar_MaximoAreaAlagada", Model.Configuracao.MaximoAreaAlagada.ToStringTrunc(2), new { @class = "text maskDecimalPonto disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20">
					<label for="Configurar_MaximoAreaAlagadaNovo">Novo valor máximo:</label>
					<%= Html.TextBox("Configurar_MaximoAreaAlagadaNovo", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorMaximoAreaAlagada", @maxlength ="18" }))%>
				</div>
			</div>
		</fieldset>
		<fieldset class="block box">
			<legend>Volume armazenado (m³)</legend>
			<div class="block">
				<div class="coluna20 append2">
					<label for="Configurar_MaximoVolumeArmazenado">Valor máximo atual:</label>
					<%= Html.TextBox("Configurar_MaximoVolumeArmazenado", Model.Configuracao.MaximoVolumeArmazenado.ToStringTrunc(2), new { @class = "text maskDecimalPonto disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20">
					<label for="Configurar_MaximoVolumeArmazenadoNovo">Novo valor máximo:</label>
					<%= Html.TextBox("Configurar_MaximoVolumeArmazenadoNovo", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorMaximoVolumeArmazenado", @maxlength ="18" }))%>
				</div>
			</div>
		</fieldset>
		<fieldset class="block box">
			<legend>Condicionantes</legend>
			<div class="block">
				<div class="coluna40 inputFileDiv">
					<label>Condicionante:Barragens sem APP</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Configuracao.BarragemSemAPP.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Configuracao.BarragemSemAPP.Nome) ? "hide" : "" %> txtArquivoSemAPPNome"><%= Html.Encode(Model.Configuracao.BarragemSemAPP.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoSemAPPJson" value="<%= Html.Encode(Model.BarragemSemAPPJSon) %>" />
					<span class="spanInputFileSemAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemSemAPP.Nome) ? "" : "hide" %>">
						<input type="file" id="fileBarragemSemAPP" class="inputFileSemAPP" style="display: block" name="file" />
					</span>
				</div>
				<div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArqSemAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemSemAPP.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArqSemAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemSemAPP.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
			</div><br />
			<div class="block">
				<div class="coluna40 inputFileDiv">
					<label>Condicionante:Barragens com APP</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Configuracao.BarragemComAPP.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Configuracao.BarragemComAPP.Nome) ? "hide" : "" %> txtArquivoComAPPNome"><%= Html.Encode(Model.Configuracao.BarragemComAPP.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoComAPPJson" value="<%= Html.Encode(Model.BarragemComAPPJSon) %>" />
					<span class="spanInputFileComAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemComAPP.Nome) ? "" : "hide" %>">
						<input type="file" id="fileBarragemComAPP" class="inputFileComAPP" style="display: block" name="file" />
					</span>
				</div>
				<div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArqComAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemComAPP.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArqComAPP <%= string.IsNullOrEmpty(Model.Configuracao.BarragemComAPP.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
			</div>
		</fieldset>
		
		<div class="block box btnTituloContainer">
			<input class="btnSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>