<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EmpreendimentoVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script type="text/javascript">
	$.extend(EmpreendimentoInline.settings, {
		urls: {
			avancar: '<%= Url.Action("Salvar", "Empreendimento") %>',
			voltar: '<%= Url.Action("LocalizarMontar", "Empreendimento") %>',
			salvarCadastrar: '<%= Url.Action("SalvarCadastrar", "Empreendimento") %>',
			editar: '<%= Url.Action("Editar", "Empreendimento") %>',
			visualizar: '<%= Url.Action("Visualizar", "Empreendimento") %>',
			associarAtividadeModal: '<%= Url.Action("AtividadeEmpListarFiltros", "Atividade") %>',
			associarResponsavelModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
			associarResponsavelEditarModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
			verificarCnpj: '<%= Url.Action("VerificarCnpj", "Empreendimento") %>',
			pessoaAssociar: '<%= Url.Action("PessoaModal", "Pessoa") %>',
			verificarCnpjEmpreendimento: '<%= Url.Action("VerificarCnpjEmpreendimento", "Empreendimento") %>',
			coordenadaGeo: '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento"}) %>',
			obterEstadosMunicipiosPorCoordenada: '<%= Url.Action("obterEstadosMunicipiosPorCoordenada", "Mapa", new {area="GeoProcessamento"}) %>',
			copiarInterno:'<%= Url.Action("CopiarDadosIdaf", "Empreendimento")%>'
		},
		msgs: <%= Model.Mensagens %>,
		denominadoresSegmentos: '<%= Model.SalvarVM.DenominadoresSegmentos %>'
	});
</script>

<% Html.RenderPartial("~/Views/Empreendimento/EmpreendimentoPartialCorte.ascx", Model); %>