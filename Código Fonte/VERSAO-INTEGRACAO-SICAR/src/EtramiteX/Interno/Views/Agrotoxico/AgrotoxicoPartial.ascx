<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AgrotoxicoVM>" %>

<fieldset class="block box">
	<%= Html.Hidden("Agrotoxico.Id", Model.Agrotoxico.Id, new { @class = "hdnAgrotoxicoId" })%>
	<%= Html.Hidden("OrdenarPor", "1", new { @class = "ordenarPor" })%>
	<div class="block">
		<div class="coluna20">
			<label for="Possui_Numero_Cadastro">Já possui nº de cadastro?</label><br />
			<label><%= Html.RadioButton("Agrotoxico.PossuiCadastro", 1, Model.Agrotoxico.PossuiCadastro, ViewModelHelper.SetaDisabled(Model.Agrotoxico.Id > 0, new { @class = "radio RadioPossuiNumCadastro rbPossuiNumCadastroSim" }))%>Sim</label>
			<label><%= Html.RadioButton("Agrotoxico.PossuiCadastro", 0, !Model.Agrotoxico.PossuiCadastro, ViewModelHelper.SetaDisabled(Model.Agrotoxico.Id > 0, new { @class = "radio RadioPossuiNumCadastro rbPossuiNumCadastroNao" }))%>Não</label>
		</div>

		<div class="coluna40 prepend2">
			<label for="Numero_Cadastro">Número do cadastro *</label>
			<%= Html.TextBox("Agrotoxico.NumeroCadastro", (Model.Agrotoxico.NumeroCadastro > 0 ? Model.Agrotoxico.NumeroCadastro.ToString() : "Gerado automaticamente"), ViewModelHelper.SetaDisabled(Model.Agrotoxico.NumeroCadastro <= 0, new { @class = "text txtNumeroCadastro maskNumInt", @maxlength = "5" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna20">
			<label for="Agrotoxico_CadastroAtivo">O cadastro do agrotóxico está ativo?</label><br />
			<label><%= Html.RadioButton("Agrotoxico.CadastroAtivo", 1, Model.Agrotoxico.CadastroAtivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioCadastroAtivo rbCadastroAtivoSim" }))%>Sim</label>
			<label><%= Html.RadioButton("Agrotoxico.CadastroAtivo", 0, !Model.Agrotoxico.CadastroAtivo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio RadioCadastroAtivo rbCadastroAtivoNao" }))%>Não</label>
		</div>

		<div class="coluna40 prepend2 divMotivo <%=Model.Agrotoxico.CadastroAtivo ? "hide" : "" %>">
			<label for="Agrotoxico_MotivoTexto">Motivo *</label>
			<%= Html.TextBox("Agrotoxico.MotivoTexto", Model.Agrotoxico.MotivoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtMotivo disabled", @maxlength = "80"}))%>
			<%= Html.Hidden("Agrotoxico.MotivoId", Model.Agrotoxico.MotivoId, new { @class = "hdnMotivoId" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna62">
			<label for="Nome_Comercial">Nome comercial *</label>
			<%= Html.TextBox("Agrotoxico.NomeComercial", Model.Agrotoxico.NomeComercial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeComercial", @maxlength = "80" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna40">
			<label for="Numero_Registro_Ministerio">Nº do registro do ministério *</label>
			<%= Html.TextBox("Agrotoxico.NumeroRegistroMinisterio", Model.Agrotoxico.NumeroRegistroMinisterio < 1?"":Model.Agrotoxico.NumeroRegistroMinisterio.ToString(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroRegistroMinisterio maskNum15"}))%>
		</div>

		<div class="coluna19 prepend2">
			<label for="Numero_Processo_Sep">Nº do processo SEP *</label>
			<%= Html.TextBox("Agrotoxico.NumeroProcessoSEP", Model.Agrotoxico.NumeroProcessoSep < 1?"":Model.Agrotoxico.NumeroProcessoSep.ToString(), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNumeroProcessoSep maskNum15" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna62">
			<label for="Titular_Registro">Titular do registro *</label>
			<%= Html.TextBox("Agrotoxico.TitularRegistro.NomeRazaoSocial", Model.Agrotoxico.TitularRegistro.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTitularRegistroNome", @maxlength = "80" }))%>
			<%=Html.Hidden("TitularRegistroId", Model.Agrotoxico.TitularRegistro.Id, new { @class="hdnTitularRegistroId"})%>
		</div>

		<%if(!Model.IsVisualizar){ %>
		<div class="coluna20 prepend1">
			<button class="inlineBotao btnBuscarTitularRegistro" type="button" value="Buscar"><span>Buscar</span></button>
		</div>
		<%} %>
	</div>
</fieldset>

<fieldset class="block box" id="IngredienteAtivo_Container">
	<legend>Ingrediente Ativo</legend>

	<%if(!Model.IsVisualizar) { %>
	<div class="ingredienteAtivo">
		<div class="block">
			<div class="coluna62">
				<label for="Ingrediente_Ativo">Ingrediente Ativo *</label>
				<%= Html.TextBox("Agrotoxico.IngredienteAtivo.IngredienteAtivo", "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtIngredienteAtivo"}))%>
				<%=Html.Hidden("IngredienteAtivoJson", "", new { @class="hdnIngredienteAtivoJson"})%>
			</div>

			<div class="coluna20 prepend1">
				<button class="inlineBotao btnBuscarIngredienteAtivo" type="button" value="Buscar"><span>Buscar</span></button>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label for="Agrotoxico_IngredienteAtivo_Concentracao">Concentração</label>
				<%= Html.TextBox("Agrotoxico.IngredienteAtivo.Concentracao", "",  new { @class = "text txtConcentracao maskDecimalPonto4", @maxlength="13"})%>
			</div>
			
			<div class="coluna19 prepend2">
				<label for="Unidade_Medida">Unidade de medida</label>
				<%=Html.DropDownList("Unidade_Medida", Model.IngredienteAtivoUnidadeMedidaLst, new { @class="text ddlIngredienteAtivoUnidadeMedida" })%>
			</div>

			<div class="coluna19 prepend2 hide divUnidadeMedidaOutro">
				<label for="Unidade_Medida_Outro">Texto</label>
				<%= Html.TextBox("Unidade_Medida_Outro", null, new { @class = "text txtUnidadeMedidaOutro", @maxlength="80"})%>
			</div>

			<div class="coluna20 prepend1">
				<button class="inlineBotao btnAddIngredienteAtivo" type="button" value="Adicionar"><span>Adicionar</span></button>
			</div>
		</div>
	</div>
	<%} %>

	<div class="block">
		<table class="dataGridTable gridIngredientesAtivos" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Ingrediente Ativo</th>
					<th width="40%">Concentração</th>
					<th width="15%">Situação</th>
					<%if (!Model.IsVisualizar) {%><th class="semOrdenacao" width="5%">Ações</th><%} %>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.Agrotoxico.IngredientesAtivos) { %>
				<% string unidadeMedida = (string.IsNullOrEmpty(item.UnidadeMedidaOutro) ? item.UnidadeMedidaTexto : item.UnidadeMedidaOutro); %>
				<tr>
					<td>
						<label class="lblNome" title="<%=item.Texto %>"><%=item.Texto%> </label>
					</td>
					<td>
						<label class="lblConcentracao" title="<%= item.Concentracao + " " + unidadeMedida %>"><%= item.Concentracao + " " + unidadeMedida %> </label>
					</td>
					<td>
						<label class="lblSituacao" title="<%=item.SituacaoTexto %>"><%=item.SituacaoTexto%> </label>
					</td>
					<%if (!Model.IsVisualizar) {%>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item)  %>' class="hdnItemJson" />
					</td>
					<%} %>
				</tr>
				<% } %>

				<%if (!Model.IsVisualizar) {%>
				<tr class="trTemplate hide">
					<td>
						<label class="lblNome"></label>
					</td>
					<td>
						<label class="lblConcentracao"></label>
					</td>
					<td>
						<label class="lblSituacao"></label>
					</td>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value="" class="hdnItemJson" />
					</td>
				</tr>
				<%} %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Classes de Uso</legend>

	<div class="block">
		<%foreach (var item in Model.ClassesUso){ %>
		<div class="coluna30 floatLeft">
			<label><%=Html.CheckBox("AgrotoxicoClasseUso", Model.Agrotoxico.ClassesUso.Count(x => x.Id == item.Id) > 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="cbClasseUso"}))%> <%=item.Texto %> </label>
			<input type="hidden" value='<%=ViewModelHelper.Json(item)%>' class="hdnItemJson" />
		</div>
		<%} %>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Classificação Toxicológica</legend>

	<div class="block">
		<div class="coluna62">
			<label for="Agrotoxic_ClassificacaoToxicologica_Id">Classificação Toxicológica *</label>
			<%=Html.DropDownList("Agrotoxico.ClassificacaoToxicologica.Id", Model.ClassToxicologicas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlClassificacaoToxicologica text" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Periculosidade ambiental</legend>

	<div class="block">
		<div class="coluna62">
			<label >Periculosidade ambiental *</label>
			<%=Html.DropDownList("Agrotoxico.PericulosidadeAmbiental.Id", Model.PericulosidadesAmbientais, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlPericulosidadeAmbiental text" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Grupo químico</legend>

	<%if(!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna62">
			<label>Grupo químico *</label>
			<%=Html.DropDownList("Agrotoxico.GrupoQuimico.Id", Model.GruposQuimicos, new { @class="ddlGrupoQuimico text" })%>
		</div>

		<div class="coluna20 prepend1">
			<button class="inlineBotao btnAddGrupoQuimico" type="button" value="Adicionar"><span>Adicionar</span></button>
		</div>
	</div>
	<%} %>

	<div class="block">
		<table class="dataGridTable gridGruposQuimicos" width="70%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="90%">Grupo Quimico</th>
					<%if (!Model.IsVisualizar) {%><th class="semOrdenacao" width="10%">Ações</th><% } %>
				</tr>
			</thead>
			<tbody>
				<%foreach (var item in Model.Agrotoxico.GruposQuimicos){%>
				<tr>
					<td>
						<label class="lblNome" title="<%=item.Texto %>"><%=item.Texto%> </label>
					</td>

					<%if (!Model.IsVisualizar) {%>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item) %>' class="hdnItemJson" />
					</td>
					<% } %>
				</tr>
				<% } %>

				<%if (!Model.IsVisualizar) {%>
				<tr class="trTemplate hide">
					<td>
						<label class="lblNome"></label>
					</td>

					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value="" class="hdnItemJson" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Forma de apresentação</legend>

	<div class="block">
		<div class="coluna62">
			<label>Forma de apresentação *</label>
			<%=Html.DropDownList("Agrotoxico.FormaApresentacao.Id", Model.FormasApresentacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="ddlFormaApresentacao text" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto">
	<legend class="titFiltros">Observações</legend>

	<div class="block filtroCorpo">
		<div class="block">
			<div class="block ultima">
				<label>Observação interna</label>
				<%=Html.TextArea("Agrotoxico.ObservacaoInterna", Model.Agrotoxico.ObservacaoInterna, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="txtObservacaoInterna textarea media", @maxlength="3000" }))%>
			</div>
		</div>

		<div class="block">
			<div class="block ultima">
				<label>Observação geral</label>
				<%=Html.TextArea("Agrotoxico.ObservacaoGeral", Model.Agrotoxico.ObservacaoGeral, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="txtObservacaoGeral textarea media", @maxlength="3000" }))%>
			</div>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Cultura</legend>

	<%if(!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna100">
			<button class="inlineBotao btnAddCultura floatRight" type="button" value="Adicionar"><span>Adicionar</span></button>
		</div>
	</div>
	<%} %>

	<div class="block">
		<table class="dataGridTable gridCulturas" width="70%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="90%">Cultura</th>
					<th class="semOrdenacao" width="10%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<%foreach (var item in Model.Agrotoxico.Culturas){%>
				<tr>
					<td>
						<label class="lblNome" title="<%=item.Cultura.Nome%>"><%=item.Cultura.Nome%> </label>
					</td>
					<%if (!Model.IsVisualizar) {%>
					<td>
						<a title="Editar" class="icone editar btnEditarCultura"></a>
						<a title="Excluir" class="icone excluir btnExcluir"></a>
						<a title="Copiar" class="icone comparar btnCopiar"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item)  %>' class="hdnItemJson" />
					</td>
					<% } else{%>
					<td>
						<a class="icone visualizar btnVisualizarCultura"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item)  %>' class="hdnItemJson" />
					</td>
					<%} %>
				</tr>
				<% } %>

				<%if (!Model.IsVisualizar) {%>
				<tr class="trTemplate hide">
					<td>
						<label class="lblNome"></label>
					</td>
					<td>
						<a title="Editar" class="icone editar btnEditarCultura"></a>
						<a title="Excluir" class="icone excluir btnExcluir"></a>
						<a title="Copiar" class="icone comparar btnCopiar"></a>
						<input type="hidden" value="0" class="hdnItemJson" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<div class="coluna40 inputFileDiv">
		<label>Anexar Bula</label>

		<div class="block">
			<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Agrotoxico.Bula.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Agrotoxico.Bula.Nome) ? "hide" : "" %> txtArquivoNome" target="_blank"><%= Html.Encode(Model.Agrotoxico.Bula.Nome)%></a>
		</div>

		<input type="hidden" class="hdnArquivoJson" value='<%= ViewModelHelper.Json(Model.Agrotoxico.Bula) %>' />

		<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Agrotoxico.Bula.Nome) ? "" : "hide" %>">
			<input type="file" id="fileBula" class="inputFileBula text" style="display: block" accept=".pdf" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %> />
		</span>
	</div>
	<% if (!Model.IsVisualizar){ %>
	<div style="margin-top: 8px" class="coluna40 prepend1 spanBotoes">
		<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.Agrotoxico.Bula.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
		<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Agrotoxico.Bula.Nome) ? "hide" : "" %>" title="Limpar arquivo">Limpar</button>
	</div>
	<% } %>
</fieldset>