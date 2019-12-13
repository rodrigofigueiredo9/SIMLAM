<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CARSolicitacaoAlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação da Solicitação de Inscrição</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/CARSolicitacao/alterarSituacao.js") %>"></script>

    <script>
		$(function () {
			CARSolicitacaoAlterarSituacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AlterarSituacao", "CARSolicitacao") %>'
				}

			});

			CARSolicitacaoAlterarSituacao.mensagem = <%=Model.Mensagens %> ;
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" class="hdnSolicitacaoId" value="<%:Model.Solicitacao.Id%>" />
    <div id="central">
        <h1 class="titTela">Alterar Situação da Solicitação de Inscrição</h1>
        <br />
        <fieldset class="box">
            <legend>Solicitação de Inscrição do CAR/ES</legend>
            <div class="block">
                <div class="coluna20">
                    <label for="Situacao_NumeroControle">Nº de controle da solicitação*</label>
                    <%= Html.TextBox("Situacao.NumeroControle", Model.Solicitacao.Numero, new { @class = "disabled text txtSituacaoNumeroControle", @disabled="disabled", @maxlength="7" })%>
                </div>

                <div class="coluna10 prepend1">
                    <label for="Situacao_DataEmissao">Data de emissão</label>
                    <%= Html.TextBox("Situacao.Emissao.DataTexto", Model.Solicitacao.DataEmissao.DataTexto, new { @class = "disabled text txtDataEmissao maskData", @disabled="disabled" })%>
                </div>

            </div>
        </fieldset>
		<%if (Model.Solicitacao.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido) { %>
        <fieldset class="box">
            <legend>Situação</legend>
            <div class="block">
                <div class="coluna20">
                    <label for="Situacao_Atual">Situação atual*</label>
                    <%= Html.TextBox("Situacao.Atual", Model.Solicitacao.SituacaoTexto, new { @class = "disabled text txtSituacaoAtual", @disabled = "disabled" })%>
                </div>

                <div class="coluna15 prepend1">
                    <label for="Situacao_DataAtual">Data de situação atual*</label>
                    <%= Html.TextBox("Situacao.DataAtual", Model.Solicitacao.DataSituacao.DataTexto, new { @class = "disabled text txtSituacaoDataAnterior maskData", @disabled = "disabled" })%>
                </div>
            </div>
            <%if (!Model.isVisualizar)
				{ %>
            <div class="block">
                <div class="coluna20">
                    <label for="Situacao_Nova">Nova situação*</label>
                    <%= Html.DropDownList("Situacao.Nova", Model.Situacoes, new { @class = "text ddlSituacaoNova" }) %>
                </div>

                <div class="coluna15 prepend1">
                    <label for="Situacao_DataNova">Data da nova situação*</label>
                    <%= Html.TextBox("Situacao.DataNova", DateTime.Now.Date.ToShortDateString(), new { @class = "disabled text txtDataSituacaoNova", @disabled = "disabled" })%>
                </div>

                <div class="coluna15 prepend1">
                    <label for="Situacao_DataNova">Autor*</label>
                    <%= Html.TextBox("Situacao.Autor", Model.Autor.Nome, new { @class = "disabled text txtDataSituacaoNova", @disabled = "disabled" })%>
                </div>
            </div>
            <% } %>
			<%if (Model.Solicitacao.SituacaoId != (int)eCARSolicitacaoSituacao.Pendente)
				{ %>
            <div class="block divCancelado hide">
                <div class="coluna20">
                    <label for="Situacao_Nova">Motivo*</label>
                    <%= Html.DropDownList("Situacao.Nova", Model.Motivos, new { @class = "text ddlMotivo" }) %>
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
                        <%= Html.TextBox("ArquivoCancelamento", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
                        <span class="spanInputFile <%= string.IsNullOrEmpty(Model.ArquivoAnexo.Nome) ? "" : "hide" %>">
                            <input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" /></span>
                        <input type="hidden" class="hdnArquivo" value="<%: Model.AutorizacaoJson%>" />
                    </div>

                    <div class="block ultima spanBotoes">
                        <button type="button" class="inlineBotao btnArq <%= string.IsNullOrEmpty(Model.ArquivoAnexo.Nome) ? "" : "hide" %>" title="Enviar anexo" onclick="CARSolicitacaoAlterarSituacao.enviarArquivo('<%= Url.Action("arquivo", "arquivo") %>');">Enviar</button>
                        <%if (!Model.isVisualizar)
							{%>
                        <button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.ArquivoAnexo.Nome) ? "hide" : "" %>" title="Limpar arquivo"><span>Limpar</span></button>
                        <%} %>
                    </div>
                </div>
            </div>
			<%} %>
            <div class="ultima divMotivo">
                <label for="AlterarSituacao_Motivo">Descrição do motivo*</label>
                <%= Html.TextArea("Situacao.Motivo", Model.Solicitacao.DescricaoMotivo, ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "media text txtSituacaoMotivo ", @maxlength = "300" }))%>
            </div>
        </fieldset>
		<%} %>
        <%if (Model.Solicitacao.CarCancelamento.Count() > 0)
			{
			Model.Solicitacao.CarCancelamento.ForEach(x =>
			{%>
				<%Html.RenderPartial("AlterarSituacaoPartial", x);%>
			<%});
		}%>

        <%if (!Model.isVisualizar)
			{ %>
        <div class="block box botoesSalvarCancelar">
            <div class="block">
                <button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
                <span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>

            </div>
        </div>
        <% } %>
    </div>
</asp:Content>
