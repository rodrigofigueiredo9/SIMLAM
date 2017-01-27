<%@ Import Namespace="Tecnomapas.Blocos.RelatorioPersonalizado.Entities" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<Relatorio>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Atribuir Executor Relatório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Funcionario/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizadoAtribuirExecutor.js") %>"></script>

	<script>
		$(function () {
			PersonalizadoAtribuirExecutor.load($('#central'), {
				urls: {
					associar: '<%= Url.Action("Associar", "Funcionario", new { area = ""}) %>',
					atribuir: '<%= Url.Action("AtribuirExecutor", "Personalizado") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Atribuir Executor Relatório</h1>
		<br />

		<input type="hidden" class="relatorioId" value="<%= Model.Id %>" />

		<div class="block">
			<div class="coluna60 relatorioOpcoes">
				<img src="<%= Url.Content("~/Content/_img/icone_realtorios_personalizados.jpg") %>" width="58" height="62" alt="Relatório Personalizado" />
				<h6><%: Model.Nome %></h6>
				<p><%: Model.Descricao %></p>
				<p class="quiet small">Criado em: <%: Model.DataCriacao.DataTexto.Replace('/', '-') %></p>
			</div>
		</div>

		<div class="block">
			<div class="ultima">
				<button type="button" class="floatRight inlineBotao botaoBuscar btnAssociar">Buscar</button>
			</div>
		</div>

		<div class="block">
			<table class="dataGridTable tabFuncionarios" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Nome</th>
						<th width="9%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.UsuariosPermitidos) { %>
					<tr>
						<td class="tdNome" title="<%: item.Nome%>"><%: item.Nome%></td>
						<td>
							<input type="hidden" class="hdnItemId" value="<%= item.Id %>" />
							<input title="Excluir" type="button" class="icone excluir btnExcluir"/>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>

			<table style="display: none">
				<tbody>
					<tr class="trTemplate">
						<td class="tdNome"></td>
						<td>
							<input type="hidden" class="hdnItemId" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluir"/>
						</td>
					</tr>
				</tbody>
			</table>
		</div>

		<div class="block box margemDTop">
			<input type="button" class="btnAtribuir floatLeft" value="Atribuir Executor" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>