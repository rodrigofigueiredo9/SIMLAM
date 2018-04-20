
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LocalInfracaoVM>" %>
<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script>

	FiscalizacaoLocalInfracao.settings.urls.coordenadaGeo = '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento"}) %>';
	FiscalizacaoLocalInfracao.settings.urls.obterEstadosMunicipiosPorCoordenada = '<%= Url.Action("obterEstadosMunicipiosPorCoordenada", "Mapa", new {area="GeoProcessamento"}) %>';
	FiscalizacaoLocalInfracao.settings.urls.obter = '<%= Url.Action("LocalInfracao", "Fiscalizacao") %>';
	FiscalizacaoLocalInfracao.settings.urls.associarAutuadoPessoa = '<%= Url.Action("PessoaModal", "Pessoa") %>';
	FiscalizacaoLocalInfracao.settings.urls.editarAutuadoPessoa = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
    FiscalizacaoLocalInfracao.settings.urls.localizarEmpreendimento = '<%= Url.Action("LocalizarFiscalizacao", "Empreendimento") %>';
    FiscalizacaoLocalInfracao.settings.urls.localizarEmpreendimentoPessoa = '<%= Url.Action("LocalizarFiscalizacaoPessoa", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.localizarEmpreendimentoCodigo = '<%= Url.Action("LocalizarFiscalizacaoCodigo", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.visualizarEmpreendimento = '<%= Url.Action("EmpreendimentoInline", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.editarEmpreendimento = '<%= Url.Action("Editar", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.salvar = '<%= Url.Action("Salvar", "Fiscalizacao") %>';
	FiscalizacaoLocalInfracao.settings.urls.novoEmpreendimento = '<%= Url.Action("Salvar", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.salvarCadastrar = '<%= Url.Action("SalvarCadastrar", "Empreendimento") %>';
	FiscalizacaoLocalInfracao.settings.urls.obterResponsaveis = '<%= Url.Action("ObterResponsaveis", "Fiscalizacao") %>';

</script>

<div class="divLocalInfracao">
	<%= Html.Hidden("hdnLocalInfracaoId", Model.LocalInfracao.Id, new { @class = "hdnLocalInfracaoId" })%>
	<%= Html.Hidden("EstadoDefault", Model.EstadoDefault, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.EstadoDefaultSigla, new { @class = "hdnEstadoDefaultSigla" })%>
	<%= Html.Hidden("CidadeDefault", Model.MunicipioDefault, new { @class = "hdnCidadeDefault" })%>
	<%= Html.Hidden("hdnResponsavelId", Model.LocalInfracao.ResponsavelId, new { @class = "hdnResponsavelId" })%>
	<%= Html.Hidden("hdnResponsavelPropriedadeId", Model.LocalInfracao.ResponsavelPropriedadeId, new { @class = "hdnResponsavelPropriedadeId" })%>
	<%= Html.Hidden("hdnAssinantePropriedadeId", Model.LocalInfracao.AssinantePropriedadeId, new { @class = "hdnAssinantePropriedadeId" })%>

	<fieldset class="block box">
		<div class="block">
			<div class="coluna21">
				<label for="txtNumeroInfracao">Nº da Fiscalização *</label>
				<%= Html.TextBox("txtNumeroInfracao", Model.LocalInfracao.FiscalizacaoId == 0 ? "Gerado automático" : Model.LocalInfracao.FiscalizacaoId.ToString(), ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumeroInfracao" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna70">
				<label for="LocalInfracao_SetorId">Setor de cadastro *</label>
				<%= Html.DropDownList("LocalInfracao.SetorId", Model.Setores, ViewModelHelper.SetaDisabled(Model.Setores.Count <= 2 || Model.IsVisualizar, new { @class = "text ddlSetores" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label for="LocalInfracao_AreaFiscalizacao">Área da Fiscalização *</label><br />
				<label><%= Html.RadioButton("rblAreaFiscalizacao", 0, Model.LocalInfracao.AreaFiscalizacao == 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblAreaFiscalizacao rblAreaFiscalizacaoDDSIA" }))%>DDSIA</label><br />
                <label><%= Html.RadioButton("rblAreaFiscalizacao", 1, Model.LocalInfracao.AreaFiscalizacao == 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblAreaFiscalizacao rblAreaFiscalizacaoDDSIV" }))%>DDSIV</label><br />
                <label><%= Html.RadioButton("rblAreaFiscalizacao", 2, Model.LocalInfracao.AreaFiscalizacao == 2, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblAreaFiscalizacao rblAreaFiscalizacaoDRNRE" }))%>DRNRE</label>
			</div>
		</div>
	</fieldset>

    <fieldset class="block box">
		<legend>Autuado/Fiscalizado</legend>
		<div class="block">
			<div class="coluna100">
				<label for="">Infração ocorreu dentro de empreendimento? * <span style="font-style: italic; color: Gray;">(Empreendimento = Propriedade rural/urbana, Comercio/Serviço, Indústria, Obra de Infraestrutura.)</span></label><br />
				<label><%= Html.RadioButton("rblAutuado", 1, Model.LocalInfracao.EmpreendimentoId > 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblAutuado" }))%>Sim</label>
				<label><%= Html.RadioButton("rblAutuado", 0, (Model.LocalInfracao.EmpreendimentoId == 0 || Model.LocalInfracao.EmpreendimentoId == null) && Model.LocalInfracao.PessoaId > 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblAutuado prepend2" }))%>Não</label>
			</div>
		</div>
		<div class="block divPessoa <%= Model.LocalInfracao.PessoaId > 0 ? "" : "hide" %>">

		<%= Html.Hidden("hdnAutuadoPessoaId", Model.LocalInfracao.PessoaId, new { @class = "hdnAutuadoPessoaId" })%>
		<%= Html.Hidden("hdnAutuadoPessoaTid", Model.LocalInfracao.PessoaTid, new { @class = "hdnAutuadoPessoaTid" })%>

			<div class="coluna50">
				<label for="txtNomeRazao">Nome / Razão Social *</label>
				<%= Html.TextBox("txtNomeRazao", Model.Pessoa.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNomeRazao" }))%>
			</div>
			<div class="coluna30 prepend2">
				<label for="txtCpfCnpj">CPF/CNPJ *</label>
				<%= Html.TextBox("txtCpfCnpj", Model.Pessoa.CPFCNPJ, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCpfCnpj" }))%>
			</div>
			<div class="prepend2">
				<% if (!Model.IsVisualizar) { %>
				<button type="button" class="floatLeft inlineBotao btnBuscarPessoa" title="Buscar autuado">Buscar</button>
				<% } %>
				<span class="spanVisualizarAutuado <%= (Model.LocalInfracao.PessoaId > 0) ? "" : "hide" %>"><button type="button" class="icone visualizar esquerda inlineBotao btnEditarVisualizarPessoa" title="Visualizar autuado"></button></span>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box fsLocalInfracao <%= (!String.IsNullOrWhiteSpace(Model.LocalInfracao.LonEasting)) ? "" : "hide" %>">
		<legend>Local da infração</legend>
		<div class="block">
			<div class="coluna21">
				<label>Sistema de coordenada *</label>
				<%= Html.DropDownList("LocalInfracao.SistemaCoordId", Model.CoordenadasSistema, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%>
			</div>

			<div class="coluna19 prepend2">
				<label>Datum *</label>
				<%= Html.DropDownList("LocalInfracao.Datum", Model.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%>
			</div>

			<div class="coluna19 prepend2">
				<label>Fuso *</label>
				<%= Html.DropDownList("LocalInfracao.Fuso", Model.Fusos, new { @class = "text disabled ddlFuso", @disabled = "disabled" })%>
			</div>            
		</div>

		<div class="block">
			<div class="coluna21">
				<label>Easting *</label>
				<%= Html.TextBox("LocalInfracao.Setor.Easting", Model.LocalInfracao.LonEasting, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
			</div>

			<div class="coluna19 prepend2">
				<label>Northing *</label>
				<%= Html.TextBox("LocalInfracao.Setor.Northing", Model.LocalInfracao.LatNorthing, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
			</div>

			<div class="coluna19 prepend2">
				<label>Hemisfério *</label>
				<%= Html.DropDownList("LocalInfracao.Setor.Hemisfério", Model.Hemisferios, new { @class = "text disabled ddlHemisferio", @disabled = "disabled" })%>
			</div>

			<div class="coluna21 prepend2 divAreaAbrangencia">
				<label>Área de abrangência (m) *</label>
				<%= Html.TextBox("LocalInfracao.AreaAbrangencia", Model.LocalInfracao.AreaAbrangencia, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtAreaAbran", @maxlength = "5" }))%>
			</div>
		</div>

		<div class="block divEndereco divEnderecoLocalizacao">
			<div class="coluna11">
				<label for="Filtros_EstadoId">UF *</label>
				<%= Html.DropDownList("Filtros.EstadoId", Model.Estados, new { @class = "text ddlEstado disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna51 prepend2">
				<label for="LocalInfracao_MunicipioId">Município *</label>
				<%= Html.DropDownList("LocalInfracao.MunicipioId", Model.Municipios, new { @class = "text ddlMunicipio disabled", @disabled = "disabled" })%>
			</div>

			<% if (!Model.IsVisualizar) { %>
			<div class="coluna20 prepend2">
				<button type="button" class="inlineBotao btnBuscarCoorLocal">Buscar</button>
			</div>
			<% } %>
		</div>

		<div class="block">
			<div class="coluna90">
				<label for="LocalInfracao_Local">Nome do empreendimento e Endereço da infração/ocorrência *</label>
				<%= Html.TextBox("LocalInfracao.Local", Model.LocalInfracao.Local, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtLocal", maxlength = "150" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box fsLocalInfracaoCodEmp hide">
		<legend>Identificação do Empreendimento</legend>
		<div class="block">
			<div class="coluna21">
				<label>Código do Empreendimento</label>
				<%= Html.TextBox("LocalInfracao.FiltroCodigoEmp", null, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtFiltroCodigoEmp maskNum15", maxlength = "150" }))%>
			</div>
		<% if (!Model.IsVisualizar) { %>
		<div class="coluna20 prepend2">
			<button type="button" class="inlineBotao btnVerificarPorCodEmp">Verificar</button>
		</div>
		<% } %>
		</div>
	</fieldset>

    <fieldset class="block box fsEmpreendimentoBuscar <%= (Model.LocalInfracao.EmpreendimentoId > 0) ? "" : "hide" %>">
		    <div class="divEmpreendimento <%= Model.LocalInfracao.EmpreendimentoId > 0 ? "" : "hide" %>">
			    <%= Html.Hidden("hdnAutuadoEmpreendimentoId", Model.LocalInfracao.EmpreendimentoId, new { @class = "hdnAutuadoEmpreendimentoId" })%>
			    <%= Html.Hidden("hdnAutuadoEmpreendimentoTid", Model.LocalInfracao.EmpreendimentoTid, new { @class = "hdnAutuadoEmpreendimentoTid" })%>

			    <div class="block divBtnVerificarEmpreendimento hide">
				    <div class="coluna30">
                        <label>Empreendimentos associados</label>
					    <button type="button" class="btnVerificarEmpPessoa">Verificar Empreendimento</button>
                        <button type="button" class="inlineBotao btnVerificarEmp hide">Verificar Empreendimento</button>
				    </div>
			    </div>
                
			    <fieldset class="block boxBranca fdsEmpreendimento hide">
				    <legend>Empreendimento Autuado</legend>
				    <div class="divResultados">

				    </div>
				    <div class="block divDadosEmpreendimento">
					    <div class="empreendimentoPartial">

					    </div>
				    </div>

				    <% if (!Model.IsVisualizar) { %>
				    <div class="block box divBotoes">
					    <span class="modoVisualizar">
						    <span class="spanBotoes spanEmpAssociar hide">
							    <input class="floatLeft btnEmpAssociar" type="button" value="Associar" />
						    </span>
						    <span class="spanBotoes spanEmpSalvar hide">
							    <input class="floatLeft btnEmpSalvar" type="button" value="Editar" />
						    </span>
						    <span class="spanBotoes spanEmpNovo hide">
							    <input class="floatLeft btnEmpNovo" type="button" value="Novo" />
						    </span>
                            <span class="spanBotoes spanEmpBuscaLocal hide">
							    <input class="floatLeft btnEmpBuscaLocal" type="button" value="Buscar por Localização" />
						    </span>
							<span class="spanBotoes spanEmpBuscaEmp hide">
							    <input class="floatLeft btnEmpBuscaEmp" type="button" value="Buscar por Cód. Empreendimento" />
						    </span>
						    <span class="spanBotoes spanEmpSalvarCadastrar hide">
							    <input class="floatLeft btnEmpSalvarCadastrar" type="button" value="Salvar" />
						    </span>
						    <span class="spanBotoes spanEmpSalvarEditar hide">
							    <input class="floatLeft btnEmpSalvarEditar" type="button" value="Salvar" />
						    </span>
						    <span class="spanBotoes spanEmpAssNovoPessoa hide">
							    <input class="floatLeft btnEmpAssNovoPessoa" type="button" value="Buscar Novo" />
						    </span>
                            <span class="spanBotoes spanEmpAssNovo hide">
							    <input class="floatLeft btnEmpAssNovo" type="button" value="Buscar Novo" />
						    </span>
					    </span>
					    <span class="spanCancelarEmp cancelarCaixa">ou <a class="linkCancelar linkCancelarEmp">Cancelar</a></span>
				    </div>
				    <% } %>
			    </fieldset>
		    </div>

		    <div class="block divDdlResponsavel <%= Model.LocalInfracao.EmpreendimentoId > 0 ? "" : "hide" %>">
			    <div class="coluna70">
				    <label for="LocalInfracao_ResponsavelPropriedadeId">Responsável do Empreendimento *</label>
				    <%= Html.DropDownList("LocalInfracao.ResponsavelPropriedadeId", Model.Responsavel, ViewModelHelper.SetaDisabled(Model.Responsavel.Count == 1 || Model.IsVisualizar, new { @class = "text ddlResponsaveisPropriedade " }))%>
			    </div>
				<br />
				<div class="coluna70">
				    <label for="LocalInfracao_AssinantePropriedadeId">Assinante</label>
				    <%= Html.DropDownList("LocalInfracao.AssinantePropriedadeId", Model.Assinante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlAssinantesPropriedade " }))%>
			    </div>
		    </div>
        </fieldset>
</div>

