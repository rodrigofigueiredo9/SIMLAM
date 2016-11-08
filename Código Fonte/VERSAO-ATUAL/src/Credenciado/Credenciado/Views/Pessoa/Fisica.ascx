<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<fieldset class="block box">
	<%= Html.Hidden("Pessoa.Tipo", 1) %>
	<div class="block">
		<div class="coluna20">
			<label for="Pessoa.Fisica.CPF">CPF *</label>
			<%= Html.TextBox("Pessoa.Fisica.CPF", Model.Pessoa.Fisica.CPF, new { @maxlength = "15", @class = "inputCpfPessoa text disabled", @disabled = "disabled" })%> 
		</div>
		<% if (!Model.IsVisualizar && !Model.OcultarLimparPessoa) { %>
		<div class="coluna10">
			<button type="button" title="Limpar CPF" class="inlineBotao btnLimparCpfCnpj">Limpar</button>
		</div>
		<% } %>
	
		<%if (Model.Pessoa.InternoId > 0 && !Model.IsVisualizar && !Model.IsConjuge && !Model.OcultarIsCopiado){ %>
			<div class="coluna30 prepend11">
				<button class="inlineBotao btnCopiarInterno" type="button">Copiar Dados do IDAF</button>
			</div>
		<%} %>
	</div>
	<div class="block">
		<div class="coluna98">
			<label for="Pessoa.Fisica.Nome">Nome *</label>
			<%= Html.TextBox("Pessoa.Fisica.Nome", Model.Pessoa.Fisica.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "80	", @class = "text txtPessoaNome" }))%> 
		</div>
	</div>

	<div class="block">
		<div class="coluna70">
			<label for="Pessoa.Fisica.Nome">Apelido</label>
			<%= Html.TextBox("Pessoa.Fisica.Apelido", Model.Pessoa.Fisica.Apelido, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtApelido", @maxlength = "50" }))%> 
		</div>
		<div class="coluna24 prepend2">
			<label for="Pessoa.Fisica.RG">RG/Órgão expedidor/UF</label>
			<%= Html.TextBox("Pessoa.Fisica.RG", Model.Pessoa.Fisica.RG, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "30", @class = "text " }))%> 
		</div>
	</div>

	<div class="block">
		<div class="coluna48">
			<label for="Pessoa.Fisica.Nacionalidade">Nacionalidade *</label>
			<%= Html.TextBox("Pessoa.Fisica.Nacionalidade", Model.Pessoa.Fisica.Nacionalidade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text" }))%> 
		</div>
		<div class="coluna46 prepend2">
			<label for="Pessoa.Fisica.Naturalidade">Naturalidade *</label>
			<%= Html.TextBox("Pessoa.Fisica.Naturalidade", Model.Pessoa.Fisica.Naturalidade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text" }))%> 
		</div>
	</div>
	<div class="block">
		<div class="coluna18 ">
			<label for="Pessoa.Fisica.Sexo">Sexo *</label>
			<%= Html.DropDownList("Pessoa.Fisica.Sexo", Model.Sexos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text" }))%> 
		</div>
		<div class="coluna25 prepend2">
			<label for="Pessoa.EstadoCivil">Estado civil *</label>
			<%= Html.DropDownList("Pessoa.Fisica.EstadoCivil", Model.EstadosCivis, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlEstadoCivil" }))%> 
		</div>
		
		<div class="coluna18 prepend2">
			<label for="DataNascimento">Data de nascimento *</label>
			<%= Html.TextBox("DataNascimento", Model.DataNascimento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "10", @class = "text maskData" }))%> 
		</div>
	</div>

	<div class="divConjuge block <%= Model.MostarConjuge(Model.Pessoa.Fisica.EstadoCivil) ? "" :"hide" %>">
		<div class="coluna60">
			<label for="Pessoa.Fisica.ConjugeNome">Cônjuge </label>
			<%= Html.Hidden("Pessoa.Fisica.ConjugeId", Model.Pessoa.Fisica.ConjugeId, new { @class = "hdnConjugeId" })%>
			<%= Html.Hidden("Pessoa.Fisica.ConjugeInternoId", Model.Pessoa.Fisica.ConjugeInternoId, new { @class = "hdnConjugeInternoId" })%>
			<%= Html.TextBox("Pessoa.Fisica.ConjugeNome", Model.Pessoa.Fisica.ConjugeNome, new { @class = "text txtConjugeNome disabled", @disabled = "disabled"})%> 
		</div>
		<div class="coluna20 prepend2">
			<label for="Pessoa.Fisica.ConjugeCPF">CPF</label>
			<%= Html.Hidden("Pessoa.Fisica.ConjugeCPF", Model.Pessoa.Fisica.ConjugeCPF, new { @class = "hdnConjugeCPF" })%>
			<%= Html.TextBox("ConjugeCPF", Model.Pessoa.Fisica.ConjugeCPF, new { @maxlength = "15", @class = "text txtConjugeCPF disabled", @disabled = "disabled" })%> 
		</div>

		<% if (!Model.IsConjuge){ %>
			<div class="coluna15">
				<% if (!Model.IsVisualizar){ %>
				<button type="button" title="Buscar Cônjuge" class="inlineBotao btnAssociarConjuge <%= (Model.Pessoa.Fisica.ConjugeId > 0  || !string.IsNullOrEmpty(Model.Pessoa.Fisica.ConjugeCPF)? "hide" : "") %>">Buscar</button>
				<% } %>
				<span class="<%= (Model.Pessoa.Fisica.ConjugeId > 0  || !string.IsNullOrEmpty(Model.Pessoa.Fisica.ConjugeCPF)? "" : "hide") %>"> 
					<a title="Visualizar cônjuge" class="icone visualizar esquerda inlineBotao btnVisualizarConjuge"></a>
				</span>
				<% if (!Model.IsVisualizar){ %>
				<button type="button" title="Limpar" class="inlineBotao btnLimparConjuge <%= (Model.Pessoa.Fisica.ConjugeId > 0 ? "" : "hide") %>">Limpar</button>
				<% } %>
			</div>
		<% } %>
	</div>

</fieldset>
<fieldset class="block box">
<legend>Profissão</legend>
	<div class="block">
		<div class="coluna76">
			<label for="Pessoa.Fisica.Profissao.ProfissaoTexto">Profissão</label>
			<%= Html.TextBox("Pessoa.Fisica.Profissao.ProfissaoTexto", (Model.Pessoa.Fisica.Profissao.IdRelacionamento > 0 ? Model.Pessoa.Fisica.Profissao.ProfissaoTexto : "*** Associar uma profissão ***"), new { @class = "text txtProfissao disabled", @disabled = "disabled" })%>
			<%= Html.Hidden("Pessoa.Fisica.Profissao.Id", Model.Pessoa.Fisica.Profissao.Id, new { @class = "hdnProfissao" })%>
			<%= Html.Hidden("Pessoa.Fisica.Profissao.IdRelacionamento", Model.Pessoa.Fisica.Profissao.IdRelacionamento, new { @class = "hdnProfissaoIdRelacionamento" })%>
		</div>
		<% if (!Model.IsVisualizar) { %>
			<div class="coluna22">
				<button type="button" title="Buscar profissão" class="inlineBotao btnAssociarProfissao <%= (Model.Pessoa.Fisica.Profissao.Id > 0 ? "hide" : "") %>">Buscar</button>
				<button type="button" title="Limpar" class="inlineBotao btnLimparProfissao <%= (Model.Pessoa.Fisica.Profissao.Id > 0 ? "" : "hide") %>">Limpar</button>
			</div>
		<% } %>
	</div>
	<div class="block">
		<div class="coluna29">
			<label for="Pessoa.Fisica.Profissao.OrgaoClasseId">Orgão de classe</label>
			<%= Html.DropDownList("Pessoa.Fisica.Profissao.OrgaoClasseId", Model.OrgaoClasses, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlOrgaoClasse" }))%>
		</div>
		<div class="coluna28 prepend2">
			<label for="Pessoa.Fisica.Profissao.Registro">Registro</label>
			<%= Html.TextBox("Pessoa.Fisica.Profissao.Registro", Model.Pessoa.Fisica.Profissao.Registro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "15", @class = "text" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
<legend>Filiação</legend>
	<div class="block">
		<div class="coluna48">
			<label for="Pessoa.Fisica.NomeMae">Nome da Mãe</label>
			<%= Html.TextBox("Pessoa.Fisica.NomeMae", Model.Pessoa.Fisica.NomeMae, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text txtNomeMae" }))%> 
		</div>
		<div class="coluna47 prepend2">
			<label for="Pessoa.Fisica.NomePai">Nome do Pai</label>
			<%= Html.TextBox("Pessoa.Fisica.NomePai", Model.Pessoa.Fisica.NomePai, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text txtNomePai" })) %> 
		</div>
	</div>
</fieldset>