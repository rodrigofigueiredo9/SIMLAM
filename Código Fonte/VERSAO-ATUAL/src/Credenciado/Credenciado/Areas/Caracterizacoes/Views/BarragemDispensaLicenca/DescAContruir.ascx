<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend>Descrição da barragem a contruir</legend>

	<div class="block">
		<div class="coluna80">
			<label for="PerguntaSupressaoAContruir">Haverá supressão de vegetação em Área de Preservação Permanente (APP) para implantação da barragem? *</label>
			<%= Html.RadioButton("PerguntaSupressaoAContruir", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.isSupressaoAPP == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressaoAContruir" })) %> Sim
			<%= Html.RadioButton("PerguntaSupressaoAContruir", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.isSupressaoAPP == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressaoAContruir" })) %> Não
		</div>
	</div>
	<div class="block">
		
	<div class="block">
		<b>Dispositivo de vazão mínima</b> <br />
		<div class="coluna20 divRadioEsconder">
			<label for="MongeTipo">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMinAConstruir", Model.MongeTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlTipoDispositivoVazaoMinAConstruir" })) %>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMin">Diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.construidaConstruir.vazaoMinDiametro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMinAConstruir", maxlength = "14" })) %>
		</div>
	</div>

	<div class="block">
		<b>Dispositivo de vazão máxima</b> <br />
		<div class="coluna20 divRadioEsconder">
			<label for="VertedouroTipo">Tipo *</label>
			<%= Html.DropDownList("VertedouroTipo", Model.VertedouroTiposLst,ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class="ddlTipoDispositivoVazaoMaxAConstruir" })) %>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMax">Largura e altura ou diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMax", Model.Caracterizacao.construidaConstruir.vazaoMaxDiametro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMaxAConstruir", maxlength = "14" })) %>
		</div>
		<br />
	</div>

	<div class="block">
		<b>Previsão de execução da obra (mês/ano) *</b> <br />
		
		<div class="coluna10">
			<label for="PeriodoInicio">Início *</label>
			<%= Html.TextBox("PeriodoInicio", Model.Caracterizacao.construidaConstruir.periodoInicioObra, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskMesAno txtperiodoInicioObra", maxlength = "8" })) %>
		</div>
		<div class="coluna10">
			<label for="PeriodoTermino">Fim *</label>
			<%= Html.TextBox("PeriodoTermino", Model.Caracterizacao.construidaConstruir.periodoTerminoObra, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskMesAno txtperiodoTerminoObra", maxlength = "8" })) %>
		</div>
	</div>
	
</fieldset>