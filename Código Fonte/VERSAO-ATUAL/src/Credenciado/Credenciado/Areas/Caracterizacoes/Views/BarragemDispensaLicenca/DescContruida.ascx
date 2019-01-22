<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend>Descrição da barragem contruída</legend>

	<div class="block">
		<div class="coluna80">
			<label for="PerguntaSupressao">Houve supressão de vegetação em Área de Preservação Permanente (APP) para implantação da barragem? *</label>
			<%= Html.RadioButton("PerguntaSupressao", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressao" })) %> Sim
			<%= Html.RadioButton("PerguntaSupressao", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressao" })) %> Não
		</div>
		<div class="coluna66">
			<label for="PerguntaFaixaDemarcada">Há faixa demarcada como Área de Preservação Permanente (APP) no entorno do reservatório? *</label>
			<%= Html.RadioButton("PerguntaFaixaDemarcada", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaFaixaDemarcada" })) %> Sim
			<%= Html.RadioButton("PerguntaFaixaDemarcada", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaFaixaDemarcada" })) %> Não
		</div>
		<div class="coluna20">
			<%= Html.RadioButton("PerguntaFaixaDemarcada", ConfiguracaoSistema.Dispensado, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaFaixaDemarcada" })) %> Não se aplica (somente para barragens menores que 1 hectare de lâmina d’água onde não houve supressão de vegetação em APP para sua implantação).
		</div>
	</div>
	<div class="block boxApp hide">
		
		<div class="coluna40">
			<label for="AreaAlagada">Qual a largura demarcada (m)? *</label>
			<%= Html.TextBox("AreaAlagada", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaAlagada", maxlength = "14" })) %>
		</div>
		<div class="block coluna80">
			<div class="coluna80">
				<label for="PerguntaSupressao">A largura demarcada atende à legislaçao? *</label>
				<%= Html.RadioButton("PerguntaSupressao", String.Empty, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressao" })) %> Sim
				<%= Html.RadioButton("PerguntaSupressao", String.Empty, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaSupressao" })) %> Não
			</div>
		</div>
		<div class="blockv coluna80">
			<div class="coluna80">
				<label for="PerguntaCercada">Está cercada? *</label>
				<%= Html.RadioButton("PerguntaCercada", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaCercada" })) %> Sim
				<%= Html.RadioButton("PerguntaCercada", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaCercada" })) %> Não
				<%= Html.RadioButton("PerguntaCercada", ConfiguracaoSistema.Dispensado, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaCercada" })) %> Parcialmente
			</div>
		</div>
		<div class="coluna86">
			<label for="DescricaoDesenvolvimento">Descreva o estágio de desenvolvimento, a caracterização da vegetação na faixa de APP e as medidas necessárias para atendimento à legislação. *</label>
			<%= Html.TextBox("DescricaoDesenvolvimento", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoDesenvolvimento", maxlength = "14" })) %>
		</div>
	</div>

	<div class="block">
		<div class="coluna80">
			<label for="PerguntaBarramentoDimensionado">O barramento está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("PerguntaBarramentoDimensionado", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaBarramentoDimensionado" })) %> Sim
			<%= Html.RadioButton("PerguntaBarramentoDimensionado", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaBarramentoDimensionado" })) %> Não
		</div><br />
		<div class="coluna40 AdequacoesDimensionamentoBarramento hide">
			<label for="AdequacoesDimensionamentoBarramento">Quais adequações serão realizadas? *</label>
			<%= Html.TextBox("AdequacoesDimensionamentoBarramento", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoBarramento", maxlength = "14" })) %>
		</div>
	</div>
	 <br /> <br />
	<div class="block">
		<b>Dispositivo de vazão mínima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMin">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMin", Model.Atividades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMin" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMin">Diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
		</div>
		<br />
	</div>
	<div class="block">
		<div class="coluna40">
			<label for="PerguntaVazaoMinInstalado">Já está instalado *</label>
			<%= Html.RadioButton("PerguntaVazaoMinInstalado", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMinInstalado" })) %> Sim
			<%= Html.RadioButton("PerguntaVazaoMinInstalado", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMinInstalado" })) %> Não
		</div>
	</div>
	<br />
	<div class="block">
		<div class="coluna80 vazaoMinNormas hide">
			<label for="PerguntaVazaoMinNormas">O dispositivo de vazão mínima está dimensionado de acordo com as normas técnicas e legais? *</label>
			<%= Html.RadioButton("PerguntaVazaoMinNormas", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMinNormas" })) %> Sim
			<%= Html.RadioButton("PerguntaVazaoMinNormas", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMinNormas" })) %> Não
		</div>
		<div class="coluna40 AdequacoesDimensionamentoVazaoMin hide">
			<label for="AdequacoesDimensionamentoVazaoMin">Quais adequações serão realizadas? *</label>
			<%= Html.TextBox("AdequacoesDimensionamentoVazaoMin", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMin", maxlength = "14" })) %>
		</div>
	</div>
	 <br /> <br />
	<div class="block">
		<b>Dispositivo de vazão máxima</b> <br />
		<div class="coluna20">
			<label for="TipoDispositivoVazaoMax">Tipo *</label>
			<%= Html.DropDownList("TipoDispositivoVazaoMax", Model.Atividades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTipoDispositivoVazaoMax" }))%>
		</div>
		<div class="coluna40">
			<label for="DiametroTubulacaoVazaoMax">Largura e altura ou diâmetro da tubulação (m) *</label>
			<%= Html.TextBox("DiametroTubulacaoVazaoMax", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMax", maxlength = "14" })) %>
		</div>
		<br />
	</div>
	<div class="coluna40">
		<label for="PerguntaVazaoMinInstalado">Já está instalado *</label>
		<%= Html.RadioButton("PerguntaVazaoMaxInstalado", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMaxInstalado" })) %> Sim
		<%= Html.RadioButton("PerguntaVazaoMaxInstalado", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMaxInstalado" })) %> Não
	</div>
	<br />
	<div class="coluna80 vazaoMaxNormas hide">
		<label for="PerguntaVazaoMinNormas">O dispositivo de vazão mínima está dimensionado de acordo com as normas técnicas e legais? *</label>
		<%= Html.RadioButton("PerguntaVazaoMaxNormas", ConfiguracaoSistema.SIM, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMaxNormas" })) %> Sim
		<%= Html.RadioButton("PerguntaVazaoMaxNormas", ConfiguracaoSistema.NAO, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPerguntaVazaoMaxNormas" })) %> Não
	</div>
	<div class="coluna40 AdequacoesDimensionamentoVazaoMax hide">
		<label for="AdequacoesDimensionamentoVazaoMax">Quais adequações serão realizadas? *</label>
		<%= Html.TextBox("AdequacoesDimensionamentoVazaoMax", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAdequacoesDimensionamentoVazaoMax", maxlength = "14" })) %>
	</div>
</fieldset>