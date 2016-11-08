<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/listar.js") %>" ></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/Pessoa.js") %>" ></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/habilitarEmissaoCFOCFOC.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Credenciado/renovarDataHabilitacaoCFO.js") %>"></script>

<% if (Model.IsAjaxRequest) { %>
<script type="text/javascript">
	HabilitarEmissaoCFOCFOC.load($('.modalSalvarHabilitarEmissaoCFOCFOC'), 
	{
		urls: 
		{
			salvar: '<%= Url.Action("SalvarHabilitarEmissao", "Credenciado") %>',
			visualizarResponsavel: '<%= Url.Action("Visualizar", "Credenciado") %>',
			pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
			verificar: '<% = Url.Action("CriarVerificarCpfCnpj", "Pessoa") %>',
			visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
			criar: '<% = Url.Action("Criar", "Pessoa") %>',
			obter: '<% = Url.Action("Obter", "Credenciado") %>',
			renovarDatasPragas: '<% = Url.Action("RenovarPraga", "Credenciado") %>',
			validarPraga: '<% = Url.Action("ValidarAdicionarPraga", "Credenciado") %>',
			associarPragas: '<%= Url.Action("AssociarPraga", "ConfiguracaoVegetal") %>'
		},
		Mensagens:<%= Model.Mensagens %>
	});
</script>
<% } %>

<div class="modalSalvarHabilitarEmissaoCFOCFOC">	
	<%= Html.Hidden("HabilitarEmissao.Id", Model.HabilitarEmissao.Id, new { @class = "hdnHabilitarId" })%>
	<%= Html.Hidden("HabilitarEmissao.Tid", Model.HabilitarEmissao.Tid, new { @class = "hdnHabilitarTid" })%>
	<%= Html.Hidden("HabilitarEmissao.IsEditar", Model.IsEditar, new {@class = "hdnIsEditar"})%>
	<%= Html.Hidden("EstadoDefault", Model.HabilitarEmissao.UF, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.HabilitarEmissao.UFTexto, new { @class = "hdnEstadoDefaultSigla" })%>
	<%= Html.Hidden("hdnResponsavelId", Model.HabilitarEmissao.Responsavel.Id, new { @class = "hdnResponsavelId" })%>

	<% if (Model.HabilitarEmissao.Id > 0) { %>
		<h1 class="titTela">Editar Habilitação para Emissão de CFO e CFOC</h1>
	<% } else { %>
		<h1 class="titTela">Habilitação para Emissão de CFO e CFOC</h1>
	<% } %>
	<br />

	<fieldset class="divResponsavel block box">
		<legend>Responsável Técnico</legend>

		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna30">
				<div class="CpfPessoaContainer">
					<label for="HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ">CPF *</label>
					<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ", Model.HabilitarEmissao.Responsavel.Pessoa.CPFCNPJ, ViewModelHelper.SetaDisabled(Model.HabilitarEmissao.Responsavel.Id > 0, new { @class = "text maskCpf txtResponsavelCpf setarFoco" }))%>
				</div>
			</div>
			<div class="block ultima">
				<button type="button" class="inlineBotao esquerda btnAssociarResponsavel <%= (Model.IsEditar) ? "hide" : "" %>">Verificar</button>
				<a title="Visualizar responsável" class="icone visualizar inlineBotao esquerda btnVisualizarResponsavel <%= (Model.HabilitarEmissao.Responsavel.Id > 0 ? "" : "hide") %>">Visualizar Responsável</a>
			</div>
		</div>

		<!-- Linha 2 -->
		<div class="block">
			<div class="coluna80">
				<label for="HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial">Nome</label>
				<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial", Model.HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial, ViewModelHelper.SetaDisabled(true, new { @class = "txtResponsavelNome text" }))%>
			</div>
		</div>

		<!-- Linha 3 -->
		<div class="block">
			<div class="coluna80 inputFileDiv">
				<label for="ArquivoTexto">Arquivo foto</label>
				<% if(Model.HabilitarEmissao.Arquivo.Id.GetValueOrDefault() > 0) { %>
					<%= Html.ActionLink(Tecnomapas.EtramiteX.Interno.ViewModels.ViewModelHelper.StringFit(Model.HabilitarEmissao.Arquivo.Nome, 45), "Baixar", "Arquivo", new { @id = Model.HabilitarEmissao.Arquivo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.HabilitarEmissao.Arquivo.Nome })%>
				<% } %>
				<%= Html.TextBox("HabilitarEmissao.Arquivo.Nome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
				<span class="spanInputFile <%= string.IsNullOrEmpty(Model.HabilitarEmissao.Arquivo.Nome) ? "" : "hide" %>">
				<input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" /></span>
				<input type="hidden" class="hdnArquivo hdnAnexoArquivoJson" name="hdnAnexoArquivoJson" value="<%: Model.ObterJSon(Model.HabilitarEmissao.Arquivo) %>" />
			</div>

			<div class="block ultima spanBotoes">
				<button type="button" class="inlineBotao btnArq <%= string.IsNullOrEmpty(Model.HabilitarEmissao.Arquivo.Nome) ? "" : "hide" %>" title="Enviar anexo" onclick="HabilitarEmissaoCFOCFOC.onEnviarAnexoArquivoClick('<%= Url.Action("arquivo", "arquivo") %>');">Enviar</button>
				<button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.HabilitarEmissao.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
			</div>
		</div>
	</fieldset>

	<fieldset class="divregistro block box">
		<legend>Registro e Habilitações</legend>
		
		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Nº da habilitação *</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroHabilitacao", Model.HabilitarEmissao.NumeroHabilitacao,  ViewModelHelper.SetaDisabled(true, new { @maxlength = "8", @class = "txtNumeroHabilitacao text maskNumInt" }))%>
			</div>

			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao.ValidadeRegistro">Validade da taxa de registro *</label>
				<%= Html.TextBox("HabilitarEmissao.ValidadeRegistro", Model.HabilitarEmissao.ValidadeRegistro, ViewModelHelper.SetaDisabled(true, new { @maxlength = "10", @class = "txtValidadeRegistro text maskData" }))%>
			</div>

			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao.SituacaoTexto">Situação *</label>
				<%= Html.TextBox("HabilitarEmissao.SituacaoTexto", Model.HabilitarEmissao.SituacaoTexto,ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
			</div>
		</div>

		<!-- Linha 2 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Número do DUA *</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroDua", Model.HabilitarEmissao.NumeroDua, ViewModelHelper.SetaDisabled(true, new { @maxlength = "30", @class = "txtNumeroDua text" }))%>
			</div>

			<div class="block ultima hide">
				<button type="button" class="inlineBotao btnVerificarNumeroDua">Verificar</button>
			</div>
		</div>

		<!-- Linha 3 -->
		<div class="block">
			<div class="coluna30">
				<p><label for="HabilitarEmissao.ExtensaoHabilitacao">Extensão de habilitação?</label></p>
				<label class=""><%= Html.RadioButton("HabilitarEmissao.ExtensaoHabilitacao", "Sim", (Model.HabilitarEmissao.ExtensaoHabilitacao == 1), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbExtensaoHabilitacao" }))%> Sim</label>
				<label class=""><%= Html.RadioButton("HabilitarEmissao.ExtensaoHabilitacao", "Nao", (Model.HabilitarEmissao.ExtensaoHabilitacao == 0), ViewModelHelper.SetaDisabled(true, new { @class = "radio rdbExtensaoHabilitacao" }))%> Não</label>
			</div>

			<div class="coluna30 prepend2 divNumeroHabilitacaoOrigem <%= (Model.HabilitarEmissao.ExtensaoHabilitacao == 1) ? "" : "hide" %>">
				<label for="HabilitarEmissao.NumeroHabilitacaoOrigem">Número da habilitação de origem *</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroHabilitacaoOrigem", Model.HabilitarEmissao.NumeroHabilitacaoOrigem, ViewModelHelper.SetaDisabled(true, new { @maxlength = "8", @class = "txtHabOrigem text maskNumInt" }))%>
			</div>
		</div>

		<!-- Linha 4 -->
		<div class="block">
			<div class="coluna30">
				<label for="HabilitarEmissao.NumeroHabilitacao">Registro no CREA</label>
				<%= Html.TextBox("HabilitarEmissao.RegistroCrea",  Model.HabilitarEmissao.RegistroCrea, ViewModelHelper.SetaDisabled(true, new { @maxlength = "30", @class = "txtRegistroCrea text" }))%>
			</div>

			<div class="coluna30 prepend2">
				<label for="Model.HabilitarEmissao.UF">UF*</label>
				<%= Html.DropDownList("HabilitarEmissao.UF", Model.Estados, ViewModelHelper.SetaDisabled(true, new { @class = "ddlUf text" }))%> 
			</div>

			<div class="coluna30 prepend2 divRegistroCrea <%= (Model.HabilitarEmissao.UF != 8) ? "" : "hide" %> ">
				<label for="HabilitarEmissao.RegistroCrea">Nº visto CREA/ES *</label>
				<%= Html.TextBox("HabilitarEmissao.NumeroVistoCrea", Model.HabilitarEmissao.NumeroVistoCrea, ViewModelHelper.SetaDisabled(true, new { @maxlength = "30", @class = "txtNumeroVistoCrea text" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="divregistro block box">
		<legend>Praga</legend>

		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna80">
				<div>
					<label for="Model.HabilitarEmissao.Pragas.Nome">Praga *</label>
					<%= Html.TextBox("HabilitarEmissao.Pragas.Nome", null,   ViewModelHelper.SetaDisabled(true, new { @class = "text txtNomePraga" }))%>
				</div>
			</div>
			<div class="block ultima">
				<button type="button" class="inlineBotao botaoLocalizarPraga hide">Buscar</button>
			</div>
		</div>

		<!-- Linha 2 -->
		<div class="block">
			<div class="coluna30">
				<label for="Model.HabilitarEmissao.Pragas.DataInicial">Data inicial da habilitação *</label>
				<%= Html.TextBox("HabilitarEmissao.Pragas.DataInicialHabilitacao", null, ViewModelHelper.SetaDisabled(true, new { @maxlength = "10", @class = "txtDataInicialHabilitacao text maskData" }))%> 
			</div>

			<div class="coluna30 prepend2">
				<label for="Model.HabilitarEmissao.Pragas.DataFinal">Data final da habilitação *</label>
				<%= Html.TextBox("HabilitarEmissao.Pragas.DataFinalHabilitacao", null, ViewModelHelper.SetaDisabled(true, new { @maxlength = "10", @class = "txtDataFinalHabilitacao text maskData" }))%>
			</div>

			<div class="block ultima">
				<button type="button" class="inlineBotao botaoAdicionarPraga hide">Adicionar</button>
			</div>
		</div>

		<!-- Linha 3 -->
		<div class="block">
			<div class="gridContainer">
				<table class="dataGridTable gridPraga" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th width="20%">Nome científico</th>
							<th width="20%">Nome comum</th>
							<th width="20%">Cultura</th>
							<th width="15%">Data inicial</th>
							<th width="15%">
								<label class="lblDataFinal" title="Renovar data">Data final</label>
	 							<% if(Model.HabilitarEmissao.Id > 0) { %> <span class="floatRight"><input type="button" name="alterar situação" title="Renovar data" class="icone refresh btnRenovarDatas <%= (Model.HabilitarEmissao.Pragas.Count>0) ? "" : "hide" %>" /></span><% } %>
							</th>
							<th class="semOrdenacao" width="10%">Ações</th>
						</tr>
					</thead>
					<tbody>
						<%
							Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado.PragaHabilitarEmissao item = null;
							for (int i = 0; i < Model.HabilitarEmissao.Pragas.Count; i++) {
								item = Model.HabilitarEmissao.Pragas[i];
						%>
						<tr>
							<td>
								<label class="lblNomeCientifico" title="<%=item.Praga.NomeCientifico%>"><%=item.Praga.NomeCientifico%> </label>
							</td>
							<td>
								<label class="lblNomeComun" title="<%=item.Praga.NomeComum%>"><%=item.Praga.NomeComum%> </label>
							</td>
							<td>
								<label class="lblCultura" title="<%=item.Cultura%>"><%=item.Cultura%> </label>
							</td>
							<td>
								<label class="lblDataInicialHabilitacao" title="<%=item.DataInicialHabilitacao%>"><%=item.DataInicialHabilitacao%> </label>
							</td>
							<td>
								<label class="lblDataFinalHabilitacao" title="<%=item.DataFinalHabilitacao%>"><%=item.DataFinalHabilitacao%> </label>
							</td>
							<td>
								<a class="icone excluir btnExcluirPraga" title="Excluir"></a>
								<input type="hidden" value="<%= i %>" class="hdnItemIndex" />
								<input type="hidden" value="<%=item.Id  %>" class="hdnItemId" />
								<input type="hidden" value=<%=item.Praga.Id%>  class="hdnPragaId" />
							</td>
						</tr>
						<% } %>

						<!-- Template -->
						<tr class="hide tr_template">
							<td>
								<label class="lblNomeCientifico"></label>
							</td>
							<td>
								<label class="lblNomeComun"></label>
							</td>
							<td>
								<label class="lblCultura"></label>
							</td>
							<td>
								<label class="lblDataInicialHabilitacao"></label>
							</td>
							<td>
								<label class="lblDataFinalHabilitacao"></label>
							</td>
							<td>
								<a class="icone excluir btnExcluirPraga" title="Excluir"></a>
								<input type="hidden" value="" class="hdnItemIndex" />
								<input type="hidden" value="" class="hdnItemId" />
								<input type="hidden" value="" class="hdnPragaId" />
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>