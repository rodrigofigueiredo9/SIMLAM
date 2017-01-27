<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/OrgaosParceirosConveniados/orgaoParceiroAlterarSituacao.js") %>" ></script>

	<script>
	    $(function () {
	        OrgaoParceiroAlterarSituacao.load($('#central'), {
	            urls: {
	                salvar: '<%=Url.Action("AlterarSituacao", "OrgaosParceirosConveniados") %>'
	            },
                Mensagens: <%=Model.Mensagens%>
	        });
	    })
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<input type="hidden" class="hdnOrgaoParceiroId" value="<%:Model.OrgaoParceiroConveniado.Id%>" />
	<div id="central">
		<h1 class="titTela">Alterar Situação</h1>
		<br />
		<fieldset class="box">
			<legend>Orgão Parceiro/ Conveniado</legend>
			<div class="block">
				<div class="coluna10 append1">
					<label>Sigla</label>
					<%= Html.TextBox("Situacao.Sigla", Model.OrgaoParceiroConveniado.Sigla, new { @class = "disabled text txtSigla", @disabled="disabled" })%>
				</div>

				<div class="ultima block">
					<label>Nome do órgão</label>
					<%= Html.TextBox("Situacao.NomeOrgao", Model.OrgaoParceiroConveniado.Nome, new { @class = "disabled text txtNome", @disabled="disabled" })%>
				</div>

			</div>
		</fieldset>
		<fieldset class="box">
			<legend>Situação</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Situacao_Atual">Situação atual</label>
					<%= Html.TextBox("Situacao.Atual", Model.OrgaoParceiroConveniado.SituacaoTexto, new { @class = "disabled text txtSituacaoAtual", @disabled="disabled" })%>
				</div>

				<div class="coluna15 prepend1">
					<label for="Situacao_DataAtual">Data de situação atual</label>
					<%= Html.TextBox("Situacao.DataAtual", Model.OrgaoParceiroConveniado.SituacaoData.DataTexto, new { @class = "disabled text txtSituacaoDataAnterior maskData", @disabled="disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna20">
					<label for="Situacao_Nova">Nova situação*</label>
					<%= Html.DropDownList("Situacao.Nova", Model.Situacoes, new {@class = "text ddlSituacaoNova"}) %>
				</div>

				<div class="coluna15 prepend1">
					<label for="Situacao_DataNova">Data da nova situação*</label>
					<%= Html.TextBox("Situacao.DataNova", DateTime.Now.Date.ToShortDateString(), new { @class = "disabled text txtDataSituacaoNova", @disabled="disabled" })%>
				</div>
			</div>

			<div class="ultima divMotivo <%= Model.OrgaoParceiroConveniado.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado? "" : " hide" %> ">
				<label for="AlterarSituacao_Motivo">Motivo*</label>
				<%= Html.TextArea("Situacao.Motivo", Model.OrgaoParceiroConveniado.SituacaoMotivo, ViewModelHelper.SetaDisabled(!string.IsNullOrEmpty(Model.OrgaoParceiroConveniado.SituacaoMotivo), new { @class = "media text txtSituacaoMotivo", @maxlength="300"}))%>
			</div>

		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>

			</div>
		</div>
	</div>
</asp:Content>