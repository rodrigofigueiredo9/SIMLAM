<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<!--Fs Caracteristicas de Ocupacao-->
<fieldset class="block box fsDominialidade filtroExpansivoAberto">
	<legend class="titFiltros">Características da ocupação</legend>

	<div class="filtroCorpo">
		<div class="block divOpcaoOcupacao">
			<% Opcao opcao = Model.MontarRadioCheck(eTipoOpcao.TerrenoDevoluto); %>
			<div class="coluna25 append2">
				<div><label for="" class="labRadioTexto">É presumidamente devoluto *</label></div>
				<label ><%= Html.RadioButton("rbTerrenoDevoluto", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Sim</label>
				<label ><%= Html.RadioButton("rbTerrenoDevoluto", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Não</label>

				<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
				<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
			</div>

			<div class="coluna30 opcaoTextoSim">
				<label for="TerrenoDevoluto_Outro">Homologação aprovada *</label>
				<%= Html.DropDownList("TerrenoDevoluto.Outro", Model.Homologacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro" }))%>
			</div>

			<div class="coluna40 opcaoTextoNao">
				<label for="TerrenoDevoluto_Outro">Especificar a dominialidade *</label>
				<%= Html.TextBox("TerrenoDevoluto.Outro", Convert.ToBoolean(opcao.Valor) ? string.Empty : opcao.Outro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro", @maxlength = "80" }))%>
			</div>
		</div>

		<div class="block divOpcaoOcupacao">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.RequerenteResideNaPosse); %>
			<div class="coluna25 append2">
				<div><label class="labRadioTexto" for="">Requerente reside na posse *</label></div>
				<label ><%= Html.RadioButton("rbRequerenteResideNaPosse", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Sim</label>
				<label ><%= Html.RadioButton("rbRequerenteResideNaPosse", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Não</label>

				<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
				<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
			</div>

			<div class="coluna26 opcaoTextoSim opcaoTextoNao">
				<label for="DataArquisicao">Data da aquisição ou ocupação *</label>
				<%= Html.TextBox("DataArquisicao", opcao.Outro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro maskData" }))%>
			</div>
		</div>

		<div class="block divOpcaoOcupacao">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.ExisteLitigio); %>
			<div class="coluna25 append2">
				<div><label  class="labRadioTexto" for="">Existe litígio *</label></div>
				<label ><%= Html.RadioButton("rbExisteLitigio", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Sim</label>
				<label ><%= Html.RadioButton("rbExisteLitigio", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

				<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
				<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
			</div>

			<div class="block ultima opcaoTextoSim">
				<label for="NomeLitigo">Nome *</label>
				<%= Html.TextBox("NomeLitigo", opcao.Outro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro", @maxlength = "80" }))%>
			</div>
		</div>

		<div class="block divOpcaoOcupacao">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.SobrepoeSeDivisa); %>
			<div class="coluna25 append2">
				<div><label class="labRadioTexto" for="">Sobrepõe-se a faixa de divisa *</label></div>
				<label ><%= Html.RadioButton("rbSobrepoeSeDivisa", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio opcaoMostra" }))%>Sim</label>
				<label ><%= Html.RadioButton("rbSobrepoeSeDivisa", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

				<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
				<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
			</div>

			<div class="block ultima opcaoTextoSim">
				<label for="SobrepoeSeDivisa_Outro">A quem pertence o limite *</label>
				<%= Html.DropDownList("SobrepoeSeDivisa_Outro", Model.TipoLimite, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro" }))%>
			</div>
		</div>

		<fieldset class="boxBranca block">
			<legend>Transmitente da Posse</legend>

			<% if(!Model.IsVisualizar) { %>
			<div class="block">
				<input type="hidden" class="hdnTransmitentePessoaID campoTransmitente" />
				<div class="coluna65 append2">
					<label for="">Nome / Razão Social</label>
					<%= Html.TextBox("NomeRazaoSocial", string.Empty, new { @class = "text txtTransmitenteNome disabled campoTransmitente", @disabled = "disabled" })%>
				</div>

				<div class="coluna18 append2">
					<label for="">CPF/CNPJ</label>
					<%= Html.TextBox("CpfCpnj", string.Empty, new { @class = "text txtTransmitenteCpfCpnj disabled campoTransmitente", @disabled = "disabled" })%>
				</div>

				<div class="ultima" >
					<button class="inlineBotao btnAssociarPessoa">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="coluna25 append2">
				<label for="">Tempo de ocupação (anos) *</label>
					<%= Html.TextBox("TempoOcupacao", string.Empty, new { @class = "text txtTempoOcupacao maskNumInt campoTransmitente", @maxlength = "3" })%>
				</div>

				<div class="ultima">
					<button class = "inlineBotao botaoAdicionarIcone btnAdicionarTransmitente">Adicionar</button>
				</div>
			</div>
			<%}%>

			<div class="block dataGrid">
				<table class="dataGridTable tabTransmitentes" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th >Nome / Razão Social</th>
							<th width="20%">CPF / CNPJ </th>
							<th width="25%">Tempo de ocupação (anos) </th>
							<th width="12%">Ações</th>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.Caracterizacao.Posse.Transmitentes) { %>
						<tr>
							<td><span class="nomeRazaoSocial" title="<%: item.Transmitente.NomeRazaoSocial %>"><%: item.Transmitente.NomeRazaoSocial %></span></td>
							<td><span class="cpfCnpj" title="<%: item.Transmitente.CPFCNPJ %>"><%: item.Transmitente.CPFCNPJ %></span></td>
							<td><span class="tempoOcupacao" title="<%: item.TempoOcupacao %>"><%: item.TempoOcupacao %></span></td>

							<td class="tdAcoes">
								<input type="hidden" class="hdnTransmitenteJSON" value="<%: ViewModelHelper.Json(item) %>" />
								<input title="Visualizar" type="button" class="icone visualizar btnVisualizarTransmitente" />
								<%if (!Model.IsVisualizar){ %><input title="Excluir" type="button" class="icone excluir btnRemoverTransmitente" /><%} %>
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>

			<div class="block">
				<div class="coluna61">
					<label>Total de transmitente: </label>
					<u><strong> <span class="labTotalTransmitente"> </span></strong></u>
				</div>

				<div class="coluna35">
					<label>Total de tempo de ocupação: </label>
					<u><strong> <span class="labTotalTempoOcupacao"> </span></strong></u>
				</div>
			</div>
		</fieldset>

		<div class="divZonaContainer">
			<% Html.RenderPartial(Model.Caracterizacao.Posse.Zona == (int)eZonaLocalizacao.Urbana ? "ZonaUrbanaPartial" : "ZonaRuralPartial", Model); %>
		</div>

		<div class="block">
			<div class="coluna100 divRadioFaixaDivisa ">
				<label for="">Observações</label>
				<%= Html.TextArea("Caracterizacao.Posse.Observacoes", Model.Caracterizacao.Posse.Observacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtObservacoes", @maxlength = "4000" }))%>
			</div>
		</div>
	</div>
</fieldset>
<!--Fs Caracteristicas de Ocupacao-->

<table class="tabTransmitentesTemplate hide">
	<tr>
		<td><span class="nomeRazaoSocial"></span></td>
		<td><span class="cpfCnpj" ></span></td>
		<td><span class="tempoOcupacao" ></span></td>

		<td class="tdAcoes">
			<input type="hidden" value="" class="hdnTransmitenteJSON" />
			<input title="Visualizar" type="button" class="icone visualizar btnVisualizarTransmitente" value="" />
			<input title="Excluir" type="button" class="icone excluir btnRemoverTransmitente" value="" />
		</td>
	</tr>
</table>