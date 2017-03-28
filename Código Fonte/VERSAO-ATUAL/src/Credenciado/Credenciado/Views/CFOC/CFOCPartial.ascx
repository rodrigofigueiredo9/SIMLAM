<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CFOCVM>" %>

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


<input type="hidden" class="hdnEmissaoId" value='<%= Model.CFOC.Id %>' />
<fieldset class="block box">
	<div class="block">
		<div class="coluna22 divNumeroEnter">
			<label>Tipo emissão</label><br />
			<label>
				<%=Html.RadioButton("CFOC.TipoNumero", (int)eDocumentoFitossanitarioTipoNumero.Bloco, Model.CFOC.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco, ViewModelHelper.SetaDisabled(Model.CFOC.Id > 0, new { @class="rbTipoNumero rbTipoNumeroBloco"}))%>
				Nº Bloco
			</label>
			<label>
				<%=Html.RadioButton("CFOC.TipoNumero", (int)eDocumentoFitossanitarioTipoNumero.Digital, Model.CFOC.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital, ViewModelHelper.SetaDisabled(Model.CFOC.Id > 0, new { @class="rbTipoNumero rbTipoNumeroDigital"}))%>
				Nº Digital
			</label>
		</div>
		<div class="coluna22 prepend1 divNumeroEnter">
			<label>Número CFOC *</label>
			<%=Html.TextBox("CFOC.Numero", Model.CFOC.Numero, ViewModelHelper.SetaDisabled(Model.CFOC.Id > 0 || Model.CFOC.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="8" }))%>
		</div>

		<% if (Model.CFOC.Id <= 0) { %>
		<div class="coluna10 prepend1">
			<button type="button" class="inlineBotao btnVerificarNumero">Verificar</button>
			<button type="button" class="inlineBotao btnLimparNumero hide">Limpar</button>
		</div>
		<% } %>

		<div class="campoTela <%= Model.CFOC.Id > 0 ? "" : "hide" %>">
			<div class="coluna15 prepend1">
				<label>Data de emissão *</label>
				<%=Html.TextBox("CFOC.DataEmissao", !string.IsNullOrEmpty(Model.CFOC.DataEmissao.DataTexto)?Model.CFOC.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.CFOC.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
			</div>

			<div class="coluna22 prepend1">
				<label>Situação</label>
				<%=Html.DropDownList("CFOC.SituacaoId", Model.Situacoes, ViewModelHelper.SetaDisabled(true, new { @class="text ddlSituacoes"}))%>
			</div>
		</div>
	</div>

	<div class="campoTela <%= Model.CFOC.Id > 0 ? "" : "hide" %>">
		<div class="block">
			<div class="coluna58">
				<label>Empreendimento *</label>
				<%=Html.DropDownList("CFOC.EmpreendimentoId", Model.Empreendimentos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlEmpreendimentos"}))%>
			</div>
		</div>
	</div>
</fieldset>

<div class="campoTela <%= Model.CFOC.Id > 0 ? "" : "hide" %>">
	<fieldset id="Container_Produto" class="block box identificacao_produto">
		<legend>Identificação do produto</legend>
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna25">
				<label>Lote *</label>
				<%=Html.TextBox("CFOC.Produto.LoteId", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoLote"}))%>
				<input type="hidden" class="hdnProdutoLoteId" />
			</div>
			<div class="coluna10 prepend1">
				<button type="button" class="inlineBotao btnAssociarLote">Buscar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna48">
				<label>Cultura *</label>
				<%=Html.TextBox("CFOC.Produto.Cultura", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoCultura"}))%>
				<input type="hidden" class="hdnCulturaId" />
			</div>
			<div class="coluna48 prepend1">
				<label>Cultivar *</label>
				<%=Html.TextBox("CFOC.Produto.Cultivar", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoCultivar"}))%>
				<input type="hidden" class="hdnCultivarId" />
			</div>
		</div>

		<div class="block">
			<div class="coluna23">
				<label>Quantidade/Mês *</label>
				<%=Html.TextBox("CFOC.Produto.Quantidade", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtProdutoQuantidade maskDecimalPonto4"}))%>
			</div>
			<div class="coluna23 prepend1">
				<label>Unidade de medida *</label>
				<%=Html.TextBox("CFOC.Produto.UnidadeMedida", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoUnidadeMedida"}))%>
				<input type="hidden" class="hdnUnidadeMedidaId" />
			</div>
			<div class="coluna25 prepend1">
				<label>Data da consolidação do lote *</label>
				<%=Html.TextBox("CFOC.Produto.DataConsolidacao", "", ViewModelHelper.SetaDisabled(true, new { @class="text txtProdutoConsolidacao maskData"}))%>
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
						<th style="width: 17%">Codigo lote</th>
						<th>Cultura/Cultivar</th>
						<th style="width: 17%">Quantidade/Unidade</th>
						<th style="width: 23%">Data da consolidação do lote</th>
						<% if (!Model.IsVisualizar) { %><th style="width: 7%">Ação</th>
						<% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFOC.Produtos) 
                        {   
                              
                            decimal qtd = 0;
                            var unid = "";
                            if (item.ExibeQtdKg)
                            {
                                qtd = item.Quantidade * 1000;
                                unid = "KG";

                            }
                            else
                            {
                                qtd = item.Quantidade;
                                unid = item.UnidadeMedida;
                            } 
            
            
                            %>
					<tr>
						<td class="codigo" title="<%=item.LoteCodigo %>"><%=item.LoteCodigo %></td>
						<td class="cultura_cultivar" title="<%=item.CulturaTexto + " " + item.CultivarTexto %>"><%=item.CulturaTexto + " " + item.CultivarTexto%></td>
						<td class="quantidade" title="<%= qtd + " " + unid %>"><%= qtd + " " + unid %></td>
						<td class="data_consolidacao" title="<%=item.DataConsolidacao.DataTexto %>"><%=item.DataConsolidacao.DataTexto %></td>
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
						<td class="codigo"></td>
						<td class="cultura_cultivar"></td>
						<td class="quantidade"></td>
						<td class="data_consolidacao"></td>
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
				<%=Html.DropDownList("CFOC.PragaId", Model.Pragas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{ @class="ddlPragas text"})) %>
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
						<% if (!Model.IsVisualizar) { %><th style="width: 7%">Ação</th>
						<% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFOC.Pragas) { %>
					<tr>
						<td class="nome_cientifico" title="<%=item.NomeCientifico %>"><%=item.NomeCientifico%></td>
						<td class="nome_comum" title="<%=item.NomeComum%>"><%=item.NomeComum%></td>
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
			<label>
				Certifico que, mediante reinspeção, acompanhamento do recebimento e conferência do CFO e CFOC, PTV ou CFR das cargas que compuseram o(s) lote(s) acima especificado(s), este(s) se apresenta(m):
			</label>
		</div>
		<div class="block">
			<%foreach(var item in Model.ProdutosEspecificacoes){ %>
			<div class="block">
				<label>
					<%= Html.CheckBox("CFOCProdutosEspecificacoes", ((int.Parse(item.Codigo) & Model.CFOC.ProdutoEspecificacao) != 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "cbEspecificacoes checkbox", @title = item.Texto, @value = item.Codigo }))%>
					<%=item.Texto %>
				</label>
			</div>
			<%} %>
		</div>
	</fieldset>

	<fieldset class="block box">

	<fieldset class="block box">
		<div class="block">
			<div class="coluna22">
				<label>Partida lacrada na origem? *</label><br />
				<label>
					<%=Html.RadioButton("CFOC.PartidaLacradaOrigem", 0, !Model.CFOC.PartidaLacradaOrigem, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacrada rbPartidaLacradaNao"}))%>
					Não
				</label>
				<label>
					<%=Html.RadioButton("CFOC.PartidaLacradaOrigem", 1, Model.CFOC.PartidaLacradaOrigem, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacrada rbPartidaLacradaSim"}))%>
					Sim
				</label>
			</div>

			<div class="coluna22 prepend1 partida <%=Model.CFOC.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do lacre *</label>
				<%=Html.TextBox("NumeroLacre", Model.CFOC.NumeroLacre, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroLacre", @maxlength="15" }))%>
			</div>
			<div class="coluna22 prepend1 partida <%=Model.CFOC.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do porão *</label>
				<%=Html.TextBox("NumeroPorao", Model.CFOC.NumeroPorao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroPorao", @maxlength="15" }))%>
			</div>
			<div class="coluna22 prepend1 partida <%=Model.CFOC.PartidaLacradaOrigem?"":"hide" %>">
				<label>Nº do contêiner *</label>
				<%=Html.TextBox("NumeroContainer", Model.CFOC.NumeroContainer, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroContainer", @maxlength="15" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna22">
				<label>Validade certificado (dias) *</label>
				<%=Html.TextBox("CFOC.ValidadeCertificado", Model.CFOC.ValidadeCertificado > 0? Model.CFOC.ValidadeCertificado.ToString(): "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNumInt txtValidadeCertificado", @maxlength="2" }))%>
			</div>
		</div>
		<div class="block">
			<label>Declaração Adicional</label>
			<div class="textareaFake txtDeclaracaoAdicional">
				<%= Model.CFOC.DeclaracaoAdicionalHtml %>
			</div>
		</div>

		<div class="block">
			<div class="coluna30">
				<label>Município da Emissão *</label>
				<%=Html.DropDownList("CFOC.MunicipioEmissaoId", Model.MunicipiosEmissao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipioEmissao"}))%>
			</div>
			<div class="coluna18 prepend1">
				<label>UF *</label>
				<%=Html.DropDownList("CFOC.EstadoEmissaoId", Model.EstadosEmissao, ViewModelHelper.SetaDisabled(true, new { @class="text ddlEstadoEmissao"}))%>
			</div>
		</div>
	</fieldset>
</div>
