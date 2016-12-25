<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRelatorioEspecifico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RelatorioMapaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Relatório MAPA</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/RelatorioEspecifico/relatoriomapa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
			RelatorioMapa.load($('#central'), {
				urlPdf :'<%= Url.Action("PDFRelatorioMapa", "RelatorioEspecifico") %>',
				urlXlsx :'<%= Url.Action("XlsxRelatorioMapa", "RelatorioEspecifico") %>',
				Mensagens: <%= Model.Mensagens %>,
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Relatório MAPA</h1>
		<br />

		<fieldset class="block box">
			<legend></legend>
				<div class="block">
					<div class="coluna92">
							<label for="SetorTipo">Tipo do Relatório</label>
							<%= Html.DropDownList("TipoRelatorio", Model.TipoRelatorio, new { @class = "text ddlTipoRelatorio" })%>
					</div>
				</div>
		        <div class="block hide" style="display: block;">
					<div class="coluna45">
							<label for="DataInicial">Data Inicial</label>
								<%= Html.TextBox("DataInicial","", new { @class = "txtDataInicial text maskData" })%>
					</div>
					<div class="coluna45">
							<label for="DataFinal">Data Final</label>
								<%= Html.TextBox("DataFinal", "" , new { @class = "txtDataFinal text maskData" })%>
					</div>
				</div>

		</fieldset>
        <div class="block box botoesSalvarCancelar">
            <button class="btnRelatorioPDF floatLeft" type="button" value="Relatorio PDF"><span>Relatório em PDF</span></button>
			<button class="btnRelatorioExcel floatLeft" type="button" value="Relatorio Excel"><span>Relatório em Excel</span></button>
			<span class="cancelarCaixa cancelarCaixaPrincipal"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Personalizado", "Relatorios") %>">Cancelar</a></span>
        </div>

    </div>
</asp:Content>