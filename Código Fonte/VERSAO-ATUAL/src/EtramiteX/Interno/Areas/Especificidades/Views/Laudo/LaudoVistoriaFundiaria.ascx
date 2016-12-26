<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFundiariaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Laudo/laudoVistoriaFundiaria.js") %>"></script>

<script>
	LaudoVistoriaFundiaria.urlObterDadosLaudoVistoriaFundiaria = '<%= Url.Action("ObterDadosLaudoVistoriaFundiaria", "Laudo", new {area="Especificidades"}) %>';
	LaudoVistoriaFundiaria.mensagens = <%=Model.Mensagens%>
</script>

<input type="hidden" class="hdnRegularizacaoFundiariaId" value="<%=Model.Laudo.RegularizacaoId %>" />

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna75">
			<label for="Laudo_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Laudo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text ddlDestinatarios" }))%>
		</div>
	</div>

	<br />

	<div class="block">
			<div class="block">
			<% if (!Model.IsVisualizar) { %>
				<div class="coluna68 append1">
					<label>Comprovação de Posse - Área croqui *</label>
					<%= Html.DropDownList("Laudo.RegularizacaoDominio", Model.Posses, new { @class = "text ddlRegularizacaoDominio" })%>
				</div>

				<div class="coluna5 append1">
					<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAddRegularizacaoDominio" title="Adicionar">+</button>
				</div>

				<div class="coluna15">
					<label for="Laudo_DataVistoria_DataTexto">Data da Vistoria *</label><br />
					<%= Html.TextBox("Laudo.DataVistoria.DataTexto", Model.Laudo.DataVistoria.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea text maskData txtDataVistoria" }))%>
				</div>
			<% } %>
			</div>

		<div class="block dataGrid coluna75">
			<table class="dataGridTable dgRegularizacaoDominio" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Comprovação de Posse - Área croqui</th>
						<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
					</tr>
				</thead>
				<% foreach (RegularizacaoDominio dominio in Model.Laudo.RegularizacaoDominios) { %>
				<tbody>
					<tr>
						<td>
							<span class="comprovacao" title="<%: dominio.ComprovacaoAreaCroqui %>"><%: dominio.ComprovacaoAreaCroqui %></span>
						</td>
						<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnRegularizacaoDominioId" value='<%: dominio.Id %>' />
								<input type="hidden" class="hdnDominioId" value='<%: dominio.DominioId %>' />
								<input type="hidden" class="hdnItemJSON" value='<%: ViewModelHelper.Json(dominio)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirRegularizacaoDominio" value="" />
							</td>
						<%} %>
					</tr>
					<% } %>

					<% if(!Model.IsVisualizar) { %>
					<tr class="trTemplateRow hide">
						<td><span class="comprovacao"></span></td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnRegularizacaoDominioId" value='0' />
							<input type="hidden" class="hdnItemJSON" value='' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirRegularizacaoDominio" value="" />
						</td>
					</tr>
					<% } %>

				</tbody>
			</table>
		</div>
	</div>
</fieldset>