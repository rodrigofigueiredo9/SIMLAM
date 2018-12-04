<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<InformacaoCorteVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Dominialidade</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/dominialidade.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			Dominialidade.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "Dominialidade") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "Dominialidade") %>',
					atualizarGrupoARL: '<%= Url.Action("AtualizarGrupoARL", "Dominialidade") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">	
	<div id="central">
		<div class="divCaracterizacao">
			<%Html.RenderPartial("InformacaoCortePartial", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.EmpreendimentoId , Model.ProjetoDigitalId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>