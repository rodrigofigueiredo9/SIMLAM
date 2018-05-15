<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="divVisualizarDocumento">
	<input type="hidden" class="hdnDocumentoId" value="<%= Model.Documento.Id %>" />

	<div class="block box">
		<% if (Model.Documento.IsArquivado) { %>
		<div class="block">
			<div class="ultima">
				<label class="erro">ARQUIVADO</label>
			</div>
		</div>
		<% } %>

		<div class="block">
			<div class="coluna18">
				<label>Data de registro *</label>
				<%= Html.TextBox("Documento.DataCadastro.DataTexto", Model.Documento.DataCadastro.DataTexto, new { @class = "text maskData disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna22 prepend2">
				<label>Nº de registro *</label>
				<%= Html.TextBox("Documento.Numero", Model.Documento.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna40 prepend2">
				<label>Tipo *</label>
				<%= Html.DropDownList("Documento.Tipo.Id", Model.DocumentoTipos, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
		
		<div class="block">
			<div class="qtdFolhas coluna18 <%= Model.Tipo.PossuiInteressadoLivre  ? "" : "hide" %>">
				<label>Qtd. de Folhas</label>
				<%= Html.TextBox("Documento.Folhas", Model.Documento.Folhas, ViewModelHelper.SetaDisabled(true, new { @class = "text  maskNumInt txtQuantidadeFolhas", @maxlength = 2 }))%>
			</div>

			<div class="qtdDocumento coluna18 <%= Model.Tipo.PossuiQuantidadeDocumento || Model.Tipo.QuantidadeDocumentoObrigatorio ? "" : "hide" %>">
				<label>Qtd. de documento *</label>
				<%= Html.TextBox("Documento.Volume", Model.Documento.Volume, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="block ultima prepend2">
				<span class="floatLeft inputFileDiv coluna82 <%= Model.Tipo.PossuiAssunto ||  Model.Tipo.AssuntoObrigatorio ? "hide" : "" %>">
					<label for="ArquivoTexto">Arquivo complementar</label>
					<% if(Model.Documento.Arquivo.Id.GetValueOrDefault() > 0) { %>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Documento.Arquivo.Nome, 43), "Baixar", "Documento", new { @id = Model.Documento.Arquivo.Id }, new { @Style = "display: block", @title = Model.Documento.Arquivo.Nome })%>
					<% } else { %>
						<input type="text" value="*** Nenhum arquivo associado ***" class="text txtArquivoNome disabled" disabled="disabled" />
					<% } %>
				</span>
			</div>
		</div>

		<div class="block">
			<div class="nomeDocumento coluna86 <%= Model.Tipo.PossuiNome || Model.Tipo.NomeObrigatorio ? "" : "hide" %>">
				<label>Nome *</label>
				<%= Html.TextBox("Documento.Nome", Model.Documento.Nome, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
			  <div class="assunto coluna86 <%= Model.Tipo.PossuiAssunto || Model.Tipo.AssuntoObrigatorio ? "" : "hide" %>">
                <label class="lblAssunto">Assunto <%: Model.Asterisco(Model.Tipo.AssuntoObrigatorio) %></label>
                <%= Html.TextBox("Documento.Assunto", Model.Documento.Assunto, new { @class = "text txtAssunto disabled", @maxlength = 150 })%>
            </div>
            <div class="descricao coluna86 <%= Model.Tipo.PossuiDescricao || Model.Tipo.DescricaoObrigatoria ? "" : "hide" %>">
                <label class="lblDescricao">Descrição da Comunicação Interna <%: Model.Asterisco(Model.Tipo.DescricaoObrigatoria) %></label>
                <%= Html.TextArea("Documento.Descricao", Model.Documento.Descricao, new { @class = "text txtDescricao disabled" })%>
            </div>
		</div>
	</div>

	<!-- ------------------------------- -->
	<fieldset class="containerInteressadoLivre block box <%= Model.Tipo.PossuiInteressadoLivre  ? "" : "hide" %>">
		<legend>Interessado</legend>
			<div class="block">
				<!--div class="floatRight" style="border:0px;"-->
					<div class="coluna70">
						<label>Nome/Razão Social</label>
						<%= Html.TextBox("Documento.InteressadoLivre", Model.Documento.InteressadoLivre, new { @class = "text txtInteressadoLivre disabled", @maxlength = 100, @disabled = "disabled" })%>
					</div>
					<div class="coluna20 prepend2">
						<label>Telefone</label>
						<%= Html.TextBox("Documento.InteressadoLivreTelefone", Model.Documento.InteressadoLivreTelefone, new { @class = "text txtInteressadoLivreTelefone maskFone disabled", @maxlength = 13, @disabled = "disabled" })%>
					</div>
					
				<!--/div-->
			</div>
	</fieldset>
	
	<% if (Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() > 0)
	{ %>
	<fieldset class="block box">
		<legend><%= (Model.Documento.ProtocoloAssociado.IsProcesso)?"Processo":"Documento" %></legend>

		<div class="coluna23">			
			<label>Nº de registro *</label>

			<!-- Necessario para abrir o visualizar -->
			<label class="hide"><%= Html.RadioButton("ProtocoloAssociadoTipoVisualizar", 1, Model.Documento.ProtocoloAssociado.IsProcesso, new { @class = "radio rdbProtocoloAssociadoTipo" })%> Nº de registro de processo *</label>
			<label class="hide"><%= Html.RadioButton("ProtocoloAssociadoTipoVisualizar", 2, !Model.Documento.ProtocoloAssociado.IsProcesso, new { @class = "radio rdbProtocoloAssociadoTipo" })%> Nº de registro de documento *</label>

			<input type="hidden" class="hdnProtocoloAssociadoId" value="<%= Html.Encode(Model.Documento.ProtocoloAssociado.Id) %>" />
			<%= Html.TextBox("Documento.ProtocoloAssociado.Numero", Model.Documento.ProtocoloAssociado.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna30 prepend1">
			<button type="button" class="icone visualizar inlineBotao btnVisualizarProtocolo" title="Visualizar processo"></button>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.Documento.ChecagemPendencia.Id > 0) { %>
	<fieldset class="block box">
		<legend>Checagem de Pendência</legend>

		<div class="coluna23">
			<label>Número *</label>
			<%= Html.TextBox("Documento.ChecagemPendencia.Id", Model.Documento.ChecagemPendencia.Id, new { @class = "text disabled txtChecagemPendenciaId", @disabled = "disabled" })%>
		</div>

		<div class="coluna30 prepend1">
			<button type="button" class="icone visualizar inlineBotao btnVisualizarChecagemPendencia" title="Visualizar checagem"></button>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.Documento.ChecagemRoteiro.Id > 0) { %>
	<fieldset class="block box">
		<legend>Checagem de Itens de Roteiro</legend>

		<div class="coluna23">
			<label>Número *</label>
			<%= Html.TextBox("Documento.ChecagemRoteiro.Id", Model.Documento.ChecagemRoteiro.Id, new { @class = "text disabled txtCheckListId", @disabled = "disabled" })%>
		</div>

		<div class="coluna30 prepend1">
			<button type="button" class="icone visualizar inlineBotao btnVisualizarChecagem" title="Visualizar checagem"></button>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.RequerimentoVM.Id > 0) { %>
	<fieldset class="block box">
		<legend>Requerimento Padrão</legend>

		<div class="block">
			<div class="coluna23">
				<label>Número *</label>
				<%= Html.TextBox("Requerimento.Requerimento.Id", Model.RequerimentoVM.Numero, new { @class = "text disabled txtNumeroReq", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend1">
				<button class="icone pdf inlineBotao btnPdfRequerimento" title="PDF do requerimento"></button>
			</div>
		</div>
	</fieldset>

	<div class="divRequerimento">
		<% Html.RenderPartial("RequerimentoVisualizar", Model.RequerimentoVM); %>
	</div>
	<% } %>

    <% if(Model.Documento.Fiscalizacao.Id > 0) { %>
    <fieldset class="containerFiscalizacao block box">
		<legend>Fiscalização</legend>

		<div class="block">
			<div class="coluna15">
				<label>Número *</label>
				<%= Html.TextBox("Fiscalizacao.Id", (Model.Documento.Fiscalizacao.Id > 0) ? Model.Documento.Fiscalizacao.Id.ToString() : string.Empty, new { @class = "text disabled txtNumeroFiscalizacao", @disabled = "disabled" })%>
				<input type="hidden" class="hdnFiscalizacaoSituacao" value="<%= Html.Encode(Model.Documento.Fiscalizacao.SituacaoId) %>" />
			</div>

			<div class="coluna30 prepend1">
				<span class="spnVisualizarFiscalizacao <%= (Model.Documento.Fiscalizacao.Id > 0) ? "" : "hide" %>">
					<button type="button" title="Visualizar fiscalizacao" class="icone visualizar inlineBotao btnVisualizarFiscalizacao"></button>
				</span>
			</div>
		</div>
	</fieldset>
    <% } %>

	<% if(Model.RequerimentoVM.Id <= 0) { %>
	<fieldset class="containerInteressado block box <%= ((Model.Tipo.InteressadoObrigatorio || Model.Tipo.PossuiInteressado) && !Model.Tipo.RequerimentoObrigatorio && Model.RequerimentoVM.Id <= 0 && Model.Tipo.Id > 0) ? "" : "hide" %>">
		<legend>Interessado</legend>

		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.RequerimentoVM.Interessado.Id) %>" />
		<div class="coluna60">
			<label>Nome/Razão social *</label>
			<%= Html.TextBox("Documento.Interessado.NomeRazaoSocial", Model.RequerimentoVM.Interessado.NomeRazaoSocial, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Documento.Interessado.CPFCNPJ", Model.RequerimentoVM.Interessado.CPFCNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="prepend2">
			<button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button>
		</div>
	</fieldset>
	<% } %>

	  <fieldset class="destinatario block box <%= Model.Tipo.PossuiAssunto || Model.Tipo.AssuntoObrigatorio ? "" : "hide" %>">
        <legend>Destinatário</legend>
        <div class="block divDropDown">
            <div class="coluna48">
                <label for="Enviar_Destinatario_SetorId">Setor de destino *</label>
                <%= Html.DropDownList("Documento.DestinatarioSetor.Id", Model.SetoresDestinatario, new { @class = "text ddlSetoresDestinatario disabled" })%>
            </div>
            <div class="coluna45 prepend2 ddlFuncionario">
                <label for="Enviar_Destinatario_Id">Funcionário</label>
                <%= Html.DropDownList("Documento.Destinatario.Id", Model.DestinatarioFuncionarios, new { @class = "text ddlDestinatarios disabled", @disabled = "disabled" })%>
            </div>
        </div>
    </fieldset>

    <div class="assinantes <%= Model.Tipo.PossuiAssunto || Model.Tipo.AssuntoObrigatorio ? "" : "hide" %>">
        <% Html.RenderPartial("Assinantes", Model.AssinantesVM); %>
    </div>
</div>