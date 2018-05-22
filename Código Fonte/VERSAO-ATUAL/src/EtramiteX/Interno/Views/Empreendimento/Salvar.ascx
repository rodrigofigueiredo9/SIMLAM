<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<% if (Model.IsAjaxRequest)
   { %>
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
		msgs: <%= Model.Mensagens %>,
		idsTela: <%= Model.IdsTela%>,
		denominadoresSegmentos: '<%= Model.DenominadoresSegmentos %>'
	});
</script>
<% } %>

<div class="modalSalvarEmpreendimento">
	<%= Html.Hidden("Empreendimento.Id", Model.Empreendimento.Id, new { @class = "hdnEmpId" })%>
	<%= Html.Hidden("Empreendimento.Tid", Model.Empreendimento.Tid, new { @class = "hdnEmpTid" })%>
	<%= Html.Hidden("Empreendimento.IsEditar", Model.IsEditar, new {@class = "hdnIsEditar"})%>
	<%= Html.Hidden("EstadoDefault", Model.EstadoDefault, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.EstadoDefaultSigla, new { @class = "hdnEstadoDefaultSigla" })%>
	<h1 class="titTela">Cadastrar Empreendimento</h1>
	<br />


	<div class="block box">
		<div class="block">
			<div class="coluna30">
				<label for="Empreendimento_Codigo">Código do empreeendimento</label>
				<%= Html.TextBox("Empreendimento.Codigo", Model.Empreendimento.Codigo > 0? Model.Empreendimento.Codigo.ToString() : "Gerado automaticamente", new { @class = "txtCodigo text disabled maskIntegerObrigatorio", @disabled="disabled"})%>
			</div>
		</div>
		<div class="block">
			<div class="coluna30">
				<label for="Empreendimento_Segmento">Segmento *</label>
				<%= Html.DropDownList("Empreendimento.Segmento", Model.Segmentos, new { @class = "text ddlSegmento" })%>
			</div>
			<div class="coluna29 prepend2">
				<label for="Empreendimento_CNPJ">CNPJ</label>
				<%= Html.TextBox("Empreendimento.CNPJ", Model.Empreendimento.CNPJ, new { @class = "text maskCnpj txtCnpj" })%>
			</div>
			<div class="coluna15">
				<button class="inlineBotao btnVerificarCnpj" type="button">Verificar</button>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.Denominador" class="lblDenominador">Denominação *</label>
				<%= Html.TextBox("Empreendimento.Denominador", Model.Empreendimento.Denominador, new { @maxlength = "100", @class = "txtDenominador text" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.NomeFantasia">Nome fantasia/comercial</label>
				<%= Html.TextBox("Empreendimento.NomeFantasia", Model.Empreendimento.NomeFantasia, new { @maxlength = "100", @class = "txtNomeFantasia text" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.AtividadeNome">Atividade principal</label>
				<%= Html.TextBox("Empreendimento.Atividade.Atividade", Model.Empreendimento.Atividade.Atividade, new { @class = "txtAtividade text disabled", @disabled = "disabled" })%>
				<%= Html.Hidden("Empreendimento.Atividade.Id", Model.Empreendimento.Atividade.Id, new { @class = "hdnAtividadeId" })%>
			</div>
			<div class="coluna10">
				<button type="button" class="inlineBotao botaoBuscar btnAssociarAtividade <%= (Model.Empreendimento.Atividade.Id > 0) ? "hide" : "" %>" title="Buscar Atividade Principal">Buscar</button>
				<button type="button" class="inlineBotao btnLimparAtividade <%= (Model.Empreendimento.Atividade.Id > 0) ? "" : "hide" %>" title="Limpar Atividade Principal">Limpar</button>
			</div>
		</div>
	</div>

	<fieldset class="divResponsaveis block box">
		<legend>Responsável do Empreendimento</legend>
		<div class="asmItens">
			<% foreach (Responsavel responsavel in Model.Empreendimento.Responsaveis)
	  { %>
			<div class="asmItemContainer boxBranca borders">
				<% Html.RenderPartial("AsmResponsavel", new ResponsavelVM(responsavel, Model.TiposResponsavel)); %>
			</div>
			<% } %>
		</div>
		<div class="asmItemTemplateContainer asmItemContainer boxBranca borders hide">
			<% Html.RenderPartial("AsmResponsavel", new ResponsavelVM(new Responsavel(), Model.TiposResponsavel)); %>
		</div>
		<br />
		<button class="btnAsmAdicionar direita">Responsável</button>
	</fieldset>

	<fieldset class="block box" id="Empreendimento.Enderecos[0]">
		<legend>Localização do Empreendimento</legend>

		<fieldset class="box">
			<div class="coluna35 append2">
				<label>
					<button class="btnCopiarEnderecoResponsavel">Copiar endereço do responsável</button></label>
			</div>

			<div class="coluna40 divResponsavelEnd hide">
				<label for="Empreendimento.Responsaveis">Responsável do empreendimento</label>
				<%= Html.DropDownList("Empreendimento.Responsaveis", Model.Responsaveis, new { @class = "text ddlResponsaveis" })%>
			</div>
		</fieldset>

		<div class="block endereco enderecoEmpreendimento">
			<input type="hidden" name="Empreendimento.Enderecos[0].Id" value="<%= Model.Empreendimento.Enderecos[0].Id %>" class="hdnEnderecoId" />
			<input type="hidden" name="Empreendimento.Enderecos[0].Correspondencia" value="0" class="hdnCorrespondencia" />

			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[0].Cep">CEP</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Cep", Model.Empreendimento.Enderecos[0].Cep, new { @class = "txtCep text maskCep" })%>
				</div>

				<div class="coluna20 prepend2">
					<p>
						<label>Zona de localização *</label></p>
					<label><%= Html.RadioButton("Empreendimento.Enderecos[0].ZonaLocalizacaoId", (int)eZonaLocalizacao.Urbana, Model.Empreendimento.Enderecos[0].ZonaLocalizacaoId == (int)eZonaLocalizacao.Urbana, new { @class = "radio rdbZonaLocalizacao" })%> Urbana</label>
					<label class="append5"><%= Html.RadioButton("Empreendimento.Enderecos[0].ZonaLocalizacaoId", (int)eZonaLocalizacao.Rural, Model.Empreendimento.Enderecos[0].ZonaLocalizacaoId == (int)eZonaLocalizacao.Rural, new { @class = "radio rdbZonaLocalizacao" })%> Rural</label>
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Logradouro">Logradouro/Rua/Rodovia *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Logradouro", Model.Empreendimento.Enderecos[0].Logradouro, new { @maxlength = "500", @class = "txtLogradouro text" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna50">
					<label for="Empreendimento.Enderecos[0].Bairro">Bairro/Gleba/Comunidade *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Bairro", Model.Empreendimento.Enderecos[0].Bairro, new { @maxlength = "100", @class = "txtBairro text" })%>
				</div>

				<div class="coluna46 prepend2">
					<label for="Empreendimento.Enderecos[0].DistritoLocalizacao">Distrito/Localidade *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].DistritoLocalizacao", Model.Empreendimento.Enderecos[0].DistritoLocalizacao, new { @maxlength = "100", @class = "txtDistrito text" })%>
				</div>
			</div>

			<div class="block divEndereco divEnderecoLocalizacao">
				<div class="coluna23">
					<label for="Empreendimento.Enderecos[0].Numero">Número</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Numero", Model.Empreendimento.Enderecos[0].Numero, new { @maxlength = "4", @class = "txtNumero text maskNumEndereco" })%>
				</div>

				<div class="coluna24 prepend2">
					<label for="Empreendimento.Enderecos[0].Estado">UF *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[0].EstadoId", Model.EstadosLocalizacao, ViewModelHelper.SetaDisabled(Model.Empreendimento.Enderecos[0].EstadoId > 0, new { @class = "text ddlEstado"}))%>
				</div>

				<div class="coluna46 prepend2">
					<label for="Empreendimento.Enderecos[0].MunicipioId">Município *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[0].MunicipioId", Model.MunicipiosLocalizacao, new { @disabled = "disabled", @class = "text ddlMunicipio disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Corrego">Córrego</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Corrego", Model.Empreendimento.Enderecos[0].Corrego, new { @maxlength = "100", @class = "txtCorrego text" })%>
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Complemento">Complemento / Roteiro para localização *</label>
					<%= Html.TextArea("Empreendimento.Enderecos[0].Complemento", Model.Empreendimento.Enderecos[0].Complemento, new { @maxlength = "500", @class = "txtComplemento textarea media" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_LocalColeta">Local de coleta do ponto *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.LocalColeta", Model.LocaisColetaPonto, new { @class = "text ddlLocalColeta" })%>
				</div>
				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_FormaColeta">Forma de coleta do ponto *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.FormaColeta", Model.FormasColetaPonto, new { @class = "text ddlFormaColeta" })%>
				</div>

				<div class="coluna10 prepend2">
					<button type="button" class="inlineBotao btnBuscarCoordenada">Buscar</button>
				</div>
			</div>

			<%= Html.Hidden("Empreendimento.Coordenada.Id", Model.Empreendimento.Coordenada.Id, new { @class = "hdnCoordenadaId"})%>
			<div class="block divCamposCoordenadas">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_Tipo_Id">Sistema de coordenada *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.Tipo.Id", Model.TiposCoordenada, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%>
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_Datum_Id">Datum *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.Datum.Id", Model.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%>
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_FusoUtm">Fuso *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.FusoUtm", Model.Fusos, new { @class = "text disabled ddlFuso", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block divCamposCoordenadas">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_EastingUtmTexto">Easting *</label>
					<%= Html.TextBox("Empreendimento.Coordenada.EastingUtmTexto", Model.Empreendimento.Coordenada.EastingUtmTexto, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_NorthingUtmTexto">Northing *</label>
					<%= Html.TextBox("Empreendimento.Coordenada.NorthingUtmTexto", Model.Empreendimento.Coordenada.NorthingUtmTexto, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_HemisferioUtm">Hemisfério *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.HemisferioUtm", Model.Hemisferios, new { @class = "text disabled ddlHemisferio", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Endereço de Correspondência</legend>

		<div class="block">
			<div class="block">
				<label>Copiar endereço ja cadastrado?</label>
				<label>
					<input type="radio" name="rdbCopiarEnderecoCorrespondencia" class="rdbCopiarEnderecoCorrespondencia rdbCopiarEnderecoCorrespondenciaSim" value="1" />Sim</label>
				<label>
					<input type="radio" checked="checked" name="rdbCopiarEnderecoCorrespondencia" class="rdbCopiarEnderecoCorrespondencia rdbCopiarEnderecoCorrespondenciaNao" value="0" />Não</label>
			</div>

			<div class="block divCopiarEnderecoCorrespondencia hide">
				<div class="block coluna50">
					<label>Origem do endereço</label>
					<%= Html.DropDownList("Empreendimento.CopiarEnderecoCorrespondencia.Origem", Model.EnderecoCadastradoTipoLst, new { @class = "text ddlCopiarEnderecoCorrespondenciaOrigem" })%>
				</div>

				<div class="block coluna50 hide divCopiarEnderecoCorrespondenciaTipo">
					<label><span class="spanNomeTipoEndereco"></span></label>
					<%= Html.DropDownList("Empreendimento.CopiarEnderecoCorrespondencia.Tipo", new List<SelectListItem>() { }, new { @class = "text ddlCopiarEnderecoCorrespondenciaTipo" })%>
				</div>
			</div>
			<br />
		</div>

		<div class="block endereco correspondenciaContainer">
			<input type="hidden" name="Empreendimento.Enderecos[1].Id" value="<%= Model.Empreendimento.Enderecos[1].Id %>" class="hdnEnderecoId" />
			<input type="hidden" name="Empreendimento.Enderecos[1].Correspondencia" value="1" class="hdnCorrespondencia" />
			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].Cep">CEP *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Cep", Model.Empreendimento.Enderecos[1].Cep, new { @class = "txtCep text maskCep" })%>
				</div>
				<div class="coluna76 prepend2">
					<label for="Empreendimento.Enderecos[1].Logradouro">Logradouro/Rua/Rodovia *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Logradouro", Model.Empreendimento.Enderecos[1].Logradouro, new { @maxlength = "500", @class = "txtLogradouro text" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna47">
					<label for="Empreendimento.Enderecos[1].Bairro">Bairro/Gleba/Comunidade *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Bairro", Model.Empreendimento.Enderecos[1].Bairro, new { @maxlength = "100", @class = "txtBairro text" })%>
				</div>

				<div class="coluna49 prepend2">
					<label for="Empreendimento.Enderecos[1].DistritoLocalizacao">Distrito/Localidade</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].DistritoLocalizacao", Model.Empreendimento.Enderecos[1].DistritoLocalizacao, new { @maxlength = "100", @class = "txtDistrito text" })%>
				</div>
			</div>

			<div class="block divEndereco">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].Numero">Número</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Numero", Model.Empreendimento.Enderecos[1].Numero, new { @maxlength = "4", @class = "txtNumero text maskNumEndereco" })%>
				</div>

				<div class="coluna24 prepend2">
					<label for="Empreendimento.Enderecos[1].Estado">UF *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[1].EstadoId", Model.EstadosCorrespondencia, new { @class = "text ddlEstado" })%>
				</div>

				<div class="coluna49 prepend2">
					<label for="Empreendimento.Enderecos[1].MunicipioId">Município *</label>
					<% if (Model.Empreendimento.Enderecos[1].EstadoId == 0)
		{ %>
					<%= Html.DropDownList("Empreendimento.Enderecos[1].MunicipioId", Model.MunicipiosCorrespondencia, new { disabled = "disabled", @class = "text ddlMunicipio disabled" })%>
					<% }
		else
		{ %>
					<%= Html.DropDownList("Empreendimento.Enderecos[1].MunicipioId", Model.MunicipiosCorrespondencia, new { @class = "text ddlMunicipio" })%>
					<% } %>
				</div>
			</div>

			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].CaixaPostal">Caixa postal</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].CaixaPostal", Model.Empreendimento.Enderecos[1].CaixaPostal, new { @maxlength = "50", @class = "text txtCaixaPostal" })%>
				</div>

				<div class="coluna76 prepend2">
					<label for="Empreendimento.Enderecos[1].Complemento">Complemento</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Complemento", Model.Empreendimento.Enderecos[1].Complemento, new { @maxlength = "500", @class = "text txtComplemento" })%>
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Meios de Contato</legend>
		<div class="block">
			<div class="coluna22">
				<label for="Contato_Telefone">Telefone</label>
				<input type="hidden" class="txtTelefoneId" value="<%= Model.Contato.TelefoneId %>" />
				<%= Html.TextBox("Contato.Telefone", Model.Contato.Telefone, new { @class = "txtTelefone text maskFone" })%>
			</div>
			<div class="coluna22 prepend2">
				<label for="Contato_TelefoneFax">Telefone fax</label>
				<input type="hidden" class="txtTelFaxId" value="<%= Model.Contato.TelefoneFaxId %>" />
				<%= Html.TextBox("Contato.TelefoneFax", Model.Contato.TelefoneFax, new { @class = "txtTelFax text maskFone" })%>
			</div>
			<div class="coluna49 prepend2">
				<label for="Contato_Email">E-mail</label>
				<input type="hidden" class="txtEmailId" value="<%= Model.Contato.EmailId %>" />
				<%= Html.TextBox("Contato.Email", Model.Contato.Email, new { @maxlength = "40", @class = "txtEmail text maskEmail" })%>
			</div>
		</div>

		<div class="block">
			<div class="ultima">
				<label for="Contato_NomeContato">Nome para contato</label>
				<input type="hidden" class="hdnNomeContatoId" value="<%= Model.Contato.NomeContatoId %>" />
				<%= Html.TextBox("Contato.NomeContato", Model.Contato.NomeContato, new { @class = "text txtNomeContato", @maxlength = "80" })%>
			</div>
		</div>
	</fieldset>
</div>
