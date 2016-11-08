<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EstanteVM>" %>

<fieldset class="block box fsEstante">
	<legend>Estante</legend>
	<div class="divConteudoEstante boxBranca borders">
		<p class="block">
			<%if (!Model.IsVisualizar) { %>
				<a class="btnAsmExcluir fecharMensagem" title="Excluir estante">Excluir</a></p>
			<%} %>
		<div class="block">
			<label>
				Nome da estante *</label>
			<%= Html.TextBox("EstanteNome", Model.Texto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "50", @class = "text txtEstanteNome" }))%>
			<input type="hidden" class="hdnEstanteId asmItemId" value="<%= Model.Id == 0 ? -1 : Model.Id %>" />
		</div>
		<div class="divConteudoModo block">
			<div class="divPrateleiaItens">
				<fieldset class="block box fsPrateleira">
					<legend>Prateleira/Pasta Supensa</legend>
					<%if (!Model.IsVisualizar) { %>
					<div class="block">
						<div class="coluna80">
							<label>Modo *</label>
							<%= Html.DropDownList("Modos", Model.Modos, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "text ddlModo" }))%>
						</div>
					</div>
					
					<div class="block">
						<div class="coluna80">
							<label>Identificação *</label>
							<%= Html.TextBox("Identificacao", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @maxlength = "50", @class = "text txtIdentificacao" }))%>
						</div>
						
						<div class="coluna15">
							<button type="button" style="width: 35px" class="btnAddPrateleira inlineBotao botaoAdicionarIcone"
								title="Adicionar">
								Adicionar</button>
						</div>
					</div>
					<%} %>
					<div class="block dataGrid">
						<table class="dataGridTable tabPrateleira" width="100%" border="0" cellspacing="0" cellpadding="0">
							<thead>
								<tr>
									<th width="20%">Modo</th>
									<th>Identificação</th>
									<%if (!Model.IsVisualizar)
									{ %>
									<th width="7%">Ações</th>
									<%} %>
								</tr>
							</thead>
							<tbody>
								<%foreach (var prateleira in Model.Prateleiras) {%>
								<tr>
									<td>
										<span class="trModoTexto" title="<%= Html.Encode(prateleira.ModoTexto) %>"><%= Html.Encode(prateleira.ModoTexto)%></span> 
									</td>
									<td>
										<span class="trIdentificacaoTexto" title="<%= Html.Encode(prateleira.Texto) %>"><%= Html.Encode(prateleira.Texto)%></span>
									</td>
									<%if (!Model.IsVisualizar)
									{ %>
									<td>
										<input type="hidden" class="hdnIdRelacionamento" value="<%= prateleira.Id %>" />
										<input type="hidden" class="hdnModoId" value="<%= prateleira.ModoId %>" />
										<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
									</td>
									<%} %>
								</tr>
								<% } %>
							</tbody>
						</table>
						<table style="display: none">
							<tbody>
								<tr class="templatePrateleira hide">
									<td>
										<span class="trModoTexto"></span>
									</td>
									<td>
										<span class="trIdentificacaoTexto"></span>
									</td>
									<td>
										<input type="hidden" class="hdnIdRelacionamento" value="0" />
										<input type="hidden" class="hdnModoId" value="0" />
										<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
									</td>
								</tr>
							</tbody>
						</table>
					</div>
				</fieldset>
			</div>
		</div>
	</div>
</fieldset>

