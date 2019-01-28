<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend>Descrição da barragem contruída</legend>

	<div class="block">
		<div class="coluna80">
			<label for="isSupressaoAPP">Houve supressão de vegetação em Área de Preservação Permanente (APP) para implantação da barragem? *</label>
			<%= Html.RadioButton("isSupressaoAPP", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbSupressaoAPP" })) %> Sim
			<%= Html.RadioButton("isSupressaoAPP", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbSupressaoAPP" })) %> Não
		</div>
		<div class="coluna66">
			<label for="isDemarcacaoAPP">Há faixa demarcada como Área de Preservação Permanente (APP) no entorno do reservatório? *</label>
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Sim
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Não
		</div>
		<div class="coluna20">
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.Dispensado, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Não se aplica (somente para barragens menores que 1 hectare de lâmina d’água onde não houve supressão de vegetação em APP para sua implantação).
		</div>
	</div>
	<div class="block boxApp hide">
		
		<div class="coluna40">
			<label for="larguraDemarcada">Qual a largura demarcada (m)? *</label>
			<%= Html.TextBox("larguraDemarcada", Model.Caracterizacao.construidaConstruir.larguraDemarcada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraDemarcada", maxlength = "22" })) %>
		</div>
		<div class="block coluna80">
			<div class="coluna80">
				<label for="larguraDemarcadaLegislacao">A largura demarcada atende à legislaçao? *</label>
				<%= Html.RadioButton("larguraDemarcadaLegislacao", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.larguraDemarcadaLegislacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbLarguraDemarcadaLegislacao" })) %> Sim
				<%= Html.RadioButton("larguraDemarcadaLegislacao", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.larguraDemarcadaLegislacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbLarguraDemarcadaLegislacao" })) %> Não
			</div>
		</div>
		<div class="blockv coluna80">
			<div class="coluna80">
				<label for="FaixaCercada">Está cercada? *</label>
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Sim
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Não
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.Dispensado, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Parcialmente
			</div>
		</div>
		<div class="coluna86">
			<label for="DescricaoDesenvolvimento">Descreva o estágio de desenvolvimento, a caracterização da vegetação na faixa de APP e as medidas necessárias para atendimento à legislação. *</label>
			<%= Html.TextBox("DescricaoDesenvolvimento", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoDesenvolvimento", maxlength = "300" })) %>
		</div>
	</div>

	<div class="block">
		<div class="coluna80">
			<label for="BarramentoNormas">O barramento está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("BarramentoNormas", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarramentoNormas" })) %> Sim
			<%= Html.RadioButton("BarramentoNormas", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarramentoNormas" })) %> Não
		</div><br />
		<div class="coluna40 AdequacoesDimensionamentoBarramento hide">
			<label for="AdequacoesDimensionamentoBarramento">Quais adequações serão realizadas? *</label>
			<%= Html.TextBox("AdequacoesDimensionamentoBarramento", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoBarramento", maxlength = "300" })) %>
		</div>
	</div>
	 <br /> <br />
	<div class="block">
		<b>Dispositivo de vazão mínima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMin">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMin", Model.MongeTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMin" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMin">Diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "22" })) %>
		</div>
		<br />
	</div>
	<div class="block">
		<div class="coluna40">
			<label for="VazaoMinInstalado">Já está instalado *</label>
			<%= Html.RadioButton("VazaoMinInstalado", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinInstalado" })) %> Sim
			<%= Html.RadioButton("VazaoMinInstalado", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinInstalado" })) %> Não
		</div>
	</div>
	<br />
	<div class="block">
		<div class="coluna80 vazaoMinNormas hide">
			<label for="VazaoMinNormas">O dispositivo de vazão mínima está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("VazaoMinNormas", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinNormas" })) %> Sim
			<%= Html.RadioButton("VazaoMinNormas", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinNormas" })) %> Não
		</div>
		<div class="coluna40 AdequacoesDimensionamentoVazaoMin hide">
			<label for="AdequacoesDimensionamentoVazaoMin">Quais adequações serão realizadas? *</label>
			<%= Html.TextBox("AdequacoesDimensionamentoVazaoMin", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMin", maxlength = "300" })) %>
		</div>
	</div>
	 <br /> <br />
	<div class="block">
		<b>Dispositivo de vazão máxima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMax">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMax", Model.VertedouroTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMax" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMax">Largura e altura ou diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMax", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMax", maxlength = "22" })) %>
		</div>
		<br />
	</div>
	<div class="coluna40">
		<label for="VazaoMinInstalado">Já está instalado *</label>
		<%= Html.RadioButton("VazaoMaxInstalado", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxInstalado" })) %> Sim
		<%= Html.RadioButton("VazaoMaxInstalado", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxInstalado" })) %> Não
	</div>
	<br />
	<div class="coluna80 vazaoMaxNormas hide">
		<label for="VazaoMinNormas">O dispositivo de vazão mínima está dimensionado de acordo com as normas técnicas e legais? *</label>
		<%= Html.RadioButton("VazaoMaxNormas", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxNormas" })) %> Sim
		<%= Html.RadioButton("VazaoMaxNormas", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxNormas" })) %> Não
	</div>
	<div class="coluna40 AdequacoesDimensionamentoVazaoMax hide">
		<label for="AdequacoesDimensionamentoVazaoMax">Quais adequações serão realizadas? *</label>
		<%= Html.TextBox("AdequacoesDimensionamentoVazaoMax", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMax", maxlength = "300" })) %>
	</div>
</fieldset>