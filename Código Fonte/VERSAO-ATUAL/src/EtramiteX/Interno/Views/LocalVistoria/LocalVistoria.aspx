<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<LocalVistoriaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<% if (Model.IsEdicao) {%>
    Editar Local de Vistoria
<%} else {%>
    Cadastrar Local de Vistoria
<% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/LocalVistoria/localVistoria.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
		    LocalVistoria.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("SalvarLocalVistoria", "LocalVistoria") %>',
				    obter: '<%= Url.Action("EditarLocalVistoria", "LocalVistoria") %>',
				    escolherSetor: '<%= Url.Action("EscolherSetorLocalVistoria", "LocalVistoria") %>',
				    podeExcluir: '<%= Url.Action("PodeExcluir", "LocalVistoria") %>',
				    podeIncluirBloqueio: '<%= Url.Action("PodeAcrescentarBloqueio", "LocalVistoria") %>',
				},
		        Mensagens: <%= Model.Mensagens %>,
		    });
	        <% if (Model.IsEdicao) {%>
    		    LocalVistoria.entraEditando();
		    <%} %>

		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">
            <% if (Model.IsEdicao) {%>
                Editar Local de Vistoria
            <%} else {%>
                Cadastrar Local de Vistoria
            <% } %>
        </h1>
		<br />

		<%Html.RenderPartial("LocalVistoriaPartial", Model);%>

        <div class="block box botoesSalvarCancelar">
            <button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa cancelarCaixaPrincipal"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("LocalVistoriaListar", "LocalVistoria") %>">Cancelar</a></span>
        </div>

    </div>
</asp:Content>