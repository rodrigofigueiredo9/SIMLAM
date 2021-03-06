﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>

<script>
	FiscalizacaoFinalizar.settings.urls.finalizar = '<%= Url.Action("FinalizarFiscalizacao", "Fiscalizacao") %>';
	FiscalizacaoFinalizar.settings.urls.download = '<%= Url.Action("Baixar", "Arquivo") %>';
    FiscalizacaoFinalizar.settings.urls.pdfAuto = '<%= Url.Action("AutoTermoFiscalizacaoPdf", "Fiscalizacao") %>';
    FiscalizacaoFinalizar.settings.urls.pdfIUF = '<%= Url.Action("InstrumentoUnicoFiscalizacaoPdf", "Fiscalizacao") %>';
    FiscalizacaoFinalizar.settings.urls.pdfIUFBloco = '<%= Url.Action("Baixar", "Arquivo") %>';
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
		<legend>Caracterização da Infração/Fiscalização</legend>

		<div class="block">
			<div class="coluna76">
				<label>Classificação</label><br />
				<%= Html.DropDownList("Fiscalizacao.Infracao.Classificacao", Model.InfracaoVM.Classificacoes, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlClassificacoes" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna76">
				<label>Tipo de infração/Fiscalização</label><br />
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
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[0] != 0 ? Model.InfracaoVM.Penalidades.FirstOrDefault(i => i.Id == Model.InfracaoVM.Infracao.IdsOutrasPenalidades[0].ToString()).Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
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
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[1] != 0 ? Model.InfracaoVM.Penalidades.FirstOrDefault(i => i.Id == Model.InfracaoVM.Infracao.IdsOutrasPenalidades[1].ToString()).Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
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
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[2] != 0 ? Model.InfracaoVM.Penalidades.FirstOrDefault(i => i.Id == Model.InfracaoVM.Infracao.IdsOutrasPenalidades[2].ToString()).Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
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
                        <%= Html.TextBox("Penalidade.Descricao", (Model.InfracaoVM.Infracao.IdsOutrasPenalidades[3] != 0 ? Model.InfracaoVM.Penalidades.FirstOrDefault(i => i.Id == Model.InfracaoVM.Infracao.IdsOutrasPenalidades[3].ToString()).Codigo : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDescricaoPenalidade txtDescricaoPenalidadeOutras" }))%>
                    </div>
                </div>
            </div>
        </div>

	</fieldset>
    
    <fieldset class="box">
        <legend>Multa</legend>

        <div class="block">
            <div class="coluna15 append2">
                <label>Nº IUF</label><br />
				<%= Html.TextBox("MultaVM.Multa.NumeroIUF", (Model.MultaVM.Multa.IsDigital == null ? string.Empty : (!String.IsNullOrWhiteSpace(Model.MultaVM.Multa.NumeroIUF) ? Model.MultaVM.Multa.NumeroIUF : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumIUFMulta", @maxlength = "6"  }))%>
            </div>
            <div class="coluna10 append2">
                <label>Série</label><br />
				<%= Html.TextBox("MultaVM.Multa.Serie", (Model.MultaVM.Multa.IsDigital != null ? Model.MultaVM.Multa.SerieTexto : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtSerieIUFMulta" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Data de lavratura do IUF</label><br />
				<%= Html.TextBox("MultaVM.Multa.Data", (Model.MultaVM.Multa.IsDigital == null ? string.Empty : (Model.MultaVM.Multa.DataLavratura.Data != DateTime.MinValue ? Model.MultaVM.Multa.DataLavratura.DataTexto : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataIUFMulta maskData" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Emitido por</label><br />
				<%= Html.TextBox("MultaVM.Multa.Emitido", (Model.MultaVM.Multa.IsDigital != null ? (Model.MultaVM.Multa.IsDigital == true ? "Sistema" : "Bloco") : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmitidoIUFMulta" }))%>
            </div>
        </div>
    </fieldset>

     <fieldset class="box">
        <legend>Interdição/Embargo</legend>

        <div class="block">
            <div class="coluna15 append2">
                <label>Nº IUF</label><br />
				<%= Html.TextBox("ObjetoInfracaoVM.Entidade.NumeroIUF", (Model.ObjetoInfracaoVM.Entidade.IsDigital == null ? string.Empty : (!String.IsNullOrWhiteSpace(Model.ObjetoInfracaoVM.Entidade.NumeroIUF) ? Model.ObjetoInfracaoVM.Entidade.NumeroIUF : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumIUFInterdicaoEmbargo", @maxlength = "6"  }))%>
            </div>
            <div class="coluna10 append2">
                <label>Série</label><br />
				<%= Html.TextBox("ObjetoInfracaoVM.Entidade.Serie", (Model.ObjetoInfracaoVM.Entidade.IsDigital != null ? Model.ObjetoInfracaoVM.Entidade.SerieTexto : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtSerieIUFInterdicaoEmbargo" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Data de lavratura do IUF</label><br />
				<%= Html.TextBox("ObjetoInfracaoVM.Entidade.Data", (Model.ObjetoInfracaoVM.Entidade.IsDigital == null ? string.Empty : (Model.ObjetoInfracaoVM.Entidade.DataLavraturaTermo.Data != DateTime.MinValue ? Model.ObjetoInfracaoVM.Entidade.DataLavraturaTermo.DataTexto : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataIUFInterdicaoEmbargo maskData" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Emitido por</label><br />
				<%= Html.TextBox("ObjetoInfracaoVM.Entidade.Emitido", (Model.ObjetoInfracaoVM.Entidade.IsDigital != null ? (Model.ObjetoInfracaoVM.Entidade.IsDigital == true ? "Sistema" : "Bloco") : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmitidoIUFInterdicaoEmbargo" }))%>
            </div>
        </div>
    </fieldset>

    <fieldset class="box">
        <legend>Apreensão</legend>

        <div class="block">
            <div class="coluna15 append2">
                <label>Nº IUF</label><br />
				<%= Html.TextBox("MaterialApreendidoVM.MaterialApreendido.NumeroIUF", (Model.MaterialApreendidoVM.MaterialApreendido.IsDigital == null ? string.Empty : (!String.IsNullOrWhiteSpace(Model.MaterialApreendidoVM.MaterialApreendido.NumeroIUF) ? Model.MaterialApreendidoVM.MaterialApreendido.NumeroIUF : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumIUFApreensao", @maxlength = "6"  }))%>
            </div>
            <div class="coluna10 append2">
                <label>Série</label><br />
				<%= Html.TextBox("MaterialApreendidoVM.MaterialApreendido.Serie", (Model.MaterialApreendidoVM.MaterialApreendido.IsDigital != null ? Model.MaterialApreendidoVM.MaterialApreendido.SerieTexto : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtSerieIUFApreensao" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Data de lavratura do IUF</label><br />
				<%= Html.TextBox("MaterialApreendidoVM.MaterialApreendido.Data", (Model.MaterialApreendidoVM.MaterialApreendido.IsDigital == null ? string.Empty : (Model.MaterialApreendidoVM.MaterialApreendido.DataLavratura.Data != DateTime.MinValue ? Model.MaterialApreendidoVM.MaterialApreendido.DataLavratura.DataTexto : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataIUFApreensao maskData" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Emitido por</label><br />
				<%= Html.TextBox("MaterialApreendidoVM.MaterialApreendido.Emitido", (Model.MaterialApreendidoVM.MaterialApreendido.IsDigital != null ? (Model.MaterialApreendidoVM.MaterialApreendido.IsDigital == true ? "Sistema" : "Bloco") : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmitidoIUFApreensao" }))%>
            </div>
        </div>
    </fieldset>

    <fieldset class="box">
        <legend>Outras Penalidades</legend>

        <div class="block">
            <div class="coluna15 append2">
                <label>Nº IUF</label><br />
				<%= Html.TextBox("OutrasPenalidadesVM.OutrasPenalidades.NumeroIUF", (Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital == null ? string.Empty : (!String.IsNullOrWhiteSpace(Model.OutrasPenalidadesVM.OutrasPenalidades.NumeroIUF) ? Model.OutrasPenalidadesVM.OutrasPenalidades.NumeroIUF : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumIUFOutrasPenalidades", @maxlength = "6"  }))%>
            </div>
            <div class="coluna10 append2">
                <label>Série</label><br />
				<%= Html.TextBox("OutrasPenalidadesVM.OutrasPenalidades.Serie", (Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital != null ? Model.OutrasPenalidadesVM.OutrasPenalidades.SerieTexto : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtSerieIUFOutrasPenalidades" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Data de lavratura do IUF</label><br />
				<%= Html.TextBox("OutrasPenalidadesVM.OutrasPenalidades.Data", (Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital == null ? string.Empty : (Model.OutrasPenalidadesVM.OutrasPenalidades.DataLavratura.Data != DateTime.MinValue ? Model.OutrasPenalidadesVM.OutrasPenalidades.DataLavratura.DataTexto : "Gerado automaticamente")), ViewModelHelper.SetaDisabled(true, new { @class = "text txtDataIUFOutrasPenalidades maskData" }))%>
            </div>
            <div class="coluna17 append2">
                <label>Emitido por</label><br />
				<%= Html.TextBox("OutrasPenalidadesVM.OutrasPenalidade.Emitido", (Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital != null ? (Model.OutrasPenalidadesVM.OutrasPenalidades.IsDigital == true ? "Sistema" : "Bloco") : string.Empty), ViewModelHelper.SetaDisabled(true, new { @class = "text txtEmitidoIUFOutrasPenalidades" }))%>
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