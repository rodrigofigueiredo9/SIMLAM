<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConsideracaoFinalTestemunhaVM>" %>

<div class="block">
	<%= Html.Hidden("hdnColocacao", Model.Testemunha.Colocacao, new { @class = "hdnColocacao" })%>
	<div class="block">
		<div class="coluna19">
			<label for="">Funcionário do IDAF?</label><br />
			<%= Html.DropDownList("Span" + Model.Testemunha.Colocacao, Model.FuncionarioIDAF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFuncIDAF" }))%> 

			<span style="display: none;/*inline-block*/ border-style: solid; border-width: 1px; border-color: transparent;" class="text" id="Span<%= Model.Testemunha.Colocacao%>">
				<label><%= Html.RadioButton("rblFuncIDAF" + Model.Testemunha.Colocacao, 1, Model.Testemunha.TestemunhaIDAF.HasValue && Model.Testemunha.TestemunhaIDAF.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblFuncIDAF prepend2" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("rblFuncIDAF" + Model.Testemunha.Colocacao, 0, Model.Testemunha.TestemunhaIDAF.HasValue && !Model.Testemunha.TestemunhaIDAF.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblFuncIDAF " }))%>Não</label>
			</span>
		</div>
	</div>
	<div class="block divFuncionario<%= Model.Testemunha.TestemunhaIDAF.HasValue && Model.Testemunha.TestemunhaIDAF.Value ? "" : " hide"%>">
		<div class="coluna42 append2">
			<label for="Testemunha_TestemunhaId<%= Model.Testemunha.Colocacao%>">Testemunha *</label>
			<%= Html.DropDownList("Testemunha.TestemunhaId" + Model.Testemunha.Colocacao, Model.Funcionarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTestemunha" }))%> 
		</div>
		<div class="coluna42">
			<label for="Testemunha_TestemunhaCPF<%= Model.Testemunha.Colocacao%>">CPF *</label>
			<%= Html.TextBox("Testemunha.TestemunhaCPF" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaCPF, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTestemunhaCPF", maxlength = "200" }))%> 
		</div>
	</div>
	<div class="block divDadosTestemunha<%= Model.Testemunha.TestemunhaIDAF.HasValue && !Model.Testemunha.TestemunhaIDAF.Value ? "" : " hide" %>">
		<div class="coluna90">
			<label for="Testemunha_TestemunhaNome<%= Model.Testemunha.Colocacao%>">Nome Testemunha *</label>
			<%= Html.TextBox("Testemunha.TestemunhaNome" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaNome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTestemunhaNome", maxlength = "80" }))%> 
		</div>			
	</div><%  %>
	<%--<div class="block divDadosEndereco<%= Model.Testemunha.TestemunhaIDAF.HasValue ? "" : " hide" %>">
		<div class="coluna87">
			<label for="Testemunha_TestemunhaEndereco<%= Model.Testemunha.Colocacao%>">Endereço *</label>
			<%= Html.TextBox("Testemunha.TestemunhaEndereco" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaEndereco, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Testemunha.TestemunhaIDAF.GetValueOrDefault(), new { @class = "text txtTestemunhaEndereco", maxlength = "200" }))%> 
		</div>			
	</div>--%>
</div>