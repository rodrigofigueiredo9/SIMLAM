<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend>Descrição geral da barragem</legend>

	<div class="block">
		<div class="coluna80">
			<label for="Fase">Fase de instalação: *</label>
			<% foreach (var item in Model.FasesLst) { %>
			<label><%= Html.RadioButton("Fase", item.Id, (Model.Caracterizacao.Fase == Convert.ToInt32(item.Id)), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFase" })) %><%= item.Texto %></label>
			<% } %>
		</div>
	</div>
	<div class="block">
		
			<div class="coluna80">
			<label for="TipoBarragem">Tipo da barramento: *</label>
			<% foreach (var item in Model.BarragemTiposLst) { %>
			<label><%= Html.RadioButton("TipoBarragem", item.Id, item.IsAtivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarragemTipo" })) %><%= item.Texto %></label>
			<% } %>
		</div>
		</div>
		<br />				
	<div class="block">
		<div class="coluna40">
			<label for="areaAlagada">Área alagada na soleira do vertedouro (ha) *</label>
			<%= Html.TextBox("areaAlagada", Model.Caracterizacao.areaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaAlagada", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="volumeArmazanado">Volume armazenado (m³) *</label>
			<%= Html.TextBox("volumeArmazanado", Model.Caracterizacao.volumeArmazanado, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtVolumeArmazenamento", maxlength = "14" })) %>
		</div>
	</div>
			
	<div class="block">
		<div class="coluna40">
			<label for="alturaBarramento">Altura do barramento (m) *</label>
			<%= Html.TextBox("alturaBarramento", Model.Caracterizacao.alturaBarramento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAlturaBarramento", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="larguraBaseBarramento">Largura da base do barramento (m) *</label>
			<%= Html.TextBox("larguraBaseBarramento", Model.Caracterizacao.larguraBaseBarramento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraBaseBarramento", maxlength = "14" })) %>
		</div>
	</div>
			
	<div class="block">
		<div class="coluna40">
			<label for="comprimentoBarramento">Comprimento do barramento (m) *</label>
			<%= Html.TextBox("comprimentoBarramento", Model.Caracterizacao.comprimentoBarramento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtComprimentoBarramento", maxlength = "14" })) %>
		</div>
		<div class="coluna40">
			<label for="larguraCristaBarramento">Largura da crista (m) *</label>
			<%= Html.TextBox("larguraCristaBarramento", Model.Caracterizacao.larguraCristaBarramento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtLarguraCristaBarramento", maxlength = "14" })) %>
		</div>
	</div>
	<br />
	<fieldset class="block boxBranca">
		<legend>Coordenada UTM (SIRGAS 2000)</legend>
		<b>Barramento</b>
		<div class="block">
			<div class="coluna20">
				<label for="">Easting *</label>
				<%= Html.TextBox("eastingBarramento", Model.Caracterizacao.coordenadas[0].easting, ViewModelHelper.SetaDisabled(true, new { @class = "text txtEasting txtEastingBarramento", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="">Northing *</label>
				<%= Html.TextBox("northingBarramento", Model.Caracterizacao.coordenadas[0].northing, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNorthing txtNorthingBarramento", maxlength = "14" })) %>
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
				<label for="">Easting *</label>
				<%= Html.TextBox("eastingBoraFOra", Model.Caracterizacao.coordenadas[1].easting, ViewModelHelper.SetaDisabled(true, new { @class = "text txtEasting txtEastingBotaFora", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="">Northing *</label>
				<%= Html.TextBox("northingBoraFOra", Model.Caracterizacao.coordenadas[1].northing, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNorthing txtNorthingBotaFora", maxlength = "14" })) %>
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
				<label for="">Easting *</label>
				<%= Html.TextBox("eastingEmprestimo", Model.Caracterizacao.coordenadas[2].easting, ViewModelHelper.SetaDisabled(true, new { @class = "text txtEasting txtEastingEmprestimo", maxlength = "14" })) %>
			</div>
			<div class="coluna20">
				<label for="">Northing *</label>
				<%= Html.TextBox("northingEmprestimo", Model.Caracterizacao.coordenadas[2].northing, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNorthing txtNorthingEmprestimo", maxlength = "14" })) %>
			</div>
			<%if (!Model.IsVisualizar) { %>
				<div class="coluna20">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
			<%} %>
		</div>

	</fieldset>

			
	<div class="block boxfinalidade">
		<label><b>Finalidade *</b></label><br />
		<%foreach(var item in Model.FinalidadesAtividade){ %>
			<div class="coluna45">
				<label>
					<%= Html.CheckBox("FinalidadeAtividade", item.IsAtivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = item.Texto, @value = item.Id }))%>
					<%=item.Texto %>
				</label>
			</div>
		<%} %>
	</div>
			
	<div class="">
		<div class="block">
			<div class="coluna40">
				<label for="cursoHidrico">Nome do curso hídrico *</label>
				<%= Html.TextBox("cursoHidrico", Model.Caracterizacao.cursoHidrico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCursoHidrico", maxlength = "50" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="areaBaciaContribuicao">Área da bacia de contribuição (ha) *</label>
				<%= Html.TextBox("areaBaciaContribuicao", Model.Caracterizacao.areaBaciaContribuicao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaBacia", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="intensidadeMaxPrecipitacao">Intensidade máxima média de precipitação (mm/h) *</label>
				<%= Html.TextBox("intensidadeMaxPrecipitacao", Model.Caracterizacao.precipitacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtIntensidadeMaxPrecipitacao", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="fonteDadosIntensidadeMax">Fonte de dados *</label>
				<%= Html.TextBox("fonteDadosIntensidadeMax", Model.Caracterizacao.fonteDadosPrecipitacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosIntensidade", maxlength = "50" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="periodoRetorno">Período de retorno (em anos) *</label>
				<%= Html.TextBox("periodoRetorno", Model.Caracterizacao.periodoRetorno, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtPeriodoRetorno", maxlength = "14" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="coeficienteEscoamento">Coeficiente de escoamento (C) *</label>
				<%= Html.TextBox("coeficienteEscoamento", Model.Caracterizacao.coeficienteEscoamento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtCoeficienteEscoamento", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="fonteDadosCoeficienteEscoamento">Fonte de dados *</label>
				<%= Html.TextBox("fonteDadosCoeficienteEscoamento", Model.Caracterizacao.fonteDadosCoeficienteEscoamento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosCoeficiente", maxlength = "50" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="tempoConcentracao">Tempo de concentração (em minutos) *</label>
				<%= Html.TextBox("tempoConcentracao", Model.Caracterizacao.tempoConcentracao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtTempoConcentracao", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="tempoConcentracaoEquacaoUtilizada">Equação utilizada *</label>
				<%= Html.TextBox("tempoConcentracaoEquacaoUtilizada", Model.Caracterizacao.tempoConcentracaoEquacaoUtilizada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEquacaoTempoConcentracao", maxlength = "60" })) %>
			</div>
		</div>
		<div class="block">
			<div class="coluna40">
				<label for="vazaoEnchente">Vazão máxima de cheia (vazão de enchente) (m³) *</label>
				<%= Html.TextBox("vazaoEnchente", Model.Caracterizacao.vazaoEnchente, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtVazaoEnchente", maxlength = "14" })) %>
			</div>
			<div class="coluna40">
				<label for="fonteDadosVazaoEnchente">Fonte de dados *</label>
				<%= Html.TextBox("fonteDadosVazaoEnchente", Model.Caracterizacao.fonteDadosVazaoEnchente, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteDadosVazao", maxlength = "50" })) %>
			</div>
		</div>
	</div>
</fieldset>