<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PecaTecnicaVM>" %>

<div class="block">
	<fieldset class="block box">
		<legend>Processo/Documento</legend>
		<div class="block">
			<div class="coluna20">
				<label>Número de registro *</label>
				<%: Html.TextBox("NumeroProcDoc", "",new { @maxlength = "12", @class = "text txtNumeroProcDoc" })%>
			</div>
			<div class="coluna20">
				<button class="inlineBotao btnVerificarRegistro">Verificar</button>
				<button class="inlineBotao btnLimparRegistro hide">Limpar</button>
			</div>
		</div>
	</fieldset>

	<div class="divRequerimento">

	</div>

	<div class="block box divCancelar">
		<span class="cancelarCaixa"><a class="linkCancelar linkPecaTecnicaLimpar" title="Cancelar" href="<%= Url.Action("GerarPecaTecnica") %>">Cancelar</a></span>
	</div>

	<div class="block divSalvar hide box">
		<input class="btnSalvarPecaTecnica floatLeft " type="button" value="Salvar" />
		<span class="cancelarCaixa"><span class="btnModalOu ">ou </span> <a class="linkCancelar linkConteudoCancelar" title="Cancelar" href="<%= Url.Action("GerarPecaTecnica") %>">Cancelar</a></span>		
		<input class="btnGerarPdf hide floatRight " type="button" value="Gerar PDF" />
	</div>
</div>