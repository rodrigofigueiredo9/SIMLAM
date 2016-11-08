	<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TramitacaoArquivoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Arquivo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Tramitacao/arquivo.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			TramitacaoArquivo.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ArquivoSalvar", "Tramitacao") %>',
					validarExcluirEstante: '<%= Url.Action("ArquivoValidarExcluirEstante", "Tramitacao") %>',
					validarExcluirPrateleira: '<%= Url.Action("ArquivoValidarExcluirPrateleira", "Tramitacao") %>'
				},
				Mensagens: <%=  Model.Mensagens %>,
				id: '<%= Model.TramitacaoArquivo.Id %>'
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Arquivo</h1>
		<br />
		<% Html.RenderPartial("ArquivoPartial");%>

		<div class="block box">
			<input id="salvar" type="submit" value="Salvar" class="floatLeft btnTramitacaoArqSalvar"/>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> 
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("ArquivoListar") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>