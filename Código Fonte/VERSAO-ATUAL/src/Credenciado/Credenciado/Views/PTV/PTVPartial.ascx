<%@ Import Namespace="System.Activities.Statements" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloArquivo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVVM>" %>

<script>
	PTVEmitir.settings.Mensagens = <%= Model.Mensagens %>;
</script>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.PTV.Id %>' />
<input class="hdnVisualizar" type="hidden" value="<%= Model.IsVisualizar %>" />

<fieldset class="box">
	<div class="block">
		<div class="coluna22 divDUA">
			<label for="NumeroDua">Número DUA*</label>
			<%=Html.TextBox("NumeroDua", Model.PTV.NumeroDua , ViewModelHelper.SetaDisabled(Model.PTV.Id > 0 , new { @class="text txtNumeroDua maskNumInt", @maxlength="80"}))%>
		</div>

		<div class="coluna20 divDUA">
			<label for="PessoaTipo">Tipo *</label><br />
			<label><%= Html.RadioButton("TipoPessoa", PessoaTipo.FISICA, Model.PTV.TipoPessoa != PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class = "radio pessoaf rdbPessaoTipo" }))%> Física</label>							
			<label class="append5"><%= Html.RadioButton("TipoPessoa", PessoaTipo.JURIDICA, Model.PTV.TipoPessoa == PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class = "radio pessoaj rdbPessaoTipo" }))%> Jurídica</label>
		</div>

		<div class="coluna20 prepend7 divDUA">
			<div class="CpfPessoaFisicaContainer <%= Model.PTV.TipoPessoa != PessoaTipo.JURIDICA ? "" : "hide" %> ">
				<label for="CPFCNPJDUA">CPF *</label>
				<%= Html.TextBox("CPFCNPJDUA", Model.PTV.CPFCNPJDUA, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class = "text maskCpf txtCPFDUA" }))%>
			</div>
			<div class="CnpjPessoaJuridicaContainer <%= Model.PTV.TipoPessoa == PessoaTipo.JURIDICA ? "" : "hide" %> ">
				<label for="CPFCNPJDUA">CNPJ *</label>
				<%= Html.TextBox("CPFCNPJDUA", Model.PTV.CPFCNPJDUA, ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class = "text maskCnpj txtCNPJDUA" }))%>
			</div>
		</div>

		<% if (!Model.IsVisualizar) { %>
		<div class="coluna10">
			<button type="button" class="inlineBotao btnVerificarDua <%= Model.PTV.Id > 0 ? "hide":""%>">Verificar</button>
			<button type="button" class="inlineBotao btnLimparDua <%= Model.PTV.Id <= 0 ? "hide":""%>">Editar</button>
		</div>
		<% } %>
		</div>
</fieldset>



<div class="linhaConteudo <%= Model.PTV.Id <= 0 ? "hide":""%>">

	<fieldset class="box">
		<div class="block">
			<div class="coluna10">
				<label>Tipo emissão</label><br />
				<label>
					<%= Html.RadioButton("NumeroTipo", (int)eDocumentoFitossanitarioTipoNumero.Digital, (Model.PTV.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.PTV.NumeroTipo.GetValueOrDefault() == (int)eDocumentoFitossanitarioTipoNumero.Nulo), ViewModelHelper.SetaDisabled(Model.PTV.Id > 0, new { @class="rbTipoNumero rbTipoNumeroDigital"}))%>Nº Digital
				</label>
			</div>
			<div class="coluna22">
				<label for="Numero">Número PTV *</label>
				<%=Html.TextBox("Numero", Model.PTV.Numero > 0 ? Model.PTV.Numero : (object)"Gerado automaticamente" , ViewModelHelper.SetaDisabled(Model.PTV.Id > 0 || Model.PTV.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Bloco, new { @class="text txtNumero maskNumInt", @maxlength="10"}))%>
			</div>
			<div class="coluna15">
				<label for="DataEmissao">Data de Emissão *</label>
				<%=Html.TextBox("DataEmissao", !string.IsNullOrEmpty(Model.PTV.DataEmissao.DataTexto)?Model.PTV.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(true, new { @class="text txtDataEmissao maskData"}))%>
			</div>
			<div class="coluna20">
				<label for="Situacao">Situação</label>
				<%=Html.DropDownList("Situacao", Model.Situacoes, ViewModelHelper.SetaDisabled(true , new { @class="text ddlSituacoes"}))%>
			</div>
		</div>
	
	</fieldset>

	<fieldset class="block box identificacao_produto campoTela  <%= Model.PTV.Id <= 0 ? "hide":""%>">
		<legend>Identificação do produto</legend>
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna20 divNumeroEnter">
				<label for="OrigemTipo">Documento de origem *</label>
				<%=Html.DropDownList("OrigemTipo", Model.OrigemTipoList, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlOrigemTipo"})) %>
			</div>
			<div class="coluna22 divNumeroEnter">
				<input type="hidden" class="hdnNumeroOrigem" value="0" />
				<input type="hidden" class="hdnEmpreendimentoOrigemID" value="0" />
				<input type="hidden" class="hdnEmpreendimentoOrigemNome" value="" />
				<label for="NumeroOrigem">Número documento <span class="labelOrigem"></span> *</label>
				<%=Html.TextBox("NumeroOrigem",  (object)String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroOrigem",  @maxlength="12"})) %>
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
            <div class="coluna10">
                <button type="button" class="inlineBotao btnLimparDocumentoOrigem hide">Limpar</button>
            </div>
            <div class="coluna15 saldoContainer hide">
                <label>Saldo</label>
                <%=Html.TextBox("SaldoDocOrigem",  (object)String.Empty, ViewModelHelper.SetaDisabled(true, new { @class="text txtSaldoDocOrigem"})) %>
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
					<% foreach (var item in Model.PTV.Produtos) { 
           
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
                        unid = item.UnidadeMedidaTexto;
                    }             
            %>
					<tr>
						<td class="Origem_Tipo" title="<%=item.OrigemTipoTexto %>"><%= item.OrigemTipoTexto %></td>
						<td class="cultura_cultivar" title="<%= item.CulturaCultivar %>"><%= item.CulturaCultivar %></td>
						<td class="quantidade" title="<%=qtd %>"><%=qtd %></td>
					    <td class="unidade_medida" title="<%= unid %>"><%=unid %></td>
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
				 <% if (!string.IsNullOrEmpty(Model.PTV.EmpreendimentoSemDoc) && Model.PTV.Empreendimento == 0)
                     { %>
			        <%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoSemDoc, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
                <%} else { %>
                    <%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
			    <input type="hidden" class="hdnEmpreendimentoID" value='<%= Model.PTV.Empreendimento %>' />
                <script> $(".hdnEmpreendimentoOrigemID").val(<%= Model.PTV.Empreendimento %>);  </script>
                    <% }%>
			</div>		
		</div>

		<div class="block campoTela  <%= Model.PTV.Id <= 0 ? "hide":""%>">
			<div class="coluna58">
				<label for="ResponsavelEmpreendimento">Responsável do empreendimento</label>
				   <% if (Model.PTV.Produtos.Count > 0 && Model.PTV.Produtos[0].OrigemTipo > (int)eDocumentoFitossanitarioTipo.PTVOutroEstado )
                   { %>
			        <%=Html.TextBox("ResponsavelEmpreendimento", Model.PTV.ResponsavelSemDoc , ViewModelHelper.SetaDisabled(true, new { @class="text ddlResponsaveis"}))%>
                <% } else { %>
                    <%=Html.DropDownList("ResponsavelEmpreendimento", Model.ResponsavelList, ViewModelHelper.SetaDisabled(Model.IsVisualizar|| Model.ResponsavelList.Count == 2, new { @class="text ddlResponsaveis"}))%>
                <% } %>
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
		<div class="block">
		<div class="coluna24">
			<label for="NotaFiscalApresentacao">Possui nota fiscal da caixa ? *</label><br />
			<label>
				<%=Html.RadioButton("NotaFiscalCaixaApresentacao", (int)eApresentacaoNotaFiscal.Sim, (Model.PTV.NFCaixa.notaFiscalCaixaApresentacao == 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbApresentacaoNotaFiscalCaixa" }))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("NotaFiscalCaixaApresentacao", (int)eApresentacaoNotaFiscal.Nao, (Model.PTV.NFCaixa.notaFiscalCaixaApresentacao > 0), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbApresentacaoNotaFiscalCaixa" }))%>
				Não
			</label>
		</div>
		<div class="coluna40 isPossuiNFCaixa <%= Model.IsVisualizar ? "hide" : "" %>">
			<label for="NotaFiscalApresentacao">Tipo da caixa *</label><br />
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Madeira, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="1" }))%>
				Madeira
			</label>
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Plastico, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="2" }))%>
				Plástico
			</label>
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Papelao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="rdbTipoCaixa", @id="3" }))%>
				Papelão
			</label>
		</div>		
	</div>
	<div class="block">
		<div class="isTipoCaixaChecked hide">
			<div class="coluna36">
				<label for="NotaFiscalNumero" class="lblNumeroNFCaixa">Nº da nota fiscal de caixa *</label>
				<%= Html.TextBox("NotaFiscalCaixaNumero", Model.PTV.NFCaixa.notaFiscalCaixaNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar , new { @class="text txtNotaFiscalCaixaNumero", @maxlength="60" })) %>
			</div>
			<div class="coluna10">
				<button class="inlineBotao btnVerificarNotaCaixaCaixa">Verificar</button>
				<button class="inlineBotao btnLimparNotaCaixaCaixa hide">Limpar</button>
			</div>
		</div>
		<div class="isNFCaixaVerificado hide">
			<div class="coluna15">
				<label class="lblSaldoAtualInicial">Saldo atual</label>
				<%= Html.TextBox("SaldoAtual", Model.PTV.NFCaixa.saldoAtual, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNum8 txtNFCaixaSaldoAtual", @maxlength="8"}))%>
			</div>
			<div class="coluna15">
				<label>N° de caixas *</label>
				<%= Html.TextBox("NumeroDeCaixas", Model.PTV.NFCaixa.numeroCaixas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNum8 txtNFCaixaNumeroDeCaixas", @maxlength="8"}))%>
			</div>
			<div class="coluna10">
				<button class="inlineBotao btnAddCaixa">Adicionar</button>
			</div>
		</div>
	</div>
		<div class="gridContainer identificacaoDaCaixa <%= Model.PTV.NotaFiscalDeCaixas.Count() > 0 ? "" : "hide" %>" >
			<table class="dataGridTable gridCaixa">
				<thead>
					<tr>
						<th style="width: 30%">N° da nota fiscal de caixa </th>
						<th>Tipo da caixa</th>
						<th style="width: 10%">Saldo atual</th>
						<th style="width: 16%">N° de caixas</th>
						<% if (!Model.IsVisualizar)
				 { %><th style="width: 7%">Ação</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.PTV.NotaFiscalDeCaixas)
				 {
					%>
						<tr>
							<td class="" title="<%=item.notaFiscalCaixaNumero %>"><%= item.notaFiscalCaixaNumero %></td>
							<td class="" title="<%= item.tipoCaixaTexto %>"><%= item.tipoCaixaTexto %></td>
							<td class="" title="<%=item.saldoAtual %>"><%=item.saldoAtual %></td>
							<td class="" title="<%= item.numeroCaixas %>"><%=item.numeroCaixas %></td>
							<%if (!Model.IsVisualizar)
				 { %>
							<td>
								<a class="icone excluir btnExcluirCaixa"></a>
								<input type="hidden" class="hdnItemJson" value='<%=ViewModelHelper.Json(item) %>' />
							</td>
							<%} %>
						</tr>
					<% } %>

					<tr class="trTemplate hide">
						<td class="">
							<label class="lblNFCaixaNumero"></label>
						</td>
						<td class="">
							<label class="lblTipoCaixa"></label>
						</td>
						<td class="">
							<label class="lblSaldoAtual"></label>
						</td>
						<td class="">
							<label class="lblNumeroDeCaixas"></label>
						</td>
						<td>
							<a class="icone excluir btnExcluirCaixa" title="Remover"></a>
							<input type="hidden" value="" class="hdnOrigemID" />
							<input type="hidden" value="0" class="hdnItemJson" />
						</td>
					</tr>

				</tbody>
			</table>
		</div>
	<br />
	</div>

   <!-- Arquivo -->
	<fieldset class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
        <legend>Anexos</legend>
        <% if (!Model.IsVisualizar) { %>
        <div class="block">
            <div class="coluna60 inputFileDiv">
                <label for="ArquivoTexto">Arquivo *</label>
                <%= Html.TextBox("Roteiro.Arquivo.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome", @style = "display: none;" })%>
                <input type="hidden" class="hdnArquivo" name="hdnArquivo" />
                <div class="anexoArquivos">
                </div>
                <input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" />
            </div>
        </div>
        <div class="block">
            <div class="coluna60">
                <label for="Descricao">
                    Descrição *</label>
                <%= Html.TextBox("Descricao", null, new { @maxlength = "100", @class = "text txtAnexoDescricao" })%>
            </div>
            <div class="coluna10 botoesAnexoDiv">
                <button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAddAnexoArquivo" title="Adicionar anexo"
                    onclick="PTVEmitir.onEnviarAnexoArquivoClick('<%= Url.Action("arquivo", "arquivo") %>');">
                    Adicionar</button>
            </div>
        </div>
        <% } %>

		<div class="block dataGrid">
			<label class="lblGridVazio <%= Model.PTV.Anexos.Count > 0 ? "hide" : "" %>">Não existe anexo adicionado.</label>

			<table class="dataGridTable tabAnexos <%= Model.PTV.Anexos.Count > 0 ? "" : "hide" %>" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Arquivo</th>
						<th>Descrição</th>
						<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
					<%
						int y = 0;
						foreach (Anexo anexo in Model.PTV.Anexos) {
							y++; %>
					<tr>
						<td>
							<span class="ArquivoNome" title="<%= Html.Encode(anexo.Arquivo.Nome) %>"><%= Html.ActionLink(anexo.Arquivo.Nome, "Baixar", "Arquivo", new { @id = anexo.Arquivo.Id }, new { @Style = "display: block" })%></span>
							<input type="hidden" class="hdnAnexoIndex" name="PTV.Anexos.Index" value="<%= y %>" />
							<input type="hidden" class="hdnAnexoOrdem" name="PTV.Anexos[<%= y %>].Ordem" value="<%= Html.Encode(anexo.Ordem) %>" />
							<input type="hidden" class="hdnArquivoNome" name="PTV.Anexos[<%= y %>].Arquivo.Nome" value="<%= Html.Encode(anexo.Arquivo.Nome) %>" />
							<input type="hidden" class="hdnArquivoExtensao" name="PTV.Anexos[<%= y %>].Arquivo.Extensao" value="<%= Html.Encode(anexo.Arquivo.Extensao) %>" />
						</td>
						<td>
							<span title="<%= Html.Encode(anexo.Descricao) %>" class="AnexoDescricao"><%= Html.Encode(anexo.Descricao) %></span>
							<input type="hidden" class="hdnAnexoDescricao" name="PTV.Anexos[<%= y %>].Descricao" value="<%= Html.Encode(anexo.Descricao) %>" />
                        </td>
                        <% if (!Model.IsVisualizar) { %>
                        <td>
                            <input type="hidden" class="hdnAnexoArquivoJson" name="PTV.Anexos[<%= y %>].Arquivo" value="<%: ViewModelHelper.JsSerializer.Serialize(anexo.Arquivo) %>" />
                            <input title="Descer" class="icone abaixo btnDescerLinha" type="button" />
                            <input title="Subir" class="icone acima btnSubirLinha" type="button" />
                            <input title="Excluir" class="icone excluir btnExcluirAnexo" value="" type="button" />
                        </td>
                    </tr>
                    <% } %>
                    <% } %>
                </tbody>
            </table>
			<table style="display: none">
				<tbody>
					<tr class="trAnexoTemplate">
						<td>
							<span class="ArquivoNome">NOME</span>
							<input type="hidden" class="hdnAnexoIndex" name="templatePTV.Anexos.Index" value="#Index" />
							<input type="hidden" class="hdnAnexoOrdem" name="templatePTV.Anexos[#Index].Ordem" value="#ORDEM" />
							<input type="hidden" class="hdnArquivoNome" name="templatePTV.Anexos[#Index].Arquivo.Nome" value="#ARQUIVONOME" />
							<input type="hidden" class="hdnArquivoExtensao" name="PTV.Anexos[#Index].Arquivo.Extensao" value="#EXTENSAONOME" />
						</td>
						<td>
							<span class="AnexoDescricao">DESCRICAO</span>
							<input type="hidden" class="hdnAnexoDescricao" name="templatePTV.Anexos[#Index].Descricao" value="#DESCRICAO" />
						</td>
						<td>
                            <input type="hidden" class="hdnAnexoArquivoJson" name="templatePTV.Anexos[#Index].Arquivo" value="#ARQUIVOJSON" />
                            <% if (!Model.IsVisualizar) { %>
                            <input title="Descer" class="icone abaixo btnDescerLinha" type="button" />
                            <input title="Subir" class="icone acima btnSubirLinha" type="button" />
                            <input title="Excluir" class="icone excluir btnExcluirAnexo" value="" type="button" />
                            <% } %>
                        </td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>

	<div class="block box campoTela <%= Model.PTV.Id <= 0 ? "hide":""%>">
		<div class="block">
			<div class="coluna40">
				<label for="LocalEmissao">Local da Vistoria *</label>
				<%= Html.DropDownList("LocalVistoriaId", Model.lsLocalVistoria, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlLocalVistoria"}))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna40">
				<label for="LocalEmissao">Vistoria de Carga *</label>
                 <%= Html.TextBox("DataVistoria", Model.PTV.DataVistoria == DateTime.MinValue ?   "" : Model.PTV.DataVistoria.ToString("dd/MM/yy") , ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "txtDataHoraVistoria text" }))%>
				 <%= Html.DropDownList("DataHoraVistoriaId", Model.lsDiaHoraVistoria, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlDatahoraVistoriaporSetor"}))%>
			</div>
		</div>
	</div>

	<div class="block box">
		<div class="coluna40">
			<label for="LocalEmissao">Solicitante *</label>
                <%= Html.TextBox("ResponsavelTecnicoNome", HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : "", ViewModelHelper.SetaDisabled(true, new { @class = "txtResponsavelTecnicoNome text" }))%>
		</div>
	</div>

</div>