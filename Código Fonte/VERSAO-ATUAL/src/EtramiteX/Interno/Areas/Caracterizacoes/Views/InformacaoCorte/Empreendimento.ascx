<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="informacaoCorteInformacaoEspecieContainer">

	<fieldset class="box" id="">
		<legend>Empreendimento</legend>
		<div class="block">
			<div class="coluna17 ">
				<label for="InformacaoCorteInformacao_DataInformacao_DataTexto">Código</label>
				<%= Html.TextBox("InformacaoCorteInformacao.DataInformacao.DataTexto", Model.Caracterizacao.Emprendimento.Codigo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataInformacao" }))%>
			</div>

			<div class="coluna32 ">
				<label for="InformacaoCorteInformacao_ArvoresIsoladasRestantes">Denominação</label>
				<%= Html.TextBox("InformacaoCorteInformacao.ArvoresIsoladasRestantes", Model.Caracterizacao.Emprendimento.Denominador, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtArvoresIsoladasRestantes", @maxlength="5" }))%>
			</div>

			<div class="coluna25">
				<label for="InformacaoCorteInformacao_AreaCorteRestante">Área do imóvel (ha)</label>
				<%= Html.TextBox("InformacaoCorteInformacao.AreaCorteRestante", Model.Caracterizacao.Emprendimento.AreaImovelHA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCorteRestante maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna17">
				<label for="InformacaoCorteInformacao_DataInformacao_DataTexto">Zona de localização</label>
				<%= Html.TextBox("InformacaoCorteInformacao.DataInformacao.DataTexto", Model.Caracterizacao.Emprendimento.ZonaLocalizacaoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataInformacao" }))%>
			</div>

			<div class="coluna8 ">
				<label for="InformacaoCorteInformacao_ArvoresIsoladasRestantes">UF</label>
				<%= Html.TextBox("InformacaoCorteInformacao.ArvoresIsoladasRestantes", Model.Caracterizacao.Emprendimento.Uf, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtArvoresIsoladasRestantes", @maxlength="5" }))%>
			</div>

			<div class="coluna23">
				<label for="InformacaoCorteInformacao_AreaCorteRestante">Município</label>
				<%= Html.TextBox("InformacaoCorteInformacao.AreaCorteRestante", Model.Caracterizacao.Emprendimento.Municipio, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCorteRestante maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
			<div class="coluna25">
				<label for="InformacaoCorteInformacao_AreaCorteRestante">Área de Floresta Plantada</label>
				<%= Html.TextBox("InformacaoCorteInformacao.AreaCorteRestante", Model.Caracterizacao.Emprendimento.AreaFlorestaPlantadaHA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCorteRestante maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
		</div>
	</fieldset>
</div>
<br />