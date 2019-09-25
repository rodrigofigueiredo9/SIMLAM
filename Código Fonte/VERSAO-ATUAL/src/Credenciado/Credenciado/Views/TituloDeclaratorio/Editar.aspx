<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Título Declaratório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/TituloDeclaratorio/tituloDeclaratorio.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			TituloDeclaratorio.load($('#central'), {
				urls: {
					modelosCadastrados: '<%= Url.Action("ObterModelosCadastradosSetor", "Titulo") %>',
					associarRequerimento: '<%= Url.Action("Associar", "ProjetoDigital") %>',
					visualizarRequerimento: '<%= Url.Action("Visualizar", "Requerimento") %>',
					validarAssociarRequerimento: '<%= Url.Action("ValidarAssociarRequerimento", "TituloDeclaratorio") %>',
					tituloCamposModelo: '<%= Url.Action("TituloCamposModelo", "TituloDeclaratorio") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "TituloDeclaratorio") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "TituloDeclaratorio") %>',
					especificidade: '<%= Url.Action("Salvar", "Especificidade", new {area="Especificidades"}) %>',

					redirecionar: '<%= Url.Action("Criar", "TituloDeclaratorio") %>',
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				carregarEspecificidade: <%= Model.Titulo.Modelo.PossuiEspecificidade().ToString().ToLower() %>
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) {%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls:{
					urlGerarPdf: '<%= Url.Action("GerarPdf", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlAlterarSituacao: '<%= Url.Action("TituloDeclaratorio", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlEditar: '<%= Url.Action("Editar", "TituloDeclaratorio", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
			{
				if (Request.Params["modelo"].ToString() == "61") /*_Informacao de corte_*/
				{
					%>
					ContainerAcoes.load($(".containerAcoes"), {
						botoes: [
							{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() })%>' },
							{ label: 'Editar', url: '<%= Url.Action("Editar", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() })%>' },
							{ label: 'Emitir DUA', url: '<%= Url.Action("Listar", "DUA", new { id = Request.Params["acaoId"].ToString() })%>' }
						]
						});
			<% }
				else
				{ %>
					ContainerAcoes.load($(".containerAcoes"), {
							urls:{
							urlGerarPdf: '<%= Url.Action("GerarPdf", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
							urlAlterarSituacao: '<%= Url.Action("TituloDeclaratorio", "Titulo", new {id = Request.Params["acaoId"].ToString() }) %>',
							urlEditar: '<%= Url.Action("Editar", "TituloDeclaratorio", new {id = Request.Params["acaoId"].ToString() }) %>'
						}
					});
				<%}
			}%>
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Título Declaratório</h1>
		<br />
		<% Html.RenderPartial("TituloDeclaratorioPartial"); %>
	</div>
</asp:Content>