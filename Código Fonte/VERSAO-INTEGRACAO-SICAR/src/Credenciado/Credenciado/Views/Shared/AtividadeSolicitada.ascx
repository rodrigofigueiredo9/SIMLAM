<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtividadeSolicitadaVM>" %>

<div class="conteudoAtividadeSolicitada boxBranca borders">
	<% if (!Model.IsRequerimentoVisualizar && !Model.RetirarIconeExcluirAtividade){%>
	<p class="block"><a class="btnAsmExcluir btnRemoverAtividadeSolicitada modoVisualizar fecharMensagem" title="Excluir atividade">Excluir</a></p>
	<%} %>
	<div class="block">
		<div class="coluna<%= Model.IsRequerimento ? "87": "69" %>  ultima">
			<label>Nome *</label>
			<input type="hidden" class="hdnAtividadeId asmItemId" value="<%=Model.Id %>" />
			<input type="hidden" class="hdnAtividadeSetorId" value="<%=Model.SetorId %>" />
			<input type="hidden" class="hdnAtividadeRelId" value="<%=Model.IdRelacionamento %>" />
			<input type="hidden" class="hdnAtividadeTexto asmItemTexto" value="<%= Model.NomeAtividade %>" />

			<input type="hidden" class="hdnProtocoloId" value="<%=Model.Protocolo.Id %>" />
			<input type="hidden" class="hdnProtocoloTipo" value="<%=Model.Protocolo.IsProcesso %>" />

			<%= Html.TextBox("nomeAtividadade", Model.NomeAtividade, new { @disabled = "disabled", @maxlength = "15", @class = "text nomeAtividade  disabled" })%>
		</div>
		<% if (!Model.IsRequerimento){%>
		<div class="coluna25 prepend2">
			<label>Situação *</label>
			<input type="hidden" class="hdnSituacaoId" value="<%=Model.Situacao %>" />
			<%= Html.TextBox("situacaoAtividadade", Model.SituacaoTexto, new { @disabled = "disabled", @class = "text txtSituacaoTexto disabled" })%>
		</div>
		<%} %>
		<% if (Model.IsRequerimento && !Model.IsRequerimentoVisualizar){%>
		<div class="coluna10 prepend2">
			<button title="Adicionar atividade" class="inlineBotao btBuscar btnAsmAssociar modoVisualizar">Buscar</button>
		</div>
		<%} %>
	</div>
	<div class="divFinalidadeConteudo <%= Model.Finalidades.Count > 0 ?"": "hide"%>">
		<% if (!Model.IsRequerimentoVisualizar){%>
		<div class="block margem0 negtBot">
			<button class="btnAssociarFinalidade botaoAdicionar modoVisualizar direita" title="Adicionar título">Título</button>
		</div>
		<%} %>
		<fieldset class="margem0">
		<legend class="small quiet margem0">Finalidade | Título</legend>
			<ul class="listaObjetos">
				<%foreach (var item in Model.Finalidades) {%>
					<li>
						<% if (!Model.IsRequerimentoVisualizar){%>
						<a class="icone excluir direita btnExcluirAtividade modoVisualizar" title="Excluir">Excluir</a>
						<input type="hidden" class="hdnIdRelacionamento" value="<%= item.IdRelacionamento%>" />
						<%} %>
						<input type="hidden" class="hdnEhRenovacao" value="<%= item.EhRenovacao%>" />
						<input type="hidden" class="hdnfinalidadeId" value="<%= item.Id %>" />
						<input type="hidden" class="hdnTituloModeloId" value="<%= item.TituloModelo%>" />
						

						<p class="finalidadeTexto"><%= item.Texto %></p><span>|</span>
						<p class=" tituloModeloTexto"><%= item.TituloModeloTexto %></p>
						<%if (item.TituloModeloAnteriorTexto != null && item.TituloModeloAnteriorTexto != string.Empty)
						{ %>
						<a class="divDetalhes" title="Clique para mais Detalhes">(+ Detalhes)</a>
						<% } %>
						<div class="maisInfo paddingT10 hide coluna100">
							<input type="hidden" class="hdnTituloAnteriorOrigem" value="<%= item.TituloAnteriorTipo%>" />
							<input type="hidden" class="hdnTituloAnteriorId" value="<%= item.TituloAnteriorId %>" />
							<input type="hidden" class="hdnEmitidoPorInterno" value="<%= item.EmitidoPorInterno %>" />
							<input type="hidden" class="hdnModeloTituloAnterior" value="<%= item.TituloModeloAnteriorId %>" /> 
							<input type="hidden" class="hdnModeloTituloAnteriorSigla" value="<%= item.TituloModeloAnteriorSigla %>" /> 

							<div class="coluna99"><p class="quiet">Nº do título anterior: <span class="numeroDocumentoAnterior"><%= item.TituloAnteriorNumero %></span></p></div>
							<div class="coluna99"><p class="quiet">Título anterior: <span class="tituloAnteriorTexto"><%= item.TituloModeloAnteriorTexto %></span></p></div>
							<% if (item.OrgaoExpedidor != "") { %>
							<div class="divOrgaoExpedidor">
								<span class="coluna99"><p class="quiet">Órgão expedidor: <span class="orgaoExpedidor"><%= item.OrgaoExpedidor%></span></p></span>
							</div>
							<% } %>
						</div>
					</li>
			<%} %>
			</ul>
<ul>
	<li class="hide templateFinalidade">
		<a class="icone excluir direita btnExcluirAtividade" title="Excluir">Excluir</a>
		<input type="hidden" class="hdnIdRelacionamento" value="0" />
		<input type="hidden" class="hdnEhRenovacao" value="false" />
		<input type="hidden" class="hdnfinalidadeId" value="0" />
		<input type="hidden" class="hdnTituloModeloId" value="0" />
		<p class="finalidadeTexto"></p><span>|</span>
		<p class="tituloModeloTexto"></p>
		<a class="divDetalhes" title="Clique para mais Detalhes">(+ Detalhes)</a>
		<div class="maisInfo paddingT10 hide coluna100">
			<input type="hidden" class="hdnTituloAnteriorOrigem" value="0" />
			<input type="hidden" class="hdnTituloAnteriorId" value="0" />
			<input type="hidden" class="hdnEmitidoPorInterno" value="false" />
			<input type="hidden" class="hdnModeloTituloAnterior" value="0" />
			<input type="hidden" class="hdnModeloTituloAnteriorSigla" value="" />
			<div class="coluna99"><p class="quiet">Nº do título anterior: <span class="numeroDocumentoAnterior"></span></p></div>
			<div class="coluna99"><p class="quiet">Título anterior: <span class="tituloAnteriorTexto"></span></p></div>
			<div class="divOrgaoExpedidor hide">
				<span class="coluna99"><p class="quiet">Órgão expedidor: <span class="orgaoExpedidor"></span></p></span>
			</div>
		</div>
	</li>
</ul>
	</fieldset>
	</div>
</div>