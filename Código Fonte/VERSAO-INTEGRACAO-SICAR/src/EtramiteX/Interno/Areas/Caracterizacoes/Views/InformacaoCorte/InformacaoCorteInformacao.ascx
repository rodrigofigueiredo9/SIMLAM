<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteInformacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divInformacaoCorteInformacao">
	<input type="hidden" class="hdnInformacaoCorteInformacaoId" value="<%: Model.Entidade.Id %>" />

	<fieldset class="box" id="fsInformacaoCorte">
		<legend>Objeto declarado</legend>

		<%Html.RenderPartial("Especie", Model); %>

		<%Html.RenderPartial("Produto", Model); %>

		<div class="block">
			<div class="coluna17 append6">
				<label for="InformacaoCorteInformacao_DataInformacao_DataTexto">Data da informação *</label>
				<%= Html.TextBox("InformacaoCorteInformacao.DataInformacao.DataTexto", Model.Entidade.DataInformacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataInformacao" }))%>
			</div>

			<div class="coluna23 append6">
				<label for="InformacaoCorteInformacao_ArvoresIsoladasRestantes">Árvores isoladas restantes</label>
				<%= Html.TextBox("InformacaoCorteInformacao.ArvoresIsoladasRestantes", Model.Entidade.ArvoresIsoladasRestantes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtArvoresIsoladasRestantes", @maxlength="5" }))%>
			</div>

			<div class="coluna25">
				<label for="InformacaoCorteInformacao_AreaCorteRestante">Área de corte restante (ha)</label>
				<%= Html.TextBox("InformacaoCorteInformacao.AreaCorteRestante", Model.Entidade.AreaCorteRestante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCorteRestante maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
		</div>
	</fieldset>

	<div class="block box">
		<%if(!Model.IsVisualizar) {%>
		<input class="floatLeft btnSalvarInformacao" type="button" value="Salvar" />
		<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="#">Cancelar</a></span>
		<%} else{%>
		<span class="cancelarCaixa"><a class="linkCancelar" href="#">Cancelar</a></span>
		<%} %>
	</div>
</div>