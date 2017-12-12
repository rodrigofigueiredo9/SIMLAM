<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
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
                <label>IUF para apreensão *</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 0, (Model.MaterialApreendido.IsDigital == null ? false : Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsDigital" }))%>Digital</label><br />
		        <label><%= Html.RadioButton("MaterialApreendido.IsDigital", 1, (Model.MaterialApreendido.IsDigital == null ? false : !Model.MaterialApreendido.IsDigital.Value), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdoIsBloco" }))%>Bloco</label>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box fsCorpo">
        <div class="block">
            <div class="coluna15">
		        <label>Número do IUF *</label>
		        <%= Html.TextBox("MaterialApreendido.NumeroIUF", ((Model.MaterialApreendido.IsDigital == true && Model.MaterialApreendido.NumeroIUF == null) ? "Gerado automaticamente" : Model.MaterialApreendido.NumeroIUF), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.MaterialApreendido.IsDigital == true), new { @class = "text maskNumInt txtNumeroIUF", @maxlength = "6" }))%>
	        </div>

            <div class="coluna15">
				<label>Série *</label><br />
				<%= Html.DropDownList("MaterialApreendido.Serie", Model.Series, ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.Series.Count <= 2 || Model.MaterialApreendido.IsDigital == true), new { @class = "text ddlSeries" }))%>
			</div>

            <div class="coluna15">
				<label>Data da lavratura do IUF *</label>
				<%= Html.TextBox("MaterialApreendido.DataLavratura", (Model.MaterialApreendido.DataLavratura.Data != DateTime.MinValue ? Model.MaterialApreendido.DataLavratura.DataTexto : "Gerado automaticamente"), ViewModelHelper.SetaDisabled((Model.IsVisualizar || Model.MaterialApreendido.IsDigital == true), new { @class = "text maskData txtDataLavratura" }))%>
			</div>
        </div>

        <div class="block">
			<div class="coluna75">
				<label>Descrever a apreensão *</label>
				<%= Html.TextArea("MaterialApreendido.Descricao", Model.MaterialApreendido.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDescricao", @maxlength = "250" }))%>
			</div>
		</div>

        <div class="block">
            <div class="coluna25">
				<label>Valor dos bens apreendidos (R$)</label>
				<%= Html.TextBox("MaterialApreendido.ValorProdutosReais", Model.MaterialApreendido.ValorProdutosReaisMascara, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto txtValorBensApreendidosReais", @maxlength = "13" }))%>
			</div>
		</div>

        <div class="block">
			<div class="coluna75">
				<label>Número(s) do(s) Lacre(s) da Apreensão</label>
				<%= Html.TextBox("MaterialApreendido.NumeroLacre", Model.MaterialApreendido.NumeroLacre, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroLacre", @maxlength = "100" }))%>
			</div>
		</div>

    </fieldset>

    <fieldset class="block box fsCorpo">
		<legend>Depositário</legend>

		<input type="hidden" class="hdnDepositarioId" value="<%= Html.Encode(Model.MaterialApreendido.Depositario.Id) %>" />

		<div class="block" >
			<div class="coluna60">
				<label>Nome do Depositário</label>
				<%= Html.TextBox("Depositario.NomeRazaoSocial", Model.MaterialApreendido.Depositario.NomeRazaoSocial, new { @class = "text disabled txtNome", @disabled = "disabled" })%>
			</div>

			<div class="coluna16 prepend2">
				<label>CPF</label>
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
				<label>Logradouro / Rua / Rodovia</label>
				<%= Html.TextBox("Depositario.Logradouro", Model.MaterialApreendido.Depositario.Logradouro, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text txtLogradouro", @maxlength = "500" }))%>
			</div>
		</div>

		<div class="block" >
			<div class="coluna30 append2">
				<label>Bairro / Gleba / Comunidade</label>
				<%= Html.TextBox("Depositario.Bairro", Model.MaterialApreendido.Depositario.Bairro, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text txtBairro", @maxlength = "100" }))%>
			</div>

			<div class="coluna27">
				<label>Distrito / Localidade</label>
				<%= Html.TextBox("Depositario.Distrito", Model.MaterialApreendido.Depositario.Distrito, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDistrito", @maxlength = "100" }))%>
			</div>
		</div>

		<div class="block divEndereco" >
			<div class="coluna20 append2">
				<label>UF</label><br />
				<%= Html.DropDownList("Depositario.Estado", Model.Ufs, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Ufs.Count <= 1, new { @class = "text ddlEstado" }))%>
			</div>
			<div class="coluna37">
				<label>Município</label><br />
				<%= Html.DropDownList("Depositario.Municipio", Model.Municipios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Municipios.Count <= 1, new { @class = "text ddlMunicipio" }))%>
			</div>
		</div>

	</fieldset>

    <fieldset class="fsProdutosApreendidos block box fsCorpo">
		<legend>Produtos Apreendidos / Destinação</legend>

		<%if (!Model.IsVisualizar){%>

			<div class="block">
				<div class="coluna30">
					<label>Produtos Apreendidos *</label><br />
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
				    <input type="text" id="MaterialApreendido_ProdutosApreendidos" maxlength="13" id="MaterialApreendido_Quantidade" class="text txtQuantidade maskDecimalPonto" />
                </div>

                <div class="coluna30">
					<label>Destino</label><br />
					<%= Html.DropDownList("MaterialApreendido.ProdutosApreendidos", Model.ListaDestinos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ListaProdutosApreendidos.Count <= 1, new { @class = "text ddlDestinos" }))%>
				</div>

                <div class="coluna10">
					<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarProdutoApreendido btnAddItem" title="Adicionar">+</button>
				</div>
			</div>

		<%} %>

		<div class="block dataGrid divProdutosApreendidos">
			<div class="coluna90">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="5%">Item</th>
							<th>Produtos Apreendidos</th>
                            <th width="10%">Unidade</th>
                            <th width="10%">Quantidade</th>
                            <th width="25%">Destino</th>
							<%if (!Model.IsVisualizar){%><th width="15%">Ação</th><%} %>
						</tr>
					</thead>
					<tbody>
                        <% int cont = 0; %>
					    <% foreach (var prod in Model.MaterialApreendido.ProdutosApreendidos) { %>
						    <tr>
                                <td>
                                    <span class="item" title="<%:cont++%>"><%:cont%></span>
                                </td>
                                <td>
                                    <span class="produto" title="<%:prod.ProdutoTexto%>"><%:prod.ProdutoTexto%></span>
                                </td>
                                <td>
                                    <span class="unidade" title="<%:prod.UnidadeTexto%>"><%:prod.UnidadeTexto%></span>
                                </td>
                                <td>
                                    <span class="quantidade" title="<%:prod.Quantidade%>"><%:String.Format("{0:##,###,##0.00}", prod.Quantidade)%></span>
                                </td>
                                <td>
                                    <span class="produto" title="<%:prod.DestinoTexto%>"><%:prod.DestinoTexto%></span>
                                </td>
							    <%if (!Model.IsVisualizar){%>
								    <td class="tdAcoes">
									    <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(prod)%>' />
									    <input title="Excluir" type="button" class="icone excluir btnExcluirProdutoApreendido" value="" />
								    </td>
							    <%} %>
						    </tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="item"></span></td>
								<td><span class="produto"></span></td>
                                <td><span class="unidade"></span></td>
                                <td><span class="quantidade"></span></td>
                                <td><span class="destino"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirProdutoApreendido" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
    
    <fieldset class="fsCorpo">
        <div class="block box">
		    <div class="coluna76">
			    <label>
				    Descrever ou opinar quanto a destinação do material apreendido: *
			    </label>
			    <%= Html.TextArea("MaterialApreendido.Opiniao", Model.MaterialApreendido.Opiniao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media  txtOpiniao", @maxlength = "250" }))%>
		    </div>
	    </div>
    </fieldset>

</div>