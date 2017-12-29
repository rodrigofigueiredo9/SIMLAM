<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoAlterarSituacao.js") %>" ></script>
	<script>
		$(function () {
			FiscalizacaoAlterarSituacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AlterarSituacao", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Alterar Situação</h2><br />

		<fieldset class="box">
			<legend>Fiscalização</legend>

			<input type="hidden" class="hdnFiscalizacaoId" value="<%:Model.Fiscalizacao.Id%>" />

			<div class="block">
				<div class="coluna25 append2">
					<label for="Fiscalizacao_Numero">Nº da Fiscalização</label>
					<%= Html.TextBox("Fiscalizacao.Numero", Model.Fiscalizacao.Id, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoNumero" }))%>
				</div>

				<div class="coluna18">
					<label for="Fiscalizacao_Data_DataTexto">Data da Vistoria</label>
					<%= Html.TextBox("Fiscalizacao.Data.DataTexto", Model.Fiscalizacao.DataFiscalizacao, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoData maskData" }))%>
				</div>
			</div>
		</fieldset>

		<fieldset class="box">
			<legend>Agente Fiscal</legend>

			<div class="block">
				<div class="coluna73 append2">
					<label for="Fiscalizacao_Autuante_Nome">Nome</label>
					<%= Html.TextBox("Fiscalizacao.Autuante.Nome", Model.Fiscalizacao.Autuante.Nome, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoAutuanteNome" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna73">
					<label for="Fiscalizacao_Setor">Setor</label>
					<%= Html.DropDownList("Fiscalizacao.Setor", Model.Setores, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlFiscalizacaoSetor" }))%>
				</div>
			</div>
		</fieldset>

		<fieldset class="box">
			<legend>Autuado</legend>

			<%if(Model.Fiscalizacao.ComplementacaoDados.AutuadoTipo == (int)eTipoAutuado.Pessoa) {%>
			<div class="block">
				<div class="coluna45 append2">
					<label for="Fiscalizacao_AutuadoPessoa_NomeRazaoSocial">Nome/Razão Social *</label>
					<%= Html.TextBox("Fiscalizacao.AutuadoPessoa.NomeRazaoSocial", Model.Fiscalizacao.AutuadoPessoa.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoAutuadoNomeRazaoSocial" }))%>
				</div>

				<div class="coluna25">
					<label for="Fiscalizacao_AutuadoPessoa_CPFCNPJ">CPF/CNPJ *</label>
					<%= Html.TextBox("Fiscalizacao.AutuadoPessoa.CPFCNPJ", Model.Fiscalizacao.AutuadoPessoa.CPFCNPJ, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoAutuadoCPFCNPJ" }))%>
				</div>
			</div>
			<%}else{ %>

			<div class="block">
				<div class="coluna25">
					<label for="Fiscalizacao_Segmento">Segmento *</label>
					<%= Html.DropDownList("Fiscalizacao.Segmento", Model.Segmentos, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoSegmento" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna45 append2">
					<label for="Fiscalizacao_AutuadoEmpreendimento_Denominador">Denominação *</label>
					<%= Html.TextBox("Fiscalizacao.AutuadoEmpreendimento.Denominador", Model.Fiscalizacao.AutuadoEmpreendimento.Denominador, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoAutuadoEmpreendimentoDenominador" }))%>
				</div>

				<div class="coluna25">
					<label for="Fiscalizacao_AutuadoEmpreendimento_CNPJ">CNPJ *</label>
					<%= Html.TextBox("Fiscalizacao.AutuadoEmpreendimento.CNPJ", Model.Fiscalizacao.AutuadoEmpreendimento.CNPJ, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacaoAutuadoEmpreendimentoCNPJ" }))%>
				</div>
			</div>

			<%} %>
		</fieldset>

		<fieldset class="box">
			<legend>Situação</legend>

			<div class="block">
				<div class="coluna73">
					<label for="Fiscalizacao_SituacaoAtual">Situação atual *</label>
					<%= Html.DropDownList("Fiscalizacao.SituacaoAtual", Model.SituacaoAtual, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSituacaoAtual" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna19">
					<label for="Fiscalizacao_SituacaoAtualData_DataTexto">Data da situação atual *</label>
					<%= Html.TextBox("Fiscalizacao.SituacaoAtualData.DataTexto", Model.Fiscalizacao.SituacaoAtualData.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text txtSituacaoAtualData maskData" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna73">
					<label for="Fiscalizacao_SituacaoNovaTipo">Nova situação *</label>
					<%= Html.DropDownList("Fiscalizacao.SituacaoNovaTipo", Model.SituacaoNova, new { @class = "text ddlSituacaoNovaTipo" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna19">
					<label for="Fiscalizacao_SituacaoNovaData_DataTexto">Data da nova situação *</label>
					<%= Html.TextBox("Fiscalizacao.SituacaoNovaData.DataTexto", (Model.Fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.CadastroConcluido) ? DateTime.Now.ToShortDateString() : String.Empty, ViewModelHelper.SetaDisabled((Model.Fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.CadastroConcluido), new { @class = "text txtSituacaoNovaData maskData" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna73">
					<label for="Fiscalizacao_SituacaoNovaMotivoTexto">Motivo *</label>
					<%= Html.TextArea("Fiscalizacao.SituacaoNovaMotivoTexto", String.Empty, new { @class = "text media txtSituacaoNovaMotivoTexto", @maxlength="300"})%>
				</div>
			</div>
		</fieldset>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "Fiscalizacao")%>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>