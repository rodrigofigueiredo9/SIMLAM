<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script>

	PessoaInline.settings = {
		urls: {
			modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
			modalAssociarRepresentante: '<%= Url.Action("RepresentanteAssociar", "Pessoa") %>',
			verificar: '<% = Url.Action("CriarVerificarCpfCnpj", "Pessoa") %>',
			limpar: '<% = Url.Action("Criar", "Pessoa") %>',
			visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
			criar: '<% = Url.Action("Criar", "Pessoa") %>',
			editar: '<% = Url.Action("Editar", "Pessoa") %>',
			pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
			pessoaModalVisualizar: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
			obterEnderecoPessoa: '<%= Url.Action("ObterEndereco", "Pessoa") %>'
		},
		msgs: <%= Model.Mensagens %>
	};

	PessoaInline.modo = parseInt('<%= ((Model.IsVisualizar)?"2":"3") %>');

</script>


<% if (!Model.IsVisualizar)
   {
	   Html.RenderPartial("~/Views/Pessoa/PessoaPartial.ascx", Model);
   }
   else
   {
	   Html.RenderPartial("~/Views/Pessoa/PessoaPartialVisualizar.ascx", Model);
   }
%>
