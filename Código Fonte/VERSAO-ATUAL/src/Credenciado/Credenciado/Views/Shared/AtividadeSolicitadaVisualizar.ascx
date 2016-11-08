<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtividadeSolicitadaVM>" %>

<div class="conteudoAtividadeSolicitada boxBranca borders">
	<div class="block">
		<div class="coluna<%= Model.IsRequerimento ? "100": "69" %>  ultima">
			<label>Nome *</label>
			<%= Html.TextBox("nomeAtividadade", Model.NomeAtividade, new { @disabled = "disabled", @maxlength = "15", @class = "text nomeAtividade  disabled" })%>
		</div>
		<% if (!Model.IsRequerimento){%>
		<div class="coluna25 prepend2">
			<label>Situação *</label>
			<%= Html.TextBox("situacaoAtividadade", Model.SituacaoTexto, new { @disabled = "disabled", @class = "text txtSituacaoTexto disabled" })%>
		</div>
		<%} %>
	</div>
	<div class="divFinalidadeConteudo">
		<fieldset class="margem0">
		<legend class="small quiet margem0">Finalidade | Título</legend>
			<ul class="listaObjetos">
				<%foreach (var item in Model.Finalidades) {%>
					<li>
						<p class="finalidadeTexto"><%= item.Texto %></p><span>|</span>
						<p class=" tituloModeloTexto"><%= item.TituloModeloTexto %></p>
						<%if (item.TituloModeloAnteriorTexto != null && item.TituloModeloAnteriorTexto != string.Empty)
						{ %>
						<a class="divDetalhes" title="Clique para mais Detalhes">(+ Detalhes)</a>
						<% } %>
						<div class="maisInfo paddingT10 hide coluna100">
							<input type="hidden" class="hdnModeloTituloAnterior" value="<%= item.TituloModeloAnteriorId %>" /> 
							<div class="coluna99"><p class="quiet">Título anterior: <span class="tituloAnterior"><%= item.TituloModeloAnteriorTexto %></span></p></div>
							<div class="coluna99"><p class="quiet">Nº do título anterior: <span class="tituloAnteriorTexto"><%= item.TituloAnteriorNumero%></span></p></div>
							<% if (item.OrgaoExpedidor != "") { %>
							<div>
								<span class="coluna99"><p class="quiet">Órgão expedidor: <span class="orgaoExpedidor"><%= item.OrgaoExpedidor%></span></p></span>
							</div>
							<% } %>
						</div>
					</li>
			<%} %>
			</ul>
	</fieldset>
	</div>
</div>