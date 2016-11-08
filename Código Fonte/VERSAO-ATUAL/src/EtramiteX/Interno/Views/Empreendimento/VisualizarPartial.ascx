<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script type="text/javascript">

	var modalPessoaResp = new PessoaAssociar();
	
	$(function () {
		$('.divResponsaveis', $('.modalVisualizarEmpreendimento')).associarMultiplo({
			'editarModalObject': modalPessoaResp,
			'editarModalLoadFunction': modalPessoaResp.load,
			'editarUrl': '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
			'isVisualizarHistorico': <%=Model.IsVisualizarHistorico.ToString().ToLower()%>,
			'editarModalLoadParams': {
				tituloVisualizar: 'Visualizar Responsável',
				editarVisualizar: false
			},
			'expandirAutomatico': true,
			'tamanhoModal': Modal.tamanhoModalGrande,
			'onExpandirEsconder': function () { MasterPage.redimensionar(); }
		});

		$('.btnAsmAdicionar', $('.modalVisualizarEmpreendimento')).button({ disabled: true }).hide();
		var responsaveis = $('.asmItens', $('.modalVisualizarEmpreendimento'));
		$('.asmItemContainer', responsaveis).each(function (index, item) {
			$(item).find('.btnAsmExcluir').remove();
			$(item).find('.btnAsmAssociar').remove();
			$(item).find('.txtDataVencResponsavel').attr('disabled', 'disabled').addClass('disabled');
			$(item).find('.txtEspecificarTexto').attr('disabled', 'disabled').addClass('disabled');
			$(item).find('.ddlTipoResponsavel').attr('disabled', 'disabled').addClass('disabled');
			$(item).find('.asmConteudoInterno').hide();
			$(item).find('.asmExpansivel').text('Clique aqui para ver mais detalhes');
		});
	});
</script>

<div class="modalVisualizarEmpreendimento">
	<% if(Model.MostrarTituloTela) { %>
	<h1 class="titTela">Visualizar Empreendimento</h1>
	<br />
	<% } %>

	<%= Html.Hidden("Empreendimento.Id", Model.Empreendimento.Id, new { @class = "hdnEmpId" })%>
	<%= Html.Hidden("Empreendimento.Tid", Model.Empreendimento.Tid, new { @class = "hdnEmpTid" })%>

	<fieldset class="block box">
		<legend>Empreendimento</legend>
		<div class="block">
			<div class="coluna20">
				<label for="Empreendimento.Codigo">Código do empreendimento</label>
				<%= Html.TextBox("Empreendimento.Codigo", Model.Empreendimento.Codigo, new { @class = "text maskIntegerObrigatorio disabled", @disabled = "disabled" })%>
			</div>
			</div>
		<div class="block">
			<div class="coluna40">
				<label for="Empreendimento.Segmento">Segmento do empreendimento</label>
				<%= Html.DropDownList("Empreendimento.Segmento", Model.Segmentos, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna29 prepend2">
				<label for="Empreendimento.CNPJ">CNPJ</label>
				<%= Html.TextBox("Empreendimento.CNPJ", Model.Empreendimento.CNPJ, new { @class = "text maskCnpj disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.Denominador" class="lblDenominador"><%: Model.Empreendimento.SegmentoDenominador %> *</label>
				<%= Html.TextBox("Empreendimento.Denominador", Model.Empreendimento.Denominador, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.NomeFantasia">Nome fantasia/comercial</label>
				<%= Html.TextBox("Empreendimento.NomeFantasia", Model.Empreendimento.NomeFantasia, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna85">
				<label for="Empreendimento.AtividadeNome">Atividade principal</label>
				<%= Html.TextBox("Empreendimento.Atividade.Atividade", Model.Empreendimento.Atividade.Atividade, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div> 
	</fieldset>

	<fieldset class="divResponsaveis block box">
		<legend>Responsável do empreendimento</legend>
		<div class="asmItens">
			<% foreach (Responsavel responsavel in Model.Empreendimento.Responsaveis) { %>
				<div class="asmItemContainer boxBranca borders">
					<% Html.RenderPartial("AsmResponsavel", new ResponsavelVM(responsavel, Model.TiposResponsavel)); %>
				</div>
			<% } %>
		</div>
		<div class="asmItemTemplateContainer asmItemContainer boxBranca borders hide">
			<% Html.RenderPartial("AsmResponsavel", new ResponsavelVM(new Responsavel(), Model.TiposResponsavel)); %>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Localização do Empreendimento</legend>

		<div class="block">
			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[0].Cep">CEP</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Cep", Model.Empreendimento.Enderecos[0].Cep, new { @class = "text maskCep disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna21 prepend2">
					<p><label>Zona de localização *</label></p>
					<label><%= Html.RadioButton("Empreendimento.Enderecos[0].ZonaLocalizacaoId", (int)eZonaLocalizacao.Urbana, Model.Empreendimento.Enderecos[0].ZonaLocalizacaoId == (int)eZonaLocalizacao.Urbana, new { @class = "radio disabled", @disabled = "disabled" })%> Urbana</label>
					<label class="append5"><%= Html.RadioButton("Empreendimento.Enderecos[0].ZonaLocalizacaoId", (int)eZonaLocalizacao.Rural, Model.Empreendimento.Enderecos[0].ZonaLocalizacaoId == (int)eZonaLocalizacao.Rural, new { @class = "radio disabled", @disabled = "disabled" })%> Rural</label>
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Logradouro">Logradouro/Rua/Rodovia *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Logradouro", Model.Empreendimento.Enderecos[0].Logradouro, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna50">
					<label for="Empreendimento.Enderecos[0].Bairro">Bairro/Gleba/Comunidade *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Bairro", Model.Empreendimento.Enderecos[0].Bairro, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna46 prepend2">
					<label for="Empreendimento.Enderecos[0].DistritoLocalizacao">Distrito/Localidade *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].DistritoLocalizacao", Model.Empreendimento.Enderecos[0].DistritoLocalizacao, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block divEndereco divEnderecoLocalizacao">
				<div class="coluna23">
					<label for="Empreendimento.Enderecos[0].Numero">Número</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Numero", Model.Empreendimento.Enderecos[0].Numero, new { @class = "text maskNumEndereco disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna24 prepend2">
					<label for="Empreendimento.Enderecos[0].Estado">UF *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[0].EstadoId", Model.EstadosLocalizacao, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna46 prepend2">
					<label for="Empreendimento.Enderecos[0].MunicipioId">Município *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[0].MunicipioId", Model.MunicipiosLocalizacao, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Corrego">Córrego</label>
					<%= Html.TextBox("Empreendimento.Enderecos[0].Corrego", Model.Empreendimento.Enderecos[0].Corrego, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="ultima">
					<label for="Empreendimento.Enderecos[0].Complemento">Complemento / Roteiro para localização *</label>
					<%= Html.TextArea("Empreendimento.Enderecos[0].Complemento", Model.Empreendimento.Enderecos[0].Complemento, new { @class = "textarea media disabled", @disabled = "disabled" })%>
				</div>
			</div>
			
			<div class="block">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_LocalColetaPontoId">Local de coleta do ponto *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.LocalColetaPontoId", Model.LocaisColetaPonto, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_FormaColetaPontoId">Forma de coleta do ponto *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.FormaColetaPontoId", Model.FormasColetaPonto, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>
			</div>

			<div class="block divCamposCoordenadas">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_Tipo_Id">Sistema de coordenada *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.Tipo.Id", Model.TiposCoordenada, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_Datum_Id">Datum *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.Datum.Id", Model.Datuns, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_FusoUtm">Fuso *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.FusoUtm", Model.Fusos, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>
			</div>

			<div class="block divCamposCoordenadas">
				<div class="coluna25">
					<label for="Empreendimento_Coordenada_EastingUtmTexto">Easting *</label>
					<%= Html.TextBox("Empreendimento.Coordenada.EastingUtmTexto", Model.Empreendimento.Coordenada.EastingUtmTexto, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_NorthingUtmTexto">Northing *</label>
					<%= Html.TextBox("Empreendimento.Coordenada.NorthingUtmTexto", Model.Empreendimento.Coordenada.NorthingUtmTexto, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>

				<div class="coluna25 prepend2">
					<label for="Empreendimento_Coordenada_HemisferioUtm">Hemisfério *</label>
					<%= Html.DropDownList("Empreendimento.Coordenada.HemisferioUtm", Model.Hemisferios, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Endereço de Correspondência</legend>

		<div class="block endereco correspondenciaContainer <%= (Model.Empreendimento.TemCorrespondencia > 0) ? "" : "hide" %>">
			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].Cep">CEP *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Cep", Model.Empreendimento.Enderecos[1].Cep, new { @class = "text maskCep disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna76 prepend2">
					<label for="Empreendimento.Enderecos[1].Logradouro">Logradouro *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Logradouro", Model.Empreendimento.Enderecos[1].Logradouro, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna47">
					<label for="Empreendimento.Enderecos[1].Bairro">Bairro *</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Bairro", Model.Empreendimento.Enderecos[1].Bairro, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna49 prepend2">
					<label for="Empreendimento.Enderecos[1].DistritoLocalizacao">Distrito/Localidade</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].DistritoLocalizacao", Model.Empreendimento.Enderecos[1].DistritoLocalizacao, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block divEndereco">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].Numero">Número</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Numero", Model.Empreendimento.Enderecos[1].Numero, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna24 prepend2">
					<label for="Empreendimento.Enderecos[1].Estado">Estado *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[1].EstadoId", Model.EstadosCorrespondencia, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna49 prepend2">
					<label for="Empreendimento.Enderecos[1].MunicipioId">Município *</label>
					<%= Html.DropDownList("Empreendimento.Enderecos[1].MunicipioId", Model.MunicipiosCorrespondencia, new { @class = "text disabled", @disabled = "disabled" })%> 
				</div>
			</div>

			<div class="block">
				<div class="coluna20">
					<label for="Empreendimento.Enderecos[1].CaixaPostal">Caixa postal</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].CaixaPostal", Model.Empreendimento.Enderecos[1].CaixaPostal, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>

				<div class="coluna76 prepend2">
					<label for="Empreendimento.Enderecos[1].Complemento">Complemento</label>
					<%= Html.TextBox("Empreendimento.Enderecos[1].Complemento", Model.Empreendimento.Enderecos[1].Complemento, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Meios de Contato</legend>
		<div class="block">
			<div class="coluna22">
				<label for="Contato.Telefone">Telefone</label>
				<%= Html.TextBox("Contato.Telefone", Model.Contato.Telefone, new { @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna22 prepend2">
				<label for="Contato.TelefoneFax">Telefone fax</label>
				<%= Html.TextBox("Contato.TelefoneFax", Model.Contato.TelefoneFax, new { @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna49 prepend2">
				<label for="Contato.Email">E-mail</label>
				<%= Html.TextBox("Contato.Email", Model.Contato.Email, new { @class = "text maskEmail disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="ultima">
				<label for="Contato_NomeContato">Nome para contato</label>
				<%= Html.TextBox("Contato.NomeContato", Model.Contato.NomeContato, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>
</div>