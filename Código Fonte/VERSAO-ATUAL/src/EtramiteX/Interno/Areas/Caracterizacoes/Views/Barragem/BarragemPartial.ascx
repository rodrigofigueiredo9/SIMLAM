<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script>
	Barragem.settings.dependencias = '<%= ViewModelHelper.Json(Model.Barragem.Dependencias) %>';
	Barragem.settings.textoMerge = '<%= Model.TextoMerge %>';
	Barragem.settings.atualizarDependenciasModalTitulo = '<%= Model.AtualizarDependenciasModalTitulo %>';	
	Barragem.settings.mensagens = <%=Model.Mensagens%>;
	Barragem.settings.temARL = <%= Model.TemARL.ToString().ToLower() %>;
	Barragem.settings.temARLDesconhecida = <%= Model.TemARLDesconhecida.ToString().ToLower() %>;
</script>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Barragem.EmpreendimentoId %>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Barragem.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%: (int)eCaracterizacao.Barragem %>" />
<input type="hidden" class="hdnIsEditar" value="<%: Model.IsEditar.ToString().ToLower() %>" />

<div class="divBarragem">	
	<fieldset class="block box">
		<legend class="titFiltros">Barragens</legend>
		<div class="block box boxBranca">
			<div class="coluna80">
				<label for="Barragem_Atividade">Atividade *</label>
				<%= Html.DropDownList("Barragem.Atividade", Model.Atividades, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividade", @id = "ddlAtividade" }))%>
			</div>
		</div>

		<div class="divGridBarragens block">
			<table class="dataGridTable gridBarragens" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Quantidade de barragens</th>
						<th>Finalidade</th>
						<th>Área total da lâmina (ha)</th>
						<th width="23%">Volume total armazenado (m³)</th>
						<th width="15%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Barragem.Barragens) { %>
					<tr>
						<td><span class="spanQuantidade" title="<%= item.Quantidade %>"><%= item.Quantidade %></span></td>
						<td><span class="spanFinalidade" title="<%= item.FinalidadeTexto %>"><%= item.FinalidadeTexto %></span></td>
						<td><span class="spanTotalLamina" title="<%= item.ToStringTotalLamina %>"><%= item.ToStringTotalLamina %></span></td>
						<td><span class="spanTotalArmazenamento" title="<%= item.ToStringTotalArmazenado %>"><%= item.ToStringTotalArmazenado %></span></td>
						<td>
							<input type="hidden" class="hdnItemBarragem" name="hdnItemBarragem" value='<%= Model.GetJSON(item) %>' />
							<button title="Visualizar" class="icone visualizar btnVisualizar" value="" type="button"></button>
						<% if (!Model.IsVisualizar) { %>
							<button title="Editar" class="icone editar btnEditar" value="" type="button"></button>
							<button title="Excluir" class="icone excluir btnExcluirItemBarragem" value="" type="button"></button>
						<% } %>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
			<% if (!Model.IsVisualizar) { %>
			<table style="display: none">
				<tbody>
					<tr class="trBarragemTemplate">
						<td><span class="spanQuantidade" title=""></span></td>
						<td><span class="spanFinalidade" title=""></span></td>
						<td><span class="spanTotalLamina" title=""></span></td>
						<td><span class="spanTotalArmazenamento" title=""></span></td>
						<td>
							<input type="hidden" class="hdnItemBarragem" name="hdnItemBarragem" value="" />
							<button title="Visualizar" class="icone visualizar btnVisualizar" value="" type="button"></button>
							<button title="Editar" class="icone editar btnEditar" value="" type="button"></button>
							<button title="Excluir" class="icone excluir btnExcluirItemBarragem" value="" type="button"></button>
						</td>
					</tr>
				</tbody>
			</table>
			<% } %>
		</div>
		<br />

		<fieldset class="block box boxBranca">
			<legend class="titFiltros">Total do Empreendimento</legend>
			<div class="block">
				<div class="coluna40 append2">
					<label for="Barragem_TotalLamina">Área total da lâmina (ha)</label>
					<%= Html.TextBox("Barragem.ToStringTotalLamina", Model.Barragem.ToStringTotalLamina, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTotalLamina", @id = "txtTotalLamina" }))%>
				</div>
				<div class="coluna40">
					<label for="Barragem_TotalArmazenado">Volume total armazenado (m³)</label>
					<%= Html.TextBox("Barragem.ToStringTotalArmazenado", Model.Barragem.ToStringTotalArmazenado, ViewModelHelper.SetaDisabled(true, new { @class = "text txtTotalArmazenado", @id = "txtTotalArmazenado" }))%>
				</div>
			</div>
		</fieldset>
		<% if (!Model.IsVisualizar) { %>
		<div>
			<span class="spanBotoes floatRight"><input class="btnAddBarragem" type="button" value=" + Barragem " /></span>
		</div>
		<% } %>
	</fieldset>
</div>