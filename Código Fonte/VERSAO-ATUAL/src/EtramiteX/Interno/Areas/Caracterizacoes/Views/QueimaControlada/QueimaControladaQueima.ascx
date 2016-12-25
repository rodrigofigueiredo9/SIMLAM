<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QueimaControladaQueimaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	QueimaControladaQueima.settings.mensagens = <%= Model.Mensagens %>;
	QueimaControladaQueima.settings.idsTela = <%= Model.IdsTela %>;
</script>

<div class="block filtroCorpo divQueimaControladaQueima">
	
	<input type="hidden" class="hdnQueimaControladaQueimaId" value="<%:Model.Caracterizacao.Id%>" />
    <input type="hidden" class="hdnAreaCroqui" value="<%= Model.Caracterizacao.AreaCroqui %>" />

	<div class="block">
		<div class="coluna12 append2">
			<label for="Queima_Identificacao">Identificação</label>
			<%= Html.TextBox("Queima.Identificacao", Model.Caracterizacao.Identificacao, new { @class = "text txtIdentificacao disabled", disabled = "disabled" })%>
		</div>

		<div class="coluna14 append2">
			<label for="Queima_AreaCroqui">Área croqui (m²)</label>
			<%= Html.TextBox("Queima.AreaCroqui_", ((Model.Caracterizacao.AreaCroqui > 0)? Model.Caracterizacao.AreaCroqui.ToStringTrunc():null), new { @class = "text disabled", disabled = "disabled" })%>
		</div>

		<div class="coluna20 append2">
			<label for="Queima_AreaRequerida<%=Model.Caracterizacao.Identificacao%>">Área requerida (m²) *</label>
			<%= Html.TextBox("Queima.AreaRequerida" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.AreaRequerida, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaRequerida maskDecimalPonto", @maxlength = "12" }))%>
		</div>
	</div>

	<div class="asmConteudoLink block">
		<div class="asmConteudoInterno block">
			<% if(!Model.IsVisualizar) { %>
			<div class="block">
				<div class="coluna20 append2 divTipoCultivo">
					<label for="Queima_TipoCultivo<%=Model.Caracterizacao.Identificacao%>">Tipo do cultivo *</label>
					<%= Html.DropDownList("Queima.TipoCultivo" + Model.Caracterizacao.Identificacao, Model.TipoCultivo, new { @class = "text ddlTipoCultivo " })%>
				</div>

				<div class="coluna29 append2 divFinalidadeNome">
					<label for="Queima_FinalidadeNome<%=Model.Caracterizacao.Identificacao%>">Nome da Finalidade *</label>
					<%= Html.TextBox("Queima.FinalidadeNome" + Model.Caracterizacao.Identificacao, String.Empty, new { @class = "text txtFinalidadeNome ", @maxlength = "80" })%>
				</div>

				<div class="coluna16 append1">
					<label for="Queima_AreaQueima<%=Model.Caracterizacao.Identificacao%>">Área Queima (m²) *</label>
					<%= Html.TextBox("Queima.AreaQueima" + Model.Caracterizacao.Identificacao, String.Empty, new { @class = "text txtAreaAreaQueima maskDecimalPonto", @maxlength = "12" })%>
				</div>

				<div class="coluna10">
					<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarQueima btnAddItem" title="Adicionar">+</button>
				</div>
			</div>
			<%} %>

			<div class="block coluna65 dataGrid">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th>Tipo de cultivo </th>
							<th width="20%">Área de queima</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var queima in Model.Caracterizacao.Cultivos){ %>
					<tbody>
						<tr>
							<td>
								<span class="tipoCultivo" title="<%:queima.CultivoTipoTexto%>"><%:queima.CultivoTipoTexto %></span>
							</td>
							<td>
								<span class="areaQueima" title="<%:queima.AreaQueimaTexto%>"><%:queima.AreaQueimaTexto%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(queima)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirQueima" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="tipoCultivo"></span></td>
								<td><span class="areaQueima"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirQueima" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
		<a class="linkVejaMaisCampos ativo"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ocultar detalhes</span></a>
	</div>

</div>