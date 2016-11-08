<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
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
			coordenadaGeo: '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento"}) %>',
			obterEstadosMunicipiosPorCoordenada: '<%= Url.Action("obterEstadosMunicipiosPorCoordenada", "Mapa", new {area="GeoProcessamento"}) %>',
			obterEnderecoResponsavel: '<%= Url.Action("ObterEnderecoResponsavel", "Empreendimento") %>',
			obterListaResponsaveis: '<%= Url.Action("ObterListaResponsaveis", "Empreendimento") %>',
			obterListaPessoasAssociada: '<%= Url.Action("ObterListaPessoasAssociada", "Empreendimento") %>',
			verificarLocalizaoEmpreendimento: '<%= Url.Action("VerificarLocalizacaoEmpreendimento", "Empreendimento") %>'
		},
		msgs: <%= Model.Mensagens %>,
		idsTela: <%= Model.IdsTela%>,
		denominadoresSegmentos: '<%= Model.SalvarVM.DenominadoresSegmentos %>'
	});
</script>

<% Html.RenderPartial("~/Views/Empreendimento/EmpreendimentoPartial.ascx", Model); %>