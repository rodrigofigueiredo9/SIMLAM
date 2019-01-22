<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block boxBranca">
	<legend class="titFiltros">Responsabilidade Ténica</legend>

	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração da Declaração de Dispensa de Licenciamento Ambiental de Barragem</legend>

		<div class="block">

			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração da Declaração de Dispensa de Licenciamento Ambiental de Barragem *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do projeto técnico ou do laudo de barragem construída</legend>

		<div class="block">
			<h5>Por orientação do Crea/ES somente estão aptos a serem responsáveis técnicos de barragens de terra engenheiros agrônomos,
				engenheiros agrícolas e engenheiros civis, e apenas engenheiros civis no caso de barragens de concreto ou mista.
				Demais profissionais somente serão aceitos com a apresentação de autorização específica do Crea/ES.
			</h5>

			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>

			<div class="block hide">
				<div class="coluna80 inputFileDiv">
					<label for="ArquivoTexto">Autorização do conselho de classe *</label>
					<% if(Model.Caracterizacao.Autorizacao.Id.GetValueOrDefault() > 0) { %>
					<div>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Caracterizacao.Autorizacao.Nome, 45), "Baixar", "Arquivo", new { @id = Model.Caracterizacao.Autorizacao.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Caracterizacao.Autorizacao.Nome })%>
					</div>
					<% } %>
					<%= Html.TextBox("Arquivo.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Caracterizacao.Autorizacao.Nome) ? "" : "hide" %>">
					<input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" /></span>
					<input type="hidden" class="hdnArquivo" value="<%: Model.AutorizacaoJson%>" />
				</div>

				<div class="block ultima spanBotoes">
					<button type="button" class="inlineBotao btnArq <%= string.IsNullOrEmpty(Model.Caracterizacao.Autorizacao.Nome) ? "" : "hide" %>" title="Enviar anexo" onclick="BarragemDispensaLicenca.enviarArquivo('<%= Url.Action("arquivo", "arquivo") %>');">Enviar</button>
					<%if (!Model.IsVisualizar) {%>
					<button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.Caracterizacao.Autorizacao.Nome) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
					<%} %>
				</div>
			</div>

			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do Projeto técnico/laudo de barragem construída *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>
	
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela execução da barragem ou das adequações</legend>

		<div class="block">
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Responsável técnico pela elaboração do Estudo Ambiental *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>
		
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do Estudo Ambiental</legend>

		<div class="block">
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do estudo ambiental *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>
			
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela elaboração do Plano de Recuperação de Área Degradada referente a APP no entorno do reservatório</legend>
		
		<div class="block">
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Número da ART de elaboração do Plano de Recuperação de Área Degradada *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>
				
	<fieldset class="block box">
	<legend class="titFiltros">Responsável técnico pela execução do Plano de Recuperação de Área Degradada referente à APP do entorno do reservatório</legend>
		
		<div class="block">
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Próprio Declarante
			</label> <br />
		</div>
		<div class="block">
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Nome *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Profissão *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Registro CREA/ES *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
			<div class="coluna30">
				<label for="DiametroTubulacaoVazaoMin">Número da ART de execução do Plano de Recuperação de Área Degradada *</label>
				<%= Html.TextBox("DiametroTubulacaoVazaoMin", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtDiametroTubulacaoVazaoMin", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>

</fieldset>