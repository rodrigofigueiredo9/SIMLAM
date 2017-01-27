<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editar Setor
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Setor/salvar.js") %>"></script>
	<script>
		$(function () {
			SetorSalvar.load($('#central'));
			SetorSalvar.settings.urls.salvar = '<%= Url.Action("Editar", "Setor") %>';
			SetorSalvar.settings.urls.urlObterMunicipiosPorEstado = '<%= Url.Action("ObterDadosMunicipios", "Setor") %>';
		});
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">

	<input type="hidden" class="hdnSetorId" value="<%= Model.Setor.Id%>" />

	<h1 class="titTela">Editar Setor</h1>
	<br />

	<div class="box">
		<div class="block">
			<div class="coluna25 append2">
				<label for="Setor_Agrupador">Agrupador *</label>
				<%= Html.DropDownList("Setor.Agrupador", Model.Agrupador, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAgrupador disabled " }))%>
			</div>

			<div class="coluna60">
				<label for="Setor_Setor">Setor *</label>
				<%= Html.DropDownList("Setor.Setor", Model.Setores, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSetor disabled " }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna25 append2">
				<label for="Setor_Sigla">Sigla *</label>
				<%= Html.TextBox("Setor.Sigla", Model.Setor.Sigla, new { @class = "text txtSigla" })%>
			</div>

			<div class="coluna60">
				<label for="Setor_Responsavel">Responsável do Setor *</label>
				<%= Html.TextBox("Setor.Responsavel", Model.Setor.Responsavel, ViewModelHelper.SetaDisabled(true, new { @class = "text txtResponsavel disabled " }))%>
			</div>
		</div>

		<fieldset class="block box">
			<legend class="titFiltros">Endereço</legend>

			<div class="block">
				<div class="coluna70 append2">
					<label for="Setor_Endereco_Logradouro">Logradouro *</label>
					<%= Html.TextBox("Setor.Endereco.Logradouro", Model.Setor.Endereco.Logradouro, new { @class = "text txtLogradouro" })%>
				</div>

				<div class="coluna15">
					<label for="Setor_Endereco_Numero">Numero *</label>
					<%= Html.TextBox("Setor.Endereco.Numero", Model.Setor.Endereco.Numero, new { @class = "text txtNumero maskNum15" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna30 append2">
					<label for="Setor_Endereco_Bairro">Bairro</label>
					<%= Html.TextBox("Setor.Endereco.Bairro", Model.Setor.Endereco.Bairro, new { @class = "text txtBairro" })%>
				</div>

				<div class="coluna25">
					<label for="Setor_Endereco_Cep">CEP</label>
					<%= Html.TextBox("Setor.Endereco.Cep", Model.Setor.Endereco.Cep, new { @class = "text txtCep" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna15 append2">
					<label for="Setor_Endereco_Estado">UF</label>
					<%= Html.DropDownList("Setor.Endereco.Estado", Model.Estados, new { @class = "text ddlEstado" })%>
				</div>

				<div class="coluna40">
					<label for="Setor_Endereco_Municipios">Municipio</label>
					<%= Html.DropDownList("Setor.Endereco.Municipio", Model.Municipios, new { @class = "text ddlMunicipio" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna25 append2">
					<label for="Setor_Endereco_Fone">Telefone</label>
					<%= Html.TextBox("Setor.Endereco.Fone", Model.Setor.Endereco.Fone, new { @class = "txtTelefone text maskFone" })%>
				</div>

				<div class="coluna30">
					<label for="Setor_Endereco_Fax">Fax</label>
					<%= Html.TextBox("Setor.Endereco.Fax", Model.Setor.Endereco.Fax, new { @class = "txtTelFax text maskFone" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna70">
					<label for="Setor_Endereco_Complemento">Complemento</label>
					<%= Html.TextBox("Setor.Endereco.Complemento", Model.Setor.Endereco.Complemento, new { @class = "text txtComplemento" })%>
				</div>
			</div>
		</fieldset>
	</div>

	<div class="block box botoesSalvarCancelar">
		<div class="block">
			<button class="btnSalvarSetor floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>

</div>

</asp:Content>
