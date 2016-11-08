<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProcesso" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EditarApensadosJuntadosVM>" %>

<div class="divModalEditarApenJunt">
	<input type="hidden" class="hdnProcessoId" value="<%= Model.Processo.Id %>" />

	<fieldset class="block box">
		<legend>Processo</legend>
		<div class="coluna15">
			<label for="Processo.Numero">Número de registro</label>
			<%= Html.TextBox("Processo.Numero", Model.Processo.Numero, new { @disabled = "disabled", @class = "text disabled" })%>
		</div>
		<div class="coluna30 prepend2">
			<label for="Processo.Tipo.Texto">Tipo</label>
			<%= Html.TextBox("Processo.Tipo.Texto", Model.Processo.Tipo.Texto, new { @disabled = "disabled", @class = "text disabled" })%>
		</div>
	</fieldset>

	<div class="divDocumentos divTipo">
		<fieldset class="block box">
			<legend>Documentos Juntados</legend>

			<% if (Model.Processo.Documentos.Count > 0) { %>
			<div class="associarMultiplo">
				<div class="asmItens">
					<% for (int i = 0; i < Model.Processo.Documentos.Count; i++)  {
						Documento doc = Model.Processo.Documentos[i];
						RequerimentoVM req = Model.DocumentosRequerimentos[i];
					%>
					<div class="asmItemContainer boxBranca borders">
						<div class="divHiddenItemContainer">

							<div class="asmConteudoFixo block">
								<div class="coluna18">
									<input type="hidden" class="hdnItemId" value="<%= doc.Id %>" />
									<input type="hidden" class="hdnItemTipo" value="<%= doc.Tipo.Id %>" />
									<input type="hidden" class="hdnItemCheckListId" value="<%= doc.ChecagemRoteiro.Id %>" />
									<label for="Documentos[<%= i %>].Numero">Número de registro</label>
									<%= Html.TextBox("Documentos[" + i + "].Numero", doc.Numero, new { @disabled = "disabled", @class = "text disabled" })%>
								</div>
								<div class="coluna30 prepend2">
									<label for="Documentos[<%= i %>].Tipo.Texto">Tipo Documento</label>
									<%= Html.TextBox("Documentos[" + i + "].Tipo.Texto", doc.Tipo.Texto, new { @disabled = "disabled", @class = "text disabled" })%>
								</div>
							</div>

							<div class="asmConteudoLink">
								<div class="asmConteudoInterno hide">
									<fieldset class="block box">
										<legend>Requerimento</legend>
										<div class="block">
											<div class="coluna15">
												<label for="Requerimento.Numero">Número *</label>
												<%= Html.TextBox("Requerimento.Numero", req.Numero, new { @disabled = "disabled", @class = "text disabled txtReqNumero" })%>
											</div>
											<div class="coluna30 prepend1">
												<button type="button" class="floatLeft inlineBotao botaoBuscar btnAssociarRequerimento" title="Buscar Requerimento" id="btnBuscarRequerimento">Buscar</button>
												<button type="button" class="icone pdf inlineBotao btnGerarPdfRequerimento" title="PDF"></button>
											</div>
										</div>
									</fieldset>

									<fieldset class="block box divConteudoAtividadeSolicitada">
										<legend>Atividades Solicitadas</legend>
										<div class="block asmItens">
											<% Html.RenderPartial("RequerimentoAtividadesSolicitadas", req); %>
										</div>
									</fieldset>

									<div class="block box">
										<input class="btnSalvarGrupoRequerimento floatLeft" type="button" value="Salvar" />
									</div>
								</div>
								<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
							</div>
						</div>
					</div>
					<% } %>
				</div>
			</div>
			<% } else { %>
				<label>Não há documentos juntados</label>
			<% } %>
		</fieldset>
	</div>

	<div class="divProcessos divTipo">
		<fieldset class="block box">
			<legend>Processos Apensados</legend>

			<% if (Model.Processo.Processos.Count > 0) { %>
			<div class="associarMultiplo">
				<div class="asmItens">
					<% for (int i = 0; i < Model.Processo.Processos.Count; i++)  {
						Processo proc = Model.Processo.Processos[i];
						RequerimentoVM req = Model.ProcessosRequerimentos[i];
					%>
					<div class="asmItemContainer boxBranca borders">
						<div class="divHiddenItemContainer">

							<div class="asmConteudoFixo block">
								<div class="coluna15">
									<input type="hidden" class="hdnItemId" value="<%= proc.Id %>" />
									<input type="hidden" class="hdnItemTipo" value="<%= proc.Tipo.Id %>" />
									<input type="hidden" class="hdnItemCheckListId" value="<%= proc.ChecagemRoteiro.Id %>" />
									<label for="Processos[<%= i %>].Numero">Número de registro</label>
									<%= Html.TextBox("Processos[" + i + "].Numero", proc.Numero, new { @disabled = "disabled", @class = "text disabled" })%>
								</div>
								<div class="coluna30 prepend2">
									<label for="Processos[<%= i %>].Tipo.Texto">Tipo Processo</label>
									<%= Html.TextBox("Processos[" + i + "].Tipo.Texto", proc.Tipo.Texto, new { @disabled = "disabled", @class = "text disabled" })%>
								</div>
							</div>

							<div class="asmConteudoLink">
								<div class="asmConteudoInterno hide">
									<fieldset class="block box">
										<legend>Requerimento</legend>
										<div class="block">
											<div class="coluna15">
												<label for="Requerimento.Numero">Número *</label>
												<%= Html.TextBox("Requerimento.Numero", req.Numero, new { @disabled = "disabled", @class = "text disabled txtReqNumero" })%>
											</div>
											<div class="coluna30 prepend1">
												<button type="button" class="floatLeft inlineBotao botaoBuscar btnAssociarRequerimento" title="Buscar Requerimento" id="Button1">Buscar</button>
												<button type="button" class="icone pdf inlineBotao btnGerarPdfRequerimento" title="PDF"></button>
											</div>
										</div>
									</fieldset>

									<fieldset class="block box divConteudoAtividadeSolicitada">
										<legend>Atividades Solicitadas</legend>
										<div class="block asmItens">
											<% Html.RenderPartial("RequerimentoAtividadesSolicitadas", req); %>
										</div>
									</fieldset>

									<div class="block box">
										<input class="btnSalvarGrupoRequerimento floatLeft" type="button" value="Salvar" />
									</div>
								</div>
								<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
							</div>
						</div>
					</div>
					<% } %>
				</div>
			</div>
			<% } else { %>
				<label>Não há processos apensados</label>
			<% } %>
		</fieldset>
	</div>
</div>