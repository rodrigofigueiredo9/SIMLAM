<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVVM>" %>

<fieldset class="block box">
	<%= Html.Hidden("DestinatarioID", Model.Destinatario.ID, new { @class = "hdnDestinatarioID" }) %>
	<div class="block">
		<div id="DivTipoPessoa" class="coluna32">
			<label for="PessoaTipo">Tipo *</label><br/>
			<label><%= Html.RadioButton("PessoaTipo", PessoaTipo.FISICA, (Model.Destinatario.PessoaTipo == PessoaTipo.FISICA), new { @class="radio rbPessoaTipo rbPessoaTipoCPF" }) %>Pessoa física</label>
			<label><%= Html.RadioButton("PessoaTipo", PessoaTipo.JURIDICA, (Model.Destinatario.PessoaTipo == PessoaTipo.JURIDICA), new { @class="radio rbPessoaTipo rbPessoaTipoCNPJ" }) %>Pessoa jurídica</label>
			<label><%= Html.RadioButton("PessoaTipo", PessoaTipo.EXPORTACAO, (Model.Destinatario.PessoaTipo == PessoaTipo.EXPORTACAO), new { @class="radio rbPessoaTipo rbPessoaTipoExportacao" }) %>Exportação</label>
		</div>

		<div class="coluna20">
			<label class="lblCPFCNPJ" for="CPFCNPJ">CPF *</label>
			<%= Html.TextBox("CPFCNPJ", Model.Destinatario.CPFCNPJ, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtCPFCNPJ maskCpf setarFoco"}) ) %>
			<%= Html.TextBox("CPFCNPJ", Model.Destinatario.CPFCNPJ, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtCPFCNPJ maskCnpj hide"}) ) %>
		</div>
		<%if (Model.Destinatario.ID == 0) { %>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnValidarCPFCNPJ" title="Verificar">Verificar</button>
			<button type="button" class="inlineBotao btnLimparCPFCNPJ hide" title="Limpar">Limpar</button>
		</div>
		<% } %>
	</div>
	<div class="block hide esconder divNomeRazaoSocial">
		<div class="coluna78">
			<label for="NomeRazaoSocial">Nome do destinatário *</label>
			<%= Html.TextBox("NomeRazaoSocial", Model.Destinatario.NomeRazaoSocial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtNomeRazaoSocial", @maxlength = "80" }) ) %>
		</div>
		<%if (Model.Destinatario.ID == 0) { %>
		<div class="coluna20">
			<button type="button" class="inlineBotao btnValidarExportacao hide" title="Verificar">Verificar</button>
			<button type="button" class="inlineBotao btnLimparExportacao hide" title="Limpar">Limpar</button>
		</div>
		<% } %>
	</div>
</fieldset>

<fieldset class="block box hide esconder">
	<legend>Endereço</legend>
	<div class="block">
		<div class="coluna78">
			<label for="Endereco">Endereço *</label>
			<%= Html.TextBox("Endereco", Model.Destinatario.Endereco,  ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEndereco", @maxlength = "800" }) ) %>
		</div>
	</div>

	<div class="block divEndereco">
		<div class="coluna18">
			<label for="EstadoID" class="lblUF">UF *</label>
			<%= Html.DropDownList("EstadoID", Model.Uf, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlEstado" }) ) %>
		</div>
		<div class="coluna30 prepend1">
			<label for="MunicipioID" class="lblMunicipio">Município *</label>
			<%= Html.DropDownList("MunicipioID", Model.Municipios,  ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlMunicipio" }) ) %>
		</div>
		<div class="coluna25 divPais prepend1 hide">
			<label for="Pais" class="lblPais">País</label>
			<%= Html.TextBox("Pais", Model.Destinatario.Pais, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtPais", @maxlength ="100" }) ) %>
		</div>
	</div>

	<div class="block">
		<div class="coluna78">
			<label for="Itinerario" class="lblItinerario">Itinerário *</label>
			<%= Html.TextBox("Itinerario", Model.Destinatario.Itinerario, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text txtItinerario", @maxlength ="200" }) ) %>
		</div>
	</div>
</fieldset>