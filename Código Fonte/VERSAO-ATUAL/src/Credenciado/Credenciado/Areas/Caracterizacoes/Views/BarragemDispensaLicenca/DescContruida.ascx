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
			<%= Html.RadioButton("isSupressaoAPP", true, Model.Caracterizacao.construidaConstruir.isSupressaoAPP == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbSupressaoAPP" })) %> Sim
			<%= Html.RadioButton("isSupressaoAPP", false, Model.Caracterizacao.construidaConstruir.isSupressaoAPP == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbSupressaoAPP" })) %> Não
		</div>
		<div class="coluna66">
			<label for="isDemarcacaoAPP">Há faixa demarcada como Área de Preservação Permanente (APP) no entorno do reservatório? *</label>
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.isDemarcacaoAPP == 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Sim
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.isDemarcacaoAPP == 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Não
		</div>
		<div class="coluna29  isDemarcacaoAPPNaoSeAplica">
			<%= Html.RadioButton("isDemarcacaoAPP", ConfiguracaoSistema.Dispensado, Model.Caracterizacao.construidaConstruir.isDemarcacaoAPP == 2, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDemarcaoAPP" })) %> Não se aplica (somente para barragens menores que 1 hectare de lâmina d’água onde não houve supressão de vegetação em APP para sua implantação).
		</div>
	</div>
	<div class="block boxApp <%= (Model.Caracterizacao.construidaConstruir.isDemarcacaoAPP == 1) ? "" : "hide" %> ">
		
		<div class="coluna40">
			<label for="larguraDemarcada">Qual a largura demarcada (m)? *</label>
			<%= Html.TextBox("larguraDemarcada", Model.Caracterizacao.construidaConstruir.larguraDemarcada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraDemarcada", maxlength = "10" })) %>
		</div>
		<div class="block coluna80">
			<div class="coluna80">
				<label for="larguraDemarcadaLegislacao">A largura demarcada atende à legislaçao? *</label>
				<%= Html.RadioButton("larguraDemarcadaLegislacao", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.larguraDemarcadaLegislacao == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbLarguraDemarcadaLegislacao" })) %> Sim
				<%= Html.RadioButton("larguraDemarcadaLegislacao", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.larguraDemarcadaLegislacao == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbLarguraDemarcadaLegislacao" })) %> Não
			</div>
		</div>
		<div class="blockv coluna80">
			<div class="coluna80">
				<label for="FaixaCercada">Está cercada? *</label>
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.faixaCercada == 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Sim
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.faixaCercada == 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Não
				<%= Html.RadioButton("faixaCercada", ConfiguracaoSistema.Dispensado, Model.Caracterizacao.construidaConstruir.faixaCercada == 2, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFaixaCercada" })) %> Parcialmente
			</div>
		</div>
		<div class="coluna60">
			<label for="DescricaoDesenvolvimento">Descreva o estágio de desenvolvimento, a caracterização da vegetação na faixa de APP e as medidas necessárias para atendimento à legislação. *</label>
			<%= Html.TextArea("DescricaoDesenvolvimento", Model.Caracterizacao.construidaConstruir.descricaoDesenvolvimentoAPP, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoDesenvolvimento" })) %>
		</div>
	</div>

	<div class="block">
		<div class="coluna80">
			<label for="BarramentoNormas">O barramento está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("BarramentoNormas", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.barramentoNormas == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarramentoNormas" })) %> Sim
			<%= Html.RadioButton("BarramentoNormas", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.barramentoNormas == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarramentoNormas" })) %> Não
		</div><br />		
	</div>
	<div class="block">
		<div class="coluna60 AdequacoesDimensionamentoBarramento  <%= (Model.Caracterizacao.construidaConstruir.barramentoNormas == false) ? "" : "hide"%>">
			<label for="AdequacoesDimensionamentoBarramento">Quais adequações serão realizadas? *</label>
			<%= Html.TextArea("AdequacoesDimensionamentoBarramento", Model.Caracterizacao.construidaConstruir.barramentoAdequacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoBarramento ", width ="200" })) %>
		</div>
		<br />
	</div>
	 
	<div class="block">
		<b>Dispositivo de vazão mínima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMin">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMin", Model.MongeTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMin" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMin">Diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.construidaConstruir.vazaoMinDiametro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "22" })) %>
		</div>
		<br />
	</div>
	<div class="block">
		<div class="coluna40">
			<label for="VazaoMinInstalado">Já está instalado? *</label>
			<%= Html.RadioButton("VazaoMinInstalado", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.vazaoMinInstalado == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinInstalado" })) %> Sim
			<%= Html.RadioButton("VazaoMinInstalado", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.vazaoMinInstalado == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinInstalado" })) %> Não
		</div>
	</div>
	<br />
	<div class="block">
		<div class="coluna80 vazaoMinNormas <%= (Model.Caracterizacao.construidaConstruir.vazaoMinInstalado == true) ? "" : "hide"%>">
			<label for="VazaoMinNormas">O dispositivo de vazão máxima está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("VazaoMinNormas", ConfiguracaoSistema.SIM,  Model.Caracterizacao.construidaConstruir.vazaoMinNormas == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinNormas" })) %> Sim
			<%= Html.RadioButton("VazaoMinNormas", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.vazaoMinNormas == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMinNormas" })) %> Não
		</div>
	</div>
	<div class="block">		
		<div class="coluna60 AdequacoesDimensionamentoVazaoMin <%= (Model.Caracterizacao.construidaConstruir.vazaoMinNormas == false || Model.Caracterizacao.construidaConstruir.vazaoMinInstalado == false) ? "" : "hide"%>">
			<label for="AdequacoesDimensionamentoVazaoMin">Quais adequações serão realizadas? *</label>
			<%= Html.TextArea("AdequacoesDimensionamentoVazaoMin", Model.Caracterizacao.construidaConstruir.barramentoAdequacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMin " })) %>
		</div>
		<br />
	</div>

	<div class="block">
		<b>Dispositivo de vazão máxima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMax">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMax", Model.VertedouroTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMax" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMax">Largura e altura ou diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMax", Model.Caracterizacao.construidaConstruir.vazaoMaxDiametro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDiametroTubulacaoVazaoMax", maxlength = "20" })) %>
		</div>
		<br />
	</div>
	<div class="block">		
		<div class="coluna40">
			<label for="VazaoMinInstalado">Já está instalado? *</label>
			<%= Html.RadioButton("VazaoMaxInstalado", ConfiguracaoSistema.SIM,  Model.Caracterizacao.construidaConstruir.vazaoMaxInstalado == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxInstalado" })) %> Sim
			<%= Html.RadioButton("VazaoMaxInstalado", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.vazaoMaxInstalado == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxInstalado" })) %> Não
		</div>
		<br />
		<div class="coluna80 vazaoMaxNormas <%= (Model.Caracterizacao.construidaConstruir.vazaoMaxInstalado == true) ? "" : "hide"%>">
			<label for="VazaoMinNormas">O dispositivo de vazão máxima está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("VazaoMaxNormas", ConfiguracaoSistema.SIM, Model.Caracterizacao.construidaConstruir.vazaoMaxNormas == true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxNormas" })) %> Sim
			<%= Html.RadioButton("VazaoMaxNormas", ConfiguracaoSistema.NAO, Model.Caracterizacao.construidaConstruir.vazaoMaxNormas == false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbVazaoMaxNormas" })) %> Não
		</div>
		<div class="coluna60 AdequacoesDimensionamentoVazaoMax <%= (Model.Caracterizacao.construidaConstruir.vazaoMaxNormas == false || Model.Caracterizacao.construidaConstruir.vazaoMaxInstalado == false) ? "" : "hide"%>">
			<label for="AdequacoesDimensionamentoVazaoMax">Quais adequações serão realizadas? *</label>
			<%= Html.TextArea("AdequacoesDimensionamentoVazaoMax", Model.Caracterizacao.construidaConstruir.vazaoMaxAdequacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMax ", @width ="200%" })) %>
		</div>
	</div>
</fieldset>