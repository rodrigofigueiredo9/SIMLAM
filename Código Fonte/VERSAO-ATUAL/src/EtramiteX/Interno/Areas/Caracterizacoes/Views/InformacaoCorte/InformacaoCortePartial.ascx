<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="containerDesenhador">
	<%= Html.Hidden("EmpreendimentoId", Model.Empreendimento.EmpreendimentoId, new { @class = "hdnEmpreendimentoId" }) %>
	<%= Html.Hidden("ProjetoDigitalId", Request.Params["ProjetoDigitalId"], new { @class="hdnProjetoDigitalId" })%>
    <fieldset class="block box">
        <legend>Empreendimento</legend>
        <div class="block">
            <div class="coluna20 append1">
                <label>Código</label>
                <%= Html.TextBox("EmpreendimentoCodigo", Model.Empreendimento.EmpreendimentoCodigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
            </div>
            <div class="coluna54 append1">
                <label>Denominação</label>
                <%= Html.TextBox("DenominadorValor", Model.Empreendimento.DenominadorValor, new { @maxlength = "100", @class = "text denominador disabled", @disabled = "disabled" })%>
            </div>
            <div class="coluna20">
                <label>Área do imóvel (ha)</label>
                <%= Html.TextBox("AreaImovel", Model.Empreendimento.AreaImovel.ToStringTrunc(), ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Empreendimento.AreaImovel > 0, new { @maxlength = "100", @class = "text areaImovel"}))%>
            </div>
        </div>

        <div class="block">
            <div class="coluna20 append1">
                <label>Zona de localização *</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoZonaLocalizacao, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna7 append1">
                <label>UF</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoUf, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna45 append1">
                <label>Município</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoMunicipio, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna20 ">
                <label>Área de Floresta Plantada (ha) *</label>
                <%= Html.TextBox("InformacaoCorte_AreaPlantada", Model.AreaPlantada.ToStringTrunc(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto areaPlantada", @maxlength = "12" }))%>
            </div>
        </div>
    </fieldset>

    <fieldset class="block box">
        <legend>Licença de Silvicultura</legend>

		<%if(!Model.IsVisualizar) { %>
			<div class="block">
				<div class="coluna10">
					<label>N.º Licença *</label>
					<%= Html.TextBox("InformacaoCorte_NumeroLicenca", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text numeroLicenca", @maxlength = "30"}))%>
				</div>
				<div class="coluna12">
					<label>Tipo de Licença *</label>
					<%= Html.DropDownList("InformacaoCorte_TipoLicenca", Model.TipoLicenca, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text tipoLicenca"}))%>
				</div>
				<div class="coluna17">
					<label>Atividade *</label>
					<%= Html.TextBox("InformacaoCorte_Atividade", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text atividade", @maxlength = "250"}))%>
				</div>
				<div class="coluna18">
					<label>Área Licenciada / Plantada (ha) *</label>
					<%= Html.TextBox("InformacaoCorte_AreaLicenciada", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto areaLicenciada", @maxlength = "12"}))%>
				</div>
				<div class="coluna10">
					<label>Data Vencimento *</label>
					<%= Html.TextBox("InformacaoCorte_DataVencimento", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData dataVencimento"}))%>
				</div>
				<div class="coluna10">
					<br />
					<input class="icone adicionar btnAdicionar" type="button" />
				</div>
			</div>
		<%} %>

        <div class="dataGrid">
            <table class="tabLicencas dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                        <th width="9%">Nº Linceça</th>
                        <th width="9%">Tipo de Licença</th>
                        <th width="9%">Atividade</th>
                        <th width="19%">Área Licenciada/Plantada (ha)</th>
                        <th width="12%">Vencimento</th>
						<% if(!Model.IsVisualizar) { %>
							<th class="semOrdenacao" width="9%">Ações</th>
						<%} %>
                    </tr>
                </thead>

                <tbody>
                    <% foreach (var item in Model.InformacaoCorteLicencaList)
						{ %>
                    <tr>
                        <td>
                            <span class="numero" title="<%= Html.Encode(item.NumeroLicenca)%>"><%= Html.Encode(item.NumeroLicenca)%></span>
                        </td>
						<td>
                            <span class="tipoLicenca" title="<%= Html.Encode(item.TipoLicenca)%>"><%= Html.Encode(item.TipoLicenca)%></span>
                        </td>
                        <td>
                            <span class="atividade" title="<%= Html.Encode(item.Atividade)%>"><%= Html.Encode(item.Atividade)%></span>
                        </td>
                        <td>
                            <span class="areaPlantada" title="<%= Html.Encode(item.AreaLicenca.ToStringTrunc(2))%>"><%= Html.Encode(item.AreaLicenca.ToStringTrunc())%></span>
                        </td>
                        <td>
                            <span class="dataVencimento" title="<%= Html.Encode(item.DataVencimento.DataTexto)%>"><%= Html.Encode(item.DataVencimento.DataTexto)%></span>
                        </td>
						<% if(!Model.IsVisualizar) { %>
							<td class="tdAcoes">
								<input type="hidden" class="itemJson" value='<%= ViewModelHelper.Json(item) %>' />
								<%if (Model.IsPodeExcluir) {%><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
							</td>
						<%} %>
                    </tr>
                    <% } %>
                    <tr class="trTemplateRow hide">
                        <td><span class="numero" title=""></span></td>
                        <td><span class="tipoLicenca" title=""></span></td>
                        <td><span class="atividade" title=""></span></td>
                        <td><span class="areaPlantada" title=""></span></td>
                        <td><span class="dataVencimento" title=""></span></td>
                        <td class="tdAcoes">
                            <input type="hidden" value="" class="itemJson" />
                            <input type="button" title="Excluir" class="icone excluir btnExcluir" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <label>* A partir de 100ha de área plantada é obrigatória a indicação de licença ambiental.</label>
        </div>

    </fieldset>

    <fieldset class="block box">
        <legend>Informação de Corte</legend>
	    <div class="block">
            <div class="coluna20 append1">
                <label>Código</label>
                <%= Html.TextBox("codigo", Model.Codigo > 0 ? Model.Codigo.ToString() : "Preenchido automaticamente", ViewModelHelper.SetaDisabled(true, new { @class = "text codigo"}))%>
                <%= Html.Hidden("codigoInformacaoCorte", Model.Id.ToString(), ViewModelHelper.SetaDisabled(true, new { @class = "text codigoInformacaoCorte"}))%>
            </div>

            <div class="coluna20">
                <label>Data da Informação *</label>
                <%= Html.TextBox("dataInformacao", Model.DataInformacao.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text maskData dataInformacao"}))%>
            </div>
        </div>
		
        <fieldset class="block boxBranca">
			<% if(!Model.IsVisualizar) { %>
				<div class="block">
					<div class="coluna20 append1">
						<label>Tipo de Corte *</label>
						<%= Html.DropDownList("InformacaoCorte_TipoCorte", Model.InformacaoCorteTipo.TipoCorte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text tipoCorte"}))%>
					</div>
					<div class="coluna20 append1">
						<label>Espécie Informada *</label>
						<%= Html.DropDownList("InformacaoCorte_EspecieInformada", Model.InformacaoCorteTipo.Especie, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text especieInformada"}))%>
					</div>
					<div class="coluna20 divAreaCorte">
						<label for="InformacaoCorte_AreaCorte">Área Corte(ha) *</label>
						<%= Html.TextBox("InformacaoCorte_AreaCorte", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskDecimalPonto areaCorte", @maxlength = "12"}))%>
					</div>
					<div class="coluna20 divNumArvores hide">
						<label for="InformacaoCorte_AreaCorte">Nº Árvores *</label>
						<%= Html.TextBox("InformacaoCorte_AreaCorte", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskInteger areaCorte", @maxlength = "9"}))%>
					</div>
					<div class="coluna15 append2">
						<label>Idade Plantio (anos) *</label>
						<%= Html.TextBox("InformacaoCorte_IdadePlantio", string.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskInteger idadePlantio", @maxlength = "3"}))%>
					</div>

					<div class="coluna2 adicionarTipo">
						<br />
						<input class="icone adicionar btnAdicionarTipo" type="button" />
					</div>
					<div class="coluna5 limparTipo hide">
						<br />
						<input class="btnLimparTipo" type="button"  value="Limpar" />
					</div>
				</div>
				<div class="block">

					<fieldset class="block box">
						<div class="coluna20 append1">
							<label>Destinação do Material *</label>
							<%= Html.DropDownList("InformacaoCorte_DestinacaoMaterial", Model.InformacaoCorteDestinacao.DestinacaoMaterial, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlDestinacaoMaterial"}))%>
						</div>
						<div class="coluna20 append1">
							<label>Produto *</label>
							<%= Html.DropDownList("InformacaoCorte_Produto", Model.InformacaoCorteDestinacao.Produto, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlProduto"}))%>
						</div>
						<div class="coluna20 append2">
							<label>Quantidade *</label>
							<%= Html.TextBox("InformacaoCorte_Quantidade", string.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text maskInteger txtQuantidade", @maxlength = "12"}))%>
						</div>
						<div class="coluna10">
							<br />
							<input class="icone adicionar btnAdicionarDestinacao" type="button" disabled="disabled"  />
						</div>

						<div class="dataGrid">
							<table class="tabDestinacao dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
								<thead>
									<tr>
										<th width="9%">Destinação Material</th>
										<th width="9%">Produto</th>
										<th width="19%">Quantidade</th>
										<th class="semOrdenacao" width="9%">Ações</th>
									</tr>
								</thead>
								<tbody>
									<tr class="trTemplateRow hide">
										<td><span class="destinacaoMaterial" title=""></span></td>
										<td><span class="produto" title=""></span></td>
										<td><span class="quantidade" title=""></span></td>
										<td class="tdAcoes">
											<input type="hidden" value="" class="itemJson" />
											<input type="button" title="Excluir" class="icone excluir btnExcluirDestinacao" />
										</td>
									</tr>
								</tbody>
							</table>
							<br />
							<input class="btnAdicionarInformacao" type="button" title="OK" value="OK" disabled="disabled" />
						</div>
					</fieldset>
				</div>
			<%} %>

            <div class="dataGrid">
                <table class="tabInformacaoCorte dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
                    <thead>
                        <tr>
                            <th width="9%">Tipo de Corte</th>
                            <th width="9%">Espécie</th>
                            <th width="19%">Área Corte(ha) / Nº Árvores</th>
                            <th width="9%">Idade Plantio (anos)</th>
                            <th width="9%">Destinação Material</th>
                            <th width="9%">Produto</th>
                            <th width="9%">Qtde</th>
							<% if (!Model.IsVisualizar) { %>
								<th class="semOrdenacao" width="9%">Ações</th>
							<%} %>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (var item in Model.InformacaoCorteResultados) { %>
                        <tr>
							<% if (item.Linhas > 0) {%>
                            <td rowspan="<%= item.Linhas %>">
                                <span class="tipoCorte" title="<%= Html.Encode(item.TipoCorteTexto)%>"><%= Html.Encode(item.TipoCorteTexto)%></span>
                            </td>
                            <td rowspan="<%= item.Linhas %>">
                                <span class="especie" title="<%= Html.Encode(item.EspecieTexto)%>"><%= Html.Encode(item.EspecieTexto)%></span>
                            </td>
                            <td rowspan="<%= item.Linhas %>">
                                <span class="areaCorte" title="<%= Html.Encode(item.AreaCorte.ToStringTrunc(2))%>"><%= Html.Encode(String.Concat(item.AreaCorte.ToStringTrunc(), item.TipoCorte == (int)Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.eTipoCorte.CorteSeletivo ? " un" : " ha"))%></span>
                            </td>
                            <td rowspan="<%= item.Linhas %>">
                                <span class="idadePlantio" title="<%= Html.Encode(item.IdadePlantio)%>"><%= Html.Encode(item.IdadePlantio)%></span>
                            </td>
							<% }%>
                            <td>
                                <span class="destinacaoMaterial" title="<%= Html.Encode(item.DestinacaoMaterialTexto)%>"><%= Html.Encode(item.DestinacaoMaterialTexto)%></span>
                            </td>
                            <td>
                                <span class="produto" title="<%= Html.Encode(item.ProdutoTexto)%>"><%= Html.Encode(item.ProdutoTexto)%></span>
                            </td>
                            <td>
                                <span class="quantidade" title="<%= Html.Encode(item.Quantidade.ToString("N0"))%>"><%= Html.Encode(item.Quantidade.ToString("N0"))%></span>
                            </td>
							<% if(!Model.IsVisualizar) { %>
								<td class="tdAcoes">
									<input type="hidden" class="itemJson" value='<%= ViewModelHelper.Json(item) %>' />
									<%if (Model.IsPodeExcluir)
										{%><input type="button" title="Excluir" class="icone excluir btnExcluirInformacao" /><% } %>
								</td>
							<%} %>
                        </tr>
                        <% } %>
                        <tr class="trTemplateRow hide">
                            <td><span class="tipoCorte" title=""></span></td>
                            <td><span class="especie" title=""></span></td>
                            <td><span class="areaCorte" title=""></span></td>
                            <td><span class="idadePlantio" title=""></span></td>
                            <td><span class="destinacaoMaterial" title=""></span></td>
                            <td><span class="produto" title=""></span></td>
                            <td><span class="quantidade" title=""></span></td>
                            <td class="tdAcoes">
                                <input type="hidden" value="" class="itemJson" />
                                <input type="button" title="Excluir" class="icone excluir btnExcluirInformacao" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </fieldset>
    </fieldset>

    <% if(!Model.IsVisualizar) { %>
		<fieldset class="block box">
			<legend>Declarações (aceitas)</legend>
			<div class="block">
				<div class="coluna60">
					<%=Html.CheckBox("InformacaoCorte.DeclaracaoVerdadeira", Model.Id > 0, new { @class = "ckbDeclaracaoVerdadeira" })%>
					<label for="InformacaoCorte_DeclaracaoVerdadeira">Declaro que as informações prestadas são verdadeiras, assumindo inteira responsabilidade pelas mesmas</label>
				</div>
				<div class="coluna35">
					<%=Html.CheckBox("InformacaoCorte.ResponsavelPelasDeclaracoes", Model.Id > 0, new {  @class = "ckbResponsabilidadePelasDeclaracoes" })%>
					<label for="InformacaoCorte_ResponsavelPelasDeclaracoes">As informações aqui presentes são de minha responsabilidade</label>
				</div>
			</div>
		</fieldset>
	<%} %>
</div>
