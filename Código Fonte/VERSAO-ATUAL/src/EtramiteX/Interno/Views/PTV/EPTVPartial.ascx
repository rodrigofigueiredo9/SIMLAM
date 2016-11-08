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
			<%=Html.TextBox("EmpreendimentoTexto", Model.PTV.EmpreendimentoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtEmpreendimento"}))%>
			<input type="hidden" class="hdnEmpreendimentoID" value='<%= Model.PTV.Empreendimento %>' />
		</div>
	</div>

	<div class="block campoTela">
		<div class="coluna58">
			<label for="ResponsavelEmpreendimento">Responsável do empreendimento</label>
			<%=Html.DropDownList("ResponsavelEmpreendimento", Model.ResponsavelList, ViewModelHelper.SetaDisabled(true, new { @class="text ddlResponsaveis"}))%>
		</div>
	</div>
</fieldset>

<div class="block box campoTela">
	<div class="block">
		<div class="coluna25">
			<label>Partida lacrada na Origem ?</label><br />
			<label>
				<%=Html.RadioButton("PartidaLacradaOrigem", (int)ePartidaLacradaOrigem.Sim, Model.PTV.PartidaLacradaOrigem == (int)ePartidaLacradaOrigem.Sim, ViewModelHelper.SetaDisabled(true, new { @class="rbPartidaLacradaOrigem rbLacradaOrigemSim"}))%>
				Sim
			</label>
			<label>
				<%=Html.RadioButton("PartidaLacradaOrigem", (int)ePartidaLacradaOrigem.Nao, Model.PTV.PartidaLacradaOrigem.GetValueOrDefault() == (int)ePartidaLacradaOrigem.Nao , ViewModelHelper.SetaDisabled(true, new { @class="rbPartidaLacradaOrigem rbLacradaOrigemNao"}))%>
				Não
			</label>
		</div>
		<div class="partida_lacrada <%= Model.PTV.PartidaLacradaOrigem.GetValueOrDefault() == (int)ePartidaLacradaOrigem.Sim ?"":"hide" %>">
			<div class="coluna15">
				<label for="LacreNumero">Nº do lacre</label>
				<%=Html.TextBox("LacreNumero", Model.PTV.LacreNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtNumeroLacre", @maxlength="15"}))%>
			</div>
			<div class="coluna15 ">
				<label for="PoraoNumero">Nº do porão</label>
				<%=Html.TextBox("PoraoNumero", Model.PTV.PoraoNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtNumeroPorao", @maxlength="15"}))%>
			</div>
			<div class="coluna15">
				<label for="ContainerNumero">Nº do contêiner</label>
				<%=Html.TextBox("ContainerNumero", Model.PTV.ContainerNumero, ViewModelHelper.SetaDisabled(true, new { @class="text txtNumeroContainer", @maxlength="15"}))%>
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

<div class="block box possuiLaudoLaboratorial campoTela">
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
			</tbody>
		</table>
	</div>
</div>

<fieldset class="block box tratamentoFitossanitario campoTela">
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
			</tbody>
		</table>
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
</div>

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
			<%= Html.TextArea("SituacaoMotivo", Model.PTV.SituacaoMotivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text media txtSituacaoMotivo", @maxlength="1000" })) %>
		</div>
	</div>
</fieldset>

<div class="block box divCamposSituacao divAprovar <%= Model.PTV.Situacao != (int)eSolicitarPTVSituacao.Aprovado ? "hide":""%>">
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
