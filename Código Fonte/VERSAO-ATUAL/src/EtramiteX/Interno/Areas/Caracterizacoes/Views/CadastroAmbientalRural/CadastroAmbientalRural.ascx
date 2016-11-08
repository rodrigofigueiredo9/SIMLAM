<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.CadastroAmbientalRuralVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript">
	CadastroAmbientalRural.settings.dependencias = '<%= ViewModelHelper.Json(Model.Caracterizacao.Dependencias) %>';
	CadastroAmbientalRural.settings.textoMerge = '<%= Model.TextoMerge %>';
	CadastroAmbientalRural.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';
</script>

<input type="hidden" class="hdnCaracterizacaoId" value="<%=Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnProjetoGeoId" value="<%=Model.Caracterizacao.ProjetoGeoId %>" />
<input type="hidden" class="hdnEmpreendimentoId" value="<%=Model.Caracterizacao.EmpreendimentoId %>" />
<input type="hidden" class="hdnSituacaoId" value="<%=Model.Caracterizacao.Situacao.Id %>" />
<input type="hidden" class="hdnSituacaoProcessamentoId" value="<%: Model.Caracterizacao.SituacaoProcessamento.Id %>"/>
<input type="hidden" class="hdnAbrangenciaMBR" value="<%: Json.Encode(Model.Abrangencia) %>"/>

<fieldset class="block box">
	<legend>Dados do Imóvel Rural</legend>
	<div class="block">
		<div class="coluna30 append2 divArea">
			<% var area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ATP_CROQUI); %>
			<label>ATP croqui (m²)</label>
			<%= Html.TextBox("ATP_Croqui", area.Valor.ToStringTrunc(), new { @class = "text txtAtpCroquiM disabled", @disabled="disabled" })%>
			<input type="hidden" class="hdnAreaJson hdnAtpCroquiM" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna30 append2">
			<label>ATP croqui (ha)</label>
			<%= Html.TextBox("ATP_CroquiHa", area.Valor.Convert(eMetrica.M2ToHa).ToStringTrunc(4), new { @class = "text txtAtpCroquiHa disabled", @disabled="disabled"  })%>
			<input type="hidden" class="hdnAtpCroquiHA" value="<%: area.Valor.Convert(eMetrica.M2ToHa) %>" />
		</div>

		<div class="coluna33 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.AREA_DOCUMENTO); %>
			<label>Área documento (m²)</label>
			<%= Html.TextBox("Area_Documento", area.Valor.ToStringTrunc() , new { @class = "text txtAreaDocumento disabled", @disabled="disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>
	</div>
	<div class="block">
		<div class="coluna30 append2 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.AREA_CCRI); %>
			<label>Área no CCIR (m²)</label>
			<%= Html.TextBox("Area_CCIR", area.Valor.ToStringTrunc(), new { @class = "text txtAreaCCIR disabled", @disabled="disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna30 append2 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.AREA_USO_ALTERNATIVO); %>
			<label>Área de uso alternativo (ha)</label>
			<%= Html.TextBox("Area_Uso_Alternativo", area.Valor.Convert(eMetrica.M2ToHa).ToStringTrunc(4), new { @class = "text txtAreaUsoAlternativo disabled", @disabled="disabled"})%>
			<input type="hidden" class="hdnAreaJson hdnAreaUsoAlternativo" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna33 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.AREA_USO_RESTRITO_DECLIVIDADE); %>
			<label>Área de uso restrito por declividade (ha)</label>
			<%= Html.TextBox("Area_Uso_Restrito_Declividade", area.Valor.Convert(eMetrica.M2ToHa).ToStringTrunc(4), new { @class = "text txtAreaUsoRestritoDeclividade disabled", @disabled="disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>
	</div>
	<div class="block">
		<div class="coluna30 append2">
			<label>Ocorreu alteração no tamanho da área do imóvel após 22/07/2008 ? *</label>
			<%= Html.DropDownList("OcorreuAlteracaoApos2008", Model.ListaBoolean, ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class = "text ddlOcorreuAlteracaoApos2008 campoEditavel" }))%>
		</div>

		<div class="coluna30 divAtp2008 <%: Model.Caracterizacao.OcorreuAlteracaoApos2008 > 0 ? "" : "hide" %>">
			<label>ATP Documento em 22/07/2008 (m²) *</label>
			<%= Html.TextBox("ATPDocumento2008", Model.Caracterizacao.ATPDocumento2008.ToStringTrunc(), ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @maxlength = "21", @class = "text maskDecimalPonto setarFoco txtAtp2008 campoEditavel" }))%>
		</div>
	</div>
	
	<div class="block">
		<div class="coluna30 append2">
			<label>Vistoria para aprovação do CAR? *</label>
			<%= Html.DropDownList("VistoriaAprovacaoCAR", Model.ListaBoolean, ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class = "ddlVistoriaAprovacaoCAR campoEditavel" }))%>
		</div>

		<div class="coluna30 <%: Model.Caracterizacao.VistoriaAprovacaoCAR > 0 ? "" : "hide" %>">
			<label>Data de vistoria *</label>
			<%= Html.TextBox("DataVistoriaAprovacao", Model.Caracterizacao.DataVistoriaAprovacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class = "text maskData setarFoco txtDataVistoriaAprovacao campoEditavel" }))%>
		</div>
	</div>

</fieldset>

<fieldset class="block box">
	<legend>Módulos fiscais</legend>
	<div class="block">
		<div class="coluna30 append2">
			<label>Município *</label>
			<%= Html.DropDownList("Municipio", Model.ListaMunicipios, ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class = "text ddlMunicipio campoEditavel" }))%>
		</div>
	</div>

	<div class="block">
		<input type="hidden" class="hdnModuloFiscalId" value="<%=Model.Caracterizacao.ModuloFiscalId %>" />
		<div class="coluna30 append2">
			<label>1 Módulo Fiscal (ha)</label>
			<%= Html.TextBox("ModuloFiscalHA", Model.Caracterizacao.ModuloFiscalHA, new { @maxlength = "100", @class = "text txtModuloFiscalHa disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna30">
			<label>Qtd. Módulo fiscal da ATP</label>
			<%= Html.TextBox("ATPQuantidadeModuloFiscal", Model.Caracterizacao.ATPQuantidadeModuloFiscal.ToStringTrunc(2), new { @class = "text txtQtdModuloFiscalAtp disabled", @disabled = "disabled" })%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Áreas de Preservação Permanente</legend>
	<div class="block">
		<div class="coluna30 append2 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_TOTAL_CROQUI); %>
			<label>APP Total do croqui (m²)</label>
			<%= Html.TextBox("APP_Total_Croqui", area.Valor.ToStringTrunc(), new { @class = "text txtAppTotalCroqui disabled", @disabled = "disabled"})%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna30 append2">
			<label>% Total de APP</label>
			<%= Html.TextBox("Percent_Total_APP", Model.Caracterizacao.PercentTotalAPP.ToStringTrunc(), new { @class = "text txtPercentTotalApp disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna33">
			<label>% Máxima de recuperação de APP</label>
			<%= Html.TextBox("Percent_Max_Recuperacao_APP", Model.PercentMaximaRecuperacaoAPP, new { @class = "text txtPercentMaxRecuperacaoApp disabled", @disabled = "disabled"})%>
		</div>
	</div>

	<fieldset class="block boxBranca">
		<legend>Uso/ Ocupação da APP no croqui</legend>
		<div class="block">
			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_PRESERVADA); %>
				<label>Preservada (m²)</label>
				<%= Html.TextBox("Preservada", area.Valor.ToStringTrunc(), new { @class = "text txtPreservada disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_Preservada", Model.Caracterizacao.PercentTotalAPPPreservada.ToStringTrunc(), new { @class = "text txtPercentPreservada disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERACAO); %>
				<label>Em recuperação (m²)</label>
				<%= Html.TextBox("Em_Recuperacao", area.Valor.ToStringTrunc(), new { @class = "text txtEmRecuperacao disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_Recuperacao", Model.Caracterizacao.PercentTotalAPPRecuperacao.ToStringTrunc(), new { @class = "text txtPercentRecuperacao disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_USO); %>
				<label>Em uso (m²)</label>
				<%= Html.TextBox("Em_Uso",  area.Valor.ToStringTrunc(), new { @class = "text txtEmUso disabled", @disabled = "disabled"})%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5">
				<label>%</label>
				<%= Html.TextBox("Percent_Uso", Model.Caracterizacao.PercentTotalAPPUso.ToStringTrunc(), new { @class = "text txtPercentUso disabled", @disabled = "disabled" })%>
			</div>
		</div>
		<div class="block">
			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO); %>
				<label>A recuperar (Calculado) (m²)</label>
				<%= Html.TextBox("A_Recuperar_Calculado", area.Valor.ToStringTrunc() , new { @class = "text txtRecuperarCalculado disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson hdnAPPRecuperarCalculado" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_Recuperar_Calculado", Model.Caracterizacao.PercentTotalAPPRecuperarCalculado.ToStringTrunc(), new { @class = "text txtPercentRecuperarCalculado disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA); %>
				<label>A Recuperar (Efetiva) (m²)</label>
				<%= Html.TextBox("A_Recuperar_Efetiva", area.Valor.ToStringTrunc(), new { @class = "text txtRecuperarEfetiva disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson hdnAreaAPPRecuperarEfeitva" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_Recuperar_Efetiva", Model.Caracterizacao.PercentTotalAPPRecuperarEfetiva.ToStringTrunc(), new { @class = "text txtPercentRecuperarEfetiva disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_USO_CONSOLIDADO); %>
				<label>Com uso consolidado (m²)</label>
				<%= Html.TextBox("Uso_Consolidado", area.Valor.ToStringTrunc(), new { @class = "text txtUsoConsolidado disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson hdnAPPUsoConsolidado" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5">
				<label>%</label>
				<%= Html.TextBox("Percent_Uso_Consolidado", Model.Caracterizacao.PercentTotalAPPRecuperarUsoConsolidado.ToStringTrunc() , new { @class = "text txtPercentUsoConsolidado disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.APP_NAO_CARACTERIZADA); %>
				<label class="<%: Model.PossuiAPPNaoCaracterizada ? "labelAlerta" : "" %>" >Não caracterizada (m²)</label>
				<%= Html.TextBox("Nao_Caracterizada", area.Valor.ToStringTrunc(), new { @class = "text txtNaoCaracterizada disabled " + (Model.PossuiAPPNaoCaracterizada ? "alertaCampo" : ""), @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>
		</div>
	</fieldset>
</fieldset>

<fieldset class="block box">
	<legend>Áreas de Reserva Legal</legend>
	<div class="block">
		<div class="coluna25 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CROQUI); %>
			<label>Total ARL croqui (m²)</label>
			<%= Html.TextBox("ARL_Croqui", area.Valor.ToStringTrunc(), new { @class = "text txtArlCroqui disabled", @disabled = "disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna5 append2">
			<label>%</label>
			<%= Html.TextBox("Percent_ARL_Croqui", Model.Caracterizacao.PercentARLCroqui.ToStringTrunc(), new { @class = "text txtPercentArlCroqui disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna25 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_EMPREENDIMENTO); %>
			<label>ARL do empreendimento (m²)</label>
			<%= Html.TextBox("ARL_Empreendimento", area.Valor.ToStringTrunc(), new { @class = "text txtArlEmpreendimento disabled", @disabled = "disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna5 append2">
			<label>%</label>
			<%= Html.TextBox("Percent_ARL_Empreendimento", Model.Caracterizacao.PercentARLEmpreendimento.ToStringTrunc(), new { @class = "text txtPercentArlEmpreendimento disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna20 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_DOCUMENTO); %>
			<label>ARL documento (m²)</label>
			<%= Html.TextBox("ARL_Documento", area.Valor.ToStringTrunc(), new { @class = "text txtArlDocumento disabled", @disabled = "disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna5 append2">
			<label>%</label>
			<%= Html.TextBox("Percent_ARL_Documento", Model.Caracterizacao.PercentARLDocumento.ToStringTrunc(), new { @class = "text txtPercentArlDocumento disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna25 divArea">
			<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_APP); %>
			<label>ARL em APP (m²)</label>
			<%= Html.TextBox("ARL_APP", area.Valor.ToStringTrunc(), new { @class = "text txtArlApp disabled", @disabled = "disabled" })%>
			<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
		</div>

		<div class="coluna5">
			<label>%</label>
			<%= Html.TextBox("Percent_ARL_APP", Model.Caracterizacao.PercentARLAPP.ToStringTrunc(), new { @class = "text txtPercentArlApp disabled", @disabled = "disabled" })%>
		</div>
	</div>


	<fieldset class="block boxBranca">
		<legend>ARL do empreendimento por situação vegetal no croqui</legend>
		<div class="block">
			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_PRESERVADA); %>
				<label>Preservada (m²)</label>
				<%= Html.TextBox("Situacao_Preservada", area.Valor.ToStringTrunc(), new { @class = "text txtArlPreservada disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_ARL_Preservada", Model.Caracterizacao.PercentARLPreservada.ToStringTrunc(), new { @class = "text txtPercentArlPreservada disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECUPERACAO); %>
				<label>Em recuperação (m²)</label>
				<%= Html.TextBox("ARL_Recuperacao", area.Valor.ToStringTrunc(), new { @class = "text txtArlRecuperacao disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5 append2">
				<label>%</label>
				<%= Html.TextBox("Percent_ARL_Recuperacao", Model.Caracterizacao.PercentARLRecuperacao.ToStringTrunc(), new { @class = "text txtPercentArlRecuperacao disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna25 divArea">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECUPERAR); %>
				<label>A recuperar (m²)</label>
				<%= Html.TextBox("ARL_Recuperar", area.Valor.ToStringTrunc(), new { @class = "text txtArlRecuperar disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>

			<div class="coluna5">
				<label>%</label>
				<%= Html.TextBox("Percent_ARL_Recuperar", Model.Caracterizacao.PercentARLRecuperar.ToStringTrunc(), new { @class = "text txtPercentArlRecuperar disabled", @disabled = "disabled" })%>
			</div>
		</div>
				
		<div class="block divArea">
			<div class="coluna25">
				<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_NAO_CARACTERIZADA); %>
				<label class="<%: Model.PossuiARLNaoCaracterizada ? "labelAlerta" : "" %>" >Não caracterizada (m²)</label>
				<%= Html.TextBox("ARL_Nao_Caracterizada", area.Valor.ToStringTrunc(), new { @class = "text txtArlNaoCaracterizada disabled " + (Model.PossuiARLNaoCaracterizada ? "alertaCampo" : ""), @disabled = "disabled"})%>
				<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
			</div>
		</div>
		
		<%if(decimal.Round(Model.Caracterizacao.PercentARLEmpreendimento) < 20 && (!Model.Caracterizacao.IsCedente && !Model.Caracterizacao.IsReceptor)){ %>
		<div class="block">
			<div class="coluna90">
				<label>
					<%=Html.CheckBox("cbDispensaARL", Model.Caracterizacao.DispensaARL, ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class="cbDispensaARL campoEditavel" })) %>
					Reserva legal constituída com área ocupada com vegetação nativa existente em 22/07/2008, nos termos do Art. 67 da Lei Federal 12.651/12.
				</label>
			</div>
		</div>

		<div class="block">
			<div class="coluna30">
				<label class="inline">
					<%=Html.CheckBox("ReservaLegalEmOutroCAR", Model.Caracterizacao.ReservaLegalEmOutroCAR.GetValueOrDefault(), ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class="cbReservaLegalEmOutroCAR campoEditavel" })) %>
					Reserva legal em outro CAR.
				</label>
			</div>
			<div class="cedente <%=Model.Caracterizacao.ReservaLegalEmOutroCAR.GetValueOrDefault()? "":"hide" %>">
				<div class="coluna30 preppend2">
					<label>Nº do CAR *</label>
					<%= Html.TextBox("N_CAR_Receptor", Model.Caracterizacao.CodigoEmpreendimentoReceptor, new { @class = "text txtCodigoEmpreendimentoReceptor disabled", @disabled = "disabled" })%>
					<%= Html.Hidden("EmpeendimentoReceptor", Model.Caracterizacao.EmpreendimentoReceptorId, new { @class="hdnEmpreendimentoReceptorId"})%> 
				</div>

				<% if(!Model.IsVisualizar) { %>
				<div class="coluna10 preppend2 ">
					<input class="btnBuscarEmpreendimento inlineBotao btnreceptor <%:(Model.IsBloquearCampos) ? "hide" : "" %>" type="button" value="Buscar" />
				</div>
				<% } %>
			</div>
		</div>

		<%}%>
			
		<%if (decimal.Round(Model.Caracterizacao.PercentARLEmpreendimento) > 20 && (!Model.Caracterizacao.IsCedente && !Model.Caracterizacao.IsReceptor)){ %>
		<div class="block">
			<div class="coluna30">
				<label>
					<%=Html.CheckBox("ReservaLegalDeOutroCAR", Model.Caracterizacao.ReservaLegalDeOutroCAR.GetValueOrDefault() , ViewModelHelper.SetaDisabled(Model.IsBloquearCampos, new { @class="cbReservaLegalDeOutroCAR campoEditavel" })) %>
					Reserva legal de outro CAR.
				</label>
			</div>

			<div class="receptor <%=Model.Caracterizacao.ReservaLegalDeOutroCAR.GetValueOrDefault()? "":"hide" %>">
				<div class="coluna40 preppend2">
					<label>Nº do CAR *</label>
					<%= Html.TextBox("N_CAR_Cedente", Model.Caracterizacao.CodigoEmpreendimentoCedente, new { @class = "text txtCodigoEmpreendimentoCedente disabled", @disabled = "disabled" })%>
					<%= Html.Hidden("EmpeendimentoCedente", Model.Caracterizacao.EmpreendimentoCedenteId, new { @class="hdnEmpreendimentoCedenteId"})%> 
				</div>

				<% if(!Model.IsVisualizar) { %>
				<div class="coluna10 preppend2">
					<input class="btnBuscarEmpreendimento inlineBotao <%:(Model.IsBloquearCampos) ? "hide" : "" %>" type="button" value="Buscar" />
				</div>
				<% } %>
			</div>
		</div>
		<%} %>

	</fieldset>
</fieldset>
<%if(Model.Caracterizacao.IsCedente || Model.Caracterizacao.IsReceptor){ %>
	<fieldset class="block box">
		<legend>Áreas de Reserva Legal Compensada</legend>
	
		<%if(Model.Caracterizacao.IsCedente){ %>
			<div class="block boxBranca">
				<div class="block">
					<div class="coluna40">
						<label>Tipo de Compensação *</label>
						<%= Html.DropDownList("Tipo_Compensacao", new SelectListItem[1]{ new SelectListItem(){ Text="Cedente", Value="1"} }, new { @class = "text ddlTipoCompensacao disabled", @disabled = "disabled" })%>
					</div>
					
					<div class="coluna40 preppend2 divArea">
						<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE); %>
						<label>Total de ARL Compensada (m²)</label>
						<%= Html.TextBox("Total_ARL_Compensada", area.Valor.ToStringTrunc(), new { @class = "text txtTotalARLCompensada disabled", @disabled = "disabled" })%>
						<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					</div>
					<div class="coluna5 preppend2">
						<label>%</label>
						<%= Html.TextBox("Percent_Total_ARL_Compensada", Model.Caracterizacao.PercentTotalARLCompensadaCedente.ToStringTrunc(), new { @class = "text txtPercentTotalARLCompensada disabled", @disabled = "disabled" })%>
					</div>
				</div>
		
				<fieldset class="block box">
					<legend>Total de ARL Compensada por situação vegetal no croqui</legend>
			
					<div class="block">
						<div class="coluna25 divArea">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA); %>
							<label>Preservada (m²)</label>
							<%= Html.TextBox("Preservada", area.Valor.ToStringTrunc(), new { @class = "text txtPreservada disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Preservada_Cedente", Model.Caracterizacao.PercentARLCedentePreservada.ToStringTrunc(), new { @class = "text txtPercentARLPreservadaCedente disabled", @disabled = "disabled" })%>
						</div>

						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO); %>
							<label>Em recuperação (m²)</label>
							<%= Html.TextBox("ARL_CEDENTE_EM_RECUPERACAO", area.Valor.ToStringTrunc(), new { @class = "text txtARLCedenteEmRecuperacao disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Cedente_Recuperacao", Model.Caracterizacao.PercentARLCedenteEmRecuperacao.ToStringTrunc(), new { @class = "text txtPercentARLCedenteEmRecuperacao disabled", @disabled = "disabled" })%>
						</div>
						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR); %>
							<label>A recuperar (m²)</label>
							<%= Html.TextBox("ARL_CEDENTE_RECUPERAR", area.Valor.ToStringTrunc(), new { @class = "text txtARLCedenteRecuperar disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Preservada_Cedente", Model.Caracterizacao.PercentARLCedenteRecuperar.ToStringTrunc(), new { @class = "text txtPercentARLCedenteRecuperar disabled", @disabled = "disabled" })%>
						</div>
					</div>
					<div class="block">
						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA); %>
							<label>Não caracterizada (m²)</label>
							<%= Html.TextBox("ARL_CEDENTE_NAO_CARACTERIZADA", area.Valor.ToStringTrunc(), new { @class = "text txtARLCedenteNaoCaracterizada disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
					</div>
				</fieldset>
			</div>
		<% } %>

		<%if(Model.Caracterizacao.IsReceptor){ %>
			<div class="block boxBranca">
				<div class="block">
					<div class="coluna40">
						<label>Tipo de Compensação *</label>
						<%= Html.DropDownList("Tipo_Compensacao", new SelectListItem[1] { new SelectListItem(){ Text="Receptor", Value="2"}}, new { @class = "text ddlTipoCompensacaoReceptor disabled", @disabled = "disabled" })%>
					</div>
		
					<div class="coluna40 preppend2 divArea">
						<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA); %>
						<label>Total de ARL Compensada (m²)</label>
						<%= Html.TextBox("Total_ARL_Compensada", area.Valor.ToStringTrunc(), new { @class = "text txtTotalARLCompensada disabled", @disabled = "disabled" })%>
						<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
					</div>
					<div class="coluna5 preppend2">
						<label>%</label>
						<%= Html.TextBox("Percent_Total_ARL_Compensada", Model.Caracterizacao.PercentTotalARLCompensadaReceptor.ToStringTrunc(), new { @class = "text txtPercentTotalARLCompensada disabled", @disabled = "disabled" })%>
					</div>
				</div>

				<fieldset class="block box">
					<legend>Total de ARL Compensada por situação vegetal no croqui</legend>
			
					<div class="block">
						<div class="coluna25 divArea">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA); %>
							<label>Preservada (m²)</label>
							<%= Html.TextBox("ARL_RECEPTORA_PRESERVADA", area.Valor.ToStringTrunc(), new { @class = "text txtReceptoraPreservada disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Receptora_Preservada", Model.Caracterizacao.PercentARLReceptoraPreservada.ToStringTrunc(), new { @class = "text txtPercentARLReceptoraPreservada disabled", @disabled = "disabled" })%>
						</div>

						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO); %>
							<label>Em recuperação (m²)</label>
							<%= Html.TextBox("ARL_RECEPTORA_EM_RECUPERACAO", area.Valor.ToStringTrunc(), new { @class = "text txtARLReceptoraEmRecuperacao disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Receptora_Recuperacao", Model.Caracterizacao.PercentARLReceptoraEmRecuperacao.ToStringTrunc(), new { @class = "text txtPercentARLReceptoraEmRecuperacao disabled", @disabled = "disabled" })%>
						</div>
						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR); %>
							<label>A recuperar (m²)</label>
							<%= Html.TextBox("ARL_RECEPTORA_RECUPERAR", area.Valor.ToStringTrunc(), new { @class = "text txtARLReceptoraRecuperar disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
						<div class="coluna5 preppend2">
							<label>%</label>
							<%= Html.TextBox("Percent_ARL_Receptora_Recuperar", Model.Caracterizacao.PercentARLReceptoraRecuperar.ToStringTrunc(), new { @class = "text txtPercentARLReceptoraRecuperar disabled", @disabled = "disabled" })%>
						</div>
					</div>
					<div class="block">
						<div class="coluna25 divArea preppend3">
							<% area = Model.Caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA); %>
							<label>Não caracterizada (m²)</label>
							<%= Html.TextBox("ARL_RECEPTORA_NAO_CARACTERIZADA", area.Valor.ToStringTrunc(), new { @class = "text txtARLReceptoraNaoCaracterizada disabled", @disabled = "disabled" })%>
							<input type="hidden" class="hdnAreaJson" value="<%: Json.Encode(area) %>" />
						</div>
					</div>
				</fieldset>
			</div>
		<% }%>
	</fieldset>
<%} %>
<fieldset class="block box">
	<legend>Processamento de faixa de recuperação</legend>
	<div class="block divMapaVisualiar <%: (Model.IsFinalizado)?"":"hide" %>">
		<div class="coluna50">
			<input class="btnMapaVisualiar " type="button" value="Visualizar Desenhador" />
		</div>
	</div>
	<div class="block">
		<div class="coluna50">
			Situação do processamento: <label class="lblSituacaoProcessamento"><%:Model.Caracterizacao.SituacaoProcessamento.Texto %></label>
		</div>

		<% if(!Model.IsVisualizar) { %>
		<div class="prepend2">
			<span class="spanCancelarProcessamento <%:(Model.IsBloquearCampos) ? "" : "hide" %>">
				<input class="btnCancelarProcessamento" type="button" value="Cancelar" />
			</span>
		</div>
		<% } %>
	</div>

	<div class="block dataGrid">
		<div class="ultima">
			<table class="dataGridTable dataGridArquivosProcessados <%:(Model.ArquivosProcessamentoVM.Count > 0) ? "" : "hide" %>" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Arquivos processados para download</th>
						<th width="9%">Ação</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (ArquivoProcessamentoVM arquivo in Model.ArquivosProcessamentoVM){ %>
					<tr>
						<td class="arquivoNome" title="<%: arquivo.Texto %>"><%: arquivo.Texto %></td>
						<td>
							<button class="icone btnBaixar <%= arquivo.IsPDF ? "pdf" : "download"%>" title="baixar"></button>
							<input type="hidden" class="hdnArquivoProcessadoId" value="<%= arquivo.Id %>" />
							<input type="hidden" class="hdnArquivoProcessadoTipoId" value="<%= arquivo.Tipo %>" />
						</td>
					</tr>
					<% } %>
					<tr class="hide trTemplateArqProcessado">
						<td class='arquivoNome'></td>
						<td>
							<input type="hidden" class="hdnArquivoProcessadoId" value="0" />
							<input type="hidden" class="hdnArquivoProcessadoTipoId" value="0" />
							<button class="icone download btnBaixar"></button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>