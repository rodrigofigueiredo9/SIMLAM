<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CFOVM>" %>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.CFO.Id %>' />
<fieldset class="block box">
	<div class="block">
		<div class="coluna22">
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
		<div class="coluna22 prepend1">
			<label>Número CFO *</label>
			<%=Html.TextBox("CFO.Numero", Model.CFO.Numero, ViewModelHelper.SetaDisabled(Model.CFO.Id > 0 || Model.CFO.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="8" }))%>
		</div>

		<div class="campoTela <%= Model.CFO.Id > 0 ? "" : "hide" %>">
			<div class="coluna15 prepend1">
				<label>Data de emissão *</label>
				<%=Html.TextBox("CFO.DataEmissao", !string.IsNullOrEmpty(Model.CFO.DataEmissao.DataTexto)?Model.CFO.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.CFO.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
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

		<div class="block">
			<table class="dataGridTable gridProdutos">
				<thead>
					<tr>
						<th style="width:20%">Codigo da UP</th>
						<th>Cultura/Cultivar</th>
						<th style="width:10%">Quantidade</th>
						<th style="width:25%">Período da colheita</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.CFO.Produtos) { %>
						<tr>
							<td class="codigoUP" title="<%=item.CodigoUP %>"><%=item.CodigoUP %></td>
							<td class="cultura_cultivar" title="<%=item.CulturaTexto + " " + item.CultivarTexto %>"> <%=item.CulturaTexto + " " + item.CultivarTexto%></td>
							<td class="quantidade" title="<%= item.Quantidade + " " + item.UnidadeMedida %>"><%= item.Quantidade + " " + item.UnidadeMedida %></td>
							<td class="periodo" title="<%=item.DataInicioColheita.DataTexto + " a " + item.DataFimColheita.DataTexto %>"><%=item.DataInicioColheita.DataTexto + " a " + item.DataFimColheita.DataTexto %></td>
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
					<% foreach (var item in Model.CFO.Pragas) { %>
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
			<label>Informações complementares</label>
			<%=Html.TextArea("CFO.DeclaracaoAdicional", Model.CFO.DeclaracaoAdicional,  ViewModelHelper.SetaDisabled(true, new { @class="text media txtDeclaracaoAdicional", @maxlength="3000" } ))%>
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