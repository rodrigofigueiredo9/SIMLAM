<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AgrotoxicoCulturaVM>" %>

<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/listar.js") %>"></script>

<script>
	AgrotoxicoCultura.settings.urls.listarCulturas = '<%= Url.Action("AssociarCultura", "ConfiguracaoVegetal", new { straggCultivar = true })%>';
	AgrotoxicoCultura.settings.urls.listarPragas = '<%= Url.Action("AssociarPraga", "ConfiguracaoVegetal")%>';
	AgrotoxicoCultura.settings.mensagens = <%=Model.Mensagens%>;
</script>

<h1 class="titTela">Adicionar Cultura</h1>
<br />

<fieldset class="block box">
	<%= Html.Hidden("Agrotoxico.Id", Model.AgrotoxicoCultura.IdRelacionamento, new { @class = "hdnAgrotoxicoCulturaIdRelacionamento" })%>

	<div class="block">
		<div class="coluna60">
			<label for="Cultura">Cultura *</label>
			<%= Html.TextBox("AgrotoxicoCultura.Cultura.Nome", Model.AgrotoxicoCultura.Cultura.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCulturaNome", @maxlength = "80" }))%>
			<%=Html.Hidden("CulturaId", Model.AgrotoxicoCultura.Cultura.Id, new { @class="hdnCulturaId"})%>
		</div>

		<%if (!Model.IsVisualizar){%>
		<div class="coluna20 prepend2">
			<button class="inlineBotao btnBuscarCultura" type="button" value="Buscar"><span>Buscar</span></button>
		</div>
		<%} %>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Praga</legend>

	<%if (!Model.IsVisualizar){%>
	<div class="block ">
		<div class="coluna98 prepend2">
			<button class="inlineBotao btnBuscarPraga floatRight" type="button" value="Buscar"><span>Adicionar</span></button>
		</div>
	</div>
	<%} %>

	<div class="block">
		<table class="dataGridTable gridPraga" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Praga</th>
					<%if (!Model.IsVisualizar){%><th class="semOrdenacao" width="5%">Ações</th><% } %>
				</tr>
			</thead>
			<tbody>
				<%foreach (var item in Model.AgrotoxicoCultura.Pragas){%>
				<tr>
					<td><label class="lblNome" title="<%=item.NomeCientifico %>"><%=item.NomeCientifico%> </label></td>
					<%if (!Model.IsVisualizar){%>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value='<%=ViewModelHelper.Json(item)  %>' class="hdnItemJson" />
					</td>
					<%} %>
				</tr>
				<% } %>
				<%if (!Model.IsVisualizar){%>
				<tr class="trTemplate hide">
					<td><label class="lblNome"></label></td>
					<td>
						<a class="icone excluir btnExcluir"></a>
						<input type="hidden" value="" class="hdnItemJson" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Modalidade de aplicação</legend>

	<div class="block modalidadesAplicacao">
		<%foreach (var item in Model.ModalidadesAplicacoes){ %>
		<div class="coluna30 floatLeft">
			<label><%=Html.CheckBox("AgrotoxicoModalidadeAplicacao", Model.AgrotoxicoCultura.ModalidadesAplicacao.Count(x => x.Id == item.Id) > 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="cbModalidadeAplicacao"}))%> <%=item.Texto %> </label>
			<input type="hidden" value='<%=ViewModelHelper.Json(item)  %>' class="hdnItemJson" />
		</div>
		<%}%>
	</div>
</fieldset>

<fieldset class="block box">
	<div class="block ultima coluna20">
		<label>Intervalo de segurança (Dias) *</label>
		<%=Html.TextBox("AgrotoxicoCultura.IntervaloSeguranca", Model.AgrotoxicoCultura.IntervaloSeguranca, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="txtIntervaloSeguranca text", @maxlength = "3" }))%>
	</div>
</fieldset>