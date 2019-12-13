<%@ Import Namespace="Tecnomapas.Blocos.Etx.ModuloExtensao.Entities" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CARCancelamento>" %>

<fieldset class="box">
    <legend>Situação</legend>
    <div class="block">
        <div class="coluna20">
            <label for="Situacao_Atual">Situação atual*</label>
            <%= Html.TextBox("CarCancelamento..Atual", Model.Situacao.ToDescription(), new { @class = "disabled text", @disabled="disabled" })%>
        </div>

        <div class="coluna15 prepend1">
            <label for="Situacao_DataAtual">Data de situação atual*</label>
            <%= Html.TextBox("CarCancelamento..DataAtual", Model.SituacaoData.DataTexto, new { @class = "disabled text maskData", @disabled="disabled" })%>
        </div>

        <div class="coluna15 prepend1">
            <label for="Situacao_DataNova">Autor*</label>
            <%= Html.TextBox("CarCancelamento..Autor", Model.Autor.Nome, new { @class = "disabled text", @disabled = "disabled" })%>
        </div>
    </div>
	<% if(Model.Situacao != eCARSolicitacaoSituacao.Suspenso) {%>
		<div class="block">
			<div class="coluna20">
				<label for="Situacao_Nova">Motivo*</label>
				<%= Html.TextBox("CarCancelamento.Motivo", Model.Motivo.ToDescription(), new { @class = "disabled text", @disabled = "disabled" }) %>
			</div>

			<div class="block">
				<div class="coluna41 prepend1 inputFileDiv">
					<label for="ArquivoTexto">Arquivo de autorização *</label>
					<% if (Model.ArquivoAnexo.Id.GetValueOrDefault() > 0)
						{ %>
					<div>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.ArquivoAnexo.Nome, 45), "Baixar", "Arquivo", new { @id = Model.ArquivoAnexo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.ArquivoAnexo.Nome })%>
					</div>
					<% } %>
				</div>
			</div>
		</div>
	<%} %>
    <div class="ultima divMotivo">
        <label for="AlterarSituacao_Motivo">Descrição do motivo*</label>
        <%= Html.TextArea("CarCancelamento.Motivo", Model.DecricaoMotivo, ViewModelHelper.SetaDisabled(true, new { @class = "media text ", @maxlength="300" }))%>
    </div>
</fieldset>
