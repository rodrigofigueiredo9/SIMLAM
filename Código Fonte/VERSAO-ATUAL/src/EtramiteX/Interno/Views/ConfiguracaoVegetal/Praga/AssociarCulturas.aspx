<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PragaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Associar Culturas à Praga
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/associarCulturas.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script>
		$(function () {
			AssociarCulturas.load($('#central'), {
				urls: {
					salvar:'<%=Url.Action("AssociarCulturasSalvar", "ConfiguracaoVegetal")%>',
					associar: '<%=Url.Action("AssociarCultura", "ConfiguracaoVegetal")%>'
				},
				Mensagens:<%= Model.Mensagens %>
				});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Associar Culturas à Praga</h1>
		<br />

		<fieldset class="block box">
			<div class="block">
				<input type="hidden" value="<%=Model.Praga.Id %>" class="hdnId" />
				<div class="coluna100">
					<label>Praga*</label>
					<%=Html.TextBox("Praga.NomeCientifico", Model.Praga.NomeCientifico, new { @class="txtPraga text disabled", @maxlength="100", @disabled = "disabled" }) %>
				</div>
			</div>
			<div class="block">
				<div class="coluna100">
					<button class="btnBuscarCultura direita" type="button" value="Cultura"><span>Cultura</span></button>
				</div>
			</div>
			<div class="block">
				<div class="coluna100">
					<table class="dataGridTable gridCulturas" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th width="95%">Cultura</th>
								<th class="semOrdenacao" width="5%">Ações</th>
							</tr>
						</thead>
						<tbody>
							<%foreach (var item in Model.Praga.Culturas)
							{%>
							<tr>
								<td>
									<span class="nome" title="<%=item.Nome %>"><%=item.Nome %> </span>
								</td>
								<td>
									<a class="icone excluir btnExcluir"></a>
									<input type="hidden" value="<%= item.Id %>" class="hdnItemId" />
									<input type="hidden" value="<%= item.IdRelacionamento %>" class="hdnItemIdRelacionamento" />

								</td>
							</tr>
							<% } %>
							<tr class="hide">
								<td>
									<span class="nome" title=""></span>
								</td>
								<td>
									<a class="icone excluir btnExcluir"></a>
									<input type="hidden" value="0" class="hdnItemId" />
									<input type="hidden" value="0" class="hdnItemIdRelacionamento" />

								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>

		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Pragas", "ConfiguracaoVegetal") %>">Cancelar</a></span>

			</div>
		</div>
	</div>
</asp:Content>


