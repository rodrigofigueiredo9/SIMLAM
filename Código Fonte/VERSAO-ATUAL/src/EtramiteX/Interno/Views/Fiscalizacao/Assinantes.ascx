<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoAssinanteVM>" %>

<fieldset class="block box fdsAssinante" id="blocoAssinante">
	<legend>Assinantes do Laudo de Fiscalização</legend>

	<%if(!Model.IsVisualizar) {%>

	<div class="block">
		<div class="coluna70 append2">
			<label >Setor assinante *</label>
			<%= Html.DropDownList("Setor", Model.Setores, ViewModelHelper.SetaDisabled(Model.Setores.Count <= 1, new { @class = "text ddlAssinanteSetores" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna70 append2">
			<label >Cargo assinante *</label>
			<%= Html.DropDownList("Cargo", Model.Cargos, ViewModelHelper.SetaDisabled(Model.Cargos.Count <= 1, new { @class = "text ddlAssinanteCargos" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna70 append2">
			<label >Nome assinante *</label>
			<%= Html.DropDownList("Funcionario", Model.Funcionarios, ViewModelHelper.SetaDisabled(Model.Funcionarios.Count <= 1, new { @class = "text ddlAssinanteFuncionarios" }))%>
		</div>
		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarAssinante btnAddItem" title="Adicionar">+</button>
		</div>
	</div>

	<% }  %>

	<div class="block dataGrid">
		<div class="coluna70">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="54%">Nome</th>
						<th width="37%">Cargo</th>
						<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
					</tr>
				</thead>
				<% foreach (var item in Model.Assinantes){ %>
				<tbody>
					<tr>
						<td>
							<span class="Funcionario" title="<%:item.FuncionarioNome%>"><%:item.FuncionarioNome%></span>
						</td>
						<td>
							<span class="Cargo" title="<%:item.FuncionarioCargoNome%>"><%:item.FuncionarioCargoNome%></span>
						</td>
						<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirAssinante" value="" />
							</td>
						<%} %>
					</tr>
					<% } %>
					<% if(!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="Funcionario"></span></td>
							<td><span class="Cargo"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluirAssinante" value="" />
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>
