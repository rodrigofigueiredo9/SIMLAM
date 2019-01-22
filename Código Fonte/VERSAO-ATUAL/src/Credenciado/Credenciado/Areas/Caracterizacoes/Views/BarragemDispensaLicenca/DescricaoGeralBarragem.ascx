<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend>Descrição geral da barragem</legend>

	<div class="block">
		<div class="coluna80">
			<label for="Fase">Fase de Instalação*</label>
			<% foreach (var item in Model.FasesLst) { %>
			<label><%= Html.RadioButton("Fase", item.Id, (Model.Caracterizacao.Fase == Convert.ToInt32(item.Id)), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFase" })) %><%= item.Texto %></label>
			<% } %>
		</div>
	</div>
	<div class="block">
			<div class="coluna80">
			<label for="TipoBarragem">Tipo da barragem *</label>
			<% foreach (var item in Model.BarragemTiposLst) { %>
			<label><%= Html.RadioButton("TipoBarragem", item.Id, (Model.Caracterizacao.BarragemTipo == Convert.ToInt32(item.Id)), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarragemTipo" })) %><%= item.Texto %></label>
			<% } %>
		</div>
		</div>
		<br />				
	<div class="block">
		<div class="coluna40">
			<label for="AreaAlagada">Área alagada na soleira do vertedouro (ha) *</label>
			<%= Html.TextBox("AreaAlagada", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaAlagada", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="VolumeArmazenamento">Volume armazenamento (m³) *</label>
			<%= Html.TextBox("VolumeArmazenamento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtVolumeArmazenamento", maxlength = "14" })) %>
		</div>
	</div>
			
	<div class="block">
		<div class="coluna40">
			<label for="AlturaBarramento">Altura do barramento (m) *</label>
			<%= Html.TextBox("AlturaBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAlturaBarramento", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="LarguraBaseBarramento">Largura da base do barramento (m) *</label>
			<%= Html.TextBox("LarguraBaseBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBaseBarramento", maxlength = "14" })) %>
		</div>
	</div>
			
	<div class="block">
		<div class="coluna40">
			<label for="ComprimentoBarramento">Comprimento do barramento (m) *</label>
			<%= Html.TextBox("ComprimentoBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtComprimentoBarramento", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="LarguraBarramento">Largura da crista do barramento (m) *</label>
			<%= Html.TextBox("LarguraCristaBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBarramento", maxlength = "14" })) %>
		</div>
	</div>
	<br />
	<fieldset class="block boxBranca">
		<legend>Coordenada UTM (SIRGAS 2000)</legend>
		<b>Barramento</b>
		<div class="block">
			<div class="coluna20">
				<label for="AlturaBarramento">Easting *</label>
				<%= Html.TextBox("AlturaBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAlturaBarramento", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="LarguraBaseBarramento">Northing *</label>
				<%= Html.TextBox("LarguraBaseBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBaseBarramento", maxlength = "14" })) %>
			</div>
			<%if (!Model.IsVisualizar) { %>
				<div class="coluna20">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
			<%} %>
		</div>
		<b>Área de bota-fora</b>
		<div class="block">
			<div class="coluna20">
				<label for="AlturaBarramento">Easting *</label>
				<%= Html.TextBox("AlturaBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAlturaBarramento", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="LarguraBaseBarramento">Northing *</label>
				<%= Html.TextBox("LarguraBaseBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBaseBarramento", maxlength = "14" })) %>
			</div>
			<%if (!Model.IsVisualizar) { %>
				<div class="coluna20">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
			<%} %>
		</div>
		<b>Área de empréstimo</b>
		<div class="block">
			<div class="coluna20">
				<label for="AlturaBarramento">Easting *</label>
				<%= Html.TextBox("AlturaBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAlturaBarramento", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="LarguraBaseBarramento">Northing *</label>
				<%= Html.TextBox("LarguraBaseBarramento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBaseBarramento", maxlength = "14" })) %>
			</div>
			<%if (!Model.IsVisualizar) { %>
				<div class="coluna20">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
			<%} %>
		</div>

	</fieldset>

			
	<div class="box block boxfinalidade">
		<label><b>Finalidade</b></label><br />
			<label class ="coluna40">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Abastecimento humano (exceto abastecimento público)
			</label>
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Irrigação
			</label>
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Regularização de varão
			</label>
			<label class ="coluna40">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Agricultura
			</label>
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Reservação de água
			</label>
			<label class ="coluna25">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Ecoturismo ou turismo rural
			</label>
			<label class ="coluna40">
				<%= Html.CheckBox("FinalidadeAtividade", false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = "item.txto", @value = "item.texto" }))%>
				Dessedentação de animais
			</label>
	</div>
			
	<div class="box">
		<div class="block">
			<div class="coluna40">
				<label for="NomeCursoHidrico">Nome do curso hídrico *</label>
				<%= Html.TextBox("NomeCursoHidrico", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeCursoHidrico", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="AreaBacia">Área da bacia de contribuição *</label>
				<%= Html.TextBox("AreaBacia", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaBacia", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="IntensidadeMaxPrecipitacao">Intensidade máxima média de precipitação (mm/h) *</label>
				<%= Html.TextBox("IntensidadeMaxPrecipitacao", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtIntensidadeMaxPrecipitacao", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="FonteDadosIntensidade">Fonte de dados *</label>
				<%= Html.TextBox("FonteDadosIntensidade", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosIntensidade", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="PeriodoRetorno">Período de retorno (em anos) *</label>
				<%= Html.TextBox("PeriodoRetorno", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtPeriodoRetorno", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="CoeficienteEscoamento">Coeficiente de escoamento (C) *</label>
				<%= Html.TextBox("CoeficienteEscoamento", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtCoeficienteEscoamento", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="FonteDadosCoeficiente">Fonte de dados *</label>
				<%= Html.TextBox("FonteDadosCoeficiente", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosCoeficiente", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="TempoConcentracao">Tempo de concentração (em minutos) *</label>
				<%= Html.TextBox("TempoConcentracao", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtTempoConcentracao", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="EquacaoTempoConcentracao">Equação utilizada *</label>
				<%= Html.TextBox("EquacaoTempoConcentracao", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEquacaoTempoConcentracao", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="CoeficienteEscoamento">Vazão máxima de cheia (vazão de enchente)(m³) *</label>
				<%= Html.TextBox("CoeficienteEscoamento", Model.Caracterizacao.VazaoEnchente, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtCoeficienteEscoamento", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="FonteDadosVazao">Fonte de dados *</label>
				<%= Html.TextBox("FonteDadosVazao", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosVazao", maxlength = "14" })) %>
			</div>
		</div>
	</div>
</fieldset>