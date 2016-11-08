<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<LoteVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Lote</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/lote.js") %>"></script>

	<script type="text/javascript">
		$(function () { 
			Lote.load($('#central'), { 
				urls: {
					urlEmpreendimento: '<%=Url.Action("ObterCodigoCaracterizacaoUC", "CFOC")%>',
					urlVefiricar: '<%=Url.Action("LoteVerificarNumero", "CFOC")%>',
					associarCultura: '<%= Url.Action("Caracterizacoes", "ConfiguracaoVegetal/AssociarCultura") %>',
					urlAdicionarLoteItem: '<%=Url.Action("LoteValidarItem", "CFOC")%>',
					urlObterCultivar: '<%=Url.Action("ObterCultivar", "CFOC")%>',
					urlUnidadeMedida: '<%=Url.Action("ObterUnidadeMedida", "CFOC")%>',
					urlSalvar: '<%=Url.Action("LoteSalvar", "CFOC")%>'
				},
				idsTela: <%= Model.IdsTela %>
				});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Lote</h1>
		<br />

		<%Html.RenderPartial("LotePartial", Model);%>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("LoteIndex","CFOC") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>
