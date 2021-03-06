﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVVM>" %>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.PTV.Id %>' />
<fieldset class="box">
	<div class="block">
		<div class="coluna21 divNumeroEnter">
			<label>Tipo emissão</label><br />
			<label for="NumeroTipo">
				<%= Html.RadioButton("NumeroTipo", (int)eDocumentoFitossanitarioTipoNumero.Bloco, Model.PTV.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoNumero rbTipoNumeroBloco"}))%>Nº Bloco
			</label>
			<label>
				<%= Html.RadioButton("NumeroTipo", (int)eDocumentoFitossanitarioTipoNumero.Digital, (Model.PTV.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.PTV.NumeroTipo.GetValueOrDefault() == (int)eDocumentoFitossanitarioTipoNumero.Nulo), ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoNumero rbTipoNumeroDigital"}))%>Nº Digital
			</label>
		</div>
		<div class="coluna22 divNumeroEnter">
			<label for="Numero">Número PTV *</label>
			<%=Html.TextBox("Numero", Model.PTV.Numero > 0 ? Model.PTV.Numero : (object)"Gerado automaticamente" , ViewModelHelper.SetaDisabled(Model.PTV.Id > 0 || 
			Model.PTV.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="10"}))%>
		</div>
		<div class="coluna10 <%= Model.PTV.Id <= 0 ? "" : "hide" %>">
			<button type="button" class="inlineBotao btnVerificarPTV">Verificar</button>
			<button type="button" class="inlineBotao btnLimparPTV hide">Limpar</button>
		</div>
		<div class="coluna15">
			<label for="DataEmissao">Data de Emissão *</label>
			<%=Html.TextBox("DataEmissao", !string.IsNullOrEmpty(Model.PTV.DataEmissao.DataTexto)?Model.PTV.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(Model.PTV.Id >0 || Model.PTV.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Bloco || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>
		</div>
		<div class="coluna14">
			<label for="Situacao">Situação</label>
			<%=Html.DropDownList("Situacao", Model.Situacoes , ViewModelHelper.SetaDisabled(true , new { @class="text ddlSituacoes"}))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box identificacao_produto campoTela  <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<legend>Identificação do produto</legend>
	<% if (!Model.IsVisualizar) { %>
	<div class="block">
		<div class="coluna20 divNumeroDocumentoEnter">
			<label for="OrigemTipo">Documento de origem *</label>
			<%=Html.DropDownList("OrigemTipo", Model.OrigemTipoList, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlOrigemTipo"})) %>
		</div>
		<div class="coluna22 divNumeroDocumentoEnter">
			<input type="hidden" class="hdnNumeroOrigem" value="0" />
			<input type="hidden" class="hdnEmpreendimentoOrigemID" value="0" />
			<input type="hidden" class="hdnEmpreendimentoOrigemNome" value="" />
			<label for="NumeroOrigem">Número documento <span class="labelOrigem"></span> *</label>
			<%=Html.TextBox("NumeroOrigem",  (object)String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroOrigem maskNumInt",  @maxlength="19"})) %>
		</div>
		<div class="coluna20 identificacaoCultura">
			<label for="ProdutoCultura">Cultura *</label>
			<%=Html.DropDownList("ProdutoCultura",  new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutoCultura"}))%>
		</div>
		<div class="coluna10 culturaBuscar hide">
			<button type="button" class="inlineBotao btnAssociarCultura">Buscar</button>
		</div>
		<div class="coluna10">
			<button type="button" class="inlineBotao btnVerificarDocumentoOrigem hide">Verificar</button>
		</div>
	</div>
	<div class="block">
		<div class="coluna25">
			<label for="ProdutoCultivar">Cultivar *</label>
			<%=Html.DropDownList("ProdutoCultivar", new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutoCultivar"}))%>
		</div>
		<div class="coluna17">
			<label for="ProdutoUnidadeMedida">Unidade de medida *</label>
			<%=Html.DropDownList("ProdutoUnidadeMedida", new List<SelectListItem>(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlProdutoUnidadeMedida"}))%>
		</div>
		<div class="coluna12">
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
					<th style="width: 20%">Origem</th>
					<th>Cultura/Cultivar</th>
					<th style="width: 10%">Quantidade</th>
					<th style="width: 16%">Unidade de medida</th>
					<% if (!Model.IsVisualizar) { %><th style="width: 7%">Ação</th><% } %>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.PTV.Produtos) { %>
				<tr>
					<td class="Origem_Tipo" title="<%=item.OrigemTipoTexto %>"><%= item.OrigemTipoTexto %></td>
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
	<br />

    <div class="block campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
		<div class="coluna58">
			<label for="EmpreendimentoTexto">Empreendimento *</label>
			<%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
			<input type="hidden" class="hdnEmpreendimentoID" value='<%= Model.PTV.Empreendimento %>' />
		</div>		
	</div>

	<div class="block campoTela  <%= Model.PTV.Id <= 0 ? "hide":""%>">
		<div class="coluna58">
			<label for="ResponsavelEmpreendimento">Responsável do empreendimento</label>
			<%=Html.DropDownList("ResponsavelEmpreendimento", Model.ResponsavelList, ViewModelHelper.SetaDisabled(Model.IsVisualizar|| Model.ResponsavelList.Count == 1, new { @class="text ddlResponsaveis"}))%>
		</div>
	</div>
</fieldset>

<div class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<div class="block">
		<div class="coluna25">
			<label>Partida lacrada na Origem ?</label><br />
			<label>
				<%=Html.RadioButton("PartidaLacradaOrigem", (int)ePartidaLacradaOrigem.Sim, Model.PTV.PartidaLacradaOrigem == (int)ePartidaLacradaOrigem.Sim, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacradaOrigem rbLacradaOrigemSim"}))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("PartidaLacradaOrigem", (int)ePartidaLacradaOrigem.Nao, Model.PTV.PartidaLacradaOrigem.GetValueOrDefault() == (int)ePartidaLacradaOrigem.Nao , ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rbPartidaLacradaOrigem rbLacradaOrigemNao"}))%>
				Não
			</label>
		</div>
		<div class="partida_lacrada <%= Model.PTV.PartidaLacradaOrigem.GetValueOrDefault() == (int)ePartidaLacradaOrigem.Sim ?"":"hide" %>">
			<div class="coluna15">
				<label for="LacreNumero">Nº do lacre</label>
				<%=Html.TextBox("LacreNumero", Model.PTV.LacreNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroLacre", @maxlength="15"}))%>
			</div>
			<div class="coluna15 ">
				<label for="PoraoNumero">Nº do porão</label>
				<%=Html.TextBox("PoraoNumero", Model.PTV.PoraoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroPorao", @maxlength="15"}))%>
			</div>
			<div class="coluna15">
				<label for="ContainerNumero">Nº do contêiner</label>
				<%=Html.TextBox("ContainerNumero", Model.PTV.ContainerNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroContainer", @maxlength="15"}))%>
			</div>
		</div>
	</div>
</div>

<fieldset class="block box destinatario campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<legend>Destinatário</legend>
	<div class="asmItens">
		<div class="asmItemContainer boxBranca borders">
			<div class="block  <%= !Model.IsVisualizar ? "": "hide" %>">
				<div class="coluna35">
					<label>Tipo</label><br />
					<label for="Tipo">
						<%=Html.RadioButton("Tipo", (int)ePessoaTipo.Fisica, Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Fisica, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoDocumento rbTipoPessoaFisica"} ) ) %>
					Pessoa Física
					</label>
					<label>
						<%=Html.RadioButton("Tipo", (int)ePessoaTipo.Juridica, (Model.PTV.Destinatario.PessoaTipo == (int)ePessoaTipo.Juridica || Model.PTV.Destinatario.PessoaTipo == 0), ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoDocumento rbTipoPessoaJuridica"} ) ) %>
					Pessoa Jurídica
					</label>
				</div>

				<div class="coluna15">
					<label class="lblCPFCNPJ" for="DestinararioCPFCNPJ">CPF *</label>
					<%= Html.TextBox("DestinararioCPFCNPJ",Model.PTV.Destinatario.CPFCNPJ, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="text maskCpf txtDocumentoCpfCnpj" } ) ) %>
				</div>

				<div class="coluna6  <%= !Model.IsVisualizar ? "": "hide" %>">
					<button type="button" class="inlineBotao btnVerificarDestinatario <%= Model.PTV.Id <= 0 ? "":"hide"%>">Verificar</button>
					<button type="button" class="inlineBotao btnLimparDestinatario <%= Model.PTV.Id <= 0 ? "hide":""%>">Limpar</button>
				</div>
				<div class="coluna6 novoDestinatario hide">
					<button type="button" class="inlineBotao btnNovoDestinatario">Novo</button>
				</div>
			</div>

			<div class="destinatarioDados <%= Model.PTV.Id <= 0 ? "hide":""%>">
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
						<%= Html.TextBox("Uf", Model.PTV.Destinatario.EstadoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtUF"})) %>
					</div>
					<div class="coluna20">
						<label for="Municipio">Município *</label>
						<%= Html.TextBox("Municipio", Model.PTV.Destinatario.MunicipioTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtMunicipio"})) %>
					</div>
				</div>
			</div>
		</div>
	</div>
</fieldset>

<div class="block box possuiLaudoLaboratorial campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<div class="block">
		<div class="coluna30">
			<label>Possui laudo laboratorial? *</label><br />
			<label for="PossuiLaudoLaboratorial">
				<%=Html.RadioButton("PossuiLaudoLaboratorial", (int)eLaudoLaboratorial.Sim, Model.PTV.PossuiLaudoLaboratorial == (int)eLaudoLaboratorial.Sim, ViewModelHelper.SetaDisabled(true, new { @class="rbPossuiLaudo rbPossuiLaudoSim" }))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("PossuiLaudoLaboratorial", (int)eLaudoLaboratorial.Nao, Model.PTV.PossuiLaudoLaboratorial.GetValueOrDefault() == (int)eLaudoLaboratorial.Nao, ViewModelHelper.SetaDisabled(true, new { @class="rbPossuiLaudo rbPossuiLaudoNao" }))%>
				Não
			</label>
		</div>
	</div>
	<div class="block laudo <%= Model.PTV.PossuiLaudoLaboratorial == (int)eLaudoLaboratorial.Sim ? "":"hide"%>">
		<table class="dataGridTable gridLaudoLaboratorial">
			<thead>
				<tr>
					<th>Nome do laboratório</th>
					<th style="width: 29%">Nº do laudo com resultado da análise</th>
					<th style="width: 5%">UF</th>
					<th style="width: 20%">Municipio</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.LsLaudoLaboratorial) { %>
				<tr>
					<td class="loboratorioNome" title="<%= item.Nome %>"><%=item.Nome %></td>
					<td class="laboratorioNumero" title="<%=item.LaudoResultadoAnalise %>"><%=item.LaudoResultadoAnalise %></td>
					<td class="laboratorioUF" title="<%=item.EstadoTexto %>"><%=item.EstadoTexto %></td>
					<td class="laboratorioMunicipio" title="<%= item.MunicipioTexto %>"><%=item.MunicipioTexto %></td>
				</tr>
				<% } %>
				
				<tr class="trTemplate hide">
					<td class="loboratorioNome"></td>
					<td class="laboratorioNumero"></td>
					<td class="laboratorioUF"></td>
					<td class="laboratorioMunicipio"></td>
				</tr>
			</tbody>
		</table>
	</div>
</div>

<fieldset class="block box tratamentoFitossanitario campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<legend>Tratamento Fitossanitário com Fins Quarentenários</legend>
	<div class="block">
		<table class="dataGridTable gridTratamentoFitossa">
			<thead>
				<tr>
					<th>Nome do produto comercial</th>
					<th style="width: 20%">Igrediente ativo</th>
					<th style="width: 10%">Dose</th>
					<th style="width: 20%">Praga/Produto</th>
					<th style="width: 20%">Modo de aplicação</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.LsTratamentoFitossanitario) { %>
				<tr>
					<td class="produtoComercial" title="<%= item.ProdutoComercial %>"><%=item.ProdutoComercial %></td>
					<td class="ingrediente_ativo" title="<%=item.IngredienteAtivo %>"><%=item.IngredienteAtivo %></td>
					<td class="dose" title="<%=item.Dose %>"><%=item.Dose %></td>
					<td class="praga_produto" title="<%= item.PragaProduto %>"><%=item.PragaProduto %></td>
					<td class="modo_aplicacao" title="<%=item.ModoAplicacao%>">
						<%=item.ModoAplicacao%>
						<input type="hidden" class="hdnItemJsonFitossanitario" value='<%= ViewModelHelper.Json(item) %>' />
					</td>
				</tr>
				<% } %>
				<tr class="trTemplate hide">
					<td class="produtoComercial">
						<label class="lblProdutoComercial"></label>
					</td>
					<td class="ingrediente_ativo">
						<label class="lblIngrediente_ativo"></label>
					</td>
					<td class="dose">
						<label class="lblDose"></label>
					</td>
					<td class="praga_produto">
						<label class="lblPraga_produto"></label>
					</td>
					<td class="modo_aplicacao">
						<label class="lblModo_aplicacao"></label>
						<input type="hidden" class="hdnItemJsonFitossanitario" value='0' />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>

<div class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<div class="block">
		<div class="coluna30">
			<label for="TransporteTipo">Tipo de transporte *</label><br />
			<%= Html.DropDownList("TransporteTipo", Model.LsTipoTransporte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class= "text ddlTipoTransporte"}))%>
		</div>
		<div class="coluna30">
			<label for="VeiculoIdentificacaoNumero">Identificação do veículo nº *</label>
			<%= Html.TextBox("VeiculoIdentificacaoNumero", Model.PTV.VeiculoIdentificacaoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtIdentificacaoVeiculo", @maxlength="15"}))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna24">
			<label for="RotaTransitoDefinida">Rota de trânsito definida? *</label><br />
			<label>
				<%= Html.RadioButton("RotaTransitoDefinida", (int)eRotaTransitoDefinida.Sim, (Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Sim || Model.PTV.RotaTransitoDefinida == null) ,ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbRotaTransitoDefinida rdbRotaTransitoDefinidaSim"}))%>Sim				
			</label>
			<label>
				<%= Html.RadioButton("RotaTransitoDefinida", (int)eRotaTransitoDefinida.Nao, Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Nao ,ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbRotaTransitoDefinida rdbRotaTransitoDefinidaNao"}))%>Não
			</label>
		</div>
		<div class="coluna36 rota <%= (Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Nao)? "hide":"" %>">
			<label for="Itinerario">Itinerário *</label>
			<%=Html.TextBox("Itinerario", Model.PTV.Itinerario, ViewModelHelper.SetaDisabled(Model.IsVisualizar  , new { @class="text txtItinerario", @maxlength="200" }))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna24">
			<label for="NotaFiscalApresentacao">Apresentação de nota fiscal ?</label><br />
			<label>
				<%=Html.RadioButton("NotaFiscalApresentacao", (int)eApresentacaoNotaFiscal.Sim, (Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Sim || Model.PTV.NotaFiscalApresentacao == null), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbApresentacaoNotaFiscal rdbApresentacaoNotaFiscalSim" }))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("NotaFiscalApresentacao", (int)eApresentacaoNotaFiscal.Nao ,Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbApresentacaoNotaFiscal rdbApresentacaoNotaFiscalNao" }))%>
				Não
			</label>
		</div>
		<div class="coluna36 nota_fical <%=(Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao)? "hide":"" %>">
			<label for="NotaFiscalNumero">Nº da nota fiscal *</label>
			<%= Html.TextBox("NotaFiscalNumero", Model.PTV.NotaFiscalNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar , new { @class="text txtNotaFiscalNumero", @maxlength="60" })) %>
		</div>
	</div>
</div>

<div class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
	<div class="block">
		<div class="coluna15">
			<label for="DataValidade">Válido até *</label>
			<%=Html.TextBox("DataValidade", Model.PTV.ValidoAte.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtDataValidade maskData"}))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna40">
			<label for="ResponsavelTecnico">Responsável Técnico *</label>
			<%=Html.TextBox("ResponsavelTecnico", Model.PTV.ResponsavelTecnicoNome, ViewModelHelper.SetaDisabled(true, new { @class="text txtResponsavelTecnico", @maxlength="100"}))%>
			<input type="hidden" class="hdnResponsavelTecnicoId" value='<%= Model.PTV.ResponsavelTecnicoId %>' />
		</div>
	</div>
	<div class="block">
		<div class="coluna40">
			<label for="LocalEmissao">Local da Emissão *</label>
			<%= Html.DropDownList("LocalEmissao", Model.lsLocalEmissao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlLocalEmissao"}))%>
		</div>
	</div>
</div>