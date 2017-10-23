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
		<legend>Autoridade Autuante</legend>

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
		<legend>Local da Infração/Fiscalização</legend>

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
		<legend>Autuado/Fiscalizado</legend>

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
		<legend>Caracterização da Infração</legend>

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
		<legend>Penalidades</legend>

		<label>Enquadramento da penalidade conforme Lei 10.476/2015</label>

        <div class="block"  style="padding-top: 15px;">
            <div class="block coluna25">
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.PossuiAdvertencia == null ? false : Model.InfracaoVM.Infracao.PossuiAdvertencia.Value), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeAdvertencia" }))%>Art.2º Item I - Advertência</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.PossuiMulta == null ? false : Model.InfracaoVM.Infracao.PossuiMulta.Value), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeMulta" }))%>Art.2º Item II - Multa</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.PossuiApreensao == null ? false : Model.InfracaoVM.Infracao.PossuiApreensao.Value), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeApreensao" }))%>Art.2º Item III - Apreensão</label><br />
                </div>
                <div class="block" style="height:28px; align-self:center;">
                    <label><%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.PossuiInterdicaoEmbargo == null ? false : Model.InfracaoVM.Infracao.PossuiInterdicaoEmbargo.Value), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeInterdicaoEmbargo" }))%>Art.2º Item IV - Interdição ou embargo</label>
                </div>
            </div>

            <div class="coluna1" style="border-left: 2px solid; height:130px;">
                <%--Linha vertical--%>
            </div>

            <div class="block coluna60">

                <%foreach (var item in Model.InfracaoVM.Penalidades)
                  { %>
                    <input type="hidden" class="hdnPenalidade<%:item.Id%>" value="<%:item.Codigo %>" />
                <% } %>

                <div class="block" style="height:28px; align-self:center;">
                    <div class="coluna2 append2">
                        <%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[0] != 0 ? true : false), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.InfracaoVM.ListaPenalidades01, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[0] != 0 ? Model.InfracaoVM.Penalidades[0].Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[1] != 0 ? true : false), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.InfracaoVM.ListaPenalidades02, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[1] != 0 ? Model.InfracaoVM.Penalidades[1].Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[2] != 0 ? true : false), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.InfracaoVM.ListaPenalidades03, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[2] != 0 ? Model.InfracaoVM.Penalidades[2].Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>

                <div class="block">
                    <div class="coluna2 append2" style="height:28px; align-self:center;">
                        <%= Html.CheckBox("Penalidade.Item", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[3] != 0 ? true : false), ViewModelHelper.SetaDisabled(true, new { @class = "checkbox cbPenalidade cbPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna30 append2" style="height:28px; align-self:center;">
                        <%= Html.DropDownList("Penalidade.Tipo", Model.InfracaoVM.ListaPenalidades04, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTiposPenalidade ddlTiposPenalidadeOutras" }))%>
                    </div>
                    <div class="coluna50" style="height:28px; align-self:center;">
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[3] != 0 ? Model.InfracaoVM.Penalidades[3].Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>
            </div>
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

	<fieldset class="box" >
		<legend>Documentos gerados</legend>
		<div class="block dataGrid">
			<div class="coluna70">
				<% Html.RenderPartial("DocumentosGerados", Model); %>
			</div>
		</div>
	</fieldset>

</div>