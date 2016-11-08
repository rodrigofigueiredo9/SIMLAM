<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Gerencial.Master" Inherits="System.Web.Mvc.ViewPage<PersonalizadoExecutarVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.ViewModels" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Gerar Relatório Personalizado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizadoExecutar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			PersonalizadoExecutar.load($('#central'), { 
				urls: {
					validar: '<%= Url.Action("ValidarExecutar", "Personalizado") %>',
					gerar: '<%= Url.Action("Executar", "Personalizado") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Mensagem"); %>
		<h1 class="titTela">Gerar Relatório Personalizado</h1>		<br />		<input type="hidden" class="relatorioId" value="<%= Model.Relatorio.Id %>" />
		<div class="block">
			<div class="coluna60 relatorioOpcoes">
				<img src="<%= Url.Content("~/Content/_img/icone_realtorios_personalizados.jpg") %>" width="58" height="62" alt="Relatório Personalizado" />
				<h6><%: Model.Relatorio.Nome %></h6>
				<p><%: Model.Relatorio.Descricao %></p>
				<p class="quiet small">Criado em: <%: Model.Relatorio.DataCriacao.DataTexto.Replace('/', '-') %></p>
			</div>
		</div>		
		<fieldset class="block box">
			<legend>Setor</legend>

			<div class="block margem0">
				<div class="block coluna75">
					<label class="lblSetor" for="Setor">Setor do Usuário *</label>
					<%= Html.DropDownList("Setor", Model.SetorLst, ViewModelHelper.SetaDisabled((Model.SetorLst.Count == 1), new { @class = "text ddlSetor setarFoco" }))%>
				</div>
			</div>
		</fieldset>

		<% if (Model.TermosExecucao.Count > 0) { %>
		<fieldset class="block box">
			<legend>Dados</legend>
			<% int i = 0; 
				foreach (var item in Model.TermosExecucao) {
				i++; %>
				<div class="block margem0 divTermo">
					<input type="hidden" class="hdnTermoJSON" value="<%: Model.ObterCampo(item) %>" />
					<div class="block coluna75">
						<% if (item.Campo.PossuiListaDeValores) { %>
							<% if(item.Campo.TipoDadosEnum == Tecnomapas.Blocos.RelatorioPersonalizado.Entities.eTipoDados.Bitand) { %>
								<label class="lblFiltro" for="Filtro"><%: item.Campo.Alias + " *" %></label>
								<div class="divChecks block paddingT5 coluna100">
								<% foreach (var itemLista in item.Campo.Lista) { %>
									<span>
										<label class="labelBig">
											<input type="checkbox" class="cbCampo" value="<%= itemLista.Codigo %>" <%= (Convert.ToInt32(item.Valor) & Convert.ToInt32(itemLista.Codigo)) > 0 ? "checked=\"checked\"" : "" %> />
											<span><%: itemLista.Texto %></span>
										</label>
									</span>
								<% } %>
								</div>
							<% } else { %>
								<label class="lblFiltro" for="Filtro"><%: item.Campo.Alias + " *" %></label>
								<%= Html.DropDownList("Filtro" + i, ViewModelHelper.CriarSelectList(item.Campo.Lista, false, true, item.Valor), new { @class = "text valorFiltro ddlFiltro setarFoco" })%>
							<% } %>
						<% } else { %>
							<label class="lblFiltro" for="Filtro"><%: item.Campo.Alias + " *" %></label>
							<%= Html.TextBox("Filtro" + i, item.Valor, new { @class = "text valorFiltro txtFiltro setarFoco " + Model.ObterMascara(item.Campo), maxlength = "80" })%>
						<% } %>
					</div>
				</div>
			<% } %>
		</fieldset>		<% } %>
		<fieldset class="block box">
			<legend>Opções</legend>
			<div class="coluna25">
				<p>Tipo de Arquivo</p>
				<p>
					<label class="floatLeft" title="PDF"><input type="radio" name="tipoArquivo" value="1" checked="checked"/>PDF</label>
					<label class="floatLeft margemEsq" title="XLS"><input type="radio" name="tipoArquivo" value="2" />XLS</label>
				</p>
			</div>
		</fieldset>
		<div class="block box margemDTop">
			<input type="button" class="btnExecutar floatLeft" value="Gerar Relatório" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>