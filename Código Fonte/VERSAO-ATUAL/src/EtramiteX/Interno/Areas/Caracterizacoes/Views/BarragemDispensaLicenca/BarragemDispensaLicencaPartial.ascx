<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script>
	BarragemDispensaLicenca.settings.mensagens = <%= Model.Mensagens %>;
	BarragemDispensaLicenca.settings.idsTela = <%= Model.IdsTela %>;
</script>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoID %>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<fieldset class="block box">
	<legend class="titFiltros">Atividade Dispensada</legend>

	<div class="block boxBranca">
		<div class="coluna80">
			<label for="Atividade">Atividade *</label>
			<%= Html.DropDownList("Atividade", Model.Atividades, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividade" }))%>
		</div>
		<div class="coluna80">
			<label for="TipoBarragem">Tipo da barragem *</label>
			<% foreach (var item in Model.BarragemTiposLst) { %>
			<label><%= Html.RadioButton("TipoBarragem", item.Id, (Model.Caracterizacao.BarragemTipo == Convert.ToInt32(item.Id)), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbBarragemTipo" })) %><%= item.Texto %></label>
			<% } %>
		</div>
	</div>

	<!-- Finalidade da Atividade -->
	<fieldset class="block boxBranca">
		<legend>Finalidade da Atividade</legend>

		<div class="block">
			<label for="FinalidadeAtividade">Finalidade *</label>
		</div>
		<div class="block">
		<%foreach(var item in Model.FinalidadesAtividade){ %>
			<div class="coluna45">
				<label>
					<%= Html.CheckBox("FinalidadeAtividade", ((int.Parse(item.Codigo) & Model.Caracterizacao.FinalidadeAtividade) != 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFinalidadeAtividade", @title = item.Texto, @value = item.Codigo }))%>
					<%=item.Texto %>
				</label>
			</div>
		<%} %>
		</div>

		<div class="block">
			<div class="coluna34">
				<label for="CursoHidrico">Nome do curso hídrico *</label>
				<%= Html.TextBox("CursoHidrico", Model.Caracterizacao.CursoHidrico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCursoHidrico", maxlength = "50" })) %>
			</div>
			<div class="coluna31">
				<label for="">Vazão de enchente(m³/s) *</label>
				<%= Html.TextBox("VazaoEnchente", Model.Caracterizacao.VazaoEnchente, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto4 txtVazaoEnchente", maxlength = "13" } )) %>
			</div>
			<div class="coluna31">
				<label for="AreaBaciaContribuicao">Área da bacia de contribuição (ha) *</label>
				<%= Html.TextBox("AreaBaciaContribuicao", Model.Caracterizacao.AreaBaciaContribuicao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto4 txtAreaBaciaContribuicao", maxlength = "14" } )) %>
			</div>
		</div>

		<div class="block">
			<div class="coluna34">
				<label for="Precipitacao">Int. Máx Méd. precipitação (lm) (mm/h) *</label>
				<%= Html.TextBox("Precipitacao", Model.Caracterizacao.Precipitacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtPrecipitacao", maxlength = "14" })) %>
			</div>
			<div class="coluna31">
				<label for="PeriodoRetorno">Período de retorno (T)(anos) *</label>
				<%= Html.TextBox("PeriodoRetorno", Model.Caracterizacao.PeriodoRetorno, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtPeriodoRetorno", maxlength = "4" })) %>
			</div>
			<div class="coluna31">
				<label for="CoeficienteEscoamento">Coeficiente de escoamento (C) *</label>
				<%= Html.TextBox("CoeficienteEscoamento", Model.Caracterizacao.CoeficienteEscoamento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCoeficienteEscoamento", maxlength = "60" })) %>
			</div>
		</div>

		<div class="block">
			<div class="coluna34">
				<label for="TempoConcentracao">Tempo de concentração (tc)(min) *</label>
				<%= Html.TextBox("TempoConcentracao", Model.Caracterizacao.TempoConcentracao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTempoConcentracao", maxlength = "7" })) %>
			</div>
			<div class="coluna31">
				<label for="EquacaoCalculo">Equação utilizada no calculo (tc) *</label>
				<%= Html.TextBox("EquacaoCalculo", Model.Caracterizacao.EquacaoCalculo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEquacaoCalculo", maxlength = "60" })) %>
			</div>
		</div>

		<div class="block">
			<div class="coluna34">
				<label for="AreaAlagada">Área alagada (ha) *</label>
				<%= Html.TextBox("AreaAlagada", Model.Caracterizacao.AreaAlagada, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtAreaAlagada", maxlength = "14" })) %>
			</div>
			<div class="coluna31">
				<label for="VolumeArmazanado">Volume armazenado (m³) *</label>
				<%= Html.TextBox("VolumeArmazanado", Model.Caracterizacao.VolumeArmazanado, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto4 txtVolumeArmazenado", maxlength = "14" })) %>
			</div>
		</div>
	</fieldset>

	<!-- Fases -->
	<fieldset class="block boxBranca">
		<legend>Fases</legend>

		<div class="block">
			<div class="coluna80">
				<label for="Fase">Fase *</label>
				<% foreach (var item in Model.FasesLst) { %>
				<label><%= Html.RadioButton("Fase", item.Id, (Model.Caracterizacao.Fase == Convert.ToInt32(item.Id)), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbFase" })) %><%= item.Texto %></label>
				<% } %>
			</div>
		</div>

		<fieldset class="block box faseConstruida hide">
			<legend>Fase: Construída</legend>

			<div class="block divRadio">
				<div class="coluna22">
					<label for="PossuiMonge">Possui monge? *</label><br />
					<label>
						<%=Html.RadioButton("PossuiMonge", (int)ConfiguracaoSistema.NAO, Model.Caracterizacao.PossuiMonge.HasValue ? !Convert.ToBoolean(Model.Caracterizacao.PossuiMonge.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiMonge"}))%>
						Não
					</label>
					<label>
						<%=Html.RadioButton("PossuiMonge", (int)ConfiguracaoSistema.SIM, Model.Caracterizacao.PossuiMonge.HasValue ? Convert.ToBoolean(Model.Caracterizacao.PossuiMonge.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiMonge"}))%>
						Sim
					</label>
				</div>
				<div class="coluna20 divRadioEsconder <%= Convert.ToBoolean(Model.Caracterizacao.PossuiMonge.GetValueOrDefault()) ? "" : "hide" %>">
					<label for="MongeTipo">Tipo de monge *</label>
					<%= Html.DropDownList("MongeTipo", Model.MongeTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlMongeTipo" })) %>
				</div>
				<div class="coluna20 divRadioEsconderOutro <%= Model.Caracterizacao.MongeTipo == (int)eMongeTipo.Outros ? "" : "hide" %>">
					<label for="EspecificacaoMonge">Especificar *</label>
					<%= Html.TextBox("EspecificacaoMonge", Model.Caracterizacao.EspecificacaoMonge, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtEspecificacaoMonge", maxlength = "80" }))%>
				</div>
			</div>
			<div class="block divRadio">
				<div class="coluna22">
					<label for="PossuiVertedouro">Possui vertedouro? *</label><br />
					<label>
						<%=Html.RadioButton("PossuiVertedouro", (int)ConfiguracaoSistema.NAO, Model.Caracterizacao.PossuiVertedouro.HasValue ? !Convert.ToBoolean(Model.Caracterizacao.PossuiVertedouro.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiVertedouro"}))%>
						Não
					</label>
					<label>
						<%=Html.RadioButton("PossuiVertedouro", (int)ConfiguracaoSistema.SIM, Model.Caracterizacao.PossuiVertedouro.HasValue ? Convert.ToBoolean(Model.Caracterizacao.PossuiVertedouro.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiVertedouro"}))%>
						Sim
					</label>
				</div>
				<div class="coluna20 divRadioEsconder <%= Convert.ToBoolean(Model.Caracterizacao.PossuiVertedouro.GetValueOrDefault()) ? "" : "hide" %>">
					<label for="VertedouroTipo">Tipo de vertedouro *</label>
					<%= Html.DropDownList("VertedouroTipo", Model.VertedouroTiposLst,ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class="ddlVertedouroTipo" })) %>
				</div>
				<div class="coluna20 divRadioEsconderOutro <%= Model.Caracterizacao.VertedouroTipo == (int)eVertedouroTipo.Outros ? "" : "hide" %>">
					<label for="EspecificacaoVertedouro">Especificar *</label>
					<%= Html.TextBox("EspecificacaoVertedouro", Model.Caracterizacao.EspecificacaoVertedouro,ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtEspecificacaoVertedouro", maxlength = "80" }))%>
				</div>
			</div>
			<div class="block">
				<div class="coluna90">
					<label for="PossuiEstruturaHidraulica">As estruturas hidráulicas (monge e vertedouro) e o corpo do barramento encontram-se em funcionamento de acordo com as normas técnicas de segurança? *</label><br />
					<label>
						<%=Html.RadioButton("PossuiEstruturaHidraulica", (int)ConfiguracaoSistema.NAO, Model.Caracterizacao.PossuiEstruturaHidraulica.HasValue ? !Convert.ToBoolean(Model.Caracterizacao.PossuiEstruturaHidraulica.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiEstruturaHidraulica"}))%>
						Não
					</label>
					<label>
						<%=Html.RadioButton("PossuiEstruturaHidraulica", (int)ConfiguracaoSistema.SIM, Model.Caracterizacao.PossuiEstruturaHidraulica.HasValue ? Convert.ToBoolean(Model.Caracterizacao.PossuiEstruturaHidraulica.Value) : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio rbPossuiEstruturaHidraulica"}))%>
						Sim
					</label>
				</div>
			</div>
			<div class="block">
				<div class="coluna90">
					<label for="AdequacoesRealizada">Quais adequações serão realizadas?</label>
					<%= Html.TextArea("AdequacoesRealizada",Model.Caracterizacao.AdequacoesRealizada, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new{@class=" text media txtAdequacoesRealizada", maxlength = "3000"}) )%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box faseAConstruir hide">
			<legend>Fase: A construir</legend>

			<div class="block">
				<div class="coluna20">
					<label for="MongeTipo">Tipo de monge *</label>
					<%= Html.DropDownList("MongeTipo", Model.MongeTiposLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlMongeTipo" })) %>
				</div>
				<div class="coluna20 <%= Model.Caracterizacao.MongeTipo == (int)eMongeTipo.Outros ? "" : "hide" %>">
					<label for="EspecificacaoMonge">Especificar *</label>
					<%= Html.TextBox("EspecificacaoMonge", Model.Caracterizacao.EspecificacaoMonge, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtEspecificacaoMonge", maxlength = "80" }))%>
				</div>
			</div>
			<div class="block">
				<div class="coluna20">
					<label for="VertedouroTipo">Tipo de vertedouro *</label>
					<%= Html.DropDownList("VertedouroTipo", Model.VertedouroTiposLst,ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class="ddlVertedouroTipo" })) %>
				</div>
				<div class="coluna20 <%= Model.Caracterizacao.VertedouroTipo == (int)eVertedouroTipo.Outros ? "" : "hide" %>">
					<label for="EspecificacaoVertedouro">Especificar *</label>
					<%= Html.TextBox("EspecificacaoVertedouro", Model.Caracterizacao.EspecificacaoVertedouro,ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtEspecificacaoVertedouro", maxlength = "80" }))%>
				</div>
			</div>

			<div class="block">
				<label>As estruturas hidráulicas (monge e vertedouro) e o corpo do barramento deverão ser construídas de acordo com as normas técnicas de segurança.</label>
			</div>

			<div class="block">
				<div class="coluna30">
					<label for="DataInicioObra">Início da obra (mês/ano) *</label>
					<%= Html.TextBox("DataInicioObra",Model.Caracterizacao.DataInicioObra, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new{@class=" text txtDataInicioObra maskMesAno"}) )%>
				</div>

				<div class="coluna30">
					<label for="DataPrevisaoTerminoObra">Previsão de término (mês/ano) *</label>
					<%= Html.TextBox("DataPrevisaoTerminoObra",Model.Caracterizacao.DataPrevisaoTerminoObra, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new{@class=" text txtDataPrevisaoTerminoObra maskMesAno"}) )%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Coordenada na crista - UTM - SIRGAS 2000</legend>
			<div class="block">
				<div class="coluna23">
					<label for="EastingUtmTexto">Easting *</label>
					<%=Html.TextBox("EastingUtmTexto", Model.Caracterizacao.Coordenada.EastingUtmTexto, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
				</div>

				<div class="coluna23 prepend1">
					<label for="Coordenada_NorthingUtmTexto">Northing *</label>
					<%=Html.TextBox("NorthingUtmTexto", Model.Caracterizacao.Coordenada.NorthingUtmTexto, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
				</div>

				<%if (!Model.IsVisualizar) { %>
				<div class="coluna20">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
				<%} %>
			</div>
		</fieldset>
	</fieldset>

	<!-- Responsabilidade Técnica -->
	<fieldset class="block boxBranca">
		<legend>Responsabilidade técnica</legend>

		<fieldset class="block box">
			<legend>Responsável Técnico</legend>

			<div class="block">
				<label for="FormacaoRT">Formação *</label>
			</div>
			<div class="block">
			<%foreach(var item in Model.FormacoesRTLst){ %>
				<div class="coluna30 divFormacaoRT">
					<input type="hidden" class="hdnFormacaoRT" value="<%: item.Codigo %>" />
					<label>
						<%= Html.CheckBox("FormacaoRT", ((int.Parse(item.Codigo) & Model.Caracterizacao.FormacaoRT) != 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox cbFormacaoRT", @title = item.Texto, @value = item.Codigo }))%>
						<%=item.Texto %>
					</label>
				</div>
			<%} %>
				
			</div>
			<div class="block">
				<div class="coluna30 hide">
					<label for="EspecificacaoRT">Especificar *</label>
					<%= Html.TextBox("EspecificacaoRT", Model.Caracterizacao.EspecificacaoRT, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtEspecificacaoRT", maxlength = "50" })) %>
				</div>
			</div>

			<div class="block">
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

			<div class="block">
				<div class="coluna30">
					<label for="NumeroARTElaboracao">Número ART: Elaboração *</label>
					<%= Html.TextBox("NumeroARTElaboracao", Model.Caracterizacao.NumeroARTElaboracao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{ @class="text txtNumeroARTElaboracao", maxlength = "20"})) %>
				</div>
				<div class="coluna30">
					<label for="NumeroARTExecucao">Número ART: Execução</label>
					<%= Html.TextBox("NumeroARTExecucao", Model.Caracterizacao.NumeroARTExecucao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroARTExecucao", maxlength = "20"})) %>
				</div>
			</div>

			<div class="block ultima">
				<label>Nos casos de barragens a construir ou construídas com previsão de adequações é obrigatório o preenchimento do ART de elaboração e da ART de execução.</label>
			</div>
		</fieldset>
	</fieldset>
</fieldset>