<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa.SalvarVM>" %>


<fieldset class="block box">
	<legend>Endereço</legend>
	<div class="block">
		<div class="coluna15">
			<p><label for="Pessao.Endereco.Cep">CEP</label></p>
			<%= Html.TextBox("Pessoa.Endereco.Cep", Model.Pessoa.Endereco.Cep, new { @class = "text ", @maxlength = "10" })%>
		</div>
		<div class="coluna10">
			<button type="button" id="btnBuscarCep" class="inlineBotao">Buscar</button>
		</div>
		<div class="coluna50">
			<p><label for="Pessao.Endereco.Logradouro">Logradouro</label></p>
			<%= Html.TextBox("Pessoa.Endereco.Logradouro", Model.Pessoa.Endereco.Logradouro, new { @class = "text " } )%>
		</div>
	</div>
	<div class="block divEndereco">
		<div class="coluna25">
			<p><label for="Pessao.Endereco.Bairro">Bairro</label></p>
			<%= Html.TextBox("Pessoa.Endereco.Bairro", Model.Pessoa.Endereco.Bairro, new { @class = "text " } )%>
		</div>
		<div class="coluna20">
			<p><label for="Pessao.Endereco.Estado">Estado</label></p>
			<%= Html.DropDownList("Endereco.EstadoId", Model.Estados, new { @class = "text ddlEstado" })%>
		</div>
		<div class="coluna30">
			<p><label for="Pessao.Cidade">Município</label></p>
			<%= Html.DropDownList("Endereco.MunicipioId", Model.Municipios, new { @class = "text ddlMunicipio" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna25">
			<p><label for="Pessao.Endereco.Numero"></label></p>
			<%= Html.TextBox("Pessoa.Endereco.Numero", Model.Pessoa.Endereco.Numero, new { @class = "text " } )%>
		</div>
		<div class="coluna50">
			<p><label for="Pessao.Endereco.Complemento"></label></p>
			<%= Html.TextBox("Pessoa.Endereco.Complemento", Model.Pessoa.Endereco.Complemento, new { @class = "text " } )%>
		</div>
	</div>
</fieldset>
