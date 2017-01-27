<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConverterDocumentoVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Converter Documento em Processo</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<!-- DEPENDENCIAS DE PESSOA -->
	<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
	<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>

	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Documento/converterDocumento.js") %>"></script>
	<script>
		$(function () {

			Processo.settings.urls.editarInteressado = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
			Processo.settings.urls.editarEmpreendimento = '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>';
			Processo.settings.urls.editarResponsavelModal = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
			Processo.settings.urls.visualizarCheckList = '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>';
			Processo.settings.urls.visualizarChecagemPendencia = '<%= Url.Action("Visualizar", "ChecagemPendencia") %>';
			Processo.settings.urls.pdfRequerimento = '<%= Url.Action("GerarPdf", "Requerimento") %>';
			Processo.settings.Mensagens = <%= Model.Mensagens %>;
			Processo.settings.modo = 3;

			ConverterDocumento.load($('#central'), {
				urls: {
					converter: '<%= Url.Action("ConverterDocumento", "Documento") %>',
					verificar: '<%= Url.Action("ObterDocumentoDados", "Documento") %>',
					avancar:  '<%= Url.Action("Editar", "Processo") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Converter Documento em Processo</h2>
		<br />
		<input type="hidden" class="hdnDocumentoId" value="" />
		<fieldset class="block box">
			<legend>Documento</legend>
				<div class="block">
					<div class="coluna20">
						<label for="txtNumero">Nº de registro *</label>
						<%= Html.TextBox("txtNumero", null, new { @Id = "txtNumero", @maxlength = 12, @class = "text txtNumero" })%>
					</div>

					<div class="coluna15 prepend2">
						<span class="spanLimpar hide"><button type="button" class="inlineBotao btnLimpar" title="">Limpar</button></span>
						<span class="spanVerificar"><button type="button" class="inlineBotao btnVerificar" title="">Verificar</button></span>
					</div>
				</div>
		</fieldset>

		<div class="divDocumentoCovert">

		</div>

		<div class="block box">
			<span class="spanConverter hide"><input class="floatLeft btnConverter" type="button" value="Converter" /></span>
			<span class="cancelarCaixa"><span class="btnModalOu">&nbsp;&nbsp;</span><a class="linkCancelar" href="<%= Url.Action("", "Tramitacao") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
