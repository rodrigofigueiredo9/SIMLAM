<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfigurarVM>" %>

<fieldset class="divConfigurarTamitacao block box">
	<legend>Setores</legend>

	<% if(Model != null) { %>
	<div class="associarMultiplo">
		<div class="asmItens">
			<% for (int i = 0; i < Model.Setores.Count; i++) { %>
				<div class="asmItemTemplateContainer asmItemContainer boxBranca borders">
					<div class="divHiddenItemContainer block">

						<div class="asmConteudoFixo block">
							<div class="coluna50">
								<input type="hidden" class="hdnItemId" value="<%= Model.Setores[i].Id %>" />
								<input type="hidden" class="hdnItemSigla" value="<%= Model.Setores[i].Sigla %>" />
								<input type="hidden" class="hdnItemIdRelacao" value="<%= Model.Setores[i].IdRelacao %>" />
								<label for="Setores[<%= i %>].Nome">Setor</label>
								<%= Html.TextBox("Setores[" + i + "].Nome", Model.Setores[i].Nome, new { @disabled = "disabled", @class = "text disabled" })%>
							</div>
						</div>

						<div class="asmConteudoInterno hide">
							<div class="block">
								<div class="ultima">
									<label>Tramitação por registro?</label>
									<label class="prepend1"><input type="radio" class="radio rdbTipoTramitacao rdbNao" <%= Model.Setores[i].TramitacaoTipoId != 2 ? "checked=\"checked\"" : ""  %> name="Setores[<%= i %>].TramitacaoTipoId" />Não</label>
									<label class="prepend1"><input type="radio" class="radio rdbTipoTramitacao rdbSim" <%= Model.Setores[i].TramitacaoTipoId == 2 ? "checked=\"checked\"" : ""  %> name="Setores[<%= i %>].TramitacaoTipoId" />Sim</label>
								</div>
							</div>

							<div class="divTramitacaoRegistro <%= Model.Setores[i].TramitacaoTipoId != 2 ? "hide" : ""  %>">
								<div class="block">
									<div class="ultima">
										<button type="button" class="floatRight inlineBotao botaoBuscar btnAssociarFuncionario">Buscar</button>
									</div>
								</div>

								<div class="block">
									<table class="dataGridTable tabFuncionarios" width="100%" border="0" cellspacing="0" cellpadding="0">
										<thead>
											<tr>
												<th>Funcionário executor da tramitação</th>
												<th width="10%">Ações</th>
											</tr>
										</thead>
										<tbody>
											<% for (int j = 0; j < Model.Setores[i].Funcionarios.Count; j++) {
												var funcionario = Model.Setores[i].Funcionarios[j]; %>
											<tr class="<%= (j % 2) == 0 ? "par" : "impar"  %>">
												<td title="<%= Html.Encode(funcionario.Texto)%>">
													<input type="hidden" class="hdnFuncId" value="<%= funcionario.Id %>" />
													<input type="hidden" class="hdnFuncNome" value="<%= funcionario.Texto %>" />
													<%= Html.Encode(funcionario.Texto)%>
												</td>
												<td>
													<input title="Excluir" type="button" class="icone excluir btnExcluirFunc"/>
												</td>
											</tr>
											<% } %>
										</tbody>
									</table>
								</div>
							</div>
						</div>
						<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
					</div>
				</div>
			<% } %>
		</div>
	</div>
	<% } %>

	<table style="display: none">
		<tbody>
			<tr class="trFuncTemplate">
				<td>
					<input type="hidden" class="hdnFuncId" value="" />
					<input type="hidden" class="hdnFuncNome" value="" />
					<span class="trFuncNome">NOME</span>
				</td>
				<td>
					<input title="Excluir" type="button" class="icone excluir btnExcluirFunc"/>
				</td>
			</tr>
		</tbody>
	</table>
</fieldset>