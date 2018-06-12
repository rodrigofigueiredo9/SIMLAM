<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConsultarNumeroCFOCFOCLiberadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Consultar Número de CFO e CFOC Liberados</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/LiberacaoCFOCFOC/consultarNumeroCFOCFOCLiberado.js") %>"></script>
	<script>
		$(function () {
			ConsultarNumeroCFOCFOCLiberado.load($('#central'),
			{
				urls:
				{
					visualizarPessoa: '<%=Url.Action("Visualizar", "Credenciado")%>',
					verificarCPF: '<%=Url.Action("VerificarCPFConsulta", "LiberacaoCFOCFOC")%>',
					buscar: '<%=Url.Action("FiltrarConsulta", "LiberacaoCFOCFOC")%>',
					cancelarModal: '<%=Url.Action("MotivoCancelamento", "LiberacaoCFOCFOC")%>',
				    cancelar: '<%=Url.Action("Cancelar", "LiberacaoCFOCFOC")%>',
				    urlVerificarConsultaDUA: '<%= Url.Action("VerificarConsultaDUA", "LiberacaoCFOCFOC") %>',

				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Consultar Número de CFO e CFOC Liberado</h1>
		<br />

		<fieldset class="block box">
			<legend>Responsável Técnico</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Pessoa_Fisica_Cpf">CPF *</label>
					<%= Html.TextBox("Pessoa.Fisica.Cpf", null, new { @class = "text setarFoco maskCpf txtCpf" })%>
					<%= Html.Hidden("Credenciado.Id", 0, new { @class = "hdnCredenciadoId" })%>
				</div>

				<div class="coluna20 prepend1">
					<button type="button" class="inlineBotao esquerda btnVerificarCpf">Verificar</button>
					<button type="button" class="inlineBotao esquerda icone visualizar btnVisualizarPessoa hide mostrar"></button>
				</div>
			</div>
			<div class="block mostrar hide">
				<div class="coluna70">
					<label for="Liberacao_Nome">Nome *</label>
					<%= Html.TextBox("Liberacao.Nome", null, new { @disabled = "disabled", @class = "text maskCpf txtNome disabled" })%>
					<%= Html.Hidden("Pessoa.Fisica.Id", 0, new { @class = "hdnPessoaId" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box mostrar hide">
			<legend>Nº Bloco</legend>
			<div class="block">
				<div class="coluna20">
					<label for="TipoDocumentoBloco">Tipo do documento</label>
					<%= Html.DropDownList("TipoDocumentoBloco", Model.LstTipoDocumento, new { @class = "text ddlTipoDocumento" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="NumeroBloco">Número</label>
					<%= Html.TextBox("NumeroBloco", null, new { @class = "text txtNumero maskNumInt", maxlength = "10" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="DataInicialBloco">Data inicial da emissão</label>
					<%= Html.TextBox("DataInicialBloco", null, new { @class = "text maskData txtDataInicialEmissao" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="DataFinalBloco">Data final da emissão</label>
					<%= Html.TextBox("DataFinalBloco", null, new { @class = "text maskData txtDataFinalEmissao" })%>
				</div>

				<div class="coluna10 prepend1">
					<button type="button" class="inlineBotao esquerda bloco btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="coluna80">
					<div class="block">
						<div class="paginacaoCaixa hide">
							<a class="1 paginar comeco" title="Primeira Página">Primeira página</a>
							<a class="1 paginar anterior" title="Página Anterior">Página anterior</a>
							<span class="numerosPag"></span>
							<a class="paginar proxima" title="Próxima Página">Próxima página</a>
							<a class="paginar final" title="Ultima Página">Ultima página</a>
							<input type="hidden" class="hdnPaginaAnterior" />
							<input type="hidden" class="hdnPaginaProxima" />
							<input type="hidden" class="hdnPaginaUltima" />
						</div>
					</div>
					<div class="coluna100">
						<table class="dataGridTable hide" border="0" cellspacing="0" cellpadding="0" width="100%">
							<thead>
								<tr>
									<th width="20%">Tipo do documento</th>
									<th width="20%">Número</th>
									<th width="20%">Utilizado</th>
									<th width="20%">Situação</th>
									<th>Ações</th>
								</tr>
							</thead>
							<tbody>
								<tr class="templateRow hide">
									<td class="tipoDocumento"></td>
									<td class="Numero"></td>
									<td class="Utilizado"></td>
									<td class="Situacao"></td>
									<td class="acoes">
										<a class="btnCancelar icone dispensado" title="Cancelar número"></a>
										<button class="btnVisualizarMotivo icone pendencias" title="Visualizar motivo do cancelamento"></button>
										<%=Html.Hidden("hiddenObjeto", 0, new {@class="hdnObjetoJson"}) %>
									</td>
								</tr>
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box mostrar hide bloco">
			<legend>Nº Digital</legend>
			<div class="block">
				<div class="coluna20">
					<label for="TipoDocumentoDigital">Tipo do documento</label>
					<%= Html.DropDownList("TipoDocumentoDigital", Model.LstTipoDocumento, new { @class = "text ddlTipoDocumento" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="NumeroDigital">Número</label>
					<%= Html.TextBox("NumeroDigital", null, new { @class = "text txtNumero", maxlength = "12" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="DataInicialBloco">Data inicial da emissão</label>
					<%= Html.TextBox("DataInicialDigital", null, new { @class = "text maskData txtDataInicialEmissao" })%>
				</div>

				<div class="coluna20 prepend1">
					<label for="DataFinalBloco">Data final da emissão</label>
					<%= Html.TextBox("DataFinalDigital", null, new { @class = "text maskData txtDataFinalEmissao" })%>
				</div>

				<div class="coluna10 prepend1">
					<button type="button" class="inlineBotao esquerda digital btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFO: </label>
					<label class="lblQtdCFO">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblCFOUtilizado">0</label>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFOC: </label>
					<label class="lblQtdCFOC">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblCFOCUtilizado">0</label>
				</div>
			</div>

			<div class="block">
				<div class="coluna80">
					<div class="block">
						<div class="paginacaoCaixa hide">
							<a class="1 paginar comeco" title="Primeira Página">Primeira página</a>
							<a class="1 paginar anterior" title="Página Anterior">Página anterior</a>
							<span class="numerosPag"></span>
							<a class="paginar proxima" title="Próxima Página">Próxima página</a>
							<a class="paginar final" title="Ultima Página">Ultima página</a>
							<input type="hidden" class="hdnPaginaAnterior" />
							<input type="hidden" class="hdnPaginaProxima" />
							<input type="hidden" class="hdnPaginaUltima" />
						</div>
					</div>
					<div class="coluna100">
						<table class="dataGridTable  hide" border="0" cellspacing="0" cellpadding="0" width="100%">
							<thead>
								<tr>
									<th width="20%">Tipo do documento</th>
									<th width="20%">Número</th>
									<th width="20%">Utilizado</th>
									<th width="20%">Situação</th>
									<th>Ações</th>
								</tr>
							</thead>
							<tbody>
								<tr class="templateRow hide">
									<td class="tipoDocumento"></td>
									<td class="Numero"></td>
									<td class="Utilizado"></td>
									<td class="Situacao"></td>
									<td class="acoes">
										<a class="btnCancelar icone dispensado" title="Cancelar número"></a>
										<button class="btnVisualizarMotivo icone pendencias" title="Visualizar motivo do cancelamento"></button>
										<%=Html.Hidden("hiddenObjeto", 0, new { @class = "hdnObjetoJson" }) %>
									</td>
								</tr>
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%=Url.Action("Index", "LiberacaoCFOCFOC") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>