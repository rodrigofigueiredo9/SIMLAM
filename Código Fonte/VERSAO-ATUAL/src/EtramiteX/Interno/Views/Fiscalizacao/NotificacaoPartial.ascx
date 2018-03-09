<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NotificacaoVM>" %>

<div class="containerNotificacao">

    <%=Html.Hidden("Notificacao_FiscalizacaoId", Model.Notificacao.FiscalizacaoId, new { @class = "hdnFiscalizacaoId"}) %>
    <%=Html.Hidden("Notificacao_NotificacaoId", Model.Notificacao.Id, new { @class = "hdnNotificacaoId"}) %>

    <div class="box">
        <div class="block">
            <div class="coluna15">
                <label>N° Fiscalização</label><br />
                <%= Html.TextBox("Notificacao.FiscalizacaoId", Model.Notificacao.FiscalizacaoId, ViewModelHelper.SetaDisabled(true, new { @class = "text txtFiscalizacao setarFoco" }))%>
            </div>
            <div class="coluna15">
                <label>N° do IUF</label><br />
                <%= Html.TextBox("Notificacao.NumeroIUF", Model.Notificacao.NumeroIUF, ViewModelHelper.SetaDisabled(true, new { @class = "text txtNumeroIUF setarFoco" }))%>
            </div>
            <div class="coluna35">
                <label>Autuado</label><br />
                <%= Html.TextBox("Notificacao.AutuadoPessoa.NomeRazaoSocial", Model.Notificacao.AutuadoPessoa != null ? Model.Notificacao.AutuadoPessoa.NomeRazaoSocial : "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtAutuadoNome setarFoco" }))%>
            </div>
            <div class="coluna25">
                <label>CPF autuado</label><br />
                <%= Html.TextBox("Notificacao.AutuadoPessoa.CPFCNPJ", Model.Notificacao.AutuadoPessoa != null ? Model.Notificacao.AutuadoPessoa.CPFCNPJ : "", ViewModelHelper.SetaDisabled(true, new { @class = "text txtAutuadoCpfCnpj setarFoco" }))%>
            </div>
        </div>
    </div>

    <div class="box">
        <div class="coluna25">
            <label><b>Forma de notificação do IUF *</b></label><br />
            <span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnFormaIUF">
                <label><%= Html.RadioButton("Notificacao.FormaIUF", 1, (Model.Notificacao.FormaIUF == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaIUF" }))%>Pessoal (em mãos)</label><br />
                <label><%= Html.RadioButton("Notificacao.FormaIUF", 2, (Model.Notificacao.FormaIUF == 2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaIUF" }))%>AR - Aviso de Recebimento</label><br />
                <label><%= Html.RadioButton("Notificacao.FormaIUF", 3, (Model.Notificacao.FormaIUF == 3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaIUF" }))%>Por Edital</label><br />
            </span>
            <br />

            <label for="Notificacao_DataIUF.DataTexto">Data da Notificação</label><br />
            <%= Html.TextBox("Notificacao.DataIUF.DataTexto", Model.Notificacao.DataIUF.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataIUF setarFoco maskData" }))%>
        </div>

        <div class="coluna25">
            <label class="lblFormaJIAPI"><b>Forma de notificação do JIAPI</b></label><br />
            <span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnFormaJIAPI">
                <label class="lblFormaJIAPI"><%= Html.RadioButton("Notificacao.FormaJIAPI", 1, (Model.Notificacao.FormaJIAPI == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaJIAPI" }))%>Pessoal (em mãos)</label><br />
                <label class="lblFormaJIAPI"><%= Html.RadioButton("Notificacao.FormaJIAPI", 2, (Model.Notificacao.FormaJIAPI == 2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaJIAPI" }))%>AR - Aviso de Recebimento</label><br />
                <label class="lblFormaJIAPI"><%= Html.RadioButton("Notificacao.FormaJIAPI", 3, (Model.Notificacao.FormaJIAPI == 3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaJIAPI" }))%>Por Edital</label><br />
            </span>
            <br />

            <div class="jiapi" style="visibility: hidden;">
                <label for="Notificacao_DataJIAPI.DataTexto">Data da Notificação</label><br />
                <%= Html.TextBox("Notificacao.DataJIAPI.DataTexto", Model.Notificacao.DataJIAPI.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataJIAPI setarFoco maskData" }))%>
            </div>
        </div>

        <div class="block">
            <div class="coluna72">
                <label class="lblFormaCORE"><b>Forma de notificação do CORE</b></label><br />
                <span style="border-style: solid; border-width: 1px; padding: 0 3px 0 0; border-color: transparent;" class="text" id="SpnFormaCORE">
                    <label class="lblFormaCORE"><%= Html.RadioButton("Notificacao.FormaCORE", 1, (Model.Notificacao.FormaCORE == 1), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaCORE" }))%>Pessoal (em mãos)</label><br />
                    <label class="lblFormaCORE"><%= Html.RadioButton("Notificacao.FormaCORE", 2, (Model.Notificacao.FormaCORE == 2), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaCORE" }))%>AR - Aviso de Recebimento</label><br />
                    <label class="lblFormaCORE"><%= Html.RadioButton("Notificacao.FormaCORE", 3, (Model.Notificacao.FormaCORE == 3), ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "rdbFormaCORE" }))%>Por Edital</label><br />
                </span>
            </div>
            <div class="coluna52 append2 core" style="visibility: hidden;">
                <label for="Notificacao_DataCORE.DataTexto">
                    Data da Notificação
                </label>
                <br />
                <%= Html.TextBox("Notificacao.DataCORE.DataTexto", Model.Notificacao.DataCORE.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDataCORE setarFoco maskData" }))%>
            </div>
        </div>

        <fieldset class="block box fsArquivos">
            <legend style="color: #000000"><b>Cópia de Notificação</b></legend>
            <% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
        </fieldset>
    </div>

	<% if (Model.IsVisualizar){%>
		<fieldset class="block box">
			<legend><b>Cobrança</b></legend>
			<% Html.RenderPartial("NotificacaoCobranca", Model); %>
		</fieldset>
	<%} %>
</div>
