<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<FichaFundiariaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Visualizar Ficha Fundiária
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Visualizar Ficha Fundiária</h1>
		<br />

		<%Html.RenderPartial("FichaFundiaria", Model);%>

		<%if(Model.PodeEditar){ %>
		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnEditar floatLeft" type="button" value="Editar" onclick="window.location.href='<%= Url.Action("Editar", new {Id = Model.FichaFundiaria.Id}) %>'"><span>Editar</span></button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
			</div>
		</div>
		<%}else{ %>
			<div class="block box botoesSalvarCancelar">
				<div class="block">
					<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
				</div>
			</div>
		<%} %>
	</div>
</asp:Content>
