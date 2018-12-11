<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVVM>" %>

<input type="hidden" class="hdnEmissaoId" value='<%= Model.PTV.Id %>' />
<div class="block box">
	<div class="coluna22">
		<label for="Numero">Número DUA*</label>
		<%=Html.TextBox("NumeroDua", Model.PTV.NumeroDua , ViewModelHelper.SetaDisabled(Model.PTV.Id > 0 , new { @class="text txtNumeroDua maskNumInt", @maxlength="80"}))%>
	</div>

	<div class="coluna15">
		<p>
			<label for="Pessoa.Tipo">Tipo *</label>
		</p>
		<label><%= Html.RadioButton("TipoPessoa", PessoaTipo.FISICA, Model.PTV.TipoPessoa != PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(true, new { @class = "radio pessoaf rdbPessaoTipo" }))%> Física</label>
		<label class="append5"><%= Html.RadioButton("TipoPessoa", PessoaTipo.JURIDICA, Model.PTV.TipoPessoa == PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(true, new { @class = "radio pessoaj rdbPessaoTipo" }))%> Jurídica</label>
	</div>
	<div class="coluna20 prepend7">
		<div class="CpfPessoaFisicaContainer <%= Model.PTV.TipoPessoa != PessoaTipo.JURIDICA ? "" : "hide" %> ">
			<label for="CPFCNPJDUA">CPF *</label>
			<%= Html.TextBox("CPFCNPJDUA", Model.PTV.CPFCNPJDUA, ViewModelHelper.SetaDisabled(true, new { @class = "text maskCpf txtCPFDUA" }))%>
		</div>
		<div class="CnpjPessoaJuridicaContainer <%= Model.PTV.TipoPessoa == PessoaTipo.JURIDICA ? "" : "hide" %> ">
			<label for="CPFCNPJDUA">CNPJ *</label>
			<%= Html.TextBox("CPFCNPJDUA", Model.PTV.CPFCNPJDUA, ViewModelHelper.SetaDisabled(true, new { @class = "text maskCnpj txtCNPJDUA" }))%>
		</div>
	</div>
</div>

<div class="block box">
	<div class="coluna10">
		<label>Tipo emissão</label><br />
		<label>
			<%= Html.RadioButton("NumeroTipo", (int)eDocumentoFitossanitarioTipoNumero.Digital, (Model.PTV.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.PTV.NumeroTipo.GetValueOrDefault() == (int)eDocumentoFitossanitarioTipoNumero.Nulo), ViewModelHelper.SetaDisabled(true, new { @class="rbTipoNumero rbTipoNumeroDigital"}))%>Nº Digital
		</label>
	</div>
	<div class="coluna22">
		<label for="Numero">Número PTV *</label>
		<%=Html.TextBox("Numero", Model.PTV.Numero > 0 ? Model.PTV.Numero : (object)"Gerado automaticamente" , ViewModelHelper.SetaDisabled(true, new { @class="text txtNumero maskNumInt", @maxlength="10"}))%>
	</div>
	<div class="coluna15">
		<label for="DataEmissao">Data de Emissão *</label>
		<%=Html.TextBox("DataEmissao", !string.IsNullOrEmpty(Model.PTV.DataEmissao.DataTexto)?Model.PTV.DataEmissao.DataTexto:DateTime.Today.ToShortDateString(), ViewModelHelper.SetaDisabled(true, new { @class="text txtDataEmissao maskData"}))%>
	</div>
</div>

<fieldset class="block box identificacao_produto campoTela">
	<legend>Identificação do produto</legend>
	<div class="gridContainer">
		<table class="dataGridTable gridProdutos">
			<thead>
				<tr>
					<th style="width: 20%">Origem</th>
					<th>Cultura/Cultivar</th>
					<th style="width: 10%">Quantidade</th>
					<th style="width: 16%">Unidade de medida</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.PTV.Produtos) { %>
				<tr>
					<td class="Origem_Tipo" title="<%=item.OrigemTipoTexto %>"><%= item.OrigemTipoTexto %></td>
					<td class="cultura_cultivar" title="<%= item.CulturaCultivar %>"><%= item.CulturaCultivar %></td>
					<td class="quantidade" title="<%=item.Quantidade %>"><%=item.Quantidade %></td>
					<td class="unidade_medida" title="<%= item.UnidadeMedida %>"><%=item.UnidadeMedidaTexto %></td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
	<br />

	<div class="block campoTela">
		<div class="coluna58">
			<label for="EmpreendimentoTexto">Empreendimento *</label>
			 <% if ( !string.IsNullOrEmpty(Model.PTV.EmpreendimentoSemDoc) ) { %>
			    <%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoSemDoc, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
            <%} else { %>
                <%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
			<input type="hidden" class="hdnEmpreendimentoID" value='<%= Model.PTV.Empreendimento %>' />
              <% } %>
		</div>
	</div>

	<div class="block campoTela">
		<div class="coluna58">
			<label for="ResponsavelEmpreendimento">Responsável do empreendimento</label>
		   <%  if (Model.PTV.Produtos.Count > 0 && Model.PTV.Produtos[0].OrigemTipo > (int)eDocumentoFitossanitarioTipo.PTVOutroEstado )
               { %>
			    <%=Html.TextBox("ResponsavelEmpreendimento", Model.PTV.ResponsavelSemDoc , ViewModelHelper.SetaDisabled(true, new { @class="text ddlResponsaveis"}))%>
            <% } else { %>
                <%=Html.DropDownList("ResponsavelEmpreendimento", Model.ResponsavelList, ViewModelHelper.SetaDisabled(Model.IsVisualizar|| Model.ResponsavelList.Count == 2, new { @class="text ddlResponsaveis"}))%>
            <% } %>
		</div>
	</div>
</fieldset>

<div class="block box campoTela">
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

<fieldset class="block box destinatario campoTela">
	<legend>Destinatário</legend>
	<div class="asmItens">
		<div class="asmItemContainer boxBranca borders">
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

<div class="block box campoTela">
	<div class="block">
		<div class="coluna30">
			<label for="TransporteTipo">Tipo de transporte *</label><br />
			<%= Html.DropDownList("TransporteTipo", Model.LsTipoTransporte, ViewModelHelper.SetaDisabled(true, new { @class= "text ddlTipoTransporte"}))%>
		</div>
		<div class="coluna30">
			<label for="VeiculoIdentificacaoNumero">Identificação do veículo nº *</label>
			<%= Html.TextBox("VeiculoIdentificacaoNumero", Model.PTV.VeiculoIdentificacaoNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtIdentificacaoVeiculo", @maxlength="15"}))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna24">
			<label for="RotaTransitoDefinida">Rota de trânsito definida? *</label><br />
			<label>
				<%= Html.RadioButton("RotaTransitoDefinida", (int)eRotaTransitoDefinida.Sim, (Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Sim || Model.PTV.RotaTransitoDefinida == null) ,ViewModelHelper.SetaDisabled(true, new { @class="rdbRotaTransitoDefinida rdbRotaTransitoDefinidaSim"}))%>Sim				
			</label>
			<label>
				<%= Html.RadioButton("RotaTransitoDefinida", (int)eRotaTransitoDefinida.Nao, Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Nao ,ViewModelHelper.SetaDisabled(true, new { @class="rdbRotaTransitoDefinida rdbRotaTransitoDefinidaNao"}))%>Não
			</label>
		</div>
		<div class="coluna36 rota <%= (Model.PTV.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Nao)? "hide":"" %>">
			<label for="Itinerario">Itinerário *</label>
			<%=Html.TextBox("Itinerario", Model.PTV.Itinerario, ViewModelHelper.SetaDisabled(true, new { @class="text txtItinerario", @maxlength="200" }))%>
		</div>
	</div>
	<div class="block">
		<div class="coluna24">
			<label for="NotaFiscalApresentacao">Apresentação de nota fiscal ?</label><br />
			<label>
				<%=Html.RadioButton("NotaFiscalApresentacao", (int)eApresentacaoNotaFiscal.Sim, (Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Sim || Model.PTV.NotaFiscalApresentacao == null), ViewModelHelper.SetaDisabled(true, new { @class="rdbApresentacaoNotaFiscal rdbApresentacaoNotaFiscalSim" }))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("NotaFiscalApresentacao", (int)eApresentacaoNotaFiscal.Nao ,Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao, ViewModelHelper.SetaDisabled(true, new { @class="rdbApresentacaoNotaFiscal rdbApresentacaoNotaFiscalNao" }))%>
				Não
			</label>
		</div>
		<div class="coluna36 nota_fical <%=(Model.PTV.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao)? "hide":"" %>">
			<label for="NotaFiscalNumero">Nº da nota fiscal *</label>
			<%= Html.TextBox("NotaFiscalNumero", Model.PTV.NotaFiscalNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtNotaFiscalNumero", @maxlength="60" })) %>
		</div>
	</div>

	<div class="block">
		<div class="coluna24">
			<label for="NotaFiscalApresentacao">Possui nota fiscal da caixa ? *</label><br />
			<label>
				<%=Html.RadioButton("NotaFiscalCaixaApresentacao", (int)eApresentacaoNotaFiscal.Sim, (Model.PTV.NFCaixa.notaFiscalCaixaApresentacao == 0), ViewModelHelper.SetaDisabled(true, new { @class="rdbApresentacaoNotaFiscalCaixa" }))%>
					Sim
			</label>
			<label>
				<%=Html.RadioButton("NotaFiscalCaixaApresentacao", (int)eApresentacaoNotaFiscal.Nao, (Model.PTV.NFCaixa.notaFiscalCaixaApresentacao > 0), ViewModelHelper.SetaDisabled(true, new { @class="rdbApresentacaoNotaFiscalCaixa" }))%>
					Não
			</label>
		</div>
		<div class="coluna40 isPossuiNFCaixa <%= Model.PTV.NFCaixa.notaFiscalCaixaApresentacao > 0 ? "hide" : "" %>">
			<label for="NotaFiscalApresentacao">Tipo da caixa *</label><br />
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Madeira, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="1" }))%>
					Madeira
			</label>
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Plastico, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="2" }))%>
					Plástico
			</label>
			<label>
				<%=Html.RadioButton("tipoCaixaId", (int)eTipoNotaFiscalDeCaixa.Papelao, ViewModelHelper.SetaDisabled(true, new { @class="rdbTipoCaixa", @id="3" }))%>
					Papelão
			</label>
		</div>
	</div>
	<div class="block">
		<div class="isTipoCaixaChecked hide">
			<div class="coluna36">
				<label for="NotaFiscalNumero" class="lblNumeroNFCaixa">Nº da nota fiscal de caixa *</label>
				<%= Html.TextBox("NotaFiscalCaixaNumero", Model.PTV.NFCaixa.notaFiscalCaixaNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtNotaFiscalCaixaNumero", @maxlength="35" })) %>
			</div>
			<div class="pessoaAssociadaNfCaixa">
				<div class="coluna15 prepend1">
					<label for="PessoaTipo">Tipo *</label><br />
					<label><%= Html.RadioButton("TipoPessoaCaixa", PessoaTipo.FISICA, (int)Model.PTV.NFCaixa.PessoaAssociadaTipo != PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio pessoaf rdbPessaoNfCaixa" }))%> Física</label>							
					<label class="append5"><%= Html.RadioButton("TipoPessoaCaixa", PessoaTipo.JURIDICA, (int)Model.PTV.NFCaixa.PessoaAssociadaTipo == PessoaTipo.JURIDICA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio pessoaj rdbPessaoNfCaixa" }))%> Jurídica</label>
				</div>
				<div class="coluna20">
					<div class="CpfPessoaFisicaNfCaixaContainer <%= (int)Model.PTV.NFCaixa.PessoaAssociadaTipo != PessoaTipo.JURIDICA ? "" : "hide" %> ">
						<label for="CPFCNPJDUA">CPF *</label>
						<%= Html.TextBox("CPFCNPJCaixa", Model.PTV.NFCaixa.PessoaAssociadaCpfCnpj, ViewModelHelper.SetaDisabled(false, new { @class = "text maskCpf txtCPFCaixa" }))%>
					</div>
					<div class="CnpjPessoaJuridicaNfCaixaContainer <%= (int)Model.PTV.NFCaixa.PessoaAssociadaTipo == PessoaTipo.JURIDICA ? "" : "hide" %> ">
						<label for="CPFCNPJDUA">CNPJ *</label>
						<%= Html.TextBox("CPFCNPJCaixa", Model.PTV.NFCaixa.PessoaAssociadaCpfCnpj, ViewModelHelper.SetaDisabled(false, new { @class = "text maskCnpj txtCNPJCaixa" }))%>
					</div>
				</div>
			</div>
			<div class="coluna10">
				<button class="inlineBotao btnVerificarNotaCaixaCaixa">Verificar</button>
				<button class="inlineBotao btnLimparNotaCaixaCaixa hide">Limpar</button>
			</div>
		</div>
		
	</div>
	<div class="block">
		<div class="isNFCaixaVerificado hide">
			<div class="coluna15">
				<label class="lblSaldoAtualInicial">Saldo atual</label>
				<%= Html.TextBox("SaldoAtual", Model.PTV.NFCaixa.saldoAtual, ViewModelHelper.SetaDisabled(true, new { @class="text maskNum8 txtNFCaixaSaldoAtual", @maxlength="8"}))%>
			</div>
			<div class="coluna15">
				<label>N° de caixas *</label>
				<%= Html.TextBox("NumeroDeCaixas", Model.PTV.NFCaixa.numeroCaixas, ViewModelHelper.SetaDisabled(true, new { @class="text maskNum8 txtNFCaixaNumeroDeCaixas", @maxlength="8"}))%>
			</div>
			<div class="coluna10">
				<button class="inlineBotao btnAddCaixa">Adicionar</button>
			</div>
		</div>
	</div>
	<div class="gridContainer identificacaoDaCaixa <%= Model.PTV.NotaFiscalDeCaixas.Count() > 0 ? "" : "hide" %>">
		<table class="dataGridTable gridCaixa">
			<thead>
				<tr>
					<th style="width: 30%">N° da nota fiscal de caixa </th>
					<th>CPF/CNPJ</th>
					<th>Tipo da caixa</th>
					<th style="width: 10%">Saldo atual</th>
					<th style="width: 16%">N° de caixas utilizadas</th>
					<% if (!Model.IsVisualizar)
						{ %><th style="width: 7%">Ação</th>
					<% } %>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.PTV.NotaFiscalDeCaixas)
					{
				%>
				<tr>
					<td class="" title="<%=item.notaFiscalCaixaNumero %>"><%= item.notaFiscalCaixaNumero %></td>
					<td class="" title="<%=item.PessoaAssociadaCpfCnpj %>"><%= item.PessoaAssociadaCpfCnpj %></td>
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
						<label class="lvlCPFCNPJ"></label>
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
<fieldset class="block box campoTela">
	<legend>Anexos</legend>

	<div class="block dataGrid">
		<label class="lblGridVazio <%= Model.PTV.Anexos.Count > 0 ? "hide" : "" %>">Não existe anexo adicionado.</label>

		<table class="dataGridTable tabAnexos <%= Model.PTV.Anexos.Count > 0 ? "" : "hide" %>" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Arquivo</th>
					<th>Descrição</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (Tecnomapas.Blocos.Entities.Etx.ModuloArquivo.Anexo anexo in Model.PTV.Anexos) {  %>
				<tr>
					<td>
						<span class="ArquivoNome" title="<%= Html.Encode(anexo.Arquivo.Nome) %>"><%= Html.ActionLink(anexo.Arquivo.Nome, "BaixarCredenciado", "Arquivo", new { @id = anexo.Arquivo.Id }, new { @Style = "display: block" })%></span>
					</td>
					<td>
						<span title="<%= Html.Encode(anexo.Descricao) %>" class="AnexoDescricao"><%= Html.Encode(anexo.Descricao) %></span>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box campoTela">
	<legend>Vistoria de carga *</legend>
	<div class="block">
		<div class="coluna40">
			<label for="LocalEmissao">Local da Vistoria *</label>
			<%= Html.DropDownList("LocalVistoriaId", Model.lsLocalVistoria, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlLocalVistoria"}))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna40">
			<label for="LocalEmissao">Vistoria de Carga *</label>
			<%= Html.DropDownList("DataHoraVistoriaId", Model.lsDiaHoraVistoria, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlDatahoraVistoriaporSetor"}))%>
		</div>
	</div>
</fieldset>

<div class="block box">
    <div class="coluna40">
        <label for="LocalEmissao">Solicitante *</label>
        <%= Html.TextBox("Solicitante", Model.PTV.CredenciadoNome, ViewModelHelper.SetaDisabled(true, new { @class = "txtSolicitante text" }))%>
    </div>
</div>

<fieldset class="block box">
	<legend>Resultado da análise</legend>
	<div class="block">
		<div class="coluna20">
			<label for="SituacaoTexto">Situação</label>
			<%= Html.TextBox("SituacaoTexto", Model.PTV.SituacaoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text" })) %>
		</div>

		<div class="coluna20 prepend1">
			<label for="SituacaoData">Data</label>
			<%= Html.TextBox("SituacaoData", Model.PTV.SituacaoData.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class="text" })) %>
		</div>

		<input type="button" title="" class="icone pdf btnPDF" style="margin-top:2%" />
	</div>


	<div class="block">
		<div class="coluna90">
			<% for (int i = 0; i < Model.AcoesAlterar.Count; i++)
	         { %> 
                    <% if (Model.AcoesAlterar[i].Mostrar)  { %>

			        <label class="append5">
				        <input type="radio" 
							   name="OpcaoSituacao" 
							   value="<%= Model.AcoesAlterar[i].Id %>" 
							   <%= Model.AcoesAlterar[i].Habilitado ? "" : "disabled=\"disabled\"" %> <%= Model.IsVisualizar ? "disabled='disabled'" : "" %>
							   class="radio  <%= Model.IsVisualizar ? "" : "rdbOpcaoSituacao" %>
							   <%= Model.AcoesAlterar[i].Habilitado ? "" : "disabled" %>"
								<%= Model.AcoesAlterar[i].Id == Model.PTV.Situacao ? "checked=\"checked\"" : "" %>" />
						<%= Model.AcoesAlterar[i].Texto %>
                    </label>
			    <% } %>
			<% } %>
		</div>
	</div>


	<div class="block divCamposSituacao divMotivo <%= (Model.PTV.Situacao != (int)eSolicitarPTVSituacao.Rejeitado || Model.PTV.Situacao != (int)eSolicitarPTVSituacao.Bloqueado) ? "hide":""%>">
		<div class="block ultima">
			<label for="SituacaoMotivo">Motivo *</label>
			<%= Html.TextArea("SituacaoMotivo", Model.PTV.SituacaoMotivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text media txtSituacaoMotivo", @maxlength="500" })) %>
		</div>
	</div>

	<div class="block divCamposSituacaoAgendamento divAgendarFiscalizacao <%= Model.PTV.Situacao != (int)eSolicitarPTVSituacao.AgendarFiscalizacao ? "hide":""%>">
        <div class="block">
            <div class="coluna60">
                <label for="Local">Local *</label>
                <%= Html.TextBox("LocalFiscalizacao", Model.PTV.LocalFiscalizacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtLocalFiscalizacao", @maxlength="500" })) %>
            </div>
        </div>
        <div class="block">
            <div class="coluna10">
                <label for="HoraFiscalizacao">Hora *</label>
                <%= Html.TextBox("HoraFiscalizacao", Model.PTV.HoraFiscalizacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskHoraMinuto txtHoraFiscalizacao" })) %>
            </div>
        </div>
        <div class="block">
            <div class="coluna60">
                <label for="InformacoesAdicionais">Informações Adicionais</label>
                <%= Html.TextArea("InformacoesAdicionais", Model.PTV.InformacoesAdicionais, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text media txtInformacoesAdicionais", @maxlength="500" })) %>
            </div>
        </div>
    </div>
</fieldset>

<div class="block box divCamposSituacao divAprovar <%= Model.PTV.Situacao != (int)eSolicitarPTVSituacao.Valido ? "hide":""%>">
	<div class="block">
		<div class="coluna15">
			<label for="DataValidade">Válido até *</label>
			<%=Html.TextBox("DataValidade", Model.PTV.ValidoAte.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtDataValidade maskData"}))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna40">
			<label for="ResponsavelTecnico">Responsável Técnico *</label>
			<input type="hidden" class="hdnResponsavelTecnicoId" value='<%= Model.PTV.ResponsavelTecnicoId %>' />
			<%=Html.TextBox("ResponsavelTecnico", Model.PTV.ResponsavelTecnicoNome, ViewModelHelper.SetaDisabled(true, new { @class="text txtResponsavelTecnico", @maxlength="100"}))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna40">
			<label for="LocalEmissao">Local da Emissão *</label>
			<%= Html.DropDownList("LocalEmissao", Model.lsLocalEmissao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlLocalEmissao"}))%>
		</div>
	</div>
</div>
