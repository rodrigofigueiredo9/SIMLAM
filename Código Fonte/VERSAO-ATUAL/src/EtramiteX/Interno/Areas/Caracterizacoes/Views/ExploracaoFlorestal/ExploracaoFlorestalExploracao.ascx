<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ExploracaoFlorestalExploracaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	ExploracaoFlorestalExploracao.settings.mensagens = <%= Model.Mensagens %>;
</script>

<div class="block filtroCorpo divExploracaoFlorestalExploracao">
	<input type="hidden" class="hdnGeometriaId" value="<%:Model.ExploracaoFlorestal.GeometriaTipoId%>" />
	<input type="hidden" class="hdnExploracaoId" value="<%:Model.ExploracaoFlorestal.Id%>" />
	<input type="hidden" class="hdnClassificacaoVegetacaoId" value="<%:Model.ExploracaoFlorestal.ClassificacaoVegetacaoId%>" />

	<div class="block">
		<div class="coluna22 append2">
			<label for="ExploracaoFlorestal_Identificacao">Identificação</label>
			<%= Html.TextBox("ExploracaoFlorestal.Identificacao", Model.ExploracaoFlorestal.Identificacao, new { @class = "text txtIdentificacao disabled", disabled = "disabled" })%>
		</div>

		<div class="coluna22 append2">
			<label for="ExploracaoFlorestal_Geometrias">Geometria</label>
			<%= Html.TextBox("ExploracaoFlorestal.Geometrias", Model.ExploracaoFlorestal.GeometriaTipoTexto, new { @class = "text txtGeometria disabled", disabled = "disabled" })%>
		</div>

		<%if(Model.ExploracaoFlorestal.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono) {%>
		<div class="coluna22 append2">
			<label for="ExploracaoFlorestal_Exploracoes_AreaCroqui">Área da atividade croqui (m²)</label>
			<%= Html.TextBox("ExploracaoFlorestal.Exploracoes.AreaCroqui", Model.ExploracaoFlorestal.AreaCroqui.ToStringTrunc(), new { @class = "text txtAreaCroqui disabled maskDecimalPonto", disabled = "disabled" })%>
            <input type="hidden" class="hdnAreaCroqui" value="<%= Model.ExploracaoFlorestal.AreaCroqui %>" />
		</div>
		<%}else{%>
		<div class="coluna22 append2">
			<label for="ExploracaoFlorestal_Exploracoes_QuantidadeArvores<%: Model.ExploracaoFlorestal.Identificacao%>">N° de árvores</label>
			<%= Html.TextBox("ExploracaoFlorestal.Exploracoes.QuantidadeArvores" + Model.ExploracaoFlorestal.Identificacao, Model.ExploracaoFlorestal.QuantidadeArvores, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtQuantidadeArvores maskInteger", @maxlength = "6" }))%>
		</div>
		<%}%>

		<div class="coluna24">
			<label for="ExploracaoFlorestal_ClassificacaoVegetal<%: Model.ExploracaoFlorestal.Identificacao%>">Classificação da vegetação *</label>
			<%= Html.DropDownList("ExploracaoFlorestal.ClassificacaoVegetal" + Model.ExploracaoFlorestal.Identificacao, Model.ClassificacoesVegetais, new { @class = "text ddlClassificacoesVegetais disabled", disabled = "disabled" })%>
		</div>
	</div>

	<div class="asmConteudoLink block">
		<div class="asmConteudoInterno block">
			<div class="block">
				<%if(Model.ExploracaoFlorestal.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono) {%>
				<div class="coluna22 append2">
					<label for="ExploracaoFlorestal_Exploracoes_AreaRequerida<%: Model.ExploracaoFlorestal.Identificacao%>">Área requerida (m²) *</label>
					<%= Html.TextBox("ExploracaoFlorestal.Exploracoes.AreaRequerida" + Model.ExploracaoFlorestal.Identificacao, Model.ExploracaoFlorestal.AreaRequerida, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaRequerida maskDecimalPonto", @maxlength = "12" }))%>
				</div>
				<%}else{%>
				<div class="coluna22 append2">
					<label for="ExploracaoFlorestal_Exploracoes_ArvoresRequeridas<%: Model.ExploracaoFlorestal.Identificacao%>">N° de árvores requeridas *</label>
					<%= Html.TextBox("ExploracaoFlorestal.Exploracoes.ArvoresRequeridas" + Model.ExploracaoFlorestal.Identificacao, Model.ExploracaoFlorestal.ArvoresRequeridas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtArvoresRequeridas maskInteger", @maxlength = "8" }))%>
				</div>
				<%}%>

				<div class="coluna22">
					<label for="ExploracaoFlorestal_ExploracaoTipo<%: Model.ExploracaoFlorestal.Identificacao%>">Tipo de exploração *</label>
					<%= Html.DropDownList("ExploracaoFlorestal.ExploracaoTipo" + Model.ExploracaoFlorestal.Identificacao, Model.ExploracaoTipos, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlExploracaoTipo " }))%>
				</div>
			</div>

			<% if(!Model.IsVisualizar) { %>
			<div class="block">
				<div class="coluna22 append2">
					<label for="Produto<%= Model.ExploracaoFlorestal.Identificacao%>">Produto *</label>
					<%= Html.DropDownList("Produto" + Model.ExploracaoFlorestal.Identificacao, Model.Produtos, ViewModelHelper.SetaDisabled( Model.ExploracaoFlorestal.Produtos.Count(x=> x.ProdutoId == 7) > 0, new { @class = "text ddlProduto" }))%>
				</div>

				<div class="coluna22 append2 divQuantidade">
					<label for="Quantidade<%= Model.ExploracaoFlorestal.Identificacao%>">Quantidade *</label>
					<%= Html.TextBox("Quantidade" + Model.ExploracaoFlorestal.Identificacao, String.Empty, ViewModelHelper.SetaDisabled( Model.ExploracaoFlorestal.Produtos.Count(x=> x.ProdutoId == 7) > 0, new { @class = "text txtQuantidade maskDecimalPonto", @maxlength = "12" }))%>
				</div>

				<div class="coluna10">
					<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAdicionarProduto btnAddItem <%= Model.ExploracaoFlorestal.Produtos.Count(x=> x.ProdutoId == 7) > 0? "hide":"" %> " title="Adicionar">+</button>
				</div>
			</div>
			<% } %>

			<div class="block coluna55 dataGrid" id="exploracaoProduto<%= Model.ExploracaoFlorestal.Identificacao%>">
				<table class="dataGridTable tabExploracaoFlorestalExploracaoProduto" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th>Produto </th>
							<th width="20%">Quantidade</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var produto in Model.ExploracaoFlorestal.Produtos) { %>
						<tr >
							<td>
								<span class="produto" title="<%:produto.ProdutoTexto%>"><%: produto.ProdutoTexto%></span>
							</td>
							<td>
								<% var qtd = (String.IsNullOrEmpty(produto.Quantidade) ? "" : Convert.ToDecimal(produto.Quantidade).ToString("N2")); %>
								<span class="quantidade" title="<%:qtd%>"><%: qtd %></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(produto)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirProduto" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="produto"></span></td>
								<td><span class="quantidade"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirProduto" value="" />
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