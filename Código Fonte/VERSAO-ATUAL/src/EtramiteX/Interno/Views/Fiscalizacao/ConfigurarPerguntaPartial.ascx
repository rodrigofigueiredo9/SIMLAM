<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PerguntaInfracaoVM>" %>

<input type="hidden" class="hdnPerguntaId" value="<%:Model.Entidade.Id %>" />
<input type="hidden" class="hdnSituacaoId" value="<%:Model.Entidade.SituacaoId %>" />

<fieldset class="box">
	<div class="block">
		<div class="coluna67 append1">
			<label for="Pergunta_Pergunta">Pergunta *</label>
			<%= Html.TextBox("Pergunta.Pergunta", Model.Entidade.Texto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPergunta", @maxlength = "100" }))%>
		</div>
	</div>

	<%if (!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna45 append2">
			<label for="Pergunta_Resposta">Resposta *</label>
			<%= Html.DropDownList("Pergunta.Resposta", Model.Respostas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlResposta" }))%>
		</div>

		<div class="coluna13">
			<label for="Pergunta_EspecificarResposta">Especificar *</label><br />
			<label><%= Html.RadioButton("Pergunta.EspecificarResposta", 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbEspecificarResposta" }))%>Sim</label>
			<label><%= Html.RadioButton("Pergunta.EspecificarResposta", 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbEspecificarResposta" }))%>Não</label>
		</div>

		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarResposta btnAddItem" title="Adicionar resposta">+</button>
		</div>
	</div>
	<br />
	<%} %>

	<div class="DivItens">
		<div class="block dataGrid">
			<div class="coluna67">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="40%">Resposta</th>
							<th width="12%">Especificar</th>
							<%if (!Model.IsVisualizar){%><th width="7%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var item in Model.Entidade.Respostas){ %>
					<tbody>
						<tr>
							<td>
								<span class="resposta" title="<%:item.Texto%>"><%:item.Texto%></span>
							</td>
							<td>
								<span class="especificar" title="<%:item.EspecificarTexto%>"><%:item.EspecificarTexto%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input type="hidden" value="<%= item.Id %>" class="itemId" />
								<input title="Excluir item" type="button" class="icone excluir btnExcluirResposta" value="" />
							</td>
							<%} %>
							<%} %>
						</tr>
						<%if (!Model.IsVisualizar){%>
						<tr class="trTemplateRow hide">
							<td><span class="resposta"></span></td>
							<td><span class="especificar"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir item" type="button" class="icone excluir btnExcluirResposta" value="" />
							</td>
						</tr>
						<%} %>
					</tbody>
				</table>
			</div>
		</div>
	</div>
</fieldset>

<div class="block box">
	<%if (Model.IsVisualizar){%>
		<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("ConfigurarPerguntasListar", "Fiscalizacao") %>">Cancelar</a></span>
	<%}else{ %>
		<input class="floatLeft btnSalvar" type="button" value="Salvar" />
		<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("ConfigurarPerguntasListar", "Fiscalizacao") %>">Cancelar</a></span>
	<%} %>
</div>