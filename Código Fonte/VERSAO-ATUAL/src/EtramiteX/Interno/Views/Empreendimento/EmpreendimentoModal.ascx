<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EmpreendimentoVM>" %>

<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script>
	$.extend(EmpreendimentoAssociar.settings, {
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
			verificarLocalizaoEmpreendimento: '<%= Url.Action("VerificarLocalizacaoEmpreendimento", "Empreendimento") %>',
			obterListaResponsaveisCnpj: '<%= Url.Action("ObterListaResponsaveisCnpj", "Empreendimento") %>'
		},
		visualizando: <%= (Model.SalvarVM != null && Model.SalvarVM.Empreendimento.Id > 0).ToString().ToLower() %>,
		msgs: <%= Model.Mensagens %>,
		denominadoresSegmentos: '<%= Model.SalvarVM.DenominadoresSegmentos %>'
	});
</script>


<% Html.RenderPartial("EmpreendimentoPartial", Model); %>

<div class="block box btnEmpContainer">
	<input class="floatLeft btnEmpAvancar " type="button" value="Avançar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="btnEmpEditar floatLeft hide" type="button" value="Editar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="btnEmpAssociar floatLeft hide" type="button" value="Associar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="floatLeft btnEmpSalvar" type="button" value="Salvar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="floatLeft btnEmpVoltar" type="button" value="Voltar" />
	<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar">Cancelar</a></span>
</div>