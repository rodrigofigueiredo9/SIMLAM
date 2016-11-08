<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DescricaoLicenciamentoAtividadeVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<style type="text/css">
	.labelCheckBox
	{
		width: 150px; 
		text-align: left; 
		display: inline-block;
	}
</style>

<input type="hidden" class="hdnEmpreendimentoId" value="<%= Model.DscLicAtividade.EmpreendimentoId %>" />
<input type="hidden" class="hdnAreaTerreno" value="<%= Model.DscLicAtividade.AreaTerreno %>" />

<div class="divDscLicAtividade">
	<div class="block box">
		<div class="coluna80">
			<label for="DscLicAtividade_RespAtividade">Responsável pela atividade *</label>
			<%= Html.DropDownList("DscLicAtividade.RespAtividade", Model.ResponsaveisEmpreendimento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlRespAtiv", @id = "ddlRespAtiv" }))%>
		</div>
	</div>
	<fieldset class="block box">
		<legend class="titFiltros">Caracteristicas da área</legend>
			<div class="block">
				<div class="coluna80">
					<label for="DscLicAtividade_BaciaHidrografica">Bacia Hidrográfica *</label>
					<%= Html.TextBox("DscLicAtividade.BaciaHidrografica", Model.DscLicAtividade.BaciaHidrografica, ViewModelHelper.SetaDisabled(true, new { @class = "text txtBaciaHidrografica", @id = "txtBaciaHidrografica", }))%>
				</div>
			</div>
			<div class="block">
				<div class="coluna40">
					<label for="">Existe APP na área útil do empreendimento *</label><br />
					<label><%= Html.RadioButton("rdbExisteApp", 1, Model.DscLicAtividade.ExisteAppUtil, ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbExisteAppSim" }))%>Sim</label>
					<label class="append5"><%= Html.RadioButton("rdbExisteApp", 0, !Model.DscLicAtividade.ExisteAppUtil, ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbExisteAppNao" }))%>Não</label>
				</div>
				<div class="coluna40">
					<label for="">Tipo de vegetação na área útil</label><br />
					<%for (int i = 0; i < Model.VegetacaoAreaUtil.Count; i++) { %>
						<label class="">
						<%= Html.CheckBox("ck" + Model.VegetacaoAreaUtil[i].Texto, Model.IsChecked(Model.DscLicAtividade.TipoVegetacaoUtilCodigo, Convert.ToInt32(Model.VegetacaoAreaUtil[i].Codigo)), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox ckbVegetacaoAreaUtil", @title = Model.VegetacaoAreaUtil[i].Texto, @value = Model.VegetacaoAreaUtil[i].Codigo }))%>
						<%: Model.VegetacaoAreaUtil[i].Texto%></label>
					<% } %>				
				</div>
			</div>
			<div class="block">
				<div class="coluna30">
					<label for="">Está localizado em UC *</label><br />
					<label><%= Html.RadioButton("rdbLocalizadoUC", 1, Model.DscLicAtividade.LocalizadaUC, ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbLocalizadoUCSim" }))%>Sim</label>
					<label class="append5"><%= Html.RadioButton("rdbLocalizadoUC", 0, !Model.DscLicAtividade.LocalizadaUC, ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbLocalizadoUCNao" }))%>Não</label>
				</div>
				<div class="coluna30 <%= Model.DscLicAtividade.LocalizadaUC ? "" : "hide"%>">
					<label for="DscLicAtividade_LocalizadaUCNome">Nome - Orgão Administrador</label>
					<%= Html.TextBox("DscLicAtividade.LocalizadaUCNome", Model.DscLicAtividade.LocalizadaUCNomeOrgaoAdm, ViewModelHelper.SetaDisabled(true, new { @class = "text txtLocalizadaUCNome", @id = "txtLocalizadaUCNome", }))%>
				</div>
			</div>
		<fieldset class="block box boxBranca">
		
		<div class="block">
			<div class="coluna35">
				<label for="">Está em zona de amortecimento de UC *</label><br />
				<label><%= Html.RadioButton("rdbZonaUC", 1, Model.DscLicAtividade.ZonaAmortUC.HasValue ? Model.DscLicAtividade.ZonaAmortUC.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbZonaUCSim" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("rdbZonaUC", 0, Model.DscLicAtividade.ZonaAmortUC.HasValue ? !Model.DscLicAtividade.ZonaAmortUC.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbZonaUCNao" }))%>Não</label>
			</div>
			<div class="coluna30 divZonaAmortUC <%= Model.DscLicAtividade.ZonaAmortUC.HasValue && Model.DscLicAtividade.ZonaAmortUC.Value ? "" : "hide"%>">
				<label for="DscLicAtividade_ZonaAmortUCNome">Nome - Orgão Administrador</label>
				<%= Html.TextBox("DscLicAtividade.ZonaAmortUCNome", Model.DscLicAtividade.ZonaAmortUCNomeOrgaoAdm, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtZonaAmortUCNome", @id = "txtZonaAmortUCNome", }))%>
			</div>
		</div>
		<div class="block">
			<div class="divPatrimonioHistorico coluna40 append2">
				<label for="">Há patrimônio histórico cultural na área útil *</label><br />
				<label><%= Html.RadioButton("rdbPatrimonioHistorico", 1, Model.DscLicAtividade.PatrimonioHistorico.HasValue ? Model.DscLicAtividade.PatrimonioHistorico.HasValue : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbPatrimonioHistoricoSim" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("rdbPatrimonioHistorico", 0, Model.DscLicAtividade.PatrimonioHistorico.HasValue ? !Model.DscLicAtividade.PatrimonioHistorico.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbPatrimonioHistoricoNao" }))%>Não</label>
			</div>
			<div class="coluna25">
				<label for="">Há residência(s) no entorno *</label><br />
				<label><%= Html.RadioButton("rdbResidentesEntorno", 1, Model.DscLicAtividade.ResidentesEntorno.HasValue ? Model.DscLicAtividade.ResidentesEntorno.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbrdbResidentesEntornoSim" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("rdbResidentesEntorno", 0, Model.DscLicAtividade.ResidentesEntorno.HasValue ? !Model.DscLicAtividade.ResidentesEntorno.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbrdbResidentesEntornoNao" }))%>Não</label>
			</div>
			<div class="divResidentesEntorno coluna30 <%= Model.DscLicAtividade.ResidentesEntorno.HasValue && Model.DscLicAtividade.ResidentesEntorno.Value ? "" : "hide"%>">
				<label for="DscLicAtividade_ResidentesEnternoDistancia">Distância(m) *</label>
				<%= Html.TextBox("DscLicAtividade.ResidentesEnternoDistancia", Model.DscLicAtividade.ResidentesEnternoDistancia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtResidentesEnternoDistancia", @id = "txtResidentesEnternoDistancia", @maxlength = 7 }))%>
			</div>
		</div>			
		</fieldset>
	</fieldset>

	<fieldset class="block box">
		<legend class="titFiltros">Informações gerais sobre a atividade</legend>
		<div class="block">
			<div class="coluna18 append2">
				<label for="DscLicAtividade_AreaTerreno">Área do terreno (m²)</label>
				<%= Html.TextBox("DscLicAtividade.AreaTerreno", Model.DscLicAtividade.AreaTerreno.ToStringTrunc(format: false), ViewModelHelper.SetaDisabled(true, new { @class = "text txtAreaTerreno", @id = "txtAreaTerreno" }))%>
			</div>
			<div class="coluna16 append2">
				<label for="DscLicAtividade_AreaUtil">Área útil (m²) *</label>
				<%= Html.TextBox("DscLicAtividade.AreaUtil", Model.DscLicAtividade.AreaUtil.ToStringTrunc(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaUtil maskDecimalPonto", @id = "txtAreaUtil", @maxlength = 10 }))%>
			</div>
			<div class="coluna18">
				<label for="DscLicAtividade_TotalFuncionarios">Total de funcionários</label>
				<%= Html.TextBox("DscLicAtividade.TotalFuncionarios", Model.DscLicAtividade.TotalFuncionarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTotalFuncionarios", @id = "txtTotalFuncionarios", @maxlength = 5 }))%>
			</div>
		</div>
		<fieldset class="block box">
			<legend class="titFiltros">Regime de trabalho</legend>
			<div class="block">
				<div class="coluna13 append2">
					<label for="DscLicAtividade_HorasDias">Horas/dia</label>
					<%= Html.TextBox("DscLicAtividade.HorasDias", Model.DscLicAtividade.HorasDias, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtHorasDias", @id = "txtHorasDias", @maxlength = 5 }))%>
				</div>
				<div class="coluna13 append2">
					<label for="DscLicAtividade_DiasMes">Dias/mês</label>
					<%= Html.TextBox("DscLicAtividade.DiasMes", Model.DscLicAtividade.DiasMes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDiasMes", @id = "txtDiasMes", @maxlength = 2 }))%>
				</div>
				<div class="coluna13">
					<label for="DscLicAtividade_TurnosDia">Turnos/dia</label>
					<%= Html.TextBox("DscLicAtividade.TurnosDia", Model.DscLicAtividade.TurnosDia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTurnosDia", @id = "txtTurnosDia", @maxlength = 2 }))%>
				</div>
			</div>
		</fieldset>
	</fieldset>
	<fieldset class="block box">
		<legend class="titFiltros">Uso de recursos hídricos</legend>		

		<% if (!Model.IsVisualizar){ %>
		<div class="block">
			<div class="coluna40 append2">
				<label for="ddlFontesAbastecimentoAguaTipo">Fonte(s) de abastecimento de água *</label>
				<%= Html.DropDownList("DscLicAtividade.FonteAbastecimentoAguaTipoId", Model.FontesAbastecimentoAguaTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFontesAbastecimentoAguaTipo", @id = "ddlFontesAbastecimentoAguaTipo" }))%>
			</div>

			<div class="divFonteUso coluna40 append2">
				<label for="txtFonteUso" class="lblFonteUso">?</label>
				<%= Html.TextBox("txtFonteUso", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFonteUso", @id = "txtFonteUso", @maxlength = 100 }))%>
			</div>

			<div class="coluna10 botaoAddFonteAbastecimento">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddFonteAbastecimento" title="Adicionar fontes de abastecimento">Adicionar</button>		  
			</div>
		</div>
		<% } %>

		<div class="divGridFontesAbastecimentoAgua block">
			<table class="dataGridTable gridFontesAbastecimentoAgua" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Fonte</th>
						<th>Descrição</th>
						<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.DscLicAtividade.FontesAbastecimentoAgua) { %>
					<tr>
						<td>
							<span class="spanTipoTexto" title="<%= item.TipoTexto %>"><%= item.TipoTexto %></span>
						</td>
						<td>
							<span class="spanDescricao" title="<%= item.Descricao %>"><%= item.Descricao %></span>
						</td>
						<% if (!Model.IsVisualizar) { %>
						<td>
							<input type="hidden" class="hdnItemFonteAbastecimento" name="hdnItemFonteAbastecimento" value='<%= Model.GetJSON(item) %>' />
							<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
						</td>
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>
			<table style="display: none">
				<tbody>
					<tr class="trFonteAbastecimentoTemplate">
						<td>
							<span class="spanTipoTexto"></span>
						</td>
						<td>
							<span class="spanDescricao"></span>
						</td>
						<td>
							<input type="hidden" class="hdnItemFonteAbastecimento" name="hdnItemFonteAbastecimento" value="" />
							<button title="Excluir" class="icone excluir btnExcluirLinha btnExcluirFonteAbastecimento" value="" type="button"></button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<br />

		<% if (!Model.IsVisualizar){ %>
		<div class="block">
			<div class="coluna40 append2">
				<label for="ddlPontosLancamentoEfluenteTipo">Pontos de lançamento de efluente *</label>
				<%= Html.DropDownList("DscLicAtividade.PontoLancamentoTipoId", Model.PontosLancamentoEfluenteTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlPontosLancamentoEfluenteTipo", @id = "ddlPontosLancamentoEfluenteTipo" }))%>
			</div>
			<div class="divPontoLancamento coluna40 append2">
				<label for="txtPontoLancamento" class="lblPontoLancamento">?</label>
				<%= Html.TextBox("txtPontoLancamento", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPontoLancamento", @id = "txtPontoLancamento", @maxlength = 100 }))%>
			</div>
			<div class="coluna10 botaoAddPontoLancamento">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddPontoLancamento" title="Adicionar ponto de lançamento efluente">Adicionar</button>		  
			</div>
		</div>
		<% } %>

		<div class="divGridPontoLancamento block">
			<table class="dataGridTable gridPontoLancamento" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Ponto</th>
						<th>Descrição</th>
						<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.DscLicAtividade.PontosLancamentoEfluente) { %>
					<tr>
						<td><span class="spanTipoTexto" title="<%= item.TipoTexto %>"><%= item.TipoTexto %></span></td>
						<td><span class="spanDescricao" title="<%= item.Descricao %>"><%= item.Descricao %></span></td>
						<% if (!Model.IsVisualizar) { %>
						<td>
							<input type="hidden" class="hdnItemPontoLancamento" name="hdnItemPontoLancamento" value='<%= Model.GetJSON(item) %>' />
							<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
						</td>
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>
			<table style="display: none">
				<tbody>
					<tr class="trPontoLancamentoTemplate">
						<td><span class="spanTipoTexto"></span></td>
						<td><span class="spanDescricao"></span></td>
						<td>
							<input type="hidden" class="hdnItemPontoLancamento" name="hdnItemPontoLancamento" value="" />
							<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<br />

		<div class="block divConsumo">
			<div class="coluna20 append2">
				<label for="txtConsumoAguaLs">Consumo de água L/s *</label>
				<%= Html.TextBox("txtConsumoAguaLs", Model.DscLicAtividade.ConsumoAguaLs.ToStringTrunc(format:false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtConsumoAguaLs maskDecimal", @id = "txtConsumoAguaLs", @maxlength = 9 }))%>
			</div>
			<div class="coluna20 append2">
				<label for="txtConsumoAguaMh">Consumo de água m³/h *</label>
				<%= Html.TextBox("txtConsumoAguaMh", Model.DscLicAtividade.ConsumoAguaMh.ToStringTrunc(format:false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtConsumoAguaMh maskDecimal", @id = "txtConsumoAguaMh", @maxlength = 9 }))%>
			</div>
			<div class="coluna22 append2">
				<label for="txtConsumoAguaMdia">Consumo de água m³/dia *</label>
				<%= Html.TextBox("txtConsumoAguaMdia", Model.DscLicAtividade.ConsumoAguaMdia.ToStringTrunc(format:false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtConsumoAguaMdia maskDecimal", @id = "txtConsumoAguaMdia", @maxlength = 9 }))%>
			</div>
			<div class="coluna25">
				<label for="txtConsumoAguaMmes">Consumo de água m³/mês *</label>
				<%= Html.TextBox("txtConsumoAguaMmes", Model.DscLicAtividade.ConsumoAguaMmes.ToStringTrunc(format:false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtConsumoAguaMmes maskDecimal", @id = "txtConsumoAguaMmes", @maxlength = 12 }))%>
			</div>
		</div>
		<div class="block divConsumo">
			<div class="coluna20 append2">
				<label for="ddlOutorgaAguaTipo">Outorga de água</label>
				<%= Html.DropDownList("DscLicAtividade.TipoOutorgaId", Model.OutorgaAguaTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlOutorgaAguaTipo", @id = "ddlOutorgaAguaTipo" }))%>
			</div>
			<div class="coluna20">
				<label for="txtNumero">Número</label>
				<%= Html.TextBox("txtNumero", Model.DscLicAtividade.Numero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumero", @id = "txtNumero", @maxlength = 15 }))%>
			</div>
		</div>
	</fieldset>
	<fieldset class="block box fsEfluentesLiquidos <%= Model.IsExibirEfluentesLiquidos ? "" : "hide"%>">
		<legend class="titFiltros">Efluentes líquidos</legend>	
		<% if (!Model.IsVisualizar){ %>
		<div class="block">
			<div class="coluna25 append2">
				<label for="ddlFontesGeracaoTipo">Fontes de geração *</label>
				<%= Html.DropDownList("ddlFontesGeracaoTipo", Model.FontesGeracaoTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFontesGeracaoTipo", @id = "ddlFontesGeracaoTipo" }))%>
			</div>
			<div class="coluna12 append2">
				<label for="txtVazao">Vazão *</label>
				<%= Html.TextBox("txtVazao", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVazao", @id = "txtVazao", @maxlength = 9 }))%>
			</div>
			<div class="coluna15 append2">
				<label for="ddlUnidadeTipo">Unidade *</label>
				<%= Html.DropDownList("ddlUnidadeTipo", Model.UnidadeTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlUnidadeTipo", @id = "ddlUnidadeTipo" }))%>
			</div>
			<div class="coluna22 append2">
				<label for="txtSisTratamento">Sistema de tratamento *</label>
				<%= Html.TextBox("txtSisTratamento", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtSisTratamento", @id = "txtSisTratamento", @maxlength = 100 }))%>
			</div>
			<div class="coluna10 botaoEfluenteLiquido">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddEfluenteLiquido" title="Adicionar efluente líquido">Adicionar</button>		  
			</div>

		</div>
		<div class="divLiquidoEspecificar block hide">
			<div class="coluna30">
				<label for="txtEflLiquidoEspecificar">Especificar *</label>
				<%= Html.TextBox("txtEflLiquidoEspecificar", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEflLiquidoEspecificar", @id = "txtEflLiquidoEspecificar", @maxlength = 80 }))%>
			</div>
		</div>
		<% } %>
		<div class="block ">
			<div class="divEfluenteLiquido">
				<table class="dataGridTable gridEfluenteLiquido" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Fontes de geração</th>
							<th>Vazão</th>
							<th>Unidade</th>
							<th>Sistema de tratamento</th>
							<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.DscLicAtividade.EfluentesLiquido) { %>
						<tr>
							<td><span class="spanFonteGeracao" title="<%= (item.TipoId != (int)eFontesGeracao.Outros)?item.TipoTexto:item.Descricao %>"><%= (item.TipoId != (int)eFontesGeracao.Outros) ? item.TipoTexto : item.Descricao%></span></td>
							<td><span class="spanVazao" title="<%= item.Vazao %>"><%= item.Vazao %></span></td>
							<td><span class="spanUnidade" title="<%= item.UnidadeTexto %>"><%= item.UnidadeTexto %></span></td>
							<td><span class="spanSisTratamento" title="<%= item.SistemaTratamento %>"><%= item.SistemaTratamento %></span></td>
							<% if (!Model.IsVisualizar) { %>
							<td>
								<input type="hidden" class="hdnItemEfluenteLiquido" name="hdnItemEfluenteLiquido" value='<%= Model.GetJSON(item) %>' />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
							<% } %>
						</tr>
						<% } %>
					</tbody>
				</table>
				<table style="display: none">
					<tbody>
						<tr class="trEfluenteLiquidoTemplate">
							<td><span class="spanFonteGeracao"></span></td>
							<td><span class="spanVazao"></span></td>
							<td><span class="spanUnidade"></span></td>
							<td><span class="spanSisTratamento"></span></td>
							<td>
								<input type="hidden" class="hdnItemEfluenteLiquido" name="hdnItemEfluenteLiquido" value="" />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>	
	<fieldset class="block box">
		<legend class="titFiltros">Resíduos sólidos não inertes</legend>	
		<% if (!Model.IsVisualizar){ %>
		<div class="block">
			<div class="coluna20">
				<label for="txtClasseRediduo">Classe do resíduo *</label>
				<%= Html.TextBox("txtClasseRediduo", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtClasseRediduo", @id = "txtClasseRediduo", @maxlength = 5 }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna90">
				<label for="txtTipoRediduo">Tipo de resíduo *</label>
				<%= Html.TextBox("txtTipoRediduo", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTipoRediduo", @id = "txtTipoRediduo", @maxlength = 500 }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna40">
				<label for="">Acondicionamento *</label><br />
				<div class="divCkbAcondicionamento block">
				<%for (int i = 0; i < Model.Acondicionamento.Count; i++) { %>
					<label class="labelCheckBox">
					<%= Html.CheckBox("ck" + Model.Acondicionamento[i].Texto, false, ViewModelHelper.SetaDisabled(false, new { @class = "checkbox ckbAcondicionamento", @title = Model.Acondicionamento[i].Texto, @value = Model.Acondicionamento[i].Id + "|" + Model.Acondicionamento[i].Codigo }))%>
					<%: Model.Acondicionamento[i].Texto%></label>
				<% } %>
				</div>
			</div>
			<div class="coluna20">
				<label for="">Estocagem *</label><br />
				<div class="divCkbEstocagem">
				<%for (int i = 0; i < Model.Estocagem.Count; i++) { %>
					<label class="labelCheckBox">
					<%= Html.CheckBox("ck" + Model.Estocagem[i].Texto, false, ViewModelHelper.SetaDisabled(false, new { @class = "checkbox ckbEstocagem", @title = Model.Estocagem[i].Texto, @value = Model.Estocagem[i].Id + "|" + Model.Estocagem[i].Codigo }))%>
					<%: Model.Estocagem[i].Texto%></label>
				<% } %>
				</div>
			</div>
		</div>

		<div class="block">
			<div class="coluna55">
				<label for="">Tratamento *</label><br />
				<div class="divCkbTratamento">
				<%for (int i = 0; i < Model.Tratamento.Count; i++) { %>
					<label class="labelCheckBox">
					<%= Html.CheckBox("ck" + Model.Tratamento[i].Texto, false, ViewModelHelper.SetaDisabled(false, new { @class = "checkbox ckbTratamento", @title = Model.Tratamento[i].Texto, @value = Model.Tratamento[i].Id + "|" + Model.Tratamento[i].Codigo }))%>
					<%: Model.Tratamento[i].Texto%></label>
				<% } %>
				</div>
			</div>
			<div class="divTratamentoEspecificar coluna40 hide">
				<br />
				<label for="txtTratamentoEspecificar">Especificar *</label>
				<%= Html.TextBox("txtTratamentoEspecificar", "", ViewModelHelper.SetaDisabled(false, new { @class = "text txtTratamentoEspecificar", @id = "txtTratamentoEspecificar", @maxlength = 100 }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna30">
				<label for="">Destino Final *</label><br />
				<div class="divCkbDestinoFinal">
				<%for (int i = 0; i < Model.DestinoFinal.Count; i++) { %>
					<label class="labelCheckBox">
					<%= Html.CheckBox("ck" + Model.DestinoFinal[i].Texto, false, ViewModelHelper.SetaDisabled(false, new { @class = "checkbox ckbDestinoFinal", @title = Model.DestinoFinal[i].Texto, @value = Model.DestinoFinal[i].Id + "|" + Model.DestinoFinal[i].Codigo }))%>
					<%: Model.DestinoFinal[i].Texto%></label>
				<% } %>
				</div>
			</div>
			<div class="divDestinoFinalEspecificar coluna40 hide">
				<br />
				<label for="txtDestinoFinalEspecificar">Especificar *</label>
				<%= Html.TextBox("txtDestinoFinalEspecificar", "", ViewModelHelper.SetaDisabled(false, new { @class = "text txtDestinoFinalEspecificar", @id = "txtDestinoFinalEspecificar", @maxlength = 100 }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna100 botaoResiduoSolido">
				<button type="button" style="width:35px; float: right;" class="inlineBotao botaoAdicionarIcone btnAddResiduoSolido" title="Adicionar resíduo sólido">Adicionar</button>		  
			</div>
		</div>
		<% } %>
		<div class="block ">
			<div class="divResiduoSolido">
				<table class="dataGridTable gridResiduoSolido" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Classe</th>
							<th>Tipo</th>
							<th>Acondicionamento</th>
							<th>Estocagem</th>
							<th>Tratamento</th>
							<th>Destino final</th>
							<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.DscLicAtividade.ResiduosSolidosNaoInerte) { %>
						<tr>
							<td><span class="spanClasse" title="<%= item.ClasseResiduo %>"><%= item.ClasseResiduo %></span></td>
							<td><span class="spanTipo" title="<%= item.Tipo %>"><%= item.Tipo %></span></td>
							<td><span class="spanAcondicionamento" title="<%= item.AcondicionamentoTexto %>"><%= item.AcondicionamentoTexto %></span></td>
							<td><span class="spanEstocagem" title="<%= item.EstocagemTexto %>"><%= item.EstocagemTexto%></span></td>
							<td><span class="spanTratamento" title="<%= item.TratamentoOutros %>"><%= item.TratamentoOutros%></span></td>
							<td><span class="spanDestinoFinal" title="<%= item.DestinoFinalOutros %>"><%= item.DestinoFinalOutros%></span></td>
							<% if (!Model.IsVisualizar) { %>
							<td>
								<input type="hidden" class="hdnItemResiduoSolidoNaoInerte" name="hdnItemResiduoSolidoNaoInerte" value='<%= Model.GetJSON(item) %>' />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
							<% } %>
						</tr>
						<% } %>
					</tbody>
				</table>
				<table style="display: none">
					<tbody>
						<tr class="trResiduoSolidoTemplate">
							<td><span class="spanClasse"></span></td>
							<td><span class="spanTipo"></span></td>
							<td><span class="spanAcondicionamento"></span></td>
							<td><span class="spanEstocagem"></span></td>
							<td><span class="spanTratamento"></span></td>
							<td><span class="spanDestinoFinal"></span></td>
							<td>
								<input type="hidden" class="hdnItemResiduoSolidoNaoInerte" name="hdnItemResiduoSolidoNaoInerte" value="" />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>		

	</fieldset>	
	<fieldset class="block box">
		<legend class="titFiltros">Emissões atmosféricas</legend>
		<% if (!Model.IsVisualizar){ %>
		<div class="block">
			<div class="coluna20 append2">
				<label for="ddlCombustivel">Combustível *</label>
				<%= Html.DropDownList("ddlCombustivel", Model.CombustivelTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCombustivel", @id = "ddlCombustivel" }))%>
			</div>
			<div class="coluna20 append2">
				<label for="txtSubstanciaEmitida">Substância emitida *</label>
				<%= Html.TextBox("txtSubstanciaEmitida", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtSubstanciaEmitida", @id = "txtSubstanciaEmitida", @maxlength = 50 }))%>
			</div>

			<div class="coluna25 append2">
				<label for="txtEquipamentoControle">Equipamento de controle *</label>
				<%= Html.TextBox("txtEquipamentoControle", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEquipamentoControle", @id = "txtEquipamentoControle", @maxlength = 50 }))%>
			</div>
			<div class="coluna20 botaoEmissaoAtmosferica">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddEmissaoAtmosferica" title="Adicionar emissão atmosférica">Adicionar</button>		  
			</div>
		</div>
		<% } %>
		<div class="block ">
			<div class="divEmissaoAtmosferica">
				<table class="dataGridTable gridEmissaoAtmosferica" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Combutível</th>
							<th>Substância emitida</th>
							<th>Equipamento de controle</th>
							<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.DscLicAtividade.EmissoesAtmosfericas) { %>
						<tr>
							<td><span class="spanCombutivel" title="<%= item.TipoCombustivelTexto %>"><%= item.TipoCombustivelTexto %></span></td>
							<td><span class="spanSubstanciaEmitida" title="<%= item.SubstanciaEmitida %>"><%= item.SubstanciaEmitida %></span></td>
							<td><span class="spanEquipamentoControle" title="<%= item.EquipamentoControle %>"><%= item.EquipamentoControle %></span></td>
							<% if (!Model.IsVisualizar) { %>
							<td>
								<input type="hidden" class="hdnItemEmissaoAtmosferica" name="hdnItemEmissaoAtmosferica" value='<%= Model.GetJSON(item) %>' />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
							<% } %>
						</tr>
						<% } %>
					</tbody>
				</table>
				<table style="display: none">
					<tbody>
						<tr class="trEmissaoAtmosferica">
							<td><span class="spanCombutivel"></span></td>
							<td><span class="spanSubstanciaEmitida"></span></td>
							<td><span class="spanEquipamentoControle"></span></td>
							<td>
								<input type="hidden" class="hdnItemEmissaoAtmosferica" name="hdnItemEmissaoAtmosferica" value="" />
								<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>		
	</fieldset>
</div>