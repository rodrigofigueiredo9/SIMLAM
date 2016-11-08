<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<fieldset class="block box">
	<%= Html.Hidden("Pessoa.Tipo", 2) %>
	<div class="block">
		<div class="coluna20">
			<label for="">CNPJ *</label>
			<%= Html.TextBox("Pessoa.Juridica.CNPJ", Model.Pessoa.Juridica.CNPJ, new { @maxlength = "20", @class = "inputCnpjPessoa text disabled", @disabled = "disabled" })%>
		</div>
		<% if (!Model.IsVisualizar && !Model.OcultarLimparPessoa) { %>
		<div class="coluna10">
			<button type="button" title="Limpar CNPJ" class="inlineBotao btnLimparCpfCnpj">Limpar</button>
		</div>
		<% } %>
		<%if(Model.Pessoa.InternoId > 0 && Model.Pessoa.Id > 0 && !Model.IsVisualizar && !Model.OcultarIsCopiado){ %>
			<div class="coluna30 prepend11">
				<button class="inlineBotao btnCopiarInterno" type="button">Copiar Dados do IDAF</button>
			</div>
		<%} %>
	</div>
	<div class="block">
		<div class="coluna90">
			<label for="Pessoa.Juridica.RazaoSocial">Razão social *</label>
				<%= Html.TextBox("Pessoa.Juridica.RazaoSocial", Model.Pessoa.Juridica.RazaoSocial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ", @maxlength = "80" }))%> 
		</div>
	</div>
	<div class="block">
		<div class="coluna90">
			<label for="Pessoa.Juridica.NomeFantasia">Nome fantasia</label>
			<%= Html.TextBox("Pessoa.Juridica.NomeFantasia", Model.Pessoa.Juridica.NomeFantasia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "80", @class = "text txtPessoaNome" }))%> 
		</div>
	</div>	
	<div class="block">
		<div class="coluna20">
			<label for="Pessoa.Juridica.Ie">Inscrição estadual</label>
			<%= Html.TextBox("Pessoa.Juridica.Ie", Model.Pessoa.Juridica.IE, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "20", @class = "text" }))%> 
		</div>
	</div>
</fieldset>

<fieldset class="block box" id="Representante_Container">
	<legend>Representantes</legend>
	<% if (!Model.IsVisualizar) { %>
		<div class="block">	
			<div class="floatRight">
				<button type="button" title="Buscar representante" class="btnAssociarReprensetante">Buscar</button>
				<%= Html.Hidden("Pessoa.Juridica.RepresentanteId") %>
			</div>
		</div>
	<% } %>
	<div class="block dataGrid">
		<table class="dataGridTable tabRepresentantes" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Nome</th>
					<th>CPF</th>
					<th width="10%">Ações</th>
				</tr>
			</thead>
			<tbody>
			<%
				int idRep = 0;
				foreach (Pessoa rep in Model.Pessoa.Juridica.Representantes) {
				idRep++; %>
				<tr>
					<td>
						<span class="RepresentanteNome" title="<%: Html.Encode(rep.Fisica.Nome) %>"><%= Html.Encode(rep.Fisica.Nome) %></span>
						<input type="hidden" class="hdnRepresentanteIndex" name="Pessoa.Juridica.Representantes.Index" value="<%= idRep %>" />
						<input type="hidden" class="hdnRepresentanteId" name="Pessoa.Juridica.Representantes[<%= idRep %>].Id" value="<%= Html.Encode(rep.Id) %>" />
						<input type="hidden" class="hdnRepresentanteInternoId" name="Pessoa.Juridica.Representantes[<%= idRep %>].InternoId" value="<%= Html.Encode(rep.InternoId) %>" />
						<input type="hidden" class="hdnRepresentanteNome" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.Nome" value="<%= Html.Encode(rep.Fisica.Nome) %>" />
						<input type="hidden" class="hdnRepresentanteCpf" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.CPF" value="<%= Html.Encode(rep.Fisica.CPF) %>" />
						<input type="hidden" class="hdnIsCopiado" name="Pessoa.Juridica.Representantes[<%= idRep %>].IsCopiado" value="<%= Html.Encode(rep.IsCopiado.ToString().ToLower()) %>" />
						<input type="hidden" class="hdnConjugeId" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.ConjugeId" value="<%= Html.Encode(rep.Fisica.ConjugeId) %>" />
						<input type="hidden" class="hdnConjugeInternoId" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.ConjugeInternoId" value="<%= Html.Encode(rep.Fisica.ConjugeInternoId) %>" />
					</td>
					<td>
						<span class="RepresentanteCpf" title="<%: rep.Fisica.CPF %>"><%: rep.Fisica.CPF %></span>
					</td>
					<td>
						<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRepresentante" value="" />
					<% if (!Model.IsVisualizar) { %>										
						<input title="Excluir" type="button" class="icone excluir btnExcluirRepresentante" value="" />					
					<% } %>
					</td>	
				</tr>
			<%	} %>
			</tbody>
		</table>
		<table style="display: none">
			<tbody>
				<tr class="trRepresentanteTemplate">
					<td>
						<span class="RepresentanteNome">NOME</span>
						<input type="hidden" class="hdnRepresentanteIndex" name="templatePessoa.Representantes.Index" value="#Index" />
						<input type="hidden" class="hdnRepresentanteId" name="templatePessoa.Representantes[#Index].Id" value="#Id" />
						<input type="hidden" class="hdnRepresentanteInternoId" name="templatePessoa.Representantes[#Index].InternoId" value="#InternoId" />
						<input type="hidden" class="hdnRepresentanteNome" name="templatePessoa.Representantes[#Index].Fisica.Nome" value="#NOME" />
						<input type="hidden" class="hdnRepresentanteCpf" name="templatePessoa.Representantes[#Index].Fisica.CPF" value="#CPF" />
						<input type="hidden" class="hdnIsCopiado" name="templatePessoa.Representantes[#Index].Fisica.IsCopiado" value="#IsCopiado" />
						<input type="hidden" class="hdnConjugeId" name="templatePessoa.Representantes[#Index].Fisica.ConjugeId" value="#ConjugeId" />
					</td>
					<td>
						<span class="RepresentanteCpf">CPF</span>
					</td>
					<td>
						<% if (!Model.IsVisualizar) { %>
							<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRepresentante" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirRepresentante" value="" />
						<% } %>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>