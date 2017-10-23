<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConsideracaoFinalTestemunhaVM>" %>

<div class="block">
	<%= Html.Hidden("hdnColocacao", Model.Testemunha.Colocacao, new { @class = "hdnColocacao" })%>
    
    <!--Pergunta Funcionário do IDAF?-->
	<div class="block">
		<div class="coluna19">
			<label for="">Funcionário do IDAF?</label><br />
			<%= Html.DropDownList("Span" + Model.Testemunha.Colocacao, Model.FuncionarioIDAF, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlFuncIDAF" }))%> 

			<%--<span style="display: none;/*inline-block*/ border-style: solid; border-width: 1px; border-color: transparent;" class="text" id="Span<%= Model.Testemunha.Colocacao%>">
				<label><%= Html.RadioButton("rblFuncIDAF" + Model.Testemunha.Colocacao, 1, Model.Testemunha.TestemunhaIDAF.HasValue && Model.Testemunha.TestemunhaIDAF.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblFuncIDAF prepend2" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("rblFuncIDAF" + Model.Testemunha.Colocacao, 0, Model.Testemunha.TestemunhaIDAF.HasValue && !Model.Testemunha.TestemunhaIDAF.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblFuncIDAF " }))%>Não</label>
			</span>--%>

		</div>
	</div>

    <!--Campos "Testemunha" e "CPF", são exibidos se for um funcionário do IDAF (resposta "sim" para a pergunta acima)-->
	<div class="block divFuncionario<%= Model.Testemunha.TestemunhaIDAF.HasValue && Model.Testemunha.TestemunhaIDAF.Value ? "" : " hide"%>">
		<div class="coluna50 append2">
			<label for="Testemunha_TestemunhaId<%= Model.Testemunha.Colocacao%>">Testemunha *</label>
			<%= Html.DropDownList("Testemunha.TestemunhaId" + Model.Testemunha.Colocacao, Model.Funcionarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlTestemunha" }))%> 
		</div>
		<div class="coluna30">
			<label for="Testemunha_TestemunhaCPF<%= Model.Testemunha.Colocacao%>">CPF *</label>
			<%= Html.TextBox("Testemunha.TestemunhaCPF" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaCPF, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTestemunhaCPF", maxlength = "200" }))%> 
		</div>
	</div>

    <!--Campos "Nome" e "CPF", não exibidos se não for um funcionário do IDAF (resposta "não" para a pergunta acima)-->
	<div class="block divDadosTestemunha<%= Model.Testemunha.TestemunhaIDAF.HasValue && !Model.Testemunha.TestemunhaIDAF.Value ? "" : " hide" %>">
		<div class="block" >

            <input type="hidden" class="hdnTestemunhaId" value="<%= Html.Encode(Model.Testemunha.Id) %>" />

			<div class="coluna50">
				<label for="Testemunha_TestemunhaNome<%= Model.Testemunha.Colocacao%>">Nome *</label>
				<%= Html.TextBox("Testemunha.TestemunhaNome" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaNome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTestemunhaNome", maxlength = "80" }))%> 
			</div>

			<div class="coluna30 prepend2">
				<label>CPF *</label>
				<%= Html.TextBox("Testemunha.TestemunhaCPF" + Model.Testemunha.Colocacao, Model.Testemunha.TestemunhaCPF, ViewModelHelper.SetaDisabled(true, new { @class = "text disabled txtTestemunhaCPF"}))%>
			</div>

			<div class="prepend2">
				<% if (!Model.IsVisualizar) { %>
				<button type="button" title="Buscar testemunha" class="floatLeft inlineBotao botaoBuscar btnAssociarTestemunha">Buscar</button>
				<% } %>
				<span class="spanVisualizarTestemunha<%= (Model.Testemunha.Id > 0) ? "" : "hide" %>"><button type="button" class="icone visualizar esquerda inlineBotao btnEditarTestemunha" title="Visualizar testemunha"></button></span>
			</div>
		</div>
	</div>

</div>