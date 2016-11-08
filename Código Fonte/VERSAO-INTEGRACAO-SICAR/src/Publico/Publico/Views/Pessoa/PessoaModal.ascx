<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<script type="text/javascript">
	if (typeof PessoaAssociar != 'undefined') {
		PessoaAssociar.settings = {
			urls: {
				modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
				modalAssociarRepresentante: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				verificar: '<% = Url.Action("CriarVerificarCpfCnpj", "Pessoa") %>',
				limpar: '<% = Url.Action("Criar", "Pessoa") %>',
				criar: '<% = Url.Action("Criar", "Pessoa") %>',
				editar: '<% = Url.Action("Editar", "Pessoa") %>',
				visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
				pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				pessoaModalVisualizar: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				pessoaModalVisualizarConjuge: '<%= Url.Action("PessoaModalVisualizarConjuge", "Pessoa") %>',
				salvarConjuge: '<%= Url.Action("SalvarConjuge", "Pessoa") %>'
			},
			associarConjuge: <%= (Model.IsAssociarConjuge).ToString().ToLower() %>,
			visualizando: <%= (Model.Pessoa.Id > 0).ToString().ToLower() %>,
			tipoCadastro: <%= Model.TipoCadastro %>,
			msgs: <%= Model.Mensagens %>
		};
	}
</script>

<% Html.RenderPartial("Mensagem"); %>
<h2 class="titTela"><%= (Model.Pessoa.Id > 0 ? "Visualizar Pessoa" : "Cadastrar Pessoa") %></h2>
<br />

<% if (Model.Pessoa.Id <= 0 && Model.Pessoa.InternoId.GetValueOrDefault() == 0) { %>
	<% Html.RenderPartial("PessoaPartial", Model); %>
<% } else { %>
	<% Html.RenderPartial("PessoaPartialVisualizar", Model); %>
<% } %>

<div class="block box divPessoaContainer hide">
	<input class="btnPessoaEditar floatLeft hide" type="button" value="Editar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="btnPessoaAssociar floatLeft hide" type="button" value="Associar" />
	<span style="float: left">&nbsp;&nbsp;</span>
	<input class="btnPessoaSalvar floatLeft hide" type="button" value="Salvar" />
	<span class="btnModalCancelar cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar">Cancelar</a></span>
</div>