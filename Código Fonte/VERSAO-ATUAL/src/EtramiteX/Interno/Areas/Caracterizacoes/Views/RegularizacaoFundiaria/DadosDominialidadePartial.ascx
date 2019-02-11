<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<fieldset class="block box fsDominialidade filtroExpansivoAberto">
	<legend class="titFiltros">Dados da Dominialidade</legend>

	<div class="filtroCorpo">
		<!-- #region Dominio -->

		<div class="block">
			<div class="coluna21 append2 block ultima">
				<label for="Identificacao">Identificação</label>
				<%= Html.TextBox("Identificacao", Model.Caracterizacao.Posse.Identificacao, new { @class = "text txtIdentificacaoComprovacao disabled", @disabled = "disabled" })%>
				<input type="hidden" class="hdnIdentificacao" value='<%=Model.Caracterizacao.Posse.Identificacao%>' />
			</div>

			<div class="coluna21 append6">
				<label for="ZonaLocalizacao">Zona de localização</label>
				<%= Html.TextBox("ZonaLocalizacao", ((eZonaLocalizacao)Model.Caracterizacao.Posse.Zona).ToString(), new { @class = "text txtZonaLocalizacao disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna21 apprend1">
				<label for="Dominio_ComprovacaoId">Comprovação *</label>
				<%= Html.DropDownList("DominioComprovacaoId", Model.Caracterizacao.Posse.Comprovacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlComprovacao setarFoco"}))%>				
			</div>
		</div>

		<div class="block">
			<div class="coluna87">
				<label for="Descricao_Comprovacao">Descrição da Comprovação *</label>
				<%= Html.TextBox("Descricao_Comprovacao", Model.Caracterizacao.Posse.DescricaoComprovacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDescricaoComprovacao"})) %>
			</div>			
		</div>

		<div class="block" >
			<div class="coluna21 append1">
				<label for="Caracterizacao_Posse_AreaRequerida_">Área total requerida (m²) *</label>
				<%= Html.TextBox("Caracterizacao.Posse.AreaRequerida_", ((Model.Caracterizacao.Posse.AreaRequerida > 0)? Model.Caracterizacao.Posse.AreaRequerida.ToStringTrunc(format:true): null), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaRequerida maskDecimalPonto", @maxlength = "12" }))%>
			</div>

			<div class="coluna21 append6">
				<label for="AreaTotalPosse">Área total da posse (m²)</label>
				<%= Html.TextBox("AreaTotalPosse", Model.Caracterizacao.Posse.AreaCroqui.ToStringTrunc(2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaTotalPosse maskDecimalPonto", @maxlength = "24" }))%>
			</div>

			<%--<% if (!Model.IsVisualizar)
				{ %>
			<div class="coluna11">
				<button type="button" class="inlineBotao btnCarregarCroqui">Carregar</button>
			</div>
			<% } %>--%>

			<div class="coluna21">
				<label for="Perimetro">Perímetro (m)</label>
				<%= Html.TextBox("Perimetro", Model.Caracterizacao.Posse.Perimetro.ToStringTrunc(3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPerimetro maskDecimalPonto3", @maxlength = "25" }))%>
			</div>

			<%--<% if (!Model.IsVisualizar)
				{ %>
			<div class="coluna11">
				<button type="button" class="inlineBotao btnCarregarPerimetro">Carregar</button>
			</div>
			<% } %>--%>
		</div>

		<div class="block" >
			<div class="coluna43 append7">
				<label for="AreaPosseDocumento">Área posse Documento(m²) *</label>
				<%= Html.TextBox("AreaPosseDocumento", Model.Caracterizacao.Posse.AreaPosseDocumento.ToStringTrunc(3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaPosse maskDecimalPonto", @maxlength = "25" })) %>
			</div>
			<div class="coluna43 append2">
				<label for="Caracterizacao_Posse_RegularizacaoTipo">Tipo de regularização *</label>
				<%= Html.DropDownList("Caracterizacao_Posse_RegularizacaoTipo", Model.TipoRegularizacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlRegularizacaoTipo " }))%>
			</div>
		</div>		

		<div class="block">
			<div class="coluna21 append2 ">
				<label for ="NumeroCCIR">CCIR N°</label>
				<%= Html.TextBox("NumeroCCIR", Model.Caracterizacao.Posse.NumeroCCIR, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroCCIR"})) %>
			</div>
			<div class="coluna21 append5">
				<label for ="AreaCCIR">Área no CCIR(m²)</label>
				<%= Html.TextBox("AreaCCIR", Model.Caracterizacao.Posse.AreaCCIR.ToStringTrunc(2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCCIR maskDecimalPonto", @maxlength = "25"})) %>
			</div>
			<div class="coluna21 DataUltimaAtualizacaoCCIR">
				<label for ="DataUltimaAtualizacaoCCIR">Data da ultima atualização do CCIR</label>
				<%= Html.TextBox("DataUltimaAtulizacaoCCIR", Model.Caracterizacao.Posse.DataUltimaAtualizacaoCCIR.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataUltimaAtualizacaoCCIR "})) %>
			</div>
		</div>
		<!-- #endregion -->

		<fieldset class="boxBranca block" id="fsAreaAnexaPosse">
			<legend>Área titulada anexa à posse</legend>
			
			<!-- #region Dominio Avulso -->

			<div class="block possuiDominioAvulso">
				<div><label for="PossuiDominioAvulso">Adicionar área titulada anexa à posse? *</label></div>
				<div class="coluna26">
					<label ><%= Html.RadioButton("PossuiDominioAvulso", ConfiguracaoSistema.SIM, Model.Caracterizacao.Posse.PossuiDominioAvulso == ConfiguracaoSistema.SIM, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio radioPossuiDominioAvulso" }))%>Sim</label>
					<label ><%= Html.RadioButton("PossuiDominioAvulso", ConfiguracaoSistema.NAO, Model.Caracterizacao.Posse.PossuiDominioAvulso == ConfiguracaoSistema.NAO, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio radioPossuiDominioAvulso" }))%>Não</label>
				</div>
			</div>

			<div class="dominioAvulso hide">
				<input type="hidden" class="hdnDominioID campoMatricula" value="0" />
				<div class="block">
					<div class="coluna22 append2">
						<label for="Matricula">Matrícula Nº *</label>
						<input type="text" id="Matricula" name="Matricula" class="text txtMatricula campoMatricula" maxlength="24" />
					</div>
					<div class="coluna22 append2">
						<label for="Folha">Folha Nº *</label>
						<input type="text" id="Folha" name="Folha" class="text txtFolha campoMatricula" maxlength="24" />
					</div>
					<div class="coluna22 append2">
						<label for="Livro">Livro Nº *</label>
						<input type="text" id="Livro" name="Livro" class="text txtLivro campoMatricula" maxlength="24" />
					</div>
					<div class="coluna22">
						<label for="AreaDocumento">Área Documento (m²) *</label>
						<input type="text" id="AreaDocumento" name="AreaDocumento" class="text maskDecimalPonto txtAreaDocumento campoMatricula" maxlength="20" />
					</div>
				</div>

				<div class="block">
					<div class="coluna98">
						<label for="Cartorio">Cartório *</label>
						<input type="text" id="Cartorio" name="Cartorio" class="text txtCartorio campoMatricula" maxlength="150" />
					</div>
				</div>

				<div class="block">
					<div class="coluna22 append2">
						<label for="NumeroCCIR">CCIR Nº</label>
						<input type="text" id="NumeroCCIR" name="NumeroCCIR" class="text maskNumInt txtNumeroCCIR campoMatricula" maxlength="13" />
					</div>
					<div class="coluna22 append2">
						<label for="AreaCCIR">Área no CCIR (m²)</label>
						<input type="text" id="AreaCCIR" name="AreaCCIR" class="text maskDecimalPonto txtAreaCCIR campoMatricula" maxlength="20" />
					</div>
					<div class="coluna22 append2">
						<label for="DataUltimaAtualizacao_DataTexto">Data última atualização</label>
						<input type="text" id="DataUltimaAtualizacao_DataTexo" name="DataUltimaAtualizacao_DataTexo" class="text maskData txtDataUltimaAtualizacao campoMatricula" />
					</div>
					<div class="ultima">
						<button class="inlineBotao botaoAdicionarIcone btnAdicionarDominioAvulso">Adicionar</button>
						<button type="button" class="inlineBotao btnLimparDominioAvulso hide" title="Limpar" ><span>Limpar</span></button>
					</div>
				</div>
			</div>

			<!-- #endregion -->
			<br />

			<!-- #region Grid Área titulada anexada -->

			<% bool possuiDominioAvulso = (Model.Caracterizacao.Matriculas.Count > 0 || Model.Caracterizacao.Posse.PossuiDominioAvulsoBool); %>
			<input type="hidden" class="hdnQuantidadeMatriculaGeo" value="<%= Model.Caracterizacao.Matriculas.Count %>" />

			<div class="block box divNaoPossuiDominioAvulso <%= possuiDominioAvulso ? "hide": "" %>" >
				<label>Não existe área titulada anexa à posse</label>
			</div>
			<div class="block dataGrid <%= possuiDominioAvulso ? "": "hide" %>">
				<table class="dataGridTable tabAreasAnexadas" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="16%">Identificação</th>
							<th >Matricula - Folha - Livro</th>
							<th width="20%">Área Croqui (m²) </th>
							<th width="20%">Área Documental (m²) </th>
							<th width="14%">Ação</th>
						</tr>
					</thead>
					<tbody>
						<% foreach (var anexada in Model.Caracterizacao.Matriculas)
							{ %>
							<tr>
								<td><span class="identificacao"><%: anexada.Identificacao %></span></td>
								<td><span class="matriculaFolhaLivro"> <%: anexada.Matricula %> - <%: anexada.Folha %> - <%: anexada.Livro %></span></td>
								<td><span class="areaCroqui" ><%: anexada.AreaCroqui.ToStringTrunc() %></span></td>
								<td><span class="areaDocumental" ><%: anexada.AreaDocumentoTexto %></span></td>

								<td class="tdAcoes">
									<input type="hidden" class="hdnDominioJSON" value='<%: ViewModelHelper.Json(anexada) %>' /> 
									<input title="Visualizar" type="button" class="icone visualizar btnDominioVisualizar" value="Visualizar" />
								</td>
							</tr>
						<% } %>
						<% foreach (var anexada in Model.Caracterizacao.Posse.DominiosAvulsos)
							{ %>
							<tr>
								<td><span class="identificacao"><%: anexada.Identificacao %></span></td>
								<td><span class="matriculaFolhaLivro"><%: anexada.Matricula %> - <%: anexada.Folha %> - <%: anexada.Livro %></span></td>
								<td><span class="areaCroqui"></span></td>
								<td><span class="areaDocumental"><%: anexada.AreaDocumento.ToStringTrunc() %></span></td>

								<td class="tdAcoes">
									<input type="hidden" class="hdnDominioJSON" value='<%: ViewModelHelper.Json(anexada) %>' />
									<input title="Visualizar" type="button" class="icone visualizar btnDominioVisualizar" value="Visualizar" />
									<% if (!Model.IsVisualizar)
										{ %>
									<input title="Editar" type="button" class="icone editar btnDominioEditar" value="Editar" />
									<input title="Excluir" type="button" class="icone excluir btnDominioExcluir" value="Excluir" />
									<% } %>
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
			<!-- #endregion -->
		</fieldset>
	</div>
</fieldset>

<table class="tabAreasAnexadasTemplate hide">
	<tr>
		<td><span class="identificacao"></span></td>
		<td><span class="matriculaFolhaLivro"></span></td>
		<td><span class="areaCroqui"></span></td>
		<td><span class="areaDocumental"></span></td>

		<td class="tdAcoes">
			<input type="hidden" class="hdnDominioJSON" />
			<input title="Visualizar" type="button" class="icone visualizar btnDominioVisualizar" value="Visualizar" />
			<input title="Editar" type="button" class="icone editar btnDominioEditar" value="Editar" />
			<input title="Excluir" type="button" class="icone excluir btnDominioExcluir" value="Excluir" />
		</td>
	</tr>
</table>