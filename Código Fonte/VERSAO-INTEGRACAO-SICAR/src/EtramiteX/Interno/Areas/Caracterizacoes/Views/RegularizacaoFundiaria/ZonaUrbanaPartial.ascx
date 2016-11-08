<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<!--divZonaUrbana-->
<div class="block divZona divZonaUrbana">
	<div class="block">
		<div class="coluna24 divOpcaoOcupacao">
			<% Opcao opcao = Model.MontarRadioCheck(eTipoOpcao.RedeAgua); %>
			<div><label for="rbRedeAgua" class="labRadioTexto">Rede de água *</label></div>
			<label ><%= Html.RadioButton("rbRedeAgua", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbRedeAgua", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="coluna24 divOpcaoOcupacao ">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.RedeEsgoto); %>
			<div><label for="rbRedeEsgoto" class="labRadioTexto">Rede de esgoto *</label></div>
			<label ><%= Html.RadioButton("rbRedeEsgoto", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbRedeEsgoto", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="coluna24 divOpcaoOcupacao ">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.LuzEletrica); %>
			<div><label for="rbLuzEletrica" class="labRadioTexto">Luz elétrica domiciliar *</label></div>
			<label ><%= Html.RadioButton("rbLuzEletrica", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbLuzEletrica", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="coluna24 divOpcaoOcupacao">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.IluminacaoPublica); %>
			<div><label for="rbIluminacaoPublica" class="labRadioTexto">Iluminação da via pública *</label></div>
			<label ><%= Html.RadioButton("rbIluminacaoPublica", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbIluminacaoPublica", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>
	</div>

	<div class="block ">
		<div class="coluna24 divOpcaoOcupacao ">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.RedeTelefonica); %>
			<div><label for="rbRedeTelefonica" class="labRadioTexto">Rede telefônica *</label></div>
			<label ><%= Html.RadioButton("rbRedeTelefonica", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbRedeTelefonica", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="coluna24 divOpcaoOcupacao">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.Calcada); %>
			<div><label for="rbCalcada" class="labRadioTexto">Calçada *</label></div>
			<label ><%= Html.RadioButton("rbCalcada", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbCalcada", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>
	</div>

	<div class="block divOpcaoOcupacao">
		<div class="coluna24">
			<% opcao = Model.MontarRadioCheck(eTipoOpcao.Pavimentacao); %>
			<div><label for="rbPavimentacao" class="labRadioTexto">Pavimentação *</label></div>
			<label ><%= Html.RadioButton("rbPavimentacao", ConfiguracaoSistema.SIM, opcao.Valor == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Sim</label>
			<label ><%= Html.RadioButton("rbPavimentacao", ConfiguracaoSistema.NAO, opcao.Valor == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radioOpcao radio" }))%>Não</label>

			<input type="hidden" class="hdnRadioId" value="<%: opcao.Id %>" />
			<input type="hidden" class="hdnRadioTipo" value="<%: opcao.Tipo %>" />
		</div>

		<div class="block ultima opcaoTextoSim">
			<label for="Pavimentacao_Outro">Tipo da pavimentação *</label>
			<%= Html.TextBox("Pavimentacao.Outro", opcao.Outro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text campoOutro", @maxlength = "80" }))%>
		</div>
	</div>

	<fieldset class="block boxBranca">
		<legend>Edificações</legend>

		<% if(!Model.IsVisualizar) { %>
			<div class="block">
				<div class="coluna77 append2 divAdicionarEdificacao">
					<label for="">Tipo de edificação *</label>
					<%= Html.TextBox("Tipo.Edificacao", string.Empty, new { @class = "text txtTipoEdificacao campoEdificacao", @maxlength = "80" })%>
				</div>

				<div class="coluna12 append2 divQuemPertenceLimite">
					<label for="">Qtd. *</label>
					<%= Html.TextBox("Quantidade.Edificacao", string.Empty, new { @class = "text maskNumInt txtQuantidadeEdificacao campoEdificacao", @maxlength = "5" })%>
				</div>

				<div class="ultima">
					<button class="inlineBotao botaoAdicionarIcone btnAdicionarEdificacao">Buscar</button>
				</div>
			</div>
		<% } %>

		<br />
		<div class="block dataGrid divGridEdificacao">
			<table class="dataGridTable tabEdificacao" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th >Tipo de edificação</th>
						<th width="12%">Qtd.</th>
						<% if(!Model.IsVisualizar){ %><th width="8%">Ações</th><%}%>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Caracterizacao.Posse.Edificacoes) { %>
						<tr>
							<td><span class="tipo" title="<%: item.Tipo %>"><%: item.Tipo %></span></td>
							<td><span class="quantidade" title="<%: item.Quantidade %>"><%: item.Quantidade %></span></td>

							<% if(!Model.IsVisualizar){ %>
							<td class="tdAcoes">
								<input type="hidden" class="hdnEdificacaoJSON" value="<%: ViewModelHelper.Json(item) %>" />
								<input title="Excluir" type="button" class="icone excluir btnRemoverLinhaGrid" />
							</td>
							<%}%>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</fieldset>
</div>
<!--divZonaUrbana-->

<table class="tabEdificacaoTemplate hide">
	<tr>
		<td><span class="tipo"></span></td>
		<td><span class="quantidade"></span></td>
		<td class="tdAcoes">
			<input type="hidden" class="hdnEdificacaoJSON" />
			<input title="Excluir" type="button" class="icone excluir btnRemoverLinhaGrid" value="" />
		</td>
	</tr>
</table>