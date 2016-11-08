<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="divModalProcesso">
	<input type="hidden" class="hdnProcessoId" value="<%= Model.Processo.Id %>" />
	<input type="hidden" class="hdnPossuiSEP" value="<%= Model.PodeAutuar.ToString() %>" />

	<div class="block box">
		<% if (Model.Processo.IsArquivado) { %>
		<div class="block">
			<div class="ultima">
				<label class="erro">ARQUIVADO</label>
			</div>
		</div>
		<% } %>

		<div class="block">
			<div class="coluna86 <%= (Model.Processo.Id>0) ? "hide" : "" %>">
				<label>Setor de cadastro *</label>
				<%= Html.DropDownList("Processo.SetorId", Model.Setores, new { @class = "text ddlSetor" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label>Data de registro *</label>
				<%= Html.TextBox("Processo_DataCadastro_DataTexto", Model.Processo.DataCadastro.DataTexto, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text maskData txtDataCriacao" }))%>
			</div>

			<div class="coluna22 prepend2">
				<label>Nº de registro *</label>
				<%= Html.TextBox("Processo.Numero", string.IsNullOrEmpty(Model.Processo.Numero) ? "Gerado automaticamente" : Model.Processo.Numero, new { @class = "text txtNumero disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna40 prepend2">
				<label>Tipo *</label>
				<%= Html.DropDownList("Processo.Tipo.Id", Model.ProcessoTipos, ViewModelHelper.SetaDisabled(Model.IsEditar, new { @class = "text ddlProcessoTipos" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label>Qtd. de volumes *</label>
				<%= Html.TextBox("Processo.Volume", Model.Processo.Volume, new { @class = "text maskNumInt txtQuantidadeVolumes", @maxlength = 2 })%>
			</div>

			<div class="block ultima prepend2">
				<span class="floatLeft inputFileDiv coluna82">
					<label for="ArquivoTexto">Arquivo complementar</label>
					<% if(Model.Processo.Arquivo.Id.GetValueOrDefault() > 0) { %>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Processo.Arquivo.Nome, 45), "Baixar", "Processo", new { @id = Model.Processo.Arquivo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Processo.Arquivo.Nome })%>
					<% } %>
					<input type="text" value="<%= Model.Processo.Arquivo.Nome %>" class="text txtArquivoNome disabled hide" disabled="disabled" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Processo.Arquivo.Nome) ? "" : "hide" %>"><input type="file" class="inputFile" style="display: block; width: 100%" name="file" /></span>
					<input type="hidden" class="hdnArquivoJson" value="<%: Model.ObterJSon(Model.Processo.Arquivo) %>" />
				</span>

				<span class="spanBotoes prepend2">
					<button type="button" class="inlineBotao btnArqComplementar <%= string.IsNullOrEmpty(Model.Processo.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo"><span>Enviar</span></button>
					<button type="button" class="inlineBotao btnArqComplementarLimpar <%= string.IsNullOrEmpty(Model.Processo.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
				</span>
			</div>
		</div>

		<% if(Model.PodeAutuar) { %>
			<div class="block divPossuiSEP <%=(Model.IsEditar && !string.IsNullOrEmpty(Model.Processo.NumeroAutuacao)) ? "hide" : "" %>">
				<div class="coluna100">
					<label>Possui nº do SEP? *</label><br />
					<label><%= Html.RadioButton("Processo.PossuiSEP", 1, new { @class = "radio rdbProcessoPossuiSEP"})%>Sim</label>
					<label><%= Html.RadioButton("Processo.PossuiSEP", 2, (Model.IsEditar && string.IsNullOrEmpty(Model.Processo.NumeroAutuacao)), new { @class = "radio rdbProcessoPossuiSEP" })%>Não</label>
				</div>
			</div>

			<div class="divAutuacao <%=(Model.IsEditar && !string.IsNullOrEmpty(Model.Processo.NumeroAutuacao)) ? "" : "hide" %>">
				<%bool autuar = !(Model.IsEditar && Model.PodeAutuar);%>
				<div class="block">
					<div class="coluna18">
						<label>Nº de autuação (SEP) *</label>
						<%= Html.TextBox("Processo.NumeroAutuacao", Model.Processo.NumeroAutuacao, ViewModelHelper.SetaDisabled(autuar, new { @class = "text txtNumeroAutuacao", @maxlength = "15" }))%>
					</div>

					<div class="coluna22 prepend2">
						<label>Data de autuação (SEP) *</label>
						<%= Html.TextBox("Processo.DataAutuacao", Model.Processo.DataAutuacao.DataTexto, ViewModelHelper.SetaDisabled(autuar, new { @class = "text maskData txtDataAutuacao" }))%>
					</div>

					<% if (Model.PodeAutuar && string.IsNullOrEmpty(Model.Processo.NumeroAutuacao)){ %>
					<div class="coluna10 prepend2">
						<button type="button" class="floatLeft inlineBotao btnProcessoAutuar">Autuar</button>
					</div>
				<% } %>
				</div>
			</div>
		<% } %>
	</div>

	<fieldset class="containerFiscalizacao block box <%= (Model.Tipo.PossuiFiscalizacao || Model.Tipo.FiscalizacaoObrigatorio) ? "" : "hide" %>">
		<legend>Fiscalização</legend>

		<div class="block">
			<div class="coluna15">
				<label>Número *</label>
				<%= Html.TextBox("Fiscalizacao.Id", (Model.Processo.Fiscalizacao.Id > 0) ? Model.Processo.Fiscalizacao.Id.ToString() : string.Empty, new { @class = "text disabled txtNumeroFiscalizacao", @disabled = "disabled" })%>
				<input type="hidden" class="hdnFiscalizacaoSituacao" value="<%= Html.Encode(Model.Processo.Fiscalizacao.SituacaoId) %>" />
			</div>

			<div class="coluna30 prepend1">
				<button type="button" title="Buscar fiscalizacao" class="floatLeft inlineBotao botaoBuscar btnBuscarFiscalizacao <%= (Model.Processo.Fiscalizacao.Id > 0) ? "hide" : "" %>">Buscar</button>
				<button type="button" title="Limpar fiscalizacao" class="floatLeft inlineBotao btnLimparFiscalizacao <%= (Model.Processo.Fiscalizacao.Id > 0) ? "" : "hide" %>"><span>Limpar</span></button>

				<span class="spnVisualizarFiscalizacao <%= (Model.Processo.Fiscalizacao.Id > 0) ? "" : "hide" %>">
					<button type="button" title="Visualizar fiscalizacao" class="icone visualizar inlineBotao btnVisualizarFiscalizacao"></button>
				</span>
			</div>
		</div>
	</fieldset>

	<fieldset class="containerChecagemRoteiro block box <%= (Model.Tipo.PossuiChecagemRoteiro || Model.Tipo.ChecagemRoteiroObrigatorio)? "" : "hide" %>">
		<legend>Checagem de Itens de Roteiro</legend>

		<div class="block">
			<div class="coluna15">
				<label>Número</label>
				<%= Html.TextBox("ChecagemRoteiro.Id", (Model.Processo.ChecagemRoteiro.Id > 0) ? Model.Processo.ChecagemRoteiro.Id.ToString() : string.Empty, new { @class = "text disabled txtCheckListId", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend1">
				<button type="button" title="Buscar checagem" class="floatLeft inlineBotao botaoBuscar btnBuscarCheckList <%= (Model.Processo.ChecagemRoteiro.Id > 0) ? "hide" : "" %>">Buscar</button>
				<button type="button" title="Limpar checagem" class="floatLeft inlineBotao btnLimparCheckList <%= (Model.Processo.ChecagemRoteiro.Id > 0) ? "" : "hide" %>"><span>Limpar</span></button>
				<span class="spnVisualizarChecagem <%= (Model.Processo.ChecagemRoteiro.Id > 0) ? "" : "hide" %>">
					<button type="button" title="Visualizar checagem" class="icone visualizar inlineBotao btnVisualizarChecagem"></button>
				</span>
			</div>
		</div>
	</fieldset>

	<fieldset class="containerRequerimento block box <%= (Model.Tipo.PossuiRequerimento || Model.Tipo.RequerimentoObrigatorio) ? "" : "hide" %>">
		<legend>Requerimento Padrão</legend>

		<div class="block">
			<div class="coluna15">
				<label>Número</label>
				<%= Html.TextBox("Requerimento.Id", (Model.RequerimentoVM.Numero > 0) ? Model.RequerimentoVM.Numero.ToString() : string.Empty, new { @class = "text disabled txtNumeroReq", @disabled = "disabled" })%>
				<input type="hidden" class="hdnRequerimentoSituacao" value="<%= Html.Encode(Model.RequerimentoVM.SituacaoId) %>" />
			</div>

			<div class="coluna30 prepend1">
				<button type="button" title="Buscar requerimento" class="floatLeft inlineBotao botaoBuscar btnBuscarRequerimento <%= (Model.RequerimentoVM.Id > 0) ? "hide" : "" %>">Buscar</button>
				<button type="button" title="Limpar requerimento" class="floatLeft inlineBotao btnLimparRequerimento <%= (Model.RequerimentoVM.Id > 0) ? "" : "hide" %>"><span>Limpar</span></button>
				<span class="spnPdfRequerimento <%= (Model.RequerimentoVM.Id > 0) ? "" : "hide" %>"><button type="button" title="PDF do requerimento" class="icone pdf inlineBotao btnPdfRequerimento">Gerar PDF</button></span>
			</div>
		</div>
	</fieldset>

	<div class="containerRequerimento divRequerimento <%= (Model.Tipo.PossuiRequerimento || Model.Tipo.RequerimentoObrigatorio) ? "" : "hide" %>">
		<% if (Model.IsEditar && Model.RequerimentoVM.Id > 0) { %>
			<% Html.RenderPartial("Requerimento", Model.RequerimentoVM); %>
		<% } %>
	</div>

	<fieldset class="containerInteressado block box <%= ((!Model.Tipo.RequerimentoObrigatorio && Model.RequerimentoVM.Id <= 0 && Model.Tipo.Id > 0) || Model.Processo.Fiscalizacao.Id > 0) ? "" : "hide" %>">
		<legend class="labelInteressado"><%: Model.Tipo.LabelInteressado %></legend>
		<% Pessoa interessado = (Model.RequerimentoVM.Id > 0 && Model.Processo.Fiscalizacao.Id == 0) ? new Pessoa() : Model.RequerimentoVM.Interessado;%>
		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(interessado.Id) %>" />
		<div class="coluna60">
			<label>Nome/Razão social *</label>
			<%= Html.TextBox("Interessado.NomeRazaoSocial", interessado.NomeRazaoSocial, new { @class = "text disabled txtIntNome", @disabled = "disabled" })%>
		</div>

		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Interessado.CPFCNPJ", interessado.CPFCNPJ, new { @class = "text disabled txtIntCnpj", @disabled = "disabled" })%>
		</div>

		<div class="prepend2">
			<button type="button" title="Buscar interessado" class="floatLeft inlineBotao botaoBuscar btnAssociarInteressado <%= (!(Model.Tipo.PossuiFiscalizacao || Model.Tipo.FiscalizacaoObrigatorio)) ? "" : "hide" %>">Buscar</button>
			<span class="spanVisualizarInteressado <%= (interessado.Id > 0) ? "" : "hide" %>"><button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button></span>
		</div>
	</fieldset>
</div>