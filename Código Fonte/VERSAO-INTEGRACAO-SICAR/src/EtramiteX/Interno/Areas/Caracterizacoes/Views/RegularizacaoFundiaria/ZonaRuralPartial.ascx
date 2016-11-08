<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<!--divZonaRural-->
<div class="block divZona divZonaRural" >
	<div class="block divOpcaoOcupacao">
		<% Opcao opcao = Model.MontarRadioCheck(eTipoOpcao.BanhadoPorRioCorrego); %>
		<div class="coluna26 append2 ">
			<div><label for="rbBanhadoPorRioCorrego" class="labRadioTexto">Banhado por rios ou córregos *</label></div>
			<label ><%= Html.RadioButton("rbBanhadoPorRioCorrego", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbBanhadoPorRioCorrego", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="block ultima opcaoTextoSim">
			<label for="NomeDoRio">Nome do rio ou córrego *</label>
			<%= Html.TextBox("NomeDoRio", opcao.Outro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeRioCorrego campoOutro", @maxlength = "80" }))%>
		</div>
	</div>

	<div class="block divOpcaoOcupacao">
		<% opcao = Model.MontarRadioCheck(eTipoOpcao.PossuiNascente); %>
		<div class="coluna26 append2 ">
			<div><label for="rbPossuiNascente" class="labRadioTexto">Possui nascente *</label></div>
			<label ><%= Html.RadioButton("rbPossuiNascente", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbPossuiNascente", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="block ultima checkRelacaoTrabalho">
			<div><label for="RelacaoTrabalho">Relação de trabalho predominante *</label></div>
			<% foreach(var checkbox in Model.RelacoesTrabalho){ %>
				<label class="append2 <%= Model.IsVisualizar ? "" : "labelCheckBox" %>">
					<%= Html.CheckBox("RelacaoTrabalho", ((checkbox.Codigo & Model.Caracterizacao.Posse.RelacaoTrabalho) != 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "checkbox", @title = checkbox.Texto, @value = checkbox.Codigo }))%>
					<%: checkbox.Texto %>
				</label>
			<%} %>
		</div>
	</div>

	<fieldset class="boxBranca block" id="fsUsoSolo">
		<legend>Uso atual do solo</legend>
		
		<% if(!Model.IsVisualizar) { %>
			<div class="block ">
				<div class="coluna25 append2 divRadioFaixaDivisa ">
					<label for="TipoUso">Tipo de Uso - Estimativa *</label>
					<%= Html.DropDownList("TipoUso", Model.TipoUso, new { @class = "text ddlTipoUso " })%>
				</div>

				<div class="coluna12 append1 divQuemPertenceLimite">
					<label for="UsoSolo_Area">Área (%) *</label>
					<%= Html.TextBox("UsoSolo.Area", string.Empty, new { @class = "text txtAreaUsoPorcentagem maskNumInt", @maxlength = "3" })%>
				</div>

				<div class="ultima" >
					<button class="inlineBotao botaoAdicionarIcone btnAdicionarUsoSolo">Adicionar</button>
				</div>
			</div>
		<% } %>
		
		<div class="block dataGrid divGridUsoSolo">
			<% int total = 0; %>
			<table class="dataGridTable tabUsoSolo" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th >Tipo de Uso - Estimativa</th>
						<th width="12%">Área (%)</th>
						<% if(!Model.IsVisualizar){ %><th width="8%">Ações</th><%}%>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Caracterizacao.Posse.UsoAtualSolo) { %>
					<% total += item.AreaPorcentagem; %>
					<tr>
						<td><span class="tipoUsoTexto" title="<%: item.TipoDeUsoTexto%>"><%: item.TipoDeUsoTexto%></span></td>
						<td><span class="areaUso" title="<%: item.AreaPorcentagem.ToString() + " %" %>"><%: item.AreaPorcentagem %></span> %</td>

						<% if(!Model.IsVisualizar) { %>
						<td class="tdAcoes">
							<input type="hidden" class="hdnUsoSoloJSON" value="<%: ViewModelHelper.Json(item) %>" />
							<input title="Excluir" type="button" class="icone excluir btnRemoverUsoSolo" value="" />
						</td>
						<%} %>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>

		<div class = "block coluna20 floatRight">
			<label>Total: </label>
			<u><strong> <span class="labTotalAreaUso"> <%: total %> </span> % </strong></u>
		</div>
	</fieldset>

	<div class="block">
		<div class="coluna100 divRadioFaixaDivisa ">
			<label for="">Benfeitorias/Edificações *</label>
			<%= Html.TextArea("Benfeitorias", Model.Caracterizacao.Posse.Benfeitorias, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtBenfeitorias", @maxlength = "400" }))%>
		</div>
	</div>
</div>
<!--divZonaRural-->

<table class="tabUsoSoloTemplate hide">
	<tr>
		<td><span class="tipoUsoTexto"></span></td>
		<td><span class="areaUso"></span> %</td>
		<td class="tdAcoes">
			<input type="hidden" class="hdnUsoSoloJSON" />
			<input title="Excluir" type="button" class="icone excluir btnRemoverUsoSolo" />
		</td>
	</tr>
</table>