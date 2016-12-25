<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<OrgaoParceiroConveniadoVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Editar
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/OrgaosParceirosConveniados/orgaosParceirosConveniados.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
    <script>
        $(function () {
                OrgaosParceirosConveniados.load($('#central'), {
                    urls: {
                        salvar: '<%= Url.Action("Editar", "OrgaosParceirosConveniados")%>',
				        verificarCredenciadoAssociado: '<%= Url.Action("VerificarCredenciadoAssociado", "OrgaosParceirosConveniados") %>'

				    },
                    Mensagens: <%=Model.Mensagens %>
                    });

            <% if (!string.IsNullOrEmpty(Request.Params["acaoId"])){%>
            ContainerAcoes.load($(".containerAcoes"), {
                urls: {
                    urlAlterarSituacao: '<%= Url.Action("AlterarSituacao", "OrgaosParceirosConveniados", new { id = Request.Params["acaoId"] })%>',
                        urlEditar: '<%= Url.Action("Editar", "OrgaosParceirosConveniados", new { id = Request.Params["acaoId"] }) %>',
                        urlListar: '<%= Url.Action("Index", "OrgaosParceirosConveniados") %>',
                    }
                });

            <%}%>
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <h1 class="titTela">Editar Órgão Parceiro/ Conveniado</h1>
        <br />

        <%Html.RenderPartial("OrgaosParceirosConveniados", Model); %>

        <div class="block box botoesSalvarCancelar">
	        <div class="block">
		        <button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
                <span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		    </div>
	    </div>
    </div>
</asp:Content>

