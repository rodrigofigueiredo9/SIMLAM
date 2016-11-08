<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitacaoEmissaoPTVVM>" %>

<%= Html.Hidden("Habilitacao.Id", Model.Habilitacao.Id, new { @class = "hdnHabilitacaoId" })%>
<fieldset class="block box">
	<legend>Funcionário</legend>
	<div class="block">
		<div class="coluna21 append1">
			<label for="Pessoa.Fisica.Cpf">CPF *</label>
			<%= Html.TextBox("Pessoa.Fisica.Cpf", Model.Habilitacao.Funcionario.Cpf, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskCpf txtCpf setarFoco" }))%>
			<%= Html.Hidden("Funcionario.Id", Model.Habilitacao.Funcionario.Id, new { @class = "hdnFuncionarioId" })%>
		</div>
		<%if (!Model.IsVisualizar) { %>
		<div class="block ultima">
			<button type="button" class="inlineBotao esquerda btnVerificarCpf <%= (Model.IsVisualizar) ? "hide" : "" %>">Verificar</button>
		</div>
		<%} %>
	</div>

	<div class="block mostrar <%=Model.IsVisualizar? "": "hide" %>">
		<div class="coluna70">
			<label>Funcionário *</label>
			<%= Html.TextBox("Habilitacao.FuncionarioNome", Model.Habilitacao.Funcionario.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNome", @maxlength="80" }))%>
		</div>
	</div>
	<div class="block mostrar <%=Model.Habilitacao.Funcionario.Setores.Count > 0 ? "": "hide" %>">
		<div class="coluna70">
			<table class="dataGridTable gridSetores" border="0" cellspacing="0" cellpadding="0" width="100%">
				<thead>
					<tr>
						<th width="20%">Setor</th>
					</tr>
				</thead>
				<tbody>
					<%foreach (var setor in Model.Habilitacao.Funcionario.Setores) { %>
					<tr>
						<td class="setor">
							<%=setor.Nome %>
						</td>
					</tr>
					<%} %>
					<tr class="templateRow hide">
						<td class="setor"></td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>

	<div class="block mostrar <%=Model.Habilitacao.Id > 0?"":"hide" %>">
		<div class="coluna70 inputFileDiv append1">
			<label for="ArquivoTexto">Anexar foto</label>
			<% if (Model.Habilitacao.Arquivo.Id.GetValueOrDefault() > 0) { %>
			<div>
				<%= Html.ActionLink(Tecnomapas.EtramiteX.Interno.ViewModels.ViewModelHelper.StringFit(Model.Habilitacao.Arquivo.Nome, 45), "Baixar", "Arquivo", new { @id = Model.Habilitacao.Arquivo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Habilitacao.Arquivo.Nome })%>
			</div>
			<% } %>

			<%= Html.TextBox("HabilitarEmissao.Arquivo.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
			<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Habilitacao.Arquivo.Nome) ? "" : "hide" %>">
				<input type="file" id="arquivo" class="inputFile" style="display: block; width: 100%" name="file" /></span>
			<input type="hidden" class="hdnArquivoJson" value='<%= Model.ArquivoJson%>' />
		</div>

		<div class="block ultima spanBotoes">
			<button type="button" class="inlineBotao btnAddArq <%= string.IsNullOrEmpty(Model.Habilitacao.Arquivo.Nome) ? "" : "hide" %>" title="Enviar anexo">Enviar</button>
			<%if (!Model.IsVisualizar) {%>
			<button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.Habilitacao.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo"><span>Limpar</span></button>
			<%} %>
		</div>
	</div>
</fieldset>

<fieldset class="block box mostrar <%=Model.Habilitacao.Id > 0 ? "" : "hide" %>">
	<legend>Habilitação</legend>
	<div class="block">
		<div class="coluna21 append1">
			<label>Nº da Habilitação *</label>
			<%=Html.TextBox("Habilitacao.NumeroRegistro", Model.Habilitacao.NumeroHabilitacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNumInt txtNumeroHabilitacao", @maxlength="8"}))%>
		</div>
		<div class="coluna21 append1">
			<label>RG *</label>
			<%=Html.TextBox("Habilitacao.RG", Model.Habilitacao.RG, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtRG", @maxlength="30"}))%>
		</div>
		<div class="coluna21">
			<label>Nº da matrícula no IDAF *</label>
			<%=Html.TextBox("Habilitacao.NumeroMatricula", Model.Habilitacao.NumeroMatricula, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text maskNumInt txtNumeroMatricula", @maxlength="7"}))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box mostrar <%=Model.Habilitacao.Id > 0? "":"hide" %>">
	<legend>Profissão</legend>
	<div class="block">
		<div class="coluna70 append1">
			<label>Profissão</label>
			<%=Html.TextBox("Habilitacao.Profissao.ProfissaoTexto", Model.Habilitacao.Profissao.ProfissaoTexto, ViewModelHelper.SetaDisabled(true, new { @class="text txtProfissao"}))%>
			<%=Html.Hidden("Habilitacao.Profissao.Id", Model.Habilitacao.Profissao.Id, new {@class="hdnProfissaoId" })%>
		</div>

		<%if (!Model.IsVisualizar) { %>
		<div class="block ultima">
			<button type="button" class="inlineBotao esquerda btnBuscarProfissao">Buscar</button>
		</div>
		<%} %>
	</div>

	<div class="block">
		<div class="coluna21 append1">
			<label>Órgão de classe</label>
			<%=Html.DropDownList("Habilitacao.Profissao.OrgaoClasseId", Model.OrgaoClasse, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{@class="ddlOrgaoClasse text"}) ) %>
		</div>
		<div class="coluna21">
			<label>Registro</label>
			<%=Html.TextBox("Habilitacao.Profissao.Registro", Model.Habilitacao.Profissao.Registro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtRegistroOrgaoClasse", @maxlength="15"}))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna21 append1">
			<label>UF *</label>
			<%=Html.DropDownList("Habilitacao.EstadoRegistro", Model.UF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new{@class="ddlEstadosRegistro text"}) ) %>
		</div>
		<div class="coluna21 append1 divNumeroVistoCrea <%= Model.Habilitacao.EstadoRegistro == ViewModelHelper.EstadoDefaultId() ? "hide":"" %>">
			<label>Nº visto no CREA/ES *</label>
			<%=Html.TextBox("Habilitacao.NumeroVistoCrea", Model.Habilitacao.NumeroVistoCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroVistoCrea", @maxlength="12"}))%>
		</div>
		<div class="coluna21 divNumeroCREA <%= Model.Habilitacao.EstadoRegistro != ViewModelHelper.EstadoDefaultId() ? "hide":"" %>">
			<label for="Habilitacao_NumeroCREA">Nº CREA/ES *</label>
			<%=Html.TextBox("Habilitacao.NumeroCREA", Model.Habilitacao.NumeroCREA, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtNumeroCREA", @maxlength="12"}))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box mostrar <%=Model.Habilitacao.Id > 0? "" : "hide" %>">
	<legend>Meios de Contato</legend>
	<div class="coluna21 append1">
		<label>Telefone residencial</label>
		<%=Html.TextBox("Habilitacao.TelefoneResidencial", Model.Habilitacao.TelefoneResidencial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTelefoneResidencial maskFone", @maxlength="40"}))%>
	</div>
	<div class="coluna21 append1">
		<label>Telefone celular</label>
		<%=Html.TextBox("Habilitacao.TelefoneCelular", Model.Habilitacao.TelefoneCelular, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTelefoneCelular maskFone", @maxlength="40"}))%>
	</div>
	<div class="coluna21">
		<label>Telefone comercial</label>
		<%=Html.TextBox("Habilitacao.TelefoneComercial", Model.Habilitacao.TelefoneComercial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtTelefoneComercial maskFone", @maxlength="40"}))%>
	</div>
</fieldset>

<fieldset class="block box mostrar <%=Model.Habilitacao.Id > 0 ? "" : "hide" %>">
	<legend>Endereço Completo</legend>

	<div class="block endereco correspondenciaContainer">
		<div class="block">
			<div class="coluna21 append1">
				<label>CEP</label>
				<%= Html.TextBox("Habilitacao.Endereco.Cep", Model.Habilitacao.Endereco.Cep, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "txtCep text maskCep" }))%>
			</div>
			<div class="coluna76">
				<label>Logradouro/Rua/Rodovia *</label>
				<%= Html.TextBox("Habilitacao.Endereco.Logradouro", Model.Habilitacao.Endereco.Logradouro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "250", @class = "txtLogradouro text" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna50 append1">
				<label>Bairro/Gleba *</label>
				<%= Html.TextBox("Habilitacao.Endereco.Bairro", Model.Habilitacao.Endereco.Bairro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "250", @class = "txtBairro text" }))%>
			</div>
			<div class="coluna18 append1">
				<label>UF *</label>
				<%= Html.DropDownList("Habilitacao.Endereco.EstadoId", Model.UF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlEstadoEndereco" }))%>
			</div>
			<div class="coluna27">
				<label>Município *</label>
				<%= Html.DropDownList("Habilitacao.Endereco.MunicipioId", Model.Municipio, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Habilitacao.Endereco.EstadoId <=0,  new { @class = "text ddlMunicipio" }))%>
			</div>
		</div>

		<div class="block divEndereco">
			<div class="coluna21 append1">
				<label>Número *</label>
				<%= Html.TextBox("Habilitacao.Endereco.Numero", Model.Habilitacao.Endereco.Numero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "4", @class = "text maskNumInt txtNumero" }))%>
			</div>

			<div class="coluna25 append1">
				<label>Distrito/Localidade</label>
				<%= Html.TextBox("Habilitacao.Endereco.DistritoLocalizacao", Model.Habilitacao.Endereco.DistritoLocalizacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "250", @class = "txtDistrito text" }))%>
			</div>
			<div class="coluna49">
				<label>Complemento</label>
				<%= Html.TextBox("Habilitacao.Endereco.Complemento", Model.Habilitacao.Endereco.Complemento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "80", @class = "text txtComplemento" }))%>
			</div>
		</div>
	</div>
</fieldset>

<fieldset class="block box mostrar <%=Model.Habilitacao.Id > 0 ? "":"hide" %>">
	<legend>Operadores</legend>
	<%if (!Model.IsVisualizar) { %>
	<div class="block">
		<button type="button" class="inlineBotao direita btnBuscarOperador">Buscar</button>
	</div>
	<%} %>
	<div class="block ultima">
		<table class="dataGridTable gridOperadores" border="0" cellspacing="0" cellpadding="0" width="100%">
			<thead>
				<tr>
					<th>Nome</th>
					<%if (!Model.IsVisualizar) { %><th width="7%">Ações</th><% } %>
				</tr>
			</thead>
			<tbody>
				<%foreach (HabilitacaoEmissaoPTVOperador operador in Model.Habilitacao.Operadores) { %>
				<tr>
					<td class="nome">
						<%=operador.FuncionarioNome %>
					</td>
					<%if (!Model.IsVisualizar) { %>
					<td class="acoes">
						<input type="button" class="icone excluir btnExcluirOperador" title="Excluir" />
						<input type="hidden" class="hdnOperadorJson" value='<%=ViewModelHelper.Json(operador) %>' />
					</td>
					<% } %>
				</tr>
				<% } %>
				<% if (!Model.IsVisualizar) { %>
				<tr class="templateRow hide">
					<td class="nome"></td>
					<td class="acoes">
						<input type="button" class="icone excluir btnExcluirOperador" title="Excluir" />
						<input type="hidden" class="hdnOperadorJson" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>