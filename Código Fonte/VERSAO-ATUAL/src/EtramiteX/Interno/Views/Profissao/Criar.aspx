<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProfissaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Profissão</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">	
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Profissao/profissao.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			Profissao.load($('#central'), {
				urls: { 
					salvar: '<%= Url.Action("Criar", "Profissao") %>' 
				}
			});
		});
	</script>

	<script type="text/javascript">
		$(function () {
			<% String acaoId = Request.Params["acaoId"];
		
		if (!String.IsNullOrEmpty(acaoId)) {%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls: {
					urlEditar: '<%= Url.Action("Editar", "Profissao", new {id = acaoId }) %>',
					urlListar: '<%= Url.Action("Index", "Profissao") %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar Profissão</h1>
		<br />

		<% Html.RenderPartial("ProfissaoPartial"); %>

		<div class="block box">
			<input class="btnProfissaoSalvar floatLeft" type="button" value="Salvar" />			
			<span class="cancelarCaixa"><span class="btnModalOu">ou </span><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>