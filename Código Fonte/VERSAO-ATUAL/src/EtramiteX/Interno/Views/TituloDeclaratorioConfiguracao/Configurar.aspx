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
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</h1>
		<br />
		<fieldset class="block box">
			<legend>Área alagada na soleira do vertedouro (ha)</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Configurar_MaximoAreaAlagada">Valor máximo atual:</label>
					<%= Html.TextBox("Configurar_MaximoAreaAlagada", Model.MaximoAreaAlagada, new { @class = "text maskDecimalPonto disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20">
					<label for="Configurar_MaximoAreaAlagadaNovo">Novo valor máximo:</label>
					<%= Html.TextBox("Configurar_MaximoAreaAlagadaNovo", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorMaximo", @maxlength ="5" }))%>
				</div>
			</div>
		</fieldset>
		<fieldset class="block box">
			<legend>Volume armazenado (m³)</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Configurar_MaximoVolumeArmazenado">Valor máximo atual:</label>
					<%= Html.TextBox("Configurar_MaximoVolumeArmazenado", Model.MaximoVolumeArmazenado, new { @class = "text maskDecimalPonto disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20">
					<label for="Configurar_MaximoVolumeArmazenadoNovo">Novo valor máximo:</label>
					<%= Html.TextBox("Configurar_MaximoVolumeArmazenadoNovo", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorMaximo", @maxlength ="8" }))%>
				</div>
			</div>
		</fieldset>
	</div>
</asp:Content>