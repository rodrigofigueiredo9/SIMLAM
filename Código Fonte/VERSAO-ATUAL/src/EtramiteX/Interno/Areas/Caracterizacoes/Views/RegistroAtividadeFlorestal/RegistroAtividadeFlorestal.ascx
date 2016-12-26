<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegistroAtividadeFlorestalVM>" %>

<script>
	RegistroAtividadeFlorestal.settings.caracterizacaoID = '<%: Model.Caracterizacao.Id %>';
	RegistroAtividadeFlorestal.settings.empreendimentoID = '<%: Model.Caracterizacao.EmpreendimentoId %>';
	RegistroAtividadeFlorestal.settings.idsTela = <%= Model.IdsTela %>;
	RegistroAtividadeFlorestal.settings.mensagens = <%= Model.Mensagens %>;

	Consumo.settings.urls.validarConsumo = '<%= Url.Action("ValidarConsumo", "RegistroAtividadeFlorestal") %>';
	Consumo.settings.urls.validarFonte = '<%= Url.Action("ValidarFonte", "RegistroAtividadeFlorestal") %>';

	TituloAdicionar.settings.urls.buscarTitulo = '<%= Url.Action("ObterTitulo", "RegistroAtividadeFlorestal") %>';
</script>

<fieldset class="box">
	<div class="block divPossuiNumero">
		<div class="coluna27">
			<div><label for="PossuiNumero">Já possui número de registro? *</label></div>
			<label><%= Html.RadioButton("PossuiNumero", ConfiguracaoSistema.SIM, (Model.Caracterizacao.PossuiNumero.HasValue ? Model.Caracterizacao.PossuiNumero.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoPossuiNumero rbSim" }))%>Sim</label>
			<label><%= Html.RadioButton("PossuiNumero", ConfiguracaoSistema.NAO, (Model.Caracterizacao.PossuiNumero.HasValue ? !Model.Caracterizacao.PossuiNumero.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoPossuiNumero rbNao" }))%>Não</label>
		</div>

		<div class="coluna22">
			<label for="NumeroRegistro">Nº de registro *</label>
			<%= Html.TextBox("NumeroRegistro", (Model.Caracterizacao.PossuiNumero.HasValue ? Model.Caracterizacao.NumeroRegistro : null), ViewModelHelper.SetaDisabled(Model.IsVisualizar || !Model.Caracterizacao.PossuiNumero.GetValueOrDefault(), new { @class = "text maskNumInt txtNumeroRegistro", @maxlength = "7" }))%>
			<input type="hidden" class="hdnNumero" value="<%= Model.Caracterizacao.NumeroRegistro %>" />
		</div>
	</div>
</fieldset>

<fieldset class="block box fsConsumos">
	<legend>Consumo Real</legend>

	<div class="divConteudoConsumos associarMultiplo">
		<div class="asmItens">
			<% foreach (var consumo in Model.ConsumosVM) { %>
				<div class="asmItemContainer" style="border: 0px">
					<% Html.RenderPartial("ConsumoPartial", consumo); %>
				</div>
			<% } %>
		</div>

		<% if (!Model.IsVisualizar) { %>
		<div class="coluna100 negtTop15" style="border: 0px;">
			<p class="floatRight"><button class="btnAsmAdicionar" title="Adicionar Consumo Real">Consumo Real</button></p>
		</div>
		<div class="asmItemTemplateContainer asmItemContainer hide" style="border: 0px">
			<% Html.RenderPartial("ConsumoPartial", Model.ConsumoTemplateVM); %>
		</div>
		<%} %>
	</div>
</fieldset>