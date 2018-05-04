<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="divModalDocumento">
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
			<div class="coluna86 <%= Model.MostrarSetor ? "" : "hide" %>">
				<label>Setor de cadastro *</label>
				<%= Html.DropDownList("Documento.SetorId", Model.Setores, new { @class = "text ddlSetor" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label>Data de registro *</label>
				<%= Html.TextBox("Documento.DataCadastro.DataTexto", Model.Documento.DataCadastro.DataTexto, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text maskData txtDataCriacao" }))%>
			</div>

			<div class="coluna22 prepend2">
				<label>Nº de registro *</label>
				<%= Html.TextBox("Documento.Numero", string.IsNullOrEmpty(Model.Documento.Numero) ? "Gerado automaticamente" : Model.Documento.Numero, new { @class = "text txtNumero disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna40 prepend2">
				<label>Tipo *</label>
				<%= Html.DropDownList("Documento.Tipo.Id", Model.DocumentoTipos, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text ddlDocumentoTipos" }))%>
			</div>

			
		</div>

		<div class="block">
			<div class="qtdFolhas coluna15  <%= Model.Tipo.PossuiInteressadoLivre  ? "" : "hide" %>">
				<label>Qtd. de Folhas</label>
				<%= Html.TextBox("Documento.Folhas", Model.Documento.Folhas, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text  maskNumInt txtQuantidadeFolhas", @maxlength = 2 }))%>
			</div>

			<div class="qtdDocumento coluna18  <%= Model.Tipo.PossuiInteressadoLivre  ? "hide" : "" %>">
				<label>Qtd. de documento *</label>
				<%= Html.TextBox("Documento.Volume", Model.Documento.Volume, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text maskNumInt txtQuantidadeDocumento", @maxlength = 2 }))%>
			</div>

			<div class="block ultima prepend2">
				<span class="floatLeft inputFileDiv coluna82">
					<label for="ArquivoTexto">Arquivo complementar</label>
					<% if(Model.Documento.Arquivo.Id.GetValueOrDefault() > 0) { %>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Documento.Arquivo.Nome, 45), "Baixar", "Documento", new { @id = Model.Documento.Arquivo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Documento.Arquivo.Nome })%>
					<% } %>
					<input type="text" value="<%= Model.Documento.Arquivo.Nome %>" class="text txtArquivoNome disabled hide" disabled="disabled" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Documento.Arquivo.Nome) ? "" : "hide" %>"><input type="file" class="inputFile" style="display: block; width: 100%" name="file" /></span>
					<input type="hidden" class="hdnArquivoJson" value="<%: Model.ObterJSon(Model.Documento.Arquivo) %>" />
				</span>

				<span class="spanBotoes prepend2">
					<button type="button" class="inlineBotao btnArqComplementar <%= string.IsNullOrEmpty(Model.Documento.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo"><span>Enviar</span></button>
					<button type="button" class="inlineBotao btnArqComplementarLimpar <%= string.IsNullOrEmpty(Model.Documento.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
				</span>
			</div>
		</div>

		<div class="block">
			<div class="coluna86">
				<label>Nome do documento*</label>
				<%= Html.TextBox("Documento.Nome", Model.Documento.Nome, new { @class = "text txtNomeDocumento", @maxlength = 100 })%>
			</div>
		</div>
	</div>

	<fieldset class="containerProcessoDocumento block box <%= Model.ExibirGrupoProtocolo ? "" : "hide" %>">
		<legend class="grupoProtocoloAssociadoNome">Processo/Documento</legend>

		<div class="coluna60">
			<label><%= Html.RadioButton("ProtocoloAssociadoTipo", 1, (Model.RadioProcessoChecked), new { @class = "radio rdbProtocoloAssociadoTipo visivelSeAssociaDocumento " + (Model.ExibirRadioProcDoc ? "" : "hide") })%> Nº de registro de <label class="lblProcesso">processo<%: Model.Asterisco(Model.Tipo.ProcessoObrigatorio) %></label></label>
			<label class="visivelSeAssociaDocumento <%= (Model.ExibirRadioProcDoc) ? "" : "hide" %>"><%= Html.RadioButton("ProtocoloAssociadoTipo", 2, Model.RadioDocumentoChecked, new { @class = "radio rdbProtocoloAssociadoTipo" })%> Nº de registro de <label class="lblDocumento">documento<%: Model.Asterisco(Model.Tipo.DocumentoObrigatorio) %></label></label>

			<input type="hidden" class="hdnProtocoloAssociadoId" value="<%= Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() %>" />
			<%= Html.TextBox("Protocolo_Id", Model.Documento.ProtocoloAssociado.Numero, new { @class = "text disabled txtProtocoloAssociadoNumero", @disabled = "disabled" })%>
		</div>

		<div class="coluna30 prepend1">
			<button type="button" class="floatLeft inlineBotao botaoBuscar btnBuscarProtocolo <%= (Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() <= 0) ? "" : "hide" %>" title="Buscar protocolo">Buscar</button>
            <button type="button" class="floatLeft inlineBotao botaoBuscar btnLimparProtocolo <%= (Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() > 0) ? "" : "hide" %>" title="Limpar protocolo">Limpar</button>

            <span class="spnVisualizarProtocolo <%= (Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() > 0) ? "" : "hide" %>">
				<button type="button" class="icone visualizar inlineBotao btnVisualizarProtocolo" title="Visualizar Protocolo"></button>
			</span>
		</div>
	</fieldset>

	<!-- ------------------------------- -->
	<fieldset class="containerInteressadoLivre block box  <%= Model.Tipo.PossuiInteressadoLivre  ? "" : "hide" %>">
		<legend>Interessado</legend>
			<div class="block">
				<!--div class="floatRight" style="border:0px;"-->
					<div class="coluna70">
						<label>Nome/Razão Social</label>
						<%= Html.TextBox("Documento.InteressadoLivre", Model.Documento.InteressadoLivre, new { @class = "text txtInteressadoLivre", @maxlength = 100 })%>
					</div>
					<div class="coluna20 prepend2">
						<label>Telefone</label>
						<%= Html.TextBox("Documento.InteressadoLivreTelefone", Model.Documento.InteressadoLivreTelefone, new { @class = "text txtInteressadoLivreTelefone maskFone", @maxlength = 13 })%>
					</div>
					
				<!--/div-->
			</div>
	</fieldset>

	<fieldset class="containerChecagemPendencia block box <%= (Model.Tipo.PossuiChecagemPendencia || Model.Tipo.ChecagemPendenciaObrigatorio) ? "" : "hide" %>">
		<legend>Checagem de Pendência</legend>

		<div class="coluna23">
			<label class="lblChecagemPendenciaNum">Número<%: Model.Asterisco(Model.Tipo.ChecagemPendenciaObrigatorio) %></label>
			<%= Html.TextBox("ChecagemPendencia.Id", (Model.Documento.ChecagemPendencia.Id > 0) ? Model.Documento.ChecagemPendencia.Id.ToString() : string.Empty, new { @class = "text disabled txtChecagemPendenciaId", @disabled = "disabled" })%>
		</div>
		<div class="coluna30 prepend1">
			<% if (!Model.IsEditar) { %><button type="button" title="Buscar checagem" class="floatLeft inlineBotao botaoBuscar btnBuscarChecagemPendencia">Buscar</button><% } %>
			<span class="spnVisualizarChecagemPendencia <%= (Model.Documento.ChecagemPendencia.Id > 0) ? "" : "hide" %>">
				<button type="button" class="icone visualizar inlineBotao btnVisualizarChecagemPendencia" title="Visualizar checagem"></button>
			</span>
		</div>
	</fieldset>

	<fieldset class="containerChecagemRoteiro block box <%= (Model.Tipo.PossuiChecagemRoteiro || Model.Tipo.ChecagemRoteiroObrigatorio) ? "" : "hide" %>">
		<legend>Checagem de Itens de Roteiro</legend>

		<div class="coluna23">
			<label class="lblChecagemRoteiroNum">Número<%: Model.Asterisco(Model.Tipo.ChecagemRoteiroObrigatorio) %></label>
			<%= Html.TextBox("ChecagemRoteiro.Id", (Model.Documento.ChecagemRoteiro.Id > 0) ? Model.Documento.ChecagemRoteiro.Id.ToString() : string.Empty, new { @class = "text disabled txtCheckListId", @disabled = "disabled" })%>
		</div>
		<div class="coluna30 prepend1">
			<button type="button" title="Buscar checagem" class="floatLeft inlineBotao botaoBuscar btnBuscarCheckList">Buscar</button>
			<span class="spnVisualizarChecagem <%= (Model.Documento.ChecagemRoteiro.Id > 0) ? "" : "hide" %>">
				<button type="button" class="icone visualizar inlineBotao btnVisualizarChecagem" title="Visualizar checagem"></button>
			</span>
		</div>
	</fieldset>

	<fieldset class="containerRequerimento block box <%= (Model.Tipo.PossuiRequerimento || Model.Tipo.RequerimentoObrigatorio) ? "" : "hide" %>">
		<legend>Requerimento Padrão</legend>

		<div class="block">
			<div class="coluna23">
				<label class="lblRequerimentoNum">Número<%: Model.Asterisco(Model.Tipo.RequerimentoObrigatorio) %></label>
				<%= Html.TextBox("Requerimento.Id", (Model.RequerimentoVM.Numero > 0) ? Model.RequerimentoVM.Numero.ToString() : string.Empty, new { @class = "text disabled txtNumeroReq", @disabled = "disabled" })%>
				<input type="hidden" class="hdnRequerimentoSituacao" value="<%= Html.Encode(Model.RequerimentoVM.SituacaoId) %>" />
			</div>

			<div class="coluna30 prepend1">
				<button type="button" title="Buscar requerimento" class="floatLeft inlineBotao botaoBuscar btnBuscarRequerimento">Buscar</button>
				<span class="spnPdfRequerimento <%= (Model.RequerimentoVM.Id > 0) ? "" : "hide" %>">
					<button class="icone pdf inlineBotao btnPdfRequerimento" title="PDF do requerimento"></button>
				</span>
			</div>
		</div>
	</fieldset>

	<div class="containerRequerimento divRequerimento <%= (Model.Tipo.PossuiRequerimento || Model.Tipo.RequerimentoObrigatorio) ? "" : "hide" %>">
		<% if (Model.IsEditar && Model.RequerimentoVM.Id > 0) { %>
			<% Html.RenderPartial("Requerimento", Model.RequerimentoVM); %>
		<% } %>
	</div>

    <fieldset class="containerFiscalizacao block box <%= (Model.Tipo.PossuiFiscalizacao || Model.Tipo.FiscalizacaoObrigatorio) ? "" : "hide" %>">
		<legend>Fiscalização</legend>

		<div class="block">
			<div class="coluna15">
				<label class="lblFiscalizacaoNum">Número<%: Model.Asterisco(Model.Tipo.FiscalizacaoObrigatorio) %></label>
				<%= Html.TextBox("Fiscalizacao.Id", (Model.Documento.Fiscalizacao.Id > 0) ? Model.Documento.Fiscalizacao.Id.ToString() : string.Empty, new { @class = "text disabled txtNumeroFiscalizacao", @disabled = "disabled" })%>
				<input type="hidden" class="hdnFiscalizacaoSituacao" value="<%= Html.Encode(Model.Documento.Fiscalizacao.SituacaoId) %>" />
			</div>

			<div class="coluna30 prepend1">
				<button type="button" title="Buscar fiscalizacao" class="floatLeft inlineBotao botaoBuscar btnBuscarFiscalizacao <%= (Model.Documento.Fiscalizacao.Id > 0) ? "hide" : "" %>">Buscar</button>
				<button type="button" title="Limpar fiscalizacao" class="floatLeft inlineBotao btnLimparFiscalizacao <%= (Model.Documento.Fiscalizacao.Id > 0) ? "" : "hide" %>"><span>Limpar</span></button>

				<span class="spnVisualizarFiscalizacao <%= (Model.Documento.Fiscalizacao.Id > 0) ? "" : "hide" %>">
					<button type="button" title="Visualizar fiscalizacao" class="icone visualizar inlineBotao btnVisualizarFiscalizacao"></button>
				</span>
			</div>
		</div>
	</fieldset>

	<fieldset class="containerInteressado block box <%= ((Model.Tipo.InteressadoObrigatorio || Model.Tipo.PossuiInteressado) && !Model.Tipo.RequerimentoObrigatorio && Model.RequerimentoVM.Id <= 0 && Model.Tipo.Id > 0) ? "" : "hide" %>">
		<legend class="labelInteressado" ><%: Model.Tipo.LabelInteressado %></legend>

		<% Pessoa interessado = (Model.RequerimentoVM.Id > 0) ? new Pessoa() : Model.RequerimentoVM.Interessado; %>
		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(interessado.Id) %>" />
		<div class="coluna60">
			<label class="lblInteressadoNomeRazao">Nome/Razão social <%: Model.Asterisco(Model.Tipo.InteressadoObrigatorio) %></label>
			<%= Html.TextBox("Interessado.NomeRazaoSocial", interessado.NomeRazaoSocial, new { @class = "text disabled txtIntNome", @disabled = "disabled" })%>
		</div>

		<div class="coluna17 prepend2">
			<label class="lblInteressadoCpfCnpj">CPF/CNPJ <%: Model.Asterisco(Model.Tipo.InteressadoObrigatorio) %></label>
			<%= Html.TextBox("Interessado.CPFCNPJ", interessado.CPFCNPJ, new { @class = "text disabled txtIntCnpj", @disabled = "disabled" })%>
		</div>

		<div class="prepend2">

			<div class="divBuscarInteressado <%= (Model.Documento.Interessado.Id <= 0) ? "" : "hide" %>">
				<button type="button" title="Buscar interessado" class="floatLeft inlineBotao botaoBuscar btnAssociarInteressado">Buscar</button>
			</div>

			<div class="divLimparInteressado <%= (Model.Documento.Interessado.Id > 0) ? "" : "hide" %>">
				<button type="button" class="floatLeft inlineBotao btnLimparInteressado" title="Limpar interessado">Limpar</button>
			</div>
			<span class="containerBtnEditarInteressado <%= (interessado.Id > 0) ? "" : "hide" %>">
				<button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button>
			</span>
		</div>
	</fieldset>
</div>