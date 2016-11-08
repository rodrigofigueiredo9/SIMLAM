<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PragaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ViewBag.Titulo %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/praga.js") %>"></script>
    <script type="text/javascript">
        $(function () {
            Praga.load($('#central'), {
                urls: {
                    salvar:'<%=Url.Action("SalvarPraga", "ConfiguracaoVegetal")%>'
                },
                Mensagens:<%= Model.Mensagens %>
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
		<h1 class="titTela"><%=ViewBag.Titulo %></h1>
		<br />

		<%Html.RenderPartial("Praga/PragaPartial", Model);%>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Pragas", "ConfiguracaoVegetal") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>


