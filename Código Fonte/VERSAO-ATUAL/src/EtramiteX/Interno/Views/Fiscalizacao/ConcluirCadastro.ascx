<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>

<script>
	FiscalizacaoFinalizar.settings.urls.finalizar = '<%= Url.Action("FinalizarFiscalizacao", "Fiscalizacao") %>';
	FiscalizacaoFinalizar.settings.urls.download = '<%= Url.Action("Baixar", "Arquivo") %>';
	FiscalizacaoFinalizar.settings.urls.pdfAuto = '<%= Url.Action("AutoTermoFiscalizacaoPdf", "Fiscalizacao") %>';
	FiscalizacaoFinalizar.settings.urls.pdfLaudo = '<%= Url.Action("LaudoFiscalizacaoPdf", "Fiscalizacao") %>';
</script>

<div class="divFinalizar">

	<fieldset class="box">
		<legend>Agente Fiscal</legend>

		<div class="block">
			<div class="coluna60 ">
				<label for="Fiscalizacao_Autuante_Nome">Nome</label><br />
				<%= Html.TextBox("Fiscalizacao.Autuante.Nome", Model.Fiscalizacao.Autuante.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtAutuanteNome" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna60">
				<label for="LocalInfracaoVM_LocalInfracao_SetorId">Setor de cadastro</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.SetorId", Model.LocalInfracaoVM.Setores, ViewModelHelper.SetaDisabled(true, new { @class = "text  ddlSetores" }))%>
			</div>
		</div>

	</fieldset>

	<fieldset class="box">
		<legend>Local da infração</legend>

		<div class="block">
			<div class="coluna20 append2">
				<label>Sistema de coordenada</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.SistemaCoordId", Model.LocalInfracaoVM.CoordenadasSistema, new { @class = "text disabled ddlCoordenadaTipo", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 append2">
				<label>DATUM</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.Datum", Model.LocalInfracaoVM.Datuns, new { @class = "text disabled ddlDatum", @disabled = "disabled" })%>
			</div>
			<div class="coluna20">
				<label>Fuso</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.Fuso", Model.LocalInfracaoVM.Fusos, new { @class = "text disabled ddlFuso", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna20 append2">
				<label>Latitude/Northing</label><br />
				<%= Html.TextBox("LocalInfracaoVM.LocalInfracao.Setor.Northing", Model.LocalInfracaoVM.LocalInfracao.LatNorthing, new { @class = "text disabled txtNorthing", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 append2">
				<label>Longitude/Easting</label><br />
				<%= Html.TextBox("LocalInfracaoVM.LocalInfracao.Setor.Easting", Model.LocalInfracaoVM.LocalInfracao.LonEasting, new { @class = "text disabled txtEasting", @disabled = "disabled" })%>
			</div>
			<div class="coluna20">
				<label>Hemisfério</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.Setor.Hemisferio", Model.LocalInfracaoVM.Hemisferios, new { @class = "text disabled ddlHemisferio", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna20 append2">
				<label>UF</label><br />
				<%= Html.DropDownList("EstadoId", Model.LocalInfracaoVM.Estados, new { @class = "text ddlEstado disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna43">
				<label>Municipio</label><br />
				<%= Html.DropDownList("LocalInfracaoVM.LocalInfracao.MunicipioId", Model.LocalInfracaoVM.Municipios, new { @class = "text ddlMunicipio disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna66">
				<label>Local</label><br />
				<%= Html.TextBox("LocalInfracaoVM.LocalInfracao.Local", Model.LocalInfracaoVM.LocalInfracao.Local, ViewModelHelper.SetaDisabled(true, new { @class = "text txtLocal" }))%>
			</div>
		</div>

	</fieldset>

	<fieldset class="box">
		<legend>Autuado</legend>

		<div class="block divPessoa <%= Model.LocalInfracaoVM.LocalInfracao.PessoaId > 0 ? "" : "hide" %>">
		<%= Html.Hidden("hdnAutuadoPessoaId", Model.LocalInfracaoVM.LocalInfracao.PessoaId, new { @class = "hdnAutuadoPessoaId" })%>
			<div class="coluna50">
				<label for="txtNomeRazao">Nome / Razão Social *</label>
				<%= Html.TextBox("txtNomeRazao", Model.LocalInfracaoVM.Pessoa.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNomeRazao" }))%>
			</div>
			<div class="coluna30 prepend2">
				<label for="txtCpfCnpj">CPF/CNPJ *</label>
				<%= Html.TextBox("txtCpfCnpj", Model.LocalInfracaoVM.Pessoa.CPFCNPJ, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCpfCnpj" }))%>
			</div>
		</div>

		<div class="divEmpreendimento <%= Model.LocalInfracaoVM.LocalInfracao.EmpreendimentoId > 0 ? "" : "hide" %>">

			<div class="block">
				<div class="coluna30">
					<label for="Segmento">Segmento</label>
					<%= Html.DropDownList("Segmento", new List<SelectListItem> { new SelectListItem{ Text = Model.Fiscalizacao.AutuadoEmpreendimento.SegmentoTexto, Value = Model.Fiscalizacao.AutuadoEmpreendimento.Segmento.ToString(), Selected = true} }, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSegmento" }))%>
				</div>
			</div>
			<div class="block">
				<div class="coluna50">
					<label for="Fiscalizacao_AutuadoEmpreendimento_Denominador"><%= Model.Fiscalizacao.AutuadoEmpreendimento.SegmentoDenominador %></label>
					<%= Html.TextBox("Fiscalizacao.AutuadoEmpreendimento.Denominador", Model.Fiscalizacao.AutuadoEmpreendimento.Denominador, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNomeRazao" }))%>
				</div>
				<div class="coluna30 prepend2">
					<label for="Fiscalizacao_AutuadoEmpreendimento_CNPJ">CNPJ</label>
					<%= Html.TextBox("Fiscalizacao.AutuadoEmpreendimento.CNPJ", Model.Fiscalizacao.AutuadoEmpreendimento.CNPJ, ViewModelHelper.SetaDisabled(true, new { @class = "text maskCnpj txtCnpj" }))%>
				</div>
			</div>
		</div>

	</fieldset>

	<fieldset class="box fdsInfracao">
		<legend>Classificação da infração</legend>

		<div class="block">
			<div class="coluna76">
				<label>Classificação</label><br />
				<%= Html.DropDownList("Fiscalizacao.Infracao.Classificacao", Model.InfracaoVM.Classificacoes, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlClassificacoes" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label>Tipo de infração</label><br />
				<%= Html.DropDownList("Fiscalizacao.Infracao.Tipo", Model.InfracaoVM.Tipos, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTipos" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label>Item</label><br />
				<%= Html.DropDownList("Fiscalizacao.Infracao.Item", Model.InfracaoVM.Itens, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlItens" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label>Subitem</label><br />
				<%= Html.DropDownList("Fiscalizacao.Infracao.Subitem", Model.InfracaoVM.Subitens, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSubitens" }))%>
			</div>
		</div>

		<div class="divCamposPerguntas" >
			<% Html.RenderPartial("InfracaoCamposPerguntas", Model.InfracaoVM); %>
		</div>

	</fieldset>

	<fieldset class="box">

		<div class="block">
			<div class="coluna20 append2">
				<label>Auto de infração?</label><br />
				<label><%= Html.RadioButton("InfracaoVM.Infracao.IsAutuada", 1, (Model.InfracaoVM.Infracao.IsAutuada == null ? false : Model.InfracaoVM.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdoIsAutuadaSim" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("InfracaoVM.Infracao.IsAutuada", 0, (Model.InfracaoVM.Infracao.IsAutuada == null ? false : !Model.InfracaoVM.Infracao.IsAutuada.Value), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdoIsAutuadaNao" }))%>Não</label>
			</div>
			<div class="coluna30 append2">
				<label>Área embargada e/ou atividade interditada?</label><br />
				<label><%= Html.RadioButton("ObjetoInfracaoVM.Entidade.AreaEmbargadaAtvIntermed", 1, (Model.ObjetoInfracaoVM.Entidade.AreaEmbargadaAtvIntermed == 1), ViewModelHelper.SetaDisabled(true, new { @class = "rdbAreaEmbarcadaAtvIntermed" }))%>Sim</label>
				<label><%= Html.RadioButton("ObjetoInfracaoVM.Entidade.AreaEmbargadaAtvIntermed", 0, (Model.ObjetoInfracaoVM.Entidade.AreaEmbargadaAtvIntermed == 0), ViewModelHelper.SetaDisabled(true, new { @class = "rdbAreaEmbarcadaAtvIntermed" }))%>Não</label>
			</div>
			<div class="coluna30">
				<label>Houve a apreensão de algum material?</label><br />
				<label><%= Html.RadioButton("MaterialApreendidoVM.MaterialApreendido.IsApreendido", 1, (Model.MaterialApreendidoVM.MaterialApreendido.IsApreendido == null ? false : Model.MaterialApreendidoVM.MaterialApreendido.IsApreendido.Value), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdoIsApreendidoSim" }))%>Sim</label>
				<label class="append5"><%= Html.RadioButton("MaterialApreendidoVM.MaterialApreendido.IsApreendido", 0, (Model.MaterialApreendidoVM.MaterialApreendido.IsApreendido == null ? false : !Model.MaterialApreendidoVM.MaterialApreendido.IsApreendido.Value), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdoIsApreendidoNao" }))%>Não</label>
			</div>
		</div>

	</fieldset>

	<fieldset class="box<%= Model.InfracaoVM.Infracao.IsAutuada.GetValueOrDefault() ? "" : " hide" %>">
		<legend>Auto de infração</legend>

		<div class="block">
			<div class="coluna20 append2">
				<label>Nº AI</label><br />
				<%= Html.TextBox("N_AI", Model.N_AI, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna20 append2">
				<label>Emitido por</label><br />
				<%= Html.TextBox("N_AI_EmitidoPor", Model.N_AI_EmitidoPor, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna22">
				<label>Data da lavratura do auto</label><br />
				<%= Html.TextBox("N_AI_DataAuto", Model.N_AI_DataAuto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
		</div>

	</fieldset>

	<fieldset class="box<%= Model.ObjetoInfracaoVM.Entidade.AreaEmbargadaAtvIntermed.GetValueOrDefault() == 1 ? "" : " hide" %>">
		<legend>Termo de embargo e interdição</legend>

		<div class="block">
			<div class="coluna20 append2">
				<label>Nº TEI</label><br />
				<%= Html.TextBox("N_TEI", Model.N_TEI, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna20 append2">
				<label>Emitido por</label><br />
				<%= Html.TextBox("N_TEI_EmitidoPor", Model.N_TEI_EmitidoPor, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna22">
				<label>Data da lavratura do termo</label><br />
				<%= Html.TextBox("N_TEI_DataTermo", Model.N_TEI_DataTermo, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
		</div>

	</fieldset>

	<fieldset class="box<%= Model.MaterialApreendidoVM.MaterialApreendido.IsApreendido.GetValueOrDefault() ? "" : " hide" %>">
		<legend>Termo de apreensão e depósito</legend>

		<div class="block">
			<div class="coluna20 append2">
				<label>Nº TAD</label><br />
				<%= Html.TextBox("N_TAD", Model.N_TAD, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna20 append2">
				<label>Emitido por</label><br />
				<%= Html.TextBox("N_TAD_EmitidoPor", Model.N_TAD_EmitidoPor, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
			<div class="coluna22">
				<label>Data da lavratura do termo</label><br />
				<%= Html.TextBox("N_TAD_DataTermo", Model.N_TAD_DataTermo, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
		</div>

	</fieldset>

	<fieldset class="box" >
		<legend>Documentos gerados</legend>
		<div class="block dataGrid">
			<div class="coluna70">
				<% Html.RenderPartial("DocumentosGerados", Model); %>
			</div>
		</div>
	</fieldset>

</div>