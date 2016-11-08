<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UnidadeConsolidacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%=Html.Hidden("UnidadeConsolidacaoId",Model.UnidadeConsolidacao.Id ,new {@class="hdnUnidadeConsolidacaoId" })%>
<%=Html.Hidden("EmpreendimentoId",Model.UnidadeConsolidacao.Empreendimento.Id ,new {@class="hdnEmpreendimentoId" })%>

<div class="box">
	<fieldset class="block boxBranca">
		<div class="block">
			<div class="coluna22">
				<label for="PossuiCodigoUC">Já possui código da UC? *</label><br />
				<label><%= Html.RadioButton("PossuiCodigoUC", true, Model.UnidadeConsolidacao.PossuiCodigoUC, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeConsolidacao.Id > 0), new { @class = "radio RadioCodigoUC rbPossuiCodigoSim" }))%>Sim</label>
				<label><%= Html.RadioButton("PossuiCodigoUC", false, !Model.UnidadeConsolidacao.PossuiCodigoUC, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.UnidadeConsolidacao.Id > 0), new { @class = "radio RadioCodigoUC rbPossuiCodigoNao" }))%>Não</label>
			</div>

			<div class="coluna30 prepend1">
				<label for="CodigoPropriedade">Código da UC *</label>
				<%= Html.TextBox("CodigoPropriedade", ((Model.UnidadeConsolidacao.CodigoUC > 0) ? Model.UnidadeConsolidacao.CodigoUC.ToString() : "Gerado automaticamente"), ViewModelHelper.SetaDisabled(Model.IsVisualizar || !Model.UnidadeConsolidacao.PossuiCodigoUC, new { @class = "text txtCodigoUC maskNumInt", @maxlength = "11" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna98">
				<label for="LocalLivroDisponivel">Local em que o livro estará disponível *</label>
				<%= Html.TextBox("LocalLivroDisponivel", Model.UnidadeConsolidacao.LocalLivroDisponivel, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLocalLivroDisponivel", @maxlength = "100"}))%>
			</div>
		</div>
	</fieldset>

	<fieldset id="CapacidadeProcessamento" class="block boxBranca cultivarContainer">
		<legend>Capacidade de processamento / armazenamento</legend>

		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna82">
				<label for="Cultura">Cultura *</label>
				<%=Html.TextBox("Cultura.Nome", "", new { @class = "text disabled txtCulturaTexto clear", @disabled = "disabled" })%>
				<%=Html.Hidden("Cultura.Id", 0, new { @class="hdnCulturaId clear"})%>
			</div>

			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnBuscarCultura">Buscar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna27">
				<label for="Cultivar_Id">Cultivar *</label>
			    <%=Html.DropDownList("UnidadeProducaoItem.CultivarId", Model.LstCultivar, ViewModelHelper.SetaDisabled(Model.LstCultivar.Count <= 1 || Model.IsVisualizar, new { @class = "text ddlCultivar" })) %>
			</div>
			<div class="coluna25 prepend1">
				<label for="CapacidadeMes">Capacidade/Mês *</label>
				<%= Html.TextBox("CapacidadeMes", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtCapacidadeMes maskDecimalPonto4 clear", @maxlength = "12" }))%>
			</div>
			<div class="coluna26 prepend1">
				<label for="UnidadeMedida">Unidade de medida *</label>
				<%= Html.DropDownList("UnidadeMedida", Model.LstUnidadeMedida, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlUnidadeMedida" }))%>
			</div>

			<%if(!Model.IsVisualizar){ %>
			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAddCultura">Adicionar</button>
			</div>
			<%} %>
		</div>
		<%} %>

		<div class="block">
			<table class="dataGridTable gridCultivares" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Cultura</th>
						<th width="24%">Capacidade / Mês</th>
						<%if(!Model.IsVisualizar){ %><th width="7%">Ações</th><%} %>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.UnidadeConsolidacao.Cultivares) { %>
					<tr>
						<td>
							<label class="lblCultura" title="<%=item.CulturaTexto %>">
								<%=item.CulturaTexto%> 
								<%if(item.Id > 0){ %> <%=item.Nome %> <%} %>
							</label>
						</td>
						<td><label class="lblCapacidadeMes" title="<%=item.CapacidadeMes.ToStringTrunc(4) + " " + item.UnidadeMedidaTexto%>"><%=item.CapacidadeMes.ToStringTrunc(4) + " " + item.UnidadeMedidaTexto %> </label></td>
						<%if(!Model.IsVisualizar){ %>
						<td>
							<a class="icone excluir btnExcluir"></a>
							<input type="hidden" value='<%=ViewModelHelper.Json(item)%>' class="hdnItemJson" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<tr class="trTemplate hide">
						<td><label class="lblCultura" title=""></label></td>
						<td><label class="lblCapacidadeMes" title=""></label></td>
						<td>
							<a class="icone excluir btnExcluir"></a>
							<input type="hidden" value="0" class="hdnItemJson" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset id="ResponsavelTecnico" class="block boxBranca">
		<legend>Responsável Técnico</legend>

		<%if(!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna82">
				<label for="ResponsavelTecnico_NomeRazao">Responsável técnico *</label>
				<%=Html.TextBox("ResponsavelTecnico.NomeRazao", "", new { @class = "text txtResponsavelNome disabled responsavelClear", @disabled="disabled" })%>
				<%=Html.Hidden("ResponsavelTecnico.Id", 0, new {@class="hdnResponsavelId responsavelClear"}) %>
			</div>

			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnBuscarRespTec <%= Model.UnidadeConsolidacao.Cultivares.Count > 0 ? "" : "hide" %>">Buscar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna40">
				<label for="CFONumero">Nº da habilitação CFO/CFOC *</label>
				<%=Html.TextBox("CFONumero", "", new { @class = "text txtResponsavelNumCFOCFOC disabled responsavelClear", @disabled="disabled" })%>
			</div>

			<div class="coluna40 prepend1">
				<label for="NumeroArt">Nº da ART *</label>
				<%=Html.TextBox("NumeroArt", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text txtResponsavelNumART responsavelClear", @maxlength="20" }))%>
			</div>

			<div class="coluna10 prepend1">
				<%--Ficará oculto até fazer o web service para o botão--%>
				<button type="button" class="inlineBotao btnVerificarNumero hide">Verificar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna82">
				<div class="coluna51">
					<label for="ArtCargoFuncao">ART de cargo e função?</label><br />
					<label><%=Html.RadioButton("ArtCargoFuncao", true, true, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioARTCargoFuncao rbARTCargoFuncaoSim" }))%>Sim</label>
					<label><%=Html.RadioButton("ArtCargoFuncao", false, false, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "radio RadioARTCargoFuncao rbARTCargoFuncaoNão" }))%>Não</label>
				</div>
				<div class="coluna47 divDataValidadeART hide">
					<label for="DataValidadeART">Data de validade da ART *</label>
					<%=Html.TextBox("DataValidadeART", "", new { @class = "text txtResponsavelDataValidadeART maskData disabled responsavelClear", @disabled="disabled" })%>
				</div>
			</div>

			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAddRespTec">Adicionar</button>
			</div>
		</div>
		<%} %>

		<div class="block">
			<table class="dataGridTable gridResponsaveis" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Responsável Técnico</th>
						<th width="24%">Nº da Habilitação CFO / CFOC</th>
                        <th width="24%">Data de validade da ART</th>
						<%if(!Model.IsVisualizar) { %><th width="7%">Ações</th><%} %>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.UnidadeConsolidacao.ResponsaveisTecnicos)
						{%>
					<tr>
						<td><label class="lblNome" title="<%=item.NomeRazao %>"><%=item.NomeRazao%> </label></td>
						<td><label class="lblNumeroHabilitacao" title="<%=item.CFONumero %>"><%=item.CFONumero%> </label></td>
                        <td><label class="lblDataValidadeART" title="<%=item.DataValidadeART %>"><%=item.DataValidadeART%> </label></td>
						<%if(!Model.IsVisualizar) { %>
						<td>
							<a class="icone excluir btnExcluir"></a>
							<input type="hidden" value='<%=Model.ResponsavelTecnicoJson(item) %>' class="hdnItemJson" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<tr class="trTemplate hide">
						<td><label class="lblNome" title=""></label></td>
						<td><label class="lblNumeroHabilitacao" title=""></label></td>
                        <td><label class="lblDataValidadeART" title=""></label></td>
						<td>
							<a class="icone excluir btnExcluir"></a>
							<input type="hidden" value="0" class="hdnItemJson" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset class="block boxBranca">
		<div class="block ultima">
			<label for="TipoApresentacaoProducaoFormaIdentificacao">Tipo de apresentação do produto e forma de identificação *</label>
			<%=Html.TextArea("TipoApresentacaoProducaoFormaIdentificacao", Model.UnidadeConsolidacao.TipoApresentacaoProducaoFormaIdentificacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea media txtTipoApresentacao", maxlength = "250" }))%>
		</div>
	</fieldset>
</div>