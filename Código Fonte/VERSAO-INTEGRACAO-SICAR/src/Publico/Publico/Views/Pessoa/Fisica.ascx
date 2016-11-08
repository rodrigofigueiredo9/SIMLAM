<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<fieldset class="block box">
	<%= Html.Hidden("Pessoa.Tipo", 1) %>
	<div class="block">
		<div class="coluna20">
			<label for="Pessoa.Fisica.CPF">CPF *</label>
			<%= Html.TextBox("Pessoa.Fisica.CPF", Model.Pessoa.Fisica.CPF, new { @maxlength = "15", @class = "inputCpfPessoa text disabled", @disabled = "disabled" })%> 
		</div>
		<% if (!Model.IsVisualizar && Model.ExibirLimparPessoa) { %>
		<div class="coluna10">
			<button type="button" title="Limpar CPF" class="inlineBotao btnLimparCpfCnpj">Limpar</button>
		</div>
		<% } %>
	</div>
	<div class="block">
		<div class="coluna98">
			<label for="Pessoa.Fisica.Nome">Nome *</label>
			<%= Html.TextBox("Pessoa.Fisica.Nome", Model.Pessoa.Fisica.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "80", @class = "text txtPessoaNome" }))%> 
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
			<label for="Pessoa.EstadoCivil">Estado civíl *</label>
			<%= Html.DropDownList("Pessoa.Fisica.EstadoCivil", Model.EstadosCivis, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlEstadoCivil" }))%> 
		</div>
		
		<div class="coluna18 prepend2">
			<label for="DataNascimento">Data de nascimento *</label>
			<%= Html.TextBox("DataNascimento", Model.DataNascimento, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "10", @class = "text maskData" }))%> 
		</div>
	</div>

	<div class="divConjuge block <%= Model.MostarConjuge(Model.Pessoa.Fisica.EstadoCivil) ? "" :"hide" %>">
		<div class="coluna46">
			<label for="Pessoa.Fisica.Conjuge.Nome">Cônjuge *</label>
			
			<%
				Pessoa conjuge = Model.Pessoa.Fisica.Conjuge ?? new Pessoa();
			 %>

			<%= Html.Hidden("Pessoa.Fisica.ConjugeId", Model.Pessoa.Fisica.ConjugeId, new { @class = "hdnConjugeId" })%>
			<%= Html.Hidden("Pessoa.Fisica.Conjuge", ViewModelHelper.Json(conjuge), new { @class = "hdnConjugeJSON" })%>
			<%= Html.TextBox("Pessoa.Fisica.ConjugeNome", Model.Pessoa.Fisica.ConjugeNome, new { @class = "text txtConjugeNome disabled", @disabled = "disabled"})%> 
		</div>
		<div class="coluna18 prepend2">
			<label for="Pessoa.Fisica.ConjugeCPF">CPF *</label>
			<%= Html.TextBox("Pessoa.Fisica.ConjugeCPF", Model.Pessoa.Fisica.ConjugeCPF, new { @maxlength = "15", @class = "text txtConjugeCPF disabled", @disabled = "disabled" })%> 
		</div>
		<%if (!Model.IsVisualizar && Model.TipoCadastro <= 1 && !Model.IsConjuge){ %>
			<div class="coluna15">
				<button type="button" title="Buscar Cônjuge" class="inlineBotao btnAssociarConjuge <%= (String.IsNullOrWhiteSpace(Model.Pessoa.Fisica.ConjugeCPF) ? "" : "hide") %>">Buscar</button>
				<span class="spanVisualizarConjuge <%= (String.IsNullOrWhiteSpace(Model.Pessoa.Fisica.ConjugeCPF) ? "hide" : "") %>"> 
					<a title="Visualizar cônjugue" class="icone visualizar esquerda inlineBotao btnVisualizarConjugue"></a>
				</span>
				<button type="button" title="Limpar" class="inlineBotao btnLimparConjuge <%= (String.IsNullOrWhiteSpace(Model.Pessoa.Fisica.ConjugeCPF) ? "hide" : "") %>">Limpar</button>
			</div>
		<% } %>
	</div>

</fieldset>
<fieldset class="block box">
<legend class="lgProfissao">Profissão</legend>
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
			<label for="Pessoa.Fisica.NomeMae">Nome da Mãe<%=Model.Asterisco(Model.NomeMaeObrigatorio) %></label>
			<%= Html.TextBox("Pessoa.Fisica.NomeMae", Model.Pessoa.Fisica.NomeMae, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text txtNomeMae" }))%> 
		</div>
		<div class="coluna47 prepend2">
			<label for="Pessoa.Fisica.NomePai">Nome do Pai<%=Model.Asterisco(Model.NomePaiObrigatorio) %></label>
			<%= Html.TextBox("Pessoa.Fisica.NomePai", Model.Pessoa.Fisica.NomePai, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text txtNomePai" })) %> 
		</div>
	</div>
</fieldset>