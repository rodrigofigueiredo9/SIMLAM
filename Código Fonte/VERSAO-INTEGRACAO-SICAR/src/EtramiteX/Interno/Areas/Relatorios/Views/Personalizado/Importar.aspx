<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PersonalizadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Importar Relatório Personalizado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizadoImportar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			PersonalizadoImportar.load($('#central'), {
				mensagens: <%= Model.Mensagens %>,
				urls: {
					importar: '<%= Url.Action("Importar", "Personalizado") %>',
					importarSalvar: '<%= Url.Action("ImportarSalvar", "Personalizado") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Importar Relatório Personalizado</h1>		<br />		<div class="block box">
			<div class="block coluna95">
				<label for="Nome">Nome *</label>
				<%= Html.TextBox("Nome","", new { @class = "text txtNome", maxlength = "80" })%>
			</div>

			<div class="block coluna95">
				<label for="Descricao">Descrição *</label><br />
				<%= Html.TextArea("Descricao", "", new { @class = "text media txtDescricao", maxlength = "500" })%>
			</div>
		</div>		<fieldset class="block box">
			<legend>Arquivo</legend>
			<div class="block">
				<span class="floatLeft inputFileDiv coluna82">
					<span class="spanInputFile"><input type="file" class="inputFile" id="inputFile" style="display: block; width: 100%" name="file" /></span>
				</span>
			</div>
		</fieldset>
		<div class="block box margemDTop">
			<input type="button" class="btnImportar floatLeft" value="Importar Relatório" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>