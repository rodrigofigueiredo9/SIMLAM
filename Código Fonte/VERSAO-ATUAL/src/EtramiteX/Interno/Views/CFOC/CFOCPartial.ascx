<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CFOCVM>" %>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.CFOC.Id %>' />
<fieldset class="block box">
	<div class="block">
		<div class="coluna22">
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
		<div class="coluna22 prepend1">
			<label>Número CFOC *</label>
			<%=Html.TextBox("CFOC.Numero", Model.CFOC.Numero, ViewModelHelper.SetaDisabled(Model.CFOC.Id > 0 || Model.CFOC.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="10" }))%>
		</div>

		<div class="campoTela <%= Model.CFOC.Id > 0 ? "" : "hide" %>">
			<div class="coluna15 prepend1">
				<label>Data de emissão *</label>
				<%=Html.TextBox("CFOC.DataEmissao", !string.IsNullOrEmpty(Model.CFOC.DataEmissao.DataTexto)?Model.CFOC.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.CFOC.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
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

		<div class="block">
			<table class="dataGridTable gridProdutos">
				<thead>
					<tr>
						<th style="width:17%">Codigo lote</th>
						<th>Cultura/Cultivar</th>
						<th style="width:17%">Quantidade/Unidade</th>
						<th style="width:23%">Data da consolidação do lote</th>
						<th width="7%">Ações</th>
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
							<td class="cultura_cultivar" title="<%=item.CulturaTexto + " " + item.CultivarTexto %>"> <%=item.CulturaTexto + " " + item.CultivarTexto%></td>
							<td class="quantidade" title="<%= qtd + " " + unid %>"><%= qtd + " " + unid %></td>
							<td class="data_consolidacao" title="<%=item.DataConsolidacao.DataTexto %>"><%=item.DataConsolidacao.DataTexto %></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item) %>' />
								<input title="Visualizar Lote" type="button" class="icone visualizar btnVisualizarLote" value="" />
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset id="Container_Praga" class="block box">
		<legend>Pragas associadas à cultura</legend>

		<div class="block">
			<table class="dataGridTable gridPragas">
				<thead>
					<tr>
						<th>Nome científico</th>
						<th>Nome comum</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFOC.Pragas) { %>
						<tr>
							<td class="nome_cientifico" title="<%=item.NomeCientifico %>"><%=item.NomeCientifico%></td>
							<td class="nome_comum" title="<%=item.NomeComum%>"> <%=item.NomeComum%></td>
						</tr>
					<% } %>
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
			<label>Informações complementares</label>
			<%=Html.TextArea("CFOC.DeclaracaoAdicional", Model.CFOC.DeclaracaoAdicional, ViewModelHelper.SetaDisabled(true, new { @class="text media txtDeclaracaoAdicional", @maxlength="3000" }))%>
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