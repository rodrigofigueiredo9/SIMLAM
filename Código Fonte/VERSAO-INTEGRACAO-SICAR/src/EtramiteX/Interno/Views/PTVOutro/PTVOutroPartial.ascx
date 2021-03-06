﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTVOutro" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVOutroVM>" %>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.PTV.Id %>' />
<fieldset class="box">
	<div class="block">
		<div class="coluna23">
			<label for="Numero">Número PTV *</label>
			<%= Html.TextBox("Numero", Model.PTV.Numero == 0 ? "" : Model.PTV.Numero.ToString(), ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="text txtNumero setarFoco maskNumInt", @maxlength="10"}))%>
		</div>

		<% if (!Model.IsVisualizar) { %>
		<div class="coluna10 <%= Model.PTV.Id <= 0 ? "" : "hide" %>">
			<button type="button" class="inlineBotao btnVerificarPTV">Verificar</button>
			<button type="button" class="inlineBotao btnLimparPTV hide">Limpar</button>
		</div>
		<% } %>

		<div class="coluna15 prepend1">
			<label for="DataEmissao">Data de Emissão *</label>
			<%= Html.TextBox("DataEmissao", !string.IsNullOrEmpty(Model.PTV.DataEmissao.DataTexto)?Model.PTV.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.PTV.Id > 0 || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
		</div>
		<div class="coluna18 prepend1">
			<label for="Situacao">Situação</label>
			<%=Html.DropDownList("Situacao", Model.Situacoes, ViewModelHelper.SetaDisabled(true , new { @class="text ddlSituacoes"}))%>
		</div>
	</div>

	<div class="block interessado">
		<div class="coluna51">
			<label for="Interessado">Interessado *</label>
			<%= Html.TextBox("Interessado", Model.PTV.Interessado, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtInteressado", @maxlength = 120 }))%>
		</div>

		<div class="coluna18 prepend1">
			<label for="InteressadoCnpjCpf"><%=Html.RadioButton("CnpjCpf", (int)ePessoaTipo.Fisica, Model.PTV.InteressadoCnpjCpf.Length <= 14, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio radioPessoaCpfCnpj rbTipoPessoaFisicaPF" } ) ) %>CPF</label>
			<label for="InteressadoCnpjCpf"><%=Html.RadioButton("CnpjCpf", (int)ePessoaTipo.Juridica, Model.PTV.InteressadoCnpjCpf.Length > 14, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="radio radioPessoaCpfCnpj rbTipoPessoaJuridicaPJ" } ) ) %>CNPJ</label>
			<%= Html.TextBox("InteressadoCnpjCpf",Model.PTV.InteressadoCnpjCpf, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtInteressadoCpfCnpj maskCpfParcial" } ) ) %>
		</div>
	</div>

	<div class="block interessado">
		<div class="coluna51">
			<label for="InteressadoEndereco">Endereço *</label>
			<%= Html.TextBox("InteressadoEndereco", Model.PTV.InteressadoEndereco, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtInteressadoEndereco", @maxlength = 200}))%>
		</div>
		<div class="coluna18 prepend1">
			<label for="InteressadoEstadoId">UF *</label>
			<%=Html.DropDownList("InteressadoEstadoId", Model.EstadosInteressado ?? new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlEstadosInteressado"}))%>
		</div>
		<div class="coluna20 prepend1">
			<label for="InteressadoMunicipioId">Municipio *</label>
			<%=Html.DropDownList("InteressadoMunicipioId", Model.MunicipiosInteressado  ?? new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipiosInteressado"}))%>
		</div>
	</div>

</fieldset>

<fieldset class="block box identificacao_produto campoTela  <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<legend>Identificação do produto</legend>
	<% if (!Model.IsVisualizar) { %>
	<div class="block">
		<div class="coluna25">
			<label for="OrigemTipo">Origem *</label>
			<%=Html.DropDownList("OrigemTipo", Model.OrigemTipoList, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlOrigemTipo"})) %>
		</div>
		<div class="coluna26 prepend1 campoIdentificacao">
			<label for="NumeroOrigem">Nº Origem <span class="labelOrigem"></span> *</label>
			<%=Html.TextBox("NumeroOrigem",  (object)String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroOrigem maskNumInt campoIdentificacao2",  @maxlength="20"})) %>
		</div>
		<div class="coluna25 prepend1">
			<label for="ProdutoCultura">Cultura *</label>
			<input class="text txtCultura disabled" disabled="disabled" id="txtCultura" name="txtCultura" type="text" value="" />
			<input type="hidden" class="hdnCulturaId" id="hdnCulturaId" name="hdnCulturaId" value="0" />
			<%=Html.DropDownList("ProdutoCultura",  ViewModelHelper.CriarSelectList(new List<String>()), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutoCultura disabled hide", @disabled="disabled"}))%>
		</div>
		<div class="coluna10 prepend1 campoIdentificacao2">
			<button type="button" class="inlineBotao btnAssociarCultura">Buscar</button>
		</div>
	</div>
	<div class="block">
		<div class="coluna25">
			<label for="ProdutoCultivar">Cultivar *</label>
			<%=Html.DropDownList("ProdutoCultivar", ViewModelHelper.CriarSelectList(new List<String>()), ViewModelHelper.SetaDisabled(false, new { @class="text ddlProdutoCultivar"}))%>
		</div>
		<div class="coluna26 prepend1">
			<label for="ProdutoUnidadeMedida">Unidade de medida *</label>
			<%=Html.DropDownList("ProdutoUnidadeMedida", Model.LstUnidades, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutoUnidadeMedida"}))%>
		</div>
		<div class="coluna12 prepend1">
			<label for="ProdutoQuantidade">Quantidade *</label>
			<%=Html.TextBox("ProdutoQuantidade", "", ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtProdutoQuantidade maskDecimalPonto4", @maxlength = "12" }))%>
		</div>
		<div class="coluna10 prepend1">
			<button type="button" class="inlineBotao btnIdentificacaoProduto">Adicionar</button>
		</div>
	</div>
	<% } %>

	<div class="gridContainer">
		<table class="dataGridTable gridProdutos">
			<thead>
				<tr>
					<th style="width: 36%">Origem</th>
					<th>Cultura/Cultivar</th>
					<th style="width: 12%">Quantidade</th>
					<th style="width: 16%">Unidade de medida</th>
					<% if (!Model.IsVisualizar) { %><th style="width: 7%">Ação</th><% } %>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.PTV.Produtos) { %>
				<tr>
					<td class="Origem_Tipo" title="<%=item.OrigemTipoTexto + " - " + item.OrigemNumero %>"><%= item.OrigemTipoTexto + " - " + item.OrigemNumero %></td>
					<td class="cultura_cultivar" title="<%= item.CulturaCultivar %>"><%= item.CulturaCultivar %></td>
					<td class="quantidade" title="<%=item.Quantidade %>"><%=item.Quantidade %></td>
					<td class="unidade_medida" title="<%= item.UnidadeMedida %>"><%=item.UnidadeMedidaTexto %></td>
					<%if (!Model.IsVisualizar) { %>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" class="hdnItemJson" value='<%=ViewModelHelper.Json(item) %>' />
					</td>
					<%} %>
				</tr>
				<% } %>

				<tr class="trTemplate hide">
					<td class="OrigemTipo">
						<label class="lblOrigemTipo"></label>
					</td>
					<td class="cultura_cultivar">
						<label class="lblCulturaCultivar"></label>
					</td>
					<td class="quantidade">
						<label class="lblQuantidade"></label>
					</td>
					<td class="unidade_medida">
						<label class="lblUnidadeMedida"></label>
					</td>
					<td>
						<a class="icone excluir btnExcluir" title="Remover"></a>
						<input type="hidden" value="" class="hdnOrigemID" />
						<input type="hidden" value="0" class="hdnItemJson" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>


<fieldset class="block box destinatario campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<legend>Destinatário</legend>
	<div class="asmItens">
		<div class="asmItemContainer boxBranca borders">
			<div class="block  <%= !Model.IsVisualizar ? "": "" %>">
				<div class="coluna35">
					<label>Tipo</label><br />
					<label for="Tipo">
						<%=Html.RadioButton("Tipo", (int)ePessoaTipo.Fisica, Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Fisica,ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoDocumento rbTipoPessoaFisica"})) %>
					Pessoa Física
					</label>
					<label>
						<%=Html.RadioButton("Tipo", (int)ePessoaTipo.Juridica, Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Juridica, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoDocumento rbTipoPessoaJuridica"})) %>
					Pessoa Jurídica
					</label>
				</div>

				<div class="coluna20">
					<%if (Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Fisica) { %>
					<label class="lblCPFCNPJ" for="DestinararioCPFCNPJ">CPF</label>
					<% } else if (Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Juridica) { %>
						<label class="lblCPFCNPJ" for="DestinararioCPFCNPJ">CNPJ</label>
					<% } else { %>
						<label class="lblCPFCNPJ" for="DestinararioCPFCNPJ">CPF ou CNPJ *</label>
					<% } %>
					<%= Html.TextBox("DestinararioCPFCNPJ",Model.PTV.Destinatario.CPFCNPJ, ViewModelHelper.SetaDisabled(Model.PTV.Id == 0 || Model.IsVisualizar, new { @class="text maskCpf txtDocumentoCpfCnpj" })) %>
				</div>

				<% if (!Model.IsVisualizar) { %>
				<div class="coluna25 prepend1">
					<button type="button" class="inlineBotao btnVerificarDestinatario <%= Model.PTV.Id <= 0 ? "":"hide"%>">Verificar</button>
					<button type="button" class="inlineBotao btnLimparDestinatario <%= Model.PTV.Id <= 0 ? "hide":""%>">Limpar</button>
					<button type="button" class="inlineBotao btnNovoDestinatario hide">Novo</button>
				</div>
				<% } %>
			</div>

			<div class="destinatarioDados <%= Model.PTV.Id <= 0 ? "":""%>">
				<div class="block">
					<div class="coluna68">
						<label for="DestinatarioNome">Nome do destinatário *</label>
						<%= Html.TextBox("DestinatarioNome", Model.PTV.Destinatario.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class="text txtNomeDestinatario"})) %>
						<input type="hidden" class="hdnDestinatarioID" value='<%= Model.PTV.Destinatario.ID %>' />
					</div>
				</div>
				<div class="block">
					<div class="coluna68">
						<label for="Endereco">Endereço *</label>
						<%= Html.TextBox("Endereco", Model.PTV.Destinatario.Endereco, ViewModelHelper.SetaDisabled(true, new { @class="text txtEndereco" })) %>
					</div>
				</div>
				<div class="block">
					<div class="coluna10">
						<label for="Uf">UF *</label>
						<%= Html.TextBox("Uf", Model.PTV.Destinatario.EstadoSigla, ViewModelHelper.SetaDisabled(true, new { @class="text txtUF"})) %>
					</div>
					<div class="coluna20 prepend1">
						<label for="Municipio">Município *</label>
						<%= Html.TextBox("Municipio", Model.PTV.Destinatario.MunicipioTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtMunicipio"})) %>
					</div>
				</div>
			</div>
		</div>
	</div>
</fieldset>

<div class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<div class="block">
		<div class="coluna18">
			<label for="ValidoAte">Válido até *</label>
			<%=Html.TextBox("ValidoAte", Model.PTV.ValidoAte.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtDataValidade maskData"}))%>
		</div>
		<div class="coluna30 prepend1">
			<label for="ResponsavelTecnico">Responsável técnico *</label>
			<%=Html.TextBox("ResponsavelTecnico", Model.PTV.RespTecnico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTecnico ResponsavelTecnico", @maxlength="120"}))%>
		</div>

		<div class="coluna15 prepend1">
			<label for="RespTecnicoNumHab">Nº da habilitação *</label>
			<%=Html.TextBox("RespTecnicoNumHab", Model.PTV.RespTecnicoNumHab, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumHab", @maxlength="8"}))%>
		</div>

	</div>
	<div class="block">
		<div class="coluna18">
			<label for="EstadoId">UF *</label>
			<%=Html.DropDownList("EstadoId", Model.Estados ?? new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlEstados"}))%>
		</div>
		<div class="coluna30 prepend1">
			<label for="MunicipioId">Municipio *</label>
			<%=Html.DropDownList("MunicipioId", Model.Municipios  ?? new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipios"}))%>
		</div>
	</div>
</div>