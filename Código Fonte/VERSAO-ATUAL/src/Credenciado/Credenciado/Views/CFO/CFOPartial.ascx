<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFO" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CFOVM>" %>

<style>

	.textareaFake {
		background-color: rgb(234, 234, 234);
		border: solid 1px rgb(204, 204, 204);
		color: rgb(51, 51, 51);
		
		font-family: monospace;
		font-size: 13.3333px;
		font-stretch: normal;
		font-style: normal;
		font-variant: normal;
		font-weight: normal;
		line-height: normal;
		
		display: block;
		min-height: 70px;
		/*width: 1338.47px;*/
		
		margin-left: 0;
		margin-right: 0;
		margin-top: 0;
		margin-bottom: 6.66667px;
		padding: 1.33333px;
	}

</style>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.CFO.Id %>' />
<fieldset class="block box">
	<div class="block">
		<div class="coluna22 divNumeroEnter">
			<label>Tipo emissão</label><br />
			<label>
				<%=Html.RadioButton("CFO.TipoNumero", (int)eDocumentoFitossanitarioTipoNumero.Bloco, Model.CFO.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco, ViewModelHelper.SetaDisabled(Model.CFO.Id > 0, new { @class="rbTipoNumero rbTipoNumeroBloco"}))%>
				Nº Bloco
			</label>
			<label>
				<%=Html.RadioButton("CFO.TipoNumero", (int)eDocumentoFitossanitarioTipoNumero.Digital, Model.CFO.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital, ViewModelHelper.SetaDisabled(Model.CFO.Id > 0, new { @class="rbTipoNumero rbTipoNumeroDigital"}))%>
				Nº Digital
			</label>
		</div>
		<div class="coluna22 prepend1 divNumeroEnter">
			<label>Número CFO *</label>
			<%=Html.TextBox("CFO.Numero", Model.CFO.Numero, ViewModelHelper.SetaDisabled(Model.CFO.Id > 0 || Model.CFO.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="10" }))%>
		</div>

		<% if (Model.CFO.Id <= 0) { %>
		<div class="coluna10 prepend1">
			<button type="button" class="inlineBotao btnVerificarCFO">Verificar</button>
			<button type="button" class="inlineBotao btnLimparCFO hide">Limpar</button>
		</div>
		<% } %>

		<div class="campoTela <%= Model.CFO.Id > 0 ? "" : "hide" %>">
			<div class="coluna15 prepend1">
				<label>Data de emissão *</label>
				<%=Html.TextBox("CFO.DataEmissao", !string.IsNullOrEmpty(Model.CFO.DataEmissao.DataTexto)?Model.CFO.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.CFO.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
			</div>

			<div class="coluna22 prepend1">
				<label>Situação</label>
				<%=Html.DropDownList("CFO.SituacaoId", Model.Situacoes, ViewModelHelper.SetaDisabled(true, new { @class="text ddlSituacoes"}))%>
			</div>
		</div>
	</div>

	<div class="campoTela <%= Model.CFO.Id > 0 ? "" : "hide" %>">
		<div class="block">
			<div class="coluna58">
				<label>Produtor *</label>
				<%=Html.DropDownList("CFO.ProdutorId", Model.Produtores, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutores"}))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna58">
				<label>Empreendimento *</label>
				<%=Html.DropDownList("CFO.EmpreendimentoId", Model.Empreendimentos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlEmpreendimentos"}))%>
			</div>
		</div>
	</div>
</fieldset>

<div class="campoTela <%= Model.CFO.Id > 0 ? "" : "hide" %>">
	<fieldset id="Container_Produto" class="block box identificacao_produto">
		<legend>Identificação do produto</legend>
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna35">
				<label>Unidade de Produção *</label>
				<%=Html.DropDownList("CFO.Produto.UnidadeProducao", Model.UnidadesProducao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlUnidadesProducao"}))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna35">
				<label>Cultura *</label>
				<%=Html.TextBox("CFO.Produto.Cultura", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoCultura"}))%>
				<input type="hidden" class="hdnCulturaId" />
			</div>
			<div class="coluna35 prepend1">
				<label>Cultivar *</label>
				<%=Html.TextBox("CFO.Produto.Cultivar", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoCultivar"}))%>
				<input type="hidden" class="hdnCultivarId" />
			</div>
		</div>
		<div class="block">
			<div class="coluna16">
				<label>Quantidade *</label>
				<%=Html.TextBox("CFO.Produto.Quantidade", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtProdutoQuantidade maskDecimalPonto4", @maxlength = "12"}))%>
			</div>
			<div class="coluna17 prepend1">
				<label>Unidade de medida *</label>
				<%=Html.TextBox("CFO.Produto.UnidadeMedida", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoUnidadeMedida"}))%>
				<input type="hidden" class="hdnUnidadeMedidaId" />
			</div>
			<div class="coluna16 prepend1">
				<label>Inicio da colheita *</label>
				<%=Html.TextBox("CFO.Produto.InicioColheita", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtProdutoInicioColheita maskData"}))%>
			</div>
			<div class="coluna17 prepend1">
				<label>Fim da colheita *</label>
				<%=Html.TextBox("CFO.Produto.FimColheita", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtProdutoFimColheita maskData"}))%>
			</div>

			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAddIdentificacaoProduto">Adicionar</button>
			</div>
		</div>
		<% } %>

		<div class="block">
			<table class="dataGridTable gridProdutos">
				<thead>
					<tr>
						<th style="width:20%">Codigo da UP</th>
						<th>Cultura/Cultivar</th>
						<th style="width:10%">Quantidade</th>
						<th style="width:25%">Período da colheita</th>
						<% if (!Model.IsVisualizar) { %><th style="width:7%">Ação</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFO.Produtos) { %>
						<tr>
							<td class="codigoUP" title="<%=item.CodigoUP %>"><%=item.CodigoUP %></td>
							<td class="cultura_cultivar" title="<%=item.CulturaTexto + " " + item.CultivarTexto %>"> <%=item.CulturaTexto + " " + item.CultivarTexto%></td>
							<td class="quantidade" title="<%= item.Quantidade + " " + item.UnidadeMedida %>"><%= item.Quantidade + " " + item.UnidadeMedida %></td>
							<td class="periodo" title="<%=item.DataInicioColheita.DataTexto + " a " + item.DataFimColheita.DataTexto %>"><%=item.DataInicioColheita.DataTexto + " a " + item.DataFimColheita.DataTexto %></td>
							<%if(!Model.IsVisualizar){ %> 
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" class="hdnItemJson" value='<%=ViewModelHelper.Json(item) %>' />
							</td>
							<%} %> 
						</tr>
					<%  } %>
					<%if(!Model.IsVisualizar){ %>
						<tr class="trTemplate hide">
							<td class="codigoUP"></td>
							<td class="cultura_cultivar"></td>
							<td class="quantidade"></td>
							<td class="periodo"></td>
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" value="0" class="hdnItemJson" />
							</td>
						</tr>
					<%} %>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset id="Container_Praga" class="block box">
		<legend>Pragas associadas à cultura</legend>
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna58">
				<label>Pragas *</label>
				<%=Html.DropDownList("CFO.PragaId", Model.Pragas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{ @class="ddlPragas text"})) %>
			</div>
			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAddPraga">Adicionar</button>
			</div>
		</div>
		<% } %>

		<div class="block">
			<table class="dataGridTable gridPragas">
				<thead>
					<tr>
						<th>Nome científico</th>
						<th>Nome comum</th>
						<% if (!Model.IsVisualizar) { %><th style="width:7%">Ação</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFO.Pragas) { %>
						<tr>
							<td class="nome_cientifico" title="<%=item.NomeCientifico %>"><%=item.NomeCientifico%></td>
							<td class="nome_comum" title="<%=item.NomeComum%>"> <%=item.NomeComum%></td>
							<%if(!Model.IsVisualizar){ %> 
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" class="hdnItemJson" value='<%=ViewModelHelper.Json(item) %>' />
							</td>
							<%} %>
						</tr>
					<%  } %>
					<%if(!Model.IsVisualizar){ %>
						<tr class="trTemplate hide">
							<td class="nome_cientifico"></td>
							<td class="nome_comum"></td>
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" value="0" class="hdnItemJson" />
							</td>
						</tr>
					<%} %>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset class="block box">
		<div class="block">
			<div class="coluna20">
				<label>Possui laudo laboratorial? *</label><br />
				<label>
					<%=Html.RadioButton("CFO.PossuiLaudoLaboratorial", 0, !Model.CFO.PossuiLaudoLaboratorial.GetValueOrDefault(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPossuiLaudo rbPossuiLaudoNao"}))%>
					Não
				</label>
				<label>
					<%=Html.RadioButton("CFO.PossuiLaudoLaboratorial", 1, Model.CFO.PossuiLaudoLaboratorial.GetValueOrDefault(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPossuiLaudo rbPossuiLaudoSim"}))%>
					Sim
				</label>
			</div>

			<div class="coluna75 prepend1 laudo <%=Model.CFO.PossuiLaudoLaboratorial.GetValueOrDefault()?"":"hide" %>">
				<label>Nome do laboratório *</label>
				<%=Html.TextBox("CFO.NomeLaboratorio", Model.CFO.NomeLaboratorio, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNomeLaboratorio", @maxlength="100" }))%>
			</div>
		</div>
		<div class="block divEndereco laudo <%=Model.CFO.PossuiLaudoLaboratorial.GetValueOrDefault()?"":"hide" %>">
			<div class="coluna45">
				<label>Número do laudo com resultado da análise *</label>
				<%=Html.TextBox("CFO.NumeroLaudoResultadoAnalise", Model.CFO.NumeroLaudoResultadoAnalise, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroLaudoResultadoAnalise", @maxlength="15" }))%>
			</div>
			<div class="coluna18 prepend1">
				<label>UF *</label>
				<%=Html.DropDownList("CFO.EstadoId", Model.Estados, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlEstado"}))%>
			</div>
			<div class="coluna30 prepend1">
				<label>Município *</label>
				<%=Html.DropDownList("CFO.MunicipioId", Model.Municipios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipio"}))%>
			</div>
		</div>

		<div class="block">
			<label>
				Certifico que, mediante acompanhamento técnico, o(s) produto(s) acima especificado(s) se apresenta(m):
			</label>
		</div>
		<div class="block">
		<%foreach(var item in Model.CFOProdutosEspecificacoes){ %>
			<div class="block">
				<label>
					<%= Html.CheckBox("CFOProdutosEspecificacoes", ((int.Parse(item.Codigo) & Model.CFO.ProdutoEspecificacao) != 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "cbEspecificacoes checkbox", @title = item.Texto, @value = item.Codigo }))%>
					<%=item.Texto %>
				</label>
			</div>
		<%} %>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Tratamento Fitossanitário com Fins Quarentenários</legend>
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna45">
				<label>Possui tratamento fitossanitário com fins quarentenários? *</label><br />
				<label>
					<%=Html.RadioButton("CFO.PossuiTratamentoFinsQuarentenario", 0, !Model.CFO.PossuiTratamentoFinsQuarentenario.GetValueOrDefault(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPossuiTratamento rbPossuiTratamentoNao"}))%>
					Não
				</label>
				<label>
					<%=Html.RadioButton("CFO.PossuiTratamentoFinsQuarentenario", 1, Model.CFO.PossuiTratamentoFinsQuarentenario.GetValueOrDefault(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPossuiTratamento rbPossuiTratamentoSim"}))%>
					Sim
				</label>
			</div>
			<div class="coluna52 prepend1 tratamento <%=Model.CFO.PossuiTratamentoFinsQuarentenario.GetValueOrDefault()? "":"hide" %>">
				<label>Nome do produto comercial *</label>
				<%=Html.TextBox("CFO.TratamentoFitossanitario.NomeProduto", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTratamentoNomeProduto", @maxlength="100" }))%>
			</div>
		</div>

		<div class="block tratamento <%=Model.CFO.PossuiTratamentoFinsQuarentenario.GetValueOrDefault()? "":"hide" %>">
			<div class="coluna45">
				<label>Ingrediente ativo *</label>
				<%=Html.TextBox("CFO.TratamentoFitossanitario.IngredienteAtivo", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTratamentoIngredienteAtivo", @maxlength="100" }))%>
			</div>

			<div class="coluna52 prepend1">
				<label>Dose *</label>
				<%=Html.TextBox("CFO.TratamentoFitossanitario.Dose", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTratamentoDose maskDecimalPonto4", @maxlength = "12"}))%>
			</div>
		</div>

		<div class="block tratamento <%=Model.CFO.PossuiTratamentoFinsQuarentenario.GetValueOrDefault()? "":"hide" %>">
			<div class="coluna45">
				<label>Praga / Produto *</label><br />
				<%=Html.TextBox("CFO.TratamentoFitossanitario.PragaProduto", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTratamentoPragaProduto", @maxlength="100" }))%>
			</div>

			<div class="coluna39 prepend1">
				<label>Modo de aplicação *</label>
				<%=Html.TextBox("CFO.TratamentoFitossanitario.ModoAplicacao", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTratamentoModoAplicacao", @maxlength="100" }))%>
			</div>
			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAddTratamento">Adicionar</button>
			</div>
		</div>
		<% } %>

		<div class="block">
			<table class="dataGridTable gridTratamento">
				<thead>
					<tr>
						<th>Nome do produto comercial</th>
						<th style="width:20%">Ingrediente ativo</th>
						<th style="width:10%">Dose</th>
						<th style="width:20%">Produto / Praga</th>
						<th style="width:20%">Modo de aplicação</th>
						<% if (!Model.IsVisualizar) { %><th style="width:7%">Ação</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFO.TratamentosFitossanitarios) { %>
						<tr>
							<td class="nome_produto" title="<%=item.ProdutoComercial %>"><%=item.ProdutoComercial%></td>
							<td class="ingrediente" title="<%=item.IngredienteAtivo%>"> <%=item.IngredienteAtivo%> </td>
							<td class="dose" title="<%=item.Dose%>"> <%=item.Dose%> </td>
							<td class="produto_praga" title="<%=item.PragaProduto%>"> <%=item.PragaProduto%> </td>
							<td class="modo_aplicacao" title="<%=item.ModoAplicacao%>"> <%=item.ModoAplicacao%> </td>
							<%if(!Model.IsVisualizar){ %> 
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" class="hdnItemJson" value='<%=ViewModelHelper.Json(item) %>' />
							</td>
							<% } %>
						</tr>
					<%  } %>
					<%if(!Model.IsVisualizar){ %>
						<tr class="trTemplate hide">
							<td class="nome_produto">Nome do produto comercial</td>
							<td class="ingrediente">Ingrediente ativo</td>
							<td class="dose">Dose</td>
							<td class="produto_praga">Produto / Praga</td>
							<td class="modo_aplicacao">Modo de aplicação</td>
							<td>
								<a class="icone excluir btnExcluir"></a>
								<input type="hidden" value="0" class="hdnItemJson" />
							</td>
						</tr>
					<%} %>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset class="block box">
		<div class="block">
			<div class="coluna22">
				<label>Partida lacrada na origem? *</label><br />
				<label>
					<%=Html.RadioButton("CFO.PartidaLacradaOrigem", 0, !Model.CFO.PartidaLacradaOrigem, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacrada rbPartidaLacradaNao"}))%>
					Não
				</label>
				<label>
					<%=Html.RadioButton("CFO.PartidaLacradaOrigem", 1, Model.CFO.PartidaLacradaOrigem, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacrada rbPartidaLacradaSim"}))%>
					Sim
				</label>
			</div>

			<div class="coluna22 prepend1 partida <%=Model.CFO.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do lacre *</label>
				<%=Html.TextBox("NumeroLacre", Model.CFO.NumeroLacre, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroLacre", @maxlength="15" }))%>
			</div>
			<div class="coluna22 prepend1 partida <%=Model.CFO.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do porão *</label>
				<%=Html.TextBox("NumeroPorao", Model.CFO.NumeroPorao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroPorao", @maxlength="15" }))%>
			</div>
			<div class="coluna22 prepend1 partida <%=Model.CFO.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do contêiner *</label>
				<%=Html.TextBox("NumeroContainer", Model.CFO.NumeroContainer, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroContainer", @maxlength="15" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna22">
				<label>Validade certificado (dias) *</label>
				<%=Html.TextBox("CFO.ValidadeCertificado", Model.CFO.ValidadeCertificado > 0? Model.CFO.ValidadeCertificado.ToString(): "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNumInt txtValidadeCertificado", @maxlength="2" }))%>
			</div>
		</div>
		<div class="block">
			<label>Declaração Adicional</label>
			<div class="textareaFake txtDeclaracaoAdicional">
				<%= Model.CFO.DeclaracaoAdicionalHtml %>
			</div>
		</div>

		<div class="block">
			<div class="coluna30">
				<label>Município da Emissão *</label>
				<%=Html.DropDownList("CFO.MunicipioEmissaoId", Model.MunicipiosEmissao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipioEmissao"}))%>
			</div>
			<div class="coluna18 prepend1">
				<label>UF *</label>
				<%=Html.DropDownList("CFO.EstadoEmissaoId", Model.EstadosEmissao, ViewModelHelper.SetaDisabled(true, new { @class="text ddlEstadoEmissao"}))%>
			</div>
		</div>
	</fieldset>
</div>