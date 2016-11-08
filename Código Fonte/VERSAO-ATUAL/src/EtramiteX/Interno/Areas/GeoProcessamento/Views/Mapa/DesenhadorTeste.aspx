<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.GeoProcessamento.ViewModels.VMMapa" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CoordenadaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DesenhadorTeste
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/navegador.js") %>"></script>
<%--	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    
    var _Obter = function myfunction() {

        var objeto = {
            SituacaoId: 0,
            SituacaoTexto: 'Pasdf asdf asdf s',
            ArquivosProcessados: new Array()
        };

        /*$('.desenhadorArquivosGrid tbody tr', Desenhador.container).each(function (i, item) {
            objeto.ArquivosProcessados.push({
                Id: $('.hdnArquivoProcessadoId', item).val(),
                Texto: $('.arquivoNome', item).text(),
                IsPDF: $('.hdnArquivoProcessadoIsPdf', item).val()
            });
        });*/

        objeto.ArquivosProcessados.push({
            Id: -1,
            Texto: 'File 2',
            IsPDF: false
        });

        objeto.ArquivosProcessados.push({
            Id: -1,
            Texto: 'File 1',
            IsPDF: true
        });

        return $.toJSON(objeto);
    };
    
    Navegador.load($('#central'), {
        id: 793,
        modo: 2,
        tipo: 7,
        onCancelar: null,
        onProcessar: null,
        onBaixarArquivo: null,
        obterSituacaoInicial: _Obter,
        obterAreaAbrangencia: null
    });

</script>

<h2>DesenhadorTeste</h2>

<div id="central">
    
    <div style="height:80%; width:80%;">
        <% Html.RenderPartial("DesenhadorPartial"); %>
    </div>

</div>

</asp:Content>
