<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarCheckListRoteiroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Checagem de Itens de Roteiro</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ChecagemRoteiro/salvarChecagemRoteiro.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Roteiro/listarItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script>
		ChecagemRoteiroSalvar.visualizarRoteiroModalLink = '<%= Url.Action("Visualizar", "Roteiro") %>';
		ChecagemRoteiroSalvar.associarItemRoteiroModalLink = '<%= Url.Action("ListarItem", "Roteiro") %>';
		ChecagemRoteiroSalvar.urlObterItensRoteiro = '<%= Url.Action("ObterItensRoteiro","ChecagemRoteiro") %>';
		ChecagemRoteiroSalvar.urlAssociarRoteiro = '<%= Url.Action("Associar","Roteiro") %>';
		ChecagemRoteiroSalvar.urlValidarAssociarRoteiro = '<%= Url.Action("ValidarAssociarRoteiro","ChecagemRoteiro") %>';
		ChecagemRoteiroSalvar.urlCheckListRoteiroPdfObj = '<%= Url.Action("ChecagemRoteiroPdfObj","ChecagemRoteiro") %>';
		ChecagemRoteiroSalvar.urlCheckListRoteiroPdfObjValidar = '<%= Url.Action("ChecagemRoteiroPdfObjValidar","ChecagemRoteiro") %>';


		ChecagemRoteiroSalvar.Mensagens = <%= Model.Mensagens %>;

		$(function() {
			ChecagemRoteiroSalvar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlRequerimento: '<%= Url.Action("Criar", "Requerimento") %>'
					}
				});
		<%}%>
		});

	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Cadastrar Checagem de Itens de Roteiro</h1>
		<br />

		<% using (Html.BeginForm("ChecagemRoteiroCriar", "ChecagemRoteiro", FormMethod.Post)) { %>
		<%= Html.Hidden("vm.ChecagemRoteiro.Situacao") %>

		<div class="block box">
			<div class="block">
				<div class="coluna21">
					<label for="Id">Número *</label>
					<%= Html.TextBox("Id", "Gerado Automaticamente", new { disabled = "disabled", @class = "text disabled" })%>
				</div>

				<div class="coluna76 prepend1">
					<label for="ChecagemRoteiro_Interessado">Interessado *</label>
					<%= Html.TextBox("vm.ChecagemRoteiro.Interessado", Model.Interessado, new { @maxlength = "80", @class = "text txtInteressado" })%>
				</div>
			</div>
		</div>

		<fieldset class="block box" id="Roteiro_Container">
			<legend>Roteiro Orientativo</legend>
			<div class="block">
				<div class="floatRight">
					<button type="button" title="Buscar roteiro orientativo" class="btnAssociarRoteiro">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="dataGrid">
					<table class="tabRoteiros dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th width="10%">Número</th>
								<th width="10%">Versão</th>
								<th>Nome</th>
								<th width="10%">Ações</th>
							</tr>
						</thead>
						<tbody>
						<% int count = 0;
							JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
							foreach (var roteiro in Model.ChecagemRoteiro.Roteiros) { %>
							<tr>
								<td>
									<input type="hidden" class="hdnRoteiroJson" name="vm.RoteirosJson[<%= count %>]" value="<%= HttpUtility.HtmlEncode(_jsSerializer.Serialize(roteiro))  %>" />
									<input type="hidden" class="hdnRoteiroNumero" name="vm.RoteirosJson[<%= count %>].Numero" value="<%= Html.Encode(roteiro.Numero) %>" />
									<span class="trRoteiroNumero" title="<%= Html.Encode(roteiro.Numero) %>"><%= Html.Encode(roteiro.Numero)%></span>
								</td>
								<td>
									<input type="hidden" class="hdnRoteiroVersao" name="vm.RoteirosJson[<%= count %>].Versao" value="<%= Html.Encode(roteiro.Versao) %>" />
									<span class="trRoteiroVersao" title="<%= Html.Encode(roteiro.Versao) %>"><%= Html.Encode(roteiro.Versao)%></span>
								</td>
								<td>
									<input type="hidden" class="hdnRoteiroId" name="vm.RoteirosJson[<%= count %>].Id" value="<%= Html.Encode(roteiro.Id) %>" />
									<input type="hidden" class="hdnRoteiroIdRelacionamento" name="vm.RoteirosJson[<%= count %>].IdRelacionamento" value="<%= Html.Encode(roteiro.IdRelacionamento) %>" />
									<input type="hidden" class="hdnRoteiroTid" name="vm.RoteirosJson[<%= count %>].Tid" value="<%= Html.Encode(roteiro.Tid) %>" />
									<input type="hidden" class="hdnRoteiroNome" name="vm.RoteirosJson[<%= count %>].Nome" value="<%= Html.Encode(roteiro.Nome) %>" />
									<span class="trRoteiroNome" title="<%= Html.Encode(roteiro.Nome) %>"><%= Html.Encode(roteiro.Nome)%></span>
								</td>
								<td>
									<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRoteiro" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirRoteiro" value="" />
								</td>
							</tr>
						<% count++; } %>
						</tbody>
					</table>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Itens de Roteiro</legend>

			<div class="block">
				<div class="dataGrid">
					<table class="tabItensRoteiro dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Nome</th>
								<th>Condicionante</th>
								<th width="12%">Situação</th>
								<th width="18%">Ações</th>
							</tr>
						</thead>
						<tbody>
						<%
							count = 0;
							foreach (Item item in Model.ChecagemRoteiro.Itens) {%>
							<tr>
								<td>
									<input type="hidden" class="hdnItemId" value="<%= item.Id  %>" />
									<input type="hidden" class="hdnItemJson" name="vm.ItensJson[<%= count %>]" value="<%= HttpUtility.HtmlEncode(_jsSerializer.Serialize(item))  %>" />
									<span class="trItemRoteiroNome" title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome) %></span>
								</td>
								<td>
									<span class="trItemRoteiroCondicionante" title="<%= Html.Encode(item.Condicionante) %>"><%= Html.Encode(item.Condicionante)%></span>
								</td>

								<td>
									<span class="trItemRoteiroSituacaoTexto" title="<%= Html.Encode(item.SituacaoTexto) %>"><%= Html.Encode(item.SituacaoTexto) %></span>
								</td>
								<td>
									<input title="Conferir" type="button" class="icone recebido btnConferirItemRoteiro" value="" />
									<input title="Dispensar" type="button" class="icone dispensado btnDispensarItemRoteiro" value="" />
									<input title="Cancelar conferência ou dispensa" type="button" class="icone cancelar btnCancelarItemRoteiro" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirItemRoteiro" value="" />
								</td>
							</tr>
						<% count++; } %>
						</tbody>
					</table>
				</div>
			</div>
			<button type="button" title="Adicionar item" class="direita botaoAdicionar btnAssociarItemRoteiro">Item</button>
		</fieldset>

		<div class="block box">
			<button type="button" value="Gerar relatório de pendência" class="floatRight btnGerarPdfPendencia hide">Gerar relatório de pendência</button>
			<input id="salvar" type="submit" value="Salvar" class="floatLeft btnSalvarCheckList" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
		<% } %>

		<div class="hide divConteudoMotivo">
			<div class="block box">
				<div>
					<label>Motivo *</label>
					<%= Html.TextArea("Motivo", null, new { @class = "textarea text txtMotivo", @maxlength = "500", @style = "height: 135px;"})%>
				</div>
			</div>
			<div class="block box">
				<input type="button" value="Salvar" class="floatLeft btnSalvarMotivo" />
				<span class="cancelarCaixa">ou <a class="linkCancelar btnFechar" title="Cancelar">Cancelar</a></span>
			</div>
		</div>
	</div>

	<table style="display: none">
		<tbody>
			<tr class="trRoteiroTemplate">
				<td>
					<input type="hidden" class="hdnRoteiroJson" name="vm.RoteirosJson[contador]" value="#JSON" />
					<input type="hidden" class="hdnRoteiroNumero" name="vm.RoteirosJson[contador].Numero" value="numero" />
					<span class="trRoteiroNumero">NÚMERO</span>
				</td>
				<td>
					<input type="hidden" class="hdnRoteiroVersao" name="vm.RoteirosJson[contador].Versao" value="versao" />
					<span class="trRoteiroVersao">VERSÃO</span>
				</td>
				<td>
					<input type="hidden" class="hdnRoteiroId" name="vm.RoteirosJson[contador].Id" value="id" />
					<input type="hidden" class="hdnRoteiroIdRelacionamento" name="vm.RoteirosJson[contador].IdRelacionamento" value="IdRelacionamento" />
					<input type="hidden" class="hdnRoteiroTid" name="vm.RoteirosJson[contador].Tid" value="tid" />
					<input type="hidden" class="hdnRoteiroNome" name="vm.RoteirosJson[contador].Nome" value="nome" />
					<span class="trRoteiroNome">NOME</span>
				</td>
				<td>
					<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRoteiro" value="" />
					<input title="Excluir" type="button" class="icone excluir btnExcluirRoteiro" value="" />
				</td>
			</tr>
		</tbody>
	</table>
	<table style="display: none">
		<tbody>
			<tr class="trItemRoteiroTemplate">
				<td>
					<input type="hidden" class="hdnItemId" value="#ID" />
					<input type="hidden" class="hdnItemJson" name="vm.ItensJson[contador]" value="#JSON" />
					<span class="trItemRoteiroNome">NOME</span>
				</td>
				<td>
					<span class="trItemRoteiroCondicionante">CONDICIONANTE</span>
				</td>
				<td><span class="trItemRoteiroSituacaoTexto">SITUACAO</span></td>
				<td>
					<input title="Conferir" type="button" class="icone recebido btnConferirItemRoteiro" value="" />
					<input title="Dispensar" type="button" class="icone dispensado btnDispensarItemRoteiro" value="" />
					<input title="Cancelar conferência ou dispensa" type="button" class="icone cancelar btnCancelarItemRoteiro" value="" />
					<input title="Excluir" type="button" class="icone excluir btnExcluirItemRoteiro" value="" />
				</td>
			</tr>
		</tbody>
	</table>


</asp:Content>