<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConsumoVM>" %>

<% var indice = Model.Indice; %>
<fieldset id="Consumo_Container<%= indice %>" class="block boxBranca fsConsumo">
	<legend>Consumo Real da Categoria</legend>

	<input type="hidden" class="hdnConsumoIndex" value="<%= indice %>" />

	<% if (!Model.IsVisualizar) { %>
		<p class="block"><a class="btnAsmExcluir fecharMensagem" title="Excluir consumo real">Excluir</a></p>
	<% } %>

	<div class="divConteudoConsumo boxBranca borders" style="border: none">
		<div class="block">
			<label for="RegistroAtividadeFlorestal_Categoria">Categoria *</label>
			<%= Html.DropDownList("AtividadeId" + indice, Model.Atividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAtividades " }))%>
			<input type="hidden" class="hdnConsumoId asmItemId" value="<%= Model.Consumo.Id == 0 ? -1 : Model.Consumo.Id %>" />
		</div>

		<div class="block">
			<% bool possuiValor = Model.Consumo.PossuiLicencaAmbiental.HasValue; %>
			<div class="coluna25">
				<div><label for="PossuiLicencaAmbiental">Possui licença ambiental? *</label></div>
				<label ><%= Html.RadioButton("PossuiLicencaAmbiental" + indice, ConfiguracaoSistema.SIM, (possuiValor ? Model.Consumo.PossuiLicencaAmbiental.GetValueOrDefault() == ConfiguracaoSistema.SIM: false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPossuiLicencaAmbiental rbSim" }))%>Sim</label>
				<label ><%= Html.RadioButton("PossuiLicencaAmbiental" + indice, ConfiguracaoSistema.NAO, (possuiValor ? Model.Consumo.PossuiLicencaAmbiental.GetValueOrDefault() == ConfiguracaoSistema.NAO : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPossuiLicencaAmbiental rbNao" }))%>Não</label>
				<label ><%= Html.RadioButton("PossuiLicencaAmbiental" + indice, ConfiguracaoSistema.Dispensado, (possuiValor ? Model.Consumo.PossuiLicencaAmbiental.GetValueOrDefault() == ConfiguracaoSistema.Dispensado : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbPossuiLicencaAmbiental rbDispensado" }))%>Dispensado</label>
			</div>
		</div>

		<br />
		<%Html.RenderPartial("TituloAdicionar", Model.LicencaVM);%>

		<div class="divDispensado box block <%= (Model.Consumo.PossuiLicencaAmbiental.GetValueOrDefault() == ConfiguracaoSistema.Dispensado) ? "" : "hide" %>">
			<div class="block">
				<label ><%= Html.RadioButton("OrgaoEmissorDispensado" + indice, ConfiguracaoSistema.EmitidoIDAF, (Model.LicencaVM.Finalidade.EmitidoPorInterno.HasValue ? Model.LicencaVM.Finalidade.EmitidoPorInterno.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDispensadoOrgaoEmissor rbEmitidoIDAF" }))%>Emitido pelo <%= Model.LicencaVM.SiglaOrgao %></label>
				<label ><%= Html.RadioButton("OrgaoEmissorDispensado" + indice, ConfiguracaoSistema.EmitidoOutroOrgao, (Model.LicencaVM.Finalidade.EmitidoPorInterno.HasValue ? !Model.LicencaVM.Finalidade.EmitidoPorInterno.Value : false), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rbDispensadoOrgaoEmissor rbEmitidoOutroOrgao" }))%>Emitido por outro órgão</label>
			</div>

			<div class="block">
				<div class="coluna61 append1">
					<label for="TituloModelo">Documento *</label>
					<%= Html.TextBox("TituloModelo" + indice, Model.LicencaVM.Finalidade.TituloModeloTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "80", @class = "text txtTituloModelo" }))%>
				</div>
				<div class="coluna22">
					<label for="ProtocoloNumero">Nº protocolo *</label>
					<%= Html.TextBox("ProtocoloNumero" + indice, Model.LicencaVM.Finalidade.ProtocoloNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "20", @class = "text txtProtocoloNumero" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna86">
					<label for="OrgaoExpedidor">Órgão emissor *</label>
					<%= Html.TextBox("OrgaoExpedidor" + indice, Model.LicencaVM.Finalidade.OrgaoExpedidor, ViewModelHelper.SetaDisabled(Model.IsVisualizar || (Model.LicencaVM.Finalidade.EmitidoPorInterno.HasValue ? Model.LicencaVM.Finalidade.EmitidoPorInterno.Value : false), new { @maxlength = "80", @class = "text txtOrgaoExpedidor" }))%>
				</div>
			</div>
		</div>
		
		<fieldset class="box block">
			<legend>Taxa</legend>
			<div class="block">
				<div class="coluna27 append1">
					<label for="DUANumero">Número do DUA *</label>
					<%= Html.TextBox("DUANumero" + indice, Model.Consumo.DUANumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "40", @class = "text txtDUANumero" }))%>
				</div>
				<div class="coluna27 append1">
					<label for="DUAValor">Valor do DUA *</label>
					<%= Html.TextBox("DUAValor" + indice, ((Convert.ToDecimal(Model.Consumo.DUAValor) > 0) ? Convert.ToDecimal(Model.Consumo.DUAValor).ToStringTrunc() : null), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "10", @class = "text maskDecimalPonto txtDUAValor" }))%>
				</div>
				<div class="coluna27">
					<label for="DUADataPagamento">Data de pagamento do DUA *</label>
					<%= Html.TextBox("DUADataPagamento" + indice, Model.Consumo.DUADataPagamento.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDUADataPagamento" }))%>
				</div>
			</div>
		</fieldset>

		<div class="divConteudoFontesEnergia block <%= Model.PossuiFonte ? "" : "hide" %>">
			<fieldset class="block box" id="Consumo_Fonte<%= indice %>">
				<legend>Fonte Energia</legend>

				<%if (!Model.IsVisualizar) { %>
				<div class="block">
					<div class="coluna40 append2">
						<label for="FonteTipos">Matéria-prima floresta / Fonte de energia *</label>
						<%= Html.DropDownList("FonteTipos" + indice, Model.FonteTipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFonteTipos" }))%>
					</div>

					<div class="coluna15 append2">
						<label for="Unidade">Unidade *</label>
						<%= Html.DropDownList("Unidade" + indice, Model.Unidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlUnidades" }))%>
					</div>

					<div class="coluna20">
						<label for="Qde">Quantidade (ano) *</label>
						<%= Html.TextBox("Qde" + indice, null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "16", @class = "text txtQde maskDecimalPonto" }))%>
					</div>
				</div>

				<div class="block">
					<div class="coluna40 append2">
						<label for="QdeFlorestaPlantada">Qtd. oriunda da floresta exótica (ano)</label>
						<%= Html.TextBox("QdeFlorestaPlantada" + indice, null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "14", @class = "text txtQdeFlorestaPlantada maskDecimalPonto" }))%>
					</div>

					<div class="coluna38 append2">
						<label for="QdeFlorestaNativa">Qtd. oriunda da floresta nativa (ano)</label>
						<%= Html.TextBox("QdeFlorestaNativa" + indice, null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "14", @class = "text txtQdeFlorestaNativa maskDecimalPonto" }))%>
					</div>
				</div>

				<div class="block">
					<div class="coluna40 append2">
						<label for="QdeOutroEstado">Qtd. oriunda de outro estado (ano)</label>
						<%= Html.TextBox("QdeOutroEstado" + indice, null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "14", @class = "text txtQdeOutroEstado maskDecimalPonto" }))%>
					</div>
						
					<div class="coluna10">
						<button type="button" class="btnAddFonte inlineBotao botaoAdicionarIcone" title="Adicionar">Adicionar</button>
					</div>
				</div>
				<%} %>

				<div class="block dataGrid">
					<table class="dataGridTable tabFontes" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Matéria-prima flor. / Fonte energia</th>
								<th width="18%">Quantidade(ano)</th>
								<th width="18%">Floresta exótica (ano)</th>
								<th width="18%">Floresta nativa (ano)</th>
								<th width="18%">Outro estado (ano)</th>
								<% if (!Model.IsVisualizar) { %><th width="7%">Ações</th><% } %>
							</tr>
						</thead>
						<tbody>
							<% foreach (var item in Model.Consumo.FontesEnergia) { %>
							<tr>
								<td><span class="trFonteTipoTexto" title="<%= Html.Encode(item.TipoTexto + "/" + item.UnidadeTexto) %>"><%= Html.Encode(item.TipoTexto + "/" + item.UnidadeTexto)%></span></td>
								<td><span class="trQdeTexto" title="<%= Html.Encode(Convert.ToDecimal(item.Qde).ToStringTrunc()) %>"><%= Html.Encode(Convert.ToDecimal(item.Qde).ToStringTrunc())%></span></td>
								<td><span class="trQdeFlorestaPlantadaTexto" title="<%= Html.Encode(Convert.ToDecimal(item.QdeFlorestaPlantada).ToStringTrunc()) %>"><%= Html.Encode(Convert.ToDecimal(item.QdeFlorestaPlantada).ToStringTrunc())%></span></td>
								<td><span class="trQdeFlorestaNativaTexto" title="<%= Html.Encode(Convert.ToDecimal(item.QdeFlorestaNativa).ToStringTrunc()) %>"><%= Html.Encode(Convert.ToDecimal(item.QdeFlorestaNativa).ToStringTrunc())%></span></td>
								<td><span class="trQdeOutroEstadoTexto" title="<%= Html.Encode(Convert.ToDecimal(item.QdeOutroEstado).ToStringTrunc()) %>"><%= Html.Encode(Convert.ToDecimal(item.QdeOutroEstado).ToStringTrunc())%></span></td>

								<%if (!Model.IsVisualizar){ %>
								<td>
									<input type="hidden" class="hdnFonteJSON" value="<%: ViewModelHelper.Json(item) %>" />
									<button title="Excluir" class="icone excluir btnExcluirFonte" value="" type="button"></button>
								</td>
								<%} %>
							</tr>
							<% } %>
						</tbody>
					</table>
				</div>
			</fieldset>
		</div>
	</div>

	<table class="tabFonteTemplate hide">
		<tr>
			<td><span class="trFonteTipoTexto"></span></td>
			<td><span class="trQde"></span></td>
			<td><span class="trQdeFlorestaPlantada"></span></td>
			<td><span class="trQdeFlorestaNativa"></span></td>
			<td><span class="trQdeOutroEstado" ></span></td>
			<td>
				<input type="hidden" class="hdnFonteJSON" />
				<button title="Excluir" class="icone excluir btnExcluirFonte" type="button"></button>
			</td>
		</tr>
	</table>
</fieldset>