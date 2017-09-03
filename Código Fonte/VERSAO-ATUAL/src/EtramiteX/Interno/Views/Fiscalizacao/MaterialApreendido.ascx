﻿<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaterialApreendidoVM>" %>

<script>

	FiscalizacaoMaterialApreendido.settings.urls.salvar = '<%= Url.Action("CriarMaterialApreendido") %>';
	FiscalizacaoMaterialApreendido.settings.urls.obterSerie = '<%= Url.Action("ObterInfracaoSeries") %>';
	FiscalizacaoMaterialApreendido.settings.urls.associarDepositario = '<%= Url.Action("PessoaModal", "Pessoa") %>';
	FiscalizacaoMaterialApreendido.settings.urls.editarDepositario = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
	FiscalizacaoMaterialApreendido.settings.urls.enviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoMaterialApreendido.settings.urls.obter = '<%= Url.Action("MaterialApreendido") %>';

	FiscalizacaoMaterialApreendido.container = $('.divContainer');

	FiscalizacaoMaterialApreendido.settings.mensagens = <%= Model.Mensagens %>;
	FiscalizacaoMaterialApreendido.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>

<div class="divContainer" >

    <input type="hidden" class="hdnMaterialApreendidoId" value="<%:Model.MaterialApreendido.Id %>" />

    <fieldset class="block box">
        <legend>Apreensão</legend>

        <div class="block">
            <div class="coluna20">
                <label>IUF para Apreensão</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 0, (Model.MaterialApreendido.IsDigital == null ? false : Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 1, (Model.MaterialApreendido.IsDigital == null ? false : !Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>

        <div class="block">
            <div class="coluna15">
		        <label>Número do IUF</label>
		        <%= Html.TextBox("MaterialApreendido.NumeroIUF", Model.MaterialApreendido.NumeroIUF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "8" }))%>
	        </div>

            <div class="coluna15">
				<label>Série</label><br />
				<%= Html.DropDownList("MaterialApreendido.Serie", Model.Series, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Series.Count <= 2, new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna15">
				<label>Data da lavratura do IUF</label>
				<%= Html.TextBox("MaterialApreendido.DataLavratura", Model.DataConclusaoFiscalizacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>

        <div class="block divPDF">
            <div class="coluna50 inputFileDiv">
				<label>PDF do IUF</label>
				<div class="block">
					<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.MaterialApreendido.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.MaterialApreendido.Arquivo.Nome)%></a>
				</div>
				<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
				<span class="spanInputFile <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "" : "hide" %>">
					<input type="file" id="file" class="inputFile" style="display: block; width: 100%" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %>/>
				</span>
			</div>
			<% if (!Model.IsVisualizar) { %>
			    <div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
				    <button type="button" class="inlineBotao btnAddArq <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
				    <button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.MaterialApreendido.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
			    </div>
			<% } %>
        </div>

        <div class="block">
			<div class="coluna75">
				<label>Descrever a apreensão *</label>
				<%= Html.TextArea("MaterialApreendido.Descricao", Model.MaterialApreendido.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricao", @maxlength = "250" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Valor dos bens apreendidos (R$ e por extenso)</label>
				<%= Html.TextBox("MaterialApreendido.ValorProdutos", Model.MaterialApreendido.ValorProdutos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimal txtValorProdutos", @maxlength = "15" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Número(s) do(s) Lacre(s) da Interdição/Embargo</label>
				<%= Html.TextBox("MaterialApreendido.NumeroLacre", Model.MaterialApreendido.NumeroLacre, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimal txtValorProdutos", @maxlength = "15" }))%>
			</div>
		</div>

    </fieldset>

    <fieldset class="block box">
		<legend>Depositário</legend>

		<input type="hidden" class="hdnDepositarioId" value="<%= Html.Encode(Model.MaterialApreendido.Depositario.Id) %>" />

		<div class="block" >
			<div class="coluna60">
				<label>Nome do Depositário *</label>
				<%= Html.TextBox("Depositario.NomeRazaoSocial", Model.MaterialApreendido.Depositario.NomeRazaoSocial, new { @class = "text disabled txtNome", @disabled = "disabled" })%>
			</div>

			<div class="coluna16 prepend2">
				<label>CPF *</label>
				<%= Html.TextBox("Depositario.CPFCNPJ", Model.MaterialApreendido.Depositario.CPFCNPJ, new { @class = "text disabled txtCnpj", @disabled = "disabled" })%>
			</div>

			<div class="prepend2">
				<% if (!Model.IsVisualizar) { %>
				<button type="button" title="Buscar depositario" class="floatLeft inlineBotao botaoBuscar btnAssociarDepositario">Buscar</button>
				<% } %>
				<span class="spanVisualizarDepositario <%= (Model.MaterialApreendido.Depositario.Id > 0) ? "" : "hide" %>"><button type="button" class="icone visualizar esquerda inlineBotao btnEditarDepositario" title="Visualizar depositario"></button></span>
			</div>
		</div>

		<div class="block" >
			<div class="coluna60">
				<label>Logradouro / Rua / Rodovia *</label>
				<%= Html.TextBox("Depositario.Logradouro", Model.MaterialApreendido.Depositario.Logradouro, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text txtLogradouro", @maxlength = "500" }))%>
			</div>
		</div>

		<div class="block" >
			<div class="coluna30 append2">
				<label>Bairro / Gleba / Comunidade *</label>
				<%= Html.TextBox("Depositario.Bairro", Model.MaterialApreendido.Depositario.Bairro, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text txtBairro", @maxlength = "100" }))%>
			</div>

			<div class="coluna27">
				<label>Distrito / Localidade *</label>
				<%= Html.TextBox("Depositario.Distrito", Model.MaterialApreendido.Depositario.Distrito, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDistrito", @maxlength = "100" }))%>
			</div>
		</div>

		<div class="block divEndereco" >
			<div class="coluna20 append2">
				<label>UF *</label><br />
				<%= Html.DropDownList("Depositario.Estado", Model.Ufs, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Ufs.Count <= 1, new { @class = "text ddlEstado" }))%>
			</div>
			<div class="coluna37">
				<label>Município *</label><br />
				<%= Html.DropDownList("Depositario.Municipio", Model.Municipios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Municipios.Count <= 1, new { @class = "text ddlMunicipio" }))%>
			</div>
		</div>

	</fieldset>

    <fieldset class="fsMateriais block box">
		<legend>Produtos Apreendidos / Destinação</legend>

		<%if (!Model.IsVisualizar){%>

			<div class="block" >
				<div class="coluna30">
					<label>Produtos Apreendidos</label><br />
					<%= Html.DropDownList("MaterialApreendido.ProdutosApreendidos", Model.ListaProdutosApreendidos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ListaProdutosApreendidos.Count <= 1, new { @class = "text ddlProdutosApreendidos" }))%>
				</div>

                <div class="coluna10">
                    <% foreach (var item in Model.produtosUnidades){ %>
                        <input type="hidden" class="text hdnUnidade<%:item.Id%>" value="<%: item.Unidade %>" />
                    <% } %>

                    <label>Unidade</label>
                    <input type="text" disabled ="disabled" maxlength="50" class="text txtUnidade disabled" />
                </div>

                <div class="coluna10">
                    <label>Quantidade</label>
				    <input type="text" maxlength="50" class="text txtQuantidade maskNum8" />
                </div>

                <div class="coluna30">
					<label>Destino</label><br />
					<%= Html.DropDownList("MaterialApreendido.Destinos", Model.ListaDestinos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ListaProdutosApreendidos.Count <= 1, new { @class = "text ddlDestinos" }))%>
				</div>
			</div>

		<%} %>

		<div class="block dataGrid divMateriais">
			<div class="coluna70 ">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="45%">Tipo de material apreendido</th>
							<th width="46%">Especificação</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var item in Model.MaterialApreendido.Materiais) { %>
					<tbody>
						<tr>
							<td>
								<span class="tipo" title="<%:item.TipoTexto%>"><%:item.TipoTexto%></span>
							</td>
							<td>
								<span class="especificacao" title="<%:item.Especificacao%>"><%:item.Especificacao%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMaterial" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="tipo"></span></td>
								<td><span class="especificacao"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirMaterial" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>

	</fieldset>

</div>