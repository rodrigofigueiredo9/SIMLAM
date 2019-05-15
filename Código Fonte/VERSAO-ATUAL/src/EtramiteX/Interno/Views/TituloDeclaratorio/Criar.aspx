<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Título Declaratório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorio/tituloDeclaratorio.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script>
		$(function () {
			TituloDeclaratorio.load($('#central'), {
				urls: {
					modelosCadastrados: '<%= Url.Action("ObterModelosCadastradosSetor", "Titulo") %>',
					associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
					validarAssociarRequerimento: '<%= Url.Action("ValidarAssociarRequerimento", "TituloDeclaratorio") %>',
					tituloCamposModelo: '<%= Url.Action("TituloCamposModelo", "TituloDeclaratorio") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "TituloDeclaratorio") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "TituloDeclaratorio") %>',
					especificidade: '<%= Url.Action("Salvar", "Especificidade", new {area="Especificidades"}) %>',

					redirecionar: '<%= Url.Action("Criar", "TituloDeclaratorio") %>',
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				carregarEspecificidade: <%= Model.CarregarEspecificidade.ToString().ToLower() %>
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
			{
				if (Request.Params["modelo"].ToString() == "61") /*_Informacao de corte_*/
				{
					%>
				ContainerAcoes.load($(".containerAcoes"), {
						
					botoes: [/
						{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "Titulo", new { id = Request.Params["acaoId"].ToString() })%>' },
						{ label: 'Editar', url: '<%= Url.Action("Editar", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() })%>' },
						{ label: 'Emitir DUA', url: '<%= Url.Action("Listar", "DUA", new { id = Request.Params["acaoId"].ToString() })%>' }
					]
					});
			<% }
			else
			{ %>
					ContainerAcoes.load($(".containerAcoes"), {
						urls:{
							urlGerarPdf: '<%= Url.Action("GerarPdf", "Titulo", new { id = Request.Params["acaoId"].ToString() }) %>',
							urlAlterarSituacao: '<%= Url.Action("AlterarSituacao", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() }) %>',
							urlEditar: '<%= Url.Action("Editar", "TituloDeclaratorio", new { id = Request.Params["acaoId"].ToString() }) %>'
						}
					});
				<%}
			}%>
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar Título Declaratório</h1>
		<br />
		<% Html.RenderPartial("TituloDeclaratorioPartial"); %>
	</div>
</asp:Content>