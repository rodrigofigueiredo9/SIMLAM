<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AcompanhamentoVM>" %>

<script src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>
<script>
	Acompanhamento.TiposArquivo = <%= Model.TiposArquivoValido %>;
</script>

<div class="containerAcompanhamento">

	<%=Html.Hidden("Acompanhamento_FiscalizacaoId", Model.Acompanhamento.FiscalizacaoId, new { @class = "hdnFiscalizacaoId"}) %>
	<%=Html.Hidden("Acompanhamento_AcompanhamentoId", Model.Acompanhamento.Id, new { @class = "hdnAcompanhamentoId"}) %>
	<%=Html.Hidden("Acompanhamento_PossuiAreaEmbargadaOuAtividadeInterditada", Model.Acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada, new { @class = "hdnPossuiAreaEmbargadaOuAtividadeInterditada"}) %>
	<%=Html.Hidden("Acompanhamento_HouveApreensaoMaterial", Model.Acompanhamento.HouveApreensaoMaterial, new { @class = "hdnHouveApreensaoMaterial"}) %>

	<div class="box">
		<div class="block">
			<div class="coluna25 append2">
				<label for="Acompanhamento_NumeroSufixo">Nº acompanhamento</label><br />
				<%= Html.TextBox("Acompanhamento.NumeroSufixo", Model.Acompanhamento.Numero, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumeroSufixo" }))%>
			</div>

			<div class="coluna25 append2">
				<label for="Acompanhamento_DataVistoria.DataTexto">Data da vistoria *</label><br />
				<%= Html.TextBox("Acompanhamento.DataVistoria.DataTexto", Model.Acompanhamento.DataVistoria.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataVistoria setarFoco maskData" }))%>
			</div>

			<div class="coluna20">
				<label for="Acompanhamento_SituacaoTexto">Situação do cadastro</label><br />
				<%= Html.TextBox("Acompanhamento.SituacaoTexto", (Model.Acompanhamento.Id <= 0) ? "Em Cadastro" : Model.Acompanhamento.SituacaoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtSituacao" }))%>
			</div>
		</div>

		<% if(Model.IsVisualizar && Model.Acompanhamento.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado) { %>
		<div class="block">
			<div class="coluna76">
				<label for="MotivoSituacao">Motivo do cancelamento</label>
				<%= Html.TextArea("MotivoSituacao", Model.Acompanhamento.Motivo, ViewModelHelper.SetaDisabled(true, new { @class = "text media txtMotivoSituacao" }))%>
			</div>
		</div>
		<% } %>

		<div class="block">
			<div class="coluna76">
				<label for="Acompanhamento_AgenteNome">Agente fiscal *</label><br />
				<%=Html.Hidden("Acompanhamento_AgenteId", Model.Acompanhamento.AgenteId, new { @class = "hdnAgenteId"}) %>
				<%= Html.TextBox("Acompanhamento.AgenteNome", Model.Acompanhamento.AgenteNome, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSetores" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label for="Acompanhamento_Setor">Setor de cadastro *</label><br />
				<%= Html.DropDownList("Acompanhamento.Setor", Model.SetoresLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSetores" }))%>
			</div>
		</div>
		<br />
	</div>

	<fieldset class="box fsDadosPropriedade">
		<legend>Dados da propriedade</legend>

		<div class="block">
			<div class="coluna30 append4">
				<label for="Acompanhamento_AreaTotal">Área total informada (ha)</label>
				<%= Html.TextBox("Acompanhamento.AreaTotalInformada", Model.Acompanhamento.AreaTotal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaTotal maskDecimalPonto4", @maxlength = "14" }))%>
			</div>

			<div class="coluna42">
				<label for="Acompanhamento_AreaFlorestalNativa">Área com cobertura florestal nativa informada/ estimada (ha)</label>
				<%= Html.TextBox("Acompanhamento.AreaFlorestalNativa", Model.Acompanhamento.AreaFlorestalNativa, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaFlorestalNativa maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
		</div>

		<div class="block">
			<label for="Acompanhamento_ReservalegalTipo">Possui area de reserva legal? *</label>
		</div>

		<div class="block">
			<%foreach (var reserva in Model.ReservaLegalTipoLst){
			bool selecionado = ((Model.Acompanhamento.ReservalegalTipo != null) && (Model.Acompanhamento.ReservalegalTipo.Value & reserva.Codigo) > 0); %>
				<label class="coluna16 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
					<input class="checkboxReservasLegais <%= Model.IsVisualizar ? "disabled" : "" %> checkbox<%= reserva.Texto %> checkbox<%= reserva.Id %>" type="checkbox" title="<%= reserva.Texto %>" value="<%= reserva.Codigo %>" <%= selecionado ? "checked=\"checked\"" : "" %> <%= Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					<%= reserva.Texto%>
				</label>
			<% } %>
		</div>

	</fieldset>

	<%if (Model.Acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.HasValue && Model.Acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.Value){%>
	<fieldset class="box fsObjetoInfracao">
		<legend>Área/ Atividade objeto da infração</legend>

		<div class="block">
			<div class="ultima">
				<label for="Acompanhamento_OpniaoAreaEmbargo">Opinar quanto ao embargo/ interdição da área/ atividade, justificando sua manutenção ou a possibilidade de desembargo/ desinterdição</label>
				<%= Html.TextArea("Acompanhamento.OpniaoAreaEmbargo", Model.Acompanhamento.OpniaoAreaEmbargo, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtOpniaoAreaEmbargo", @maxlength = "1000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna72">
				<label>Está sendo desenvolvida alguma atividade na área embargada/ interditada? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnAtividadeAreaEmbargada">
					<label><%= Html.RadioButton("Acompanhamento.AtividadeAreaEmbargada", 1, (Model.Acompanhamento.AtividadeAreaEmbargada == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbAtividadeAreaEmbargada" }))%>Sim</label>
					<label><%= Html.RadioButton("Acompanhamento.AtividadeAreaEmbargada", 0, (Model.Acompanhamento.AtividadeAreaEmbargada == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbAtividadeAreaEmbargada" }))%>Não</label>
					<label><%= Html.RadioButton("Acompanhamento.AtividadeAreaEmbargada", -1, (Model.Acompanhamento.AtividadeAreaEmbargada == -1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbAtividadeAreaEmbargada" }))%>Não se aplica</label>
				</span>
			</div>
		</div>

		<div class="block divAtividadeAreaEmbargadaEspecificarTexto <%=Model.Acompanhamento.AtividadeAreaEmbargada == 1 ? "" : "hide"%>">
			<div class="ultima">
				<label for="Acompanhamento_AtividadeAreaEmbargadaEspecificarTexto">Especificar *</label>
				<%= Html.TextArea("Acompanhamento.AtividadeAreaEmbargadaEspecificarTexto", Model.Acompanhamento.AtividadeAreaEmbargadaEspecificarTexto, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtAtividadeAreaEmbargadaEspecificarTexto", @maxlength = "1000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="ultima">
				<label for="Acompanhamento_UsoAreaSoloDescricao">Qual o uso/ ocupação do solo no entorno da área/ atividade embargada?</label>
				<%= Html.TextArea("Acompanhamento.UsoAreaSoloDescricao", Model.Acompanhamento.UsoAreaSoloDescricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtUsoAreaSoloDescricao", @maxlength = "1000" }))%>
			</div>
		</div>

		<div class="block">
			<label for="Acompanhamento_CaracteristicaSoloAreaDanificadaTipo">Qual a característica do solo da área danificada?</label>
		</div>

		<div class="block">
			<%foreach (var caracteristica in Model.CaracteristicasSoloLst){
				bool selecionado = ((Model.Acompanhamento.CaracteristicaSoloAreaDanificada != null) && (Model.Acompanhamento.CaracteristicaSoloAreaDanificada.Value & caracteristica.Codigo) > 0); %>
				<label class="coluna15 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
					<input class="checkboxCaracteristicasSolo <%= Model.IsVisualizar ? "disabled" : "" %> checkbox<%= caracteristica.Texto %>" type="checkbox" title="<%= caracteristica.Texto %>" value="<%= caracteristica.Codigo %>" <%= selecionado ? "checked=\"checked\"" : "" %> <%= Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
					<%= caracteristica.Texto%>
				</label>
			<% } %>
		</div>

		<div class="block">
			<div class="coluna35">
				<label for="Acompanhamento_AreaDeclividadeMedia">Qual a declividade média da área (Graus)?</label>
				<%= Html.TextBox("ObjetoInfracao.AreaDeclividadeMedia", Model.Acompanhamento.AreaDeclividadeMedia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaDeclividadeMedia maskDecimal", @maxlength = "5" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna33">
				<label for="Acompanhamento_ResultouErosao">A infração resultou em erosão do solo? </label>
				<%= Html.DropDownList("Acompanhamento.ResultouErosao", Model.ResultouErosao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlResultouErosao" }))%>
			</div>
		</div>

		<div class="block divTxtErosao <%: Model.Acompanhamento.InfracaoResultouErosao == 1 ? "" : " hide" %>">
			<div class="ultima">
				<label for="Acompanhamento_InfracaoResultouErosaoEspecificar">Especificar *</label>
				<%= Html.TextArea("Acompanhamento.InfracaoResultouErosaoEspecificar", Model.Acompanhamento.InfracaoResultouErosaoEspecificar, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtInfracaoResultouErosaoEspecificar media", @maxlength = "500" }))%>
			</div>
		</div>
	</fieldset>
	<%} %>

	<%if (Model.Acompanhamento.HouveApreensaoMaterial.HasValue && Model.Acompanhamento.HouveApreensaoMaterial.Value){%>
	<fieldset class="box fsMaterialApreendido">
		<legend>Material apreendido</legend>

		<div class="block">
			<div class="ultima">
				<label>
					Opinar pelo destino (permanência no local, doação, uso pela instituição, entre outros) do material e/ou bens apreendidos, levando-se em
					consideração os seguintes itens: localização e sua dispersão no local, potencial impacto que a retirada do material possa causar à área, valor
					econômico, diâmetro médio das espécies, entre outros.
				</label>
				<%= Html.TextArea("Acompanhamento.OpniaoDestMaterialApreend", Model.Acompanhamento.OpniaoDestMaterialApreend, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtOpniaoDestMaterialApreend", @maxlength = "250" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna52">
				<label>Houve desrespeito ao TAD? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnHouveDesrespeitoTAD">
					<label><%= Html.RadioButton("Acompanhamento.HouveDesrespeitoTAD", 1, (Model.Acompanhamento.HouveDesrespeitoTAD == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbHouveDesrespeitoTAD" }))%>Sim</label>
					<label><%= Html.RadioButton("Acompanhamento.HouveDesrespeitoTAD", 0, (Model.Acompanhamento.HouveDesrespeitoTAD == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbHouveDesrespeitoTAD" }))%>Não</label>
				</span>
			</div>
		</div>

		<div class="block divHouveDesrespeitoTAD <%: Model.Acompanhamento.HouveDesrespeitoTAD == 1 ? "" : " hide" %>">
			<div class="ultima">
				<label for="Acompanhamento_HouveDesrespeitoTADEspecificar">Especificar *</label>
				<%= Html.TextArea("Acompanhamento.HouveDesrespeitoTADEspecificar", Model.Acompanhamento.HouveDesrespeitoTADEspecificar, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtHouveDesrespeitoTADEspecificar", @maxlength = "1000" }))%>
			</div>
		</div>
	</fieldset>
	<%} %>

	<fieldset class="box fsOutrasConsideracoes">
		<legend>Outras considerações</legend>

		<div class="block divInformacoesRelevanteProcesso">
			<div class="ultima">
				<label>Descrever outras informações que julgar relevante para um maior detalhamento e esclarecimento do processo</label>
				<%= Html.TextArea("Acompanhamento.InformacoesRelevanteProcesso", Model.Acompanhamento.InformacoesRelevanteProcesso, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtInformacoesRelevanteProcesso" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna52">
				<label>Há necessidade de reparação do dano ambiental? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnRepararDanoAmbiental">
					<label><%= Html.RadioButton("Acompanhamento.RepararDanoAmbiental", 1, (Model.Acompanhamento.RepararDanoAmbiental == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbRepararDanoAmbiental" }))%>Sim</label>
					<label><%= Html.RadioButton("Acompanhamento.RepararDanoAmbiental", 0, (Model.Acompanhamento.RepararDanoAmbiental == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbRepararDanoAmbiental" }))%>Não</label>
				</span>
			</div>
		</div>

		<div class="block divRepararDanoAmbientalEspecificar <%: Model.Acompanhamento.RepararDanoAmbiental == 1 ? "" : " hide" %>">
			<div class="ultima">
				<label>Opinar quanto à forma de reparação ou justificar caso esta não seja necessária *</label>
				<%= Html.TextArea("Acompanhamento.RepararDanoAmbientalEspecificar", Model.Acompanhamento.RepararDanoAmbientalEspecificar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtRepararDanoAmbientalEspecificar", @maxlength = "2000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna82">
				<label>Firmou termo de compromisso para reparação do dano de acordo com a forma sugerida? *</label><br />
				<span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnFirmouTermoRepararDanoAmbiental">
					<label><%= Html.RadioButton("Acompanhamento.FirmouTermoRepararDanoAmbiental", 1, (Model.Acompanhamento.FirmouTermoRepararDanoAmbiental == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFirmouTermoRepararDanoAmbiental" }))%>Sim</label>
					<label><%= Html.RadioButton("Acompanhamento.FirmouTermoRepararDanoAmbiental", 0, (Model.Acompanhamento.FirmouTermoRepararDanoAmbiental == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFirmouTermoRepararDanoAmbiental" }))%>Não</label>
				</span>
			</div>
		</div>

		<div class="block divFirmouTermoRepararDanoAmbientalEspecificar <%: Model.Acompanhamento.FirmouTermoRepararDanoAmbiental == 0 ? "" : " hide" %>">
			<div class="ultima">
				<label>Justificativa *</label>
				<%= Html.TextArea("Acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar", Model.Acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtFirmouTermoRepararDanoAmbientalEspecificar", @maxlength = "500" }))%>
			</div>
		</div>

		<div class="block divArquivo <%= Model.Acompanhamento.FirmouTermoRepararDanoAmbiental.GetValueOrDefault() == 1 ? "" : " hide" %>">
			<div class="coluna40 inputFileDiv">
				<label>PDF do termo</label>
				<div class="block">
					<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Acompanhamento.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Acompanhamento.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome" target="_blank"><%= Html.Encode(Model.Acompanhamento.Arquivo.Nome)%></a>
				</div>
				<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
				<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Acompanhamento.Arquivo.Nome) ? "" : "hide" %>">
					<input type="file" id="fileTermo" class="inputFileTermo text" style="display: block" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %>/>
				</span>
			</div>
			<% if (!Model.IsVisualizar) { %>
			<div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
				<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.Acompanhamento.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
				<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Acompanhamento.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
			</div>
			<% } %>
			</div>
	</fieldset>

	<% Html.RenderPartial("Assinantes", Model.AssinantesVM); %>

	<fieldset class="block box fsArquivos">
		<legend>Relatório fotográfico</legend>
		<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
	</fieldset>
</div>