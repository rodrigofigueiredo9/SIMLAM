<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjetoInfracaoVM>" %>

<div class="FiscalizacaoObjetoInfracaoContainer">
	<input type="hidden" class="hdnObjetoInfracaoId" value="<%:Model.Entidade.Id %>" />
	<input type="hidden" class="hdnIsVisualizar" value="<%:Model.IsVisualizar.ToString().ToLower()%>" />
	<fieldset class="box">
		<legend>Área/Atividade objeto da infração</legend>
		<div class="block">
			<div class="coluna37">
				<label for="ObjetoInfracao_AreaEmbargadaAtvIntermed">Área embargada e/ou atividade interditada? *</label><br />
				<span style="border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpanAreaEmbargadaAtvIntermed">
					<label><%= Html.RadioButton("ObjetoInfracao.AreaEmbargadaAtvIntermed", 1, (Model.Entidade.AreaEmbargadaAtvIntermed == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbAreaEmbargadaAtvIntermed" }))%>Sim</label>
					<label><%= Html.RadioButton("ObjetoInfracao.AreaEmbargadaAtvIntermed", 0, (Model.Entidade.AreaEmbargadaAtvIntermed == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbAreaEmbargadaAtvIntermed" }))%>Não</label>
				</span>
			</div>
		</div>
		<br />

		<div class="divAreaEmbarcada"><div class="block divAreaEmbarcada">
				<div class="coluna15 append2">
					<label for="ObjetoInfracao_TeiGeradoPeloSistema">Gerar nº do TEI? *</label><br />
					<span style="display: inline-block; border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpanTeiGeradoPeloSistema">
						<label><%= Html.RadioButton("ObjetoInfracao.TeiGeradoPeloSistema", 1, (Model.Entidade.TeiGeradoPeloSistema == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbTeiGeradoPeloSistema" }))%>Sim</label>
						<label><%= Html.RadioButton("ObjetoInfracao.TeiGeradoPeloSistema", 0, (Model.Entidade.TeiGeradoPeloSistema == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbTeiGeradoPeloSistema" }))%>Não</label>
					</span>
				</div>

				<div class="coluna15 append2">
					<label for="ObjetoInfracao_TeiGeradoPeloSistemaSerieTipo">Série *</label>
					<%= Html.DropDownList("ObjetoInfracao.TeiGeradoPeloSistemaSerieTipo", Model.Series, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "ddlTeiGeradoPeloSistemaSerieTipo" }))%>
				</div>

				<%if ((Model.Entidade.FiscalizacaoSituacaoId != (int)eFiscalizacaoSituacao.EmAndamento) && 
						Model.Entidade.TeiGeradoPeloSistema.GetValueOrDefault(0) > 0){%>
					<div class="coluna23 append2">
						<label for="ObjetoInfracao_DataLavraturaTermo_DataTexto">Data da lavratura do termo *</label>
						<%= Html.TextBox("ObjetoInfracao.DataLavraturaTermo.DataTexto", Model.DataConclusaoFiscalizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataLavraturaTermo maskData" }))%>
					</div>
				<%}else{ %>
					<div class="coluna23 append2 divTeiGeradoPeloSistema">
						<label for="ObjetoInfracao_DataLavraturaTermo_DataTexto">Data da lavratura do termo *</label>
						<%= Html.TextBox("ObjetoInfracao.DataLavraturaTermo.DataTexto", Model.Entidade.DataLavraturaTermo.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataLavraturaTermo maskData" }))%>
					</div>
				<%} %>

				<div class="coluna15 divTeiGeradoPeloSistema">
					<label for="ObjetoInfracao_NumTeiBloco" class="lblNumTEI">Nº do TEI - bloco *</label>
					<%= Html.TextBox("ObjetoInfracao.NumTeiBloco", Model.Entidade.NumTeiBloco, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumTeiBloco maskNumInt", @maxlength = "10" }))%>
				</div>
			</div>

			<div class="block divTeiGeradoPeloSistema">
				<div class="coluna35 inputFileDiv">
					<label>PDF do termo de embargo/interdição</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Entidade.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.Entidade.Arquivo.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "" : "hide" %>">
						<input type="file" id="file" class="inputFile <%=Model.IsVisualizar ? "disabled" : "" %>" style="display: block" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					</span>
				</div>
				<% if (!Model.IsVisualizar) { %>
				<div style="margin-top:8px" class="coluna25 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Entidade.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
				<%} %>
			</div>

			<div class="block">
				<div class="coluna75">
					<label for="ObjetoInfracao_DescricaoTermoEmbargo">Descrição do termo de embargo/interdição *</label>
					<%= Html.TextBox("ObjetoInfracao.DescricaoTermoEmbargo", Model.Entidade.DescricaoTermoEmbargo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoTermoEmbargo", @maxlength = "150" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna75">
					<label for="ObjetoInfracao_OpniaoAreaDanificada">Opinar quanto ao embargo/interdição da área/atividade, justificando sua manutenção ou a possibilidade de desembargo/desinterdição</label>
					<%= Html.TextArea("ObjetoInfracao.OpniaoAreaDanificada", Model.Entidade.OpniaoAreaDanificada, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtOpniaoAreaDanificada", @maxlength = "1000" }))%>
				</div>
			</div>
		</div>

		<div class="block">
			<div class="coluna52">
				<label for="ObjetoInfracao_ExisteAtvAreaDegrad">Está sendo desenvolvida alguma atividade na área embargada/interditada? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpanExisteAtvAreaDegrad">
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 1, (Model.Entidade.ExisteAtvAreaDegrad == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Sim</label>
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", 0, (Model.Entidade.ExisteAtvAreaDegrad == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Não</label>
					<label><%= Html.RadioButton("ObjetoInfracao.ExisteAtvAreaDegrad", -1, (Model.Entidade.ExisteAtvAreaDegrad == -1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbExisteAtvAreaDegrad" }))%>Não se aplica</label>
				</span>
			</div>
		</div>

		<div class="block divExisteAtvAreaDegradEspecificarTexto">
			<div class="coluna75">
				<label for="ObjetoInfracao_ExisteAtvAreaDegradEspecificarTexto">Especificar *</label>
				<%= Html.TextArea("ObjetoInfracao.ExisteAtvAreaDegradEspecificarTexto", Model.Entidade.ExisteAtvAreaDegradEspecificarTexto, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtExisteAtvAreaDegradEspecificarTexto", @maxlength = "1000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna75">
				<label for="ObjetoInfracao_FundamentoInfracao">Aborde os fundamentos que caracterizam a infração bem como o(s) possível(eis) dano(s) *</label>
				<%= Html.TextArea("ObjetoInfracao.FundamentoInfracao", Model.Entidade.FundamentoInfracao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtFundamentoInfracao", @maxlength = "2000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna75">
				<label for="ObjetoInfracao_UsoSoloAreaDanificada">Qual o uso/ocupação do solo no entorno da área/atividade embargada? </label>
				<%= Html.TextArea("ObjetoInfracao.UsoSoloAreaDanificada", Model.Entidade.UsoSoloAreaDanificada, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtUsoSoloAreaDanificada", @maxlength = "500" }))%>
			</div>
		</div>

		<div class="block">
			<label for="FinalidadeExploracao">Qual a característica do solo da área danificada?</label>
		</div>
		<div class="block">
			<% 
			foreach (var caracteristica in Model.CaracteristicasSolo){
				bool selecionado = ((Model.Entidade.CaracteristicaSoloAreaDanificada != null) && (Model.Entidade.CaracteristicaSoloAreaDanificada.Value & caracteristica.Codigo) > 0); %>
				<label class="coluna15 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
					<input class="checkboxCaracteristicasSolo <%= Model.IsVisualizar ? "disabled" : "" %> checkbox<%= caracteristica.Texto %>" type="checkbox" title="<%= caracteristica.Texto %>" value="<%= caracteristica.Codigo %>" <%= selecionado ? "checked=\"checked\"" : "" %> <%= Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					<%= caracteristica.Texto%>
				</label>
			<% } %>
		</div>

		<div class="block">
			<div class="coluna35">
				<label for="ObjetoInfracao_AreaDeclividadeMedia">Qual a declividade média da área (Graus)?</label>
				<%= Html.TextBox("ObjetoInfracao.AreaDeclividadeMedia", Model.Entidade.AreaDeclividadeMedia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaDeclividadeMedia maskDecimal", @maxlength = "5" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna33">
				<label for="ObjetoInfracao_InfracaoResultouErosaoTipo">A infração resultou em erosão do solo? </label>
				<%= Html.DropDownList("ObjetoInfracao.InfracaoResultouErosaoTipo", Model.ResultouErosao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlInfracaoResultouErosaoTipo" }))%>
			</div>
		</div>		
		<div class="block divTxtErosao<%: Model.Entidade.InfracaoResultouErosaoTipo == 1 ? "" : " hide" %>">
			<div class="coluna75">
				<label for="ObjetoInfracao_InfracaoResultouErosaoTipoTexto">Especificar *</label>
				<%= Html.TextBox("ObjetoInfracao.InfracaoResultouErosaoTipoTexto", Model.Entidade.InfracaoResultouErosaoTipoTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtErosao", @maxlength = "150" }))%>
			</div>
		</div>
	</fieldset>
</div>