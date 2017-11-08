<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoSalvarVM>" %>

<%= Html.Hidden("hdnConfigFiscalizacaoId", Model.Configuracao.Id, new { @class = "hdnConfigFiscalizacaoId" }) %>

<fieldset class="box">
	<legend>Classificação da infração</legend>

	<div class="block">
		<div class="coluna76">	
			<label for="Configuracao_Classificacao">Classificação *</label><br />
			<%= Html.DropDownList("Configuracao.Classificacao", Model.Classificacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Classificacoes.Count <=2, new { @class = "text ddlClassificacoes" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna76">	
			<label for="Configuracao_Tipo">Tipo de infração *</label><br />
			<%= Html.DropDownList("Configuracao.Tipo", Model.Tipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Tipos.Count <= 2, new { @class = "text ddlTipos" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna76">	
			<label for="Configuracao_Item">Item *</label><br />
			<%= Html.DropDownList("Configuracao.Item", Model.Itens, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Itens.Count <= 2, new { @class = "text ddlItens" }))%>
		</div>
	</div>

</fieldset>
	
<fieldset class="fsSubitens block box" id="fsSubitens">
	<legend>Subitens</legend>

<% if (!Model.IsVisualizar) { %>
	<div class="block" >
		<div class="coluna76">	
			<label for="Subitens">Subiten *</label><br />
			<%= Html.DropDownList("Subitens", Model.Subitens, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Subitens.Count <= 2, new { @class = "text ddlSubitens" }))%>
		</div>	
		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarSubitem btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
<% } %>
				
	<div class="block dataGrid divSubitens">
		<div class="coluna82 ">
			<table class="dataGridTable" id="gridSubiten" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="91%">Subitem</th>
						<%if(!Model.IsVisualizar){%><th width="9%">Ação</th><%}%>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Configuracao.Subitens) { %>
					<tr>
						<td>
							<span class="subitem" title="<%:item.SubItemTexto %>"><%:item.SubItemTexto%></span>
						</td>
						<% if (!Model.IsVisualizar) { %>
						<td class="tdAcoes">
							<input type="hidden" class="hdnSubItemJSON" value='<%= Model.JSON(item) %>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirSubitem" value="" />
						</td>
						<% } %>
					</tr>		
					<% } %>
				</tbody>
			</table>
			<% if (!Model.IsVisualizar) { %>
			<table style="display: none; visibility: hidden;" class="hide">
				<tr class="trSubItemTemplateRow hide">
					<td><span class="subitem"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnSubItemJSON" value="" />
						<input title="Excluir" type="button" class="icone excluir btnExcluirSubitem" value="" />
					</td>
				</tr>
			</table>
			<% } %>
		</div>
	</div>

</fieldset>

<fieldset class="fsPerguntas block box" id="fsPerguntas">
	<legend>Perguntas</legend>

<% if (!Model.IsVisualizar) { %>
	<div class="block" >
		<div class="coluna76">	
			<label for="Perguntas">Pergunta *</label><br />
			<%= Html.DropDownList("Perguntas", Model.Perguntas, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Perguntas.Count <= 2, new { @class = "text ddlPerguntas" }))%>
		</div>	
		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarPergunta btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
<% } %>
				
	<div class="block dataGrid divPerguntas">
		<div class="coluna82 ">
			<table class="dataGridTable" id="gridPerguntas" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="91%">Pergunta</th>
						<% if (!Model.IsVisualizar) { %>
						<th width="9%">Ação</th>
						<% } %>			
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Configuracao.Perguntas) { %>
					<tr>
						<td>
							<span class="pergunta" title="<%:item.PerguntaTexto %>"><%:item.PerguntaTexto %></span>
						</td>
						<% if (!Model.IsVisualizar) { %>
						<td class="tdAcoes">
							<input type="hidden" class="hdnPerguntaItemJSON" value='<%= Model.JSON(item) %>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirPergunta" value="" />
						</td>
						<% } %>
					</tr>		
					<% } %>			
				</tbody>
			</table>
			<% if (!Model.IsVisualizar) { %>
			<table style="display: none; visibility: hidden;" class="hide">
				<tr class="trPerguntaTemplateRow hide">
					<td><span class="pergunta"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnPerguntaItemJSON" value="" />
						<input title="Excluir" type="button" class="icone excluir btnExcluirPergunta" value="" />
					</td>
				</tr>
			</table>
			<% } %>
		</div>
	</div>

</fieldset>

<fieldset class="fsCampos block box" id="fsCampos">
	<legend>Campos</legend>

<% if (!Model.IsVisualizar) { %>
	<div class="block" >
		<div class="coluna76">	
			<label for="Campos">Campo *</label><br />
			<%= Html.DropDownList("Campos", Model.Campos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Campos.Count <= 2, new { @class = "text ddlCampos" }))%>
		</div>	
		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarCampo btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
<% } %>
				
	<div class="block dataGrid divCampos">
		<div class="coluna82 ">
			<table class="dataGridTable" id="gridCampos" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="91%">Campo</th>
						<% if (!Model.IsVisualizar) { %>
						<th width="9%">Ação</th>
						<% } %>			
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Configuracao.Campos) { %>
					<tr>
						<td>
							<span class="campo" title="<%:item.CampoTexto %>"><%:item.CampoTexto %></span>
						</td>
						<% if (!Model.IsVisualizar) { %>
						<td class="tdAcoes">							
							<input type="hidden" class="hdnCampoItemJSON" value='<%= Model.JSON(item) %>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirCampo" value="" />
						</td>
						<% } %>
					</tr>		
					<% } %>			
				</tbody>
			</table>
			<% if (!Model.IsVisualizar) { %>
			<table style="display: none; visibility: hidden;" class="hide">
				<tr class="trCampoTemplateRow hide">
					<td><span class="campo"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnCampoItemJSON" value="" />
						<input title="Excluir" type="button" class="icone excluir btnExcluirCampo" value="" />
					</td>
				</tr>
			</table>
			<% } %>
		</div>
	</div>

</fieldset>
