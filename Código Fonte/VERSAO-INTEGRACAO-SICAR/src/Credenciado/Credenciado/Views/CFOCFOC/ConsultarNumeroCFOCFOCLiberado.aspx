<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ConsultarNumeroCFOCFOCLiberadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Consultar Número de CFO e CFOC Liberados</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOCFOC/consultarCFOCFOC.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			ConsultarNumeroCFOCFOCLiberado.load($('#central'),
			{
				urls:
				{
					buscar: '<%=Url.Action("FiltrarConsulta", "CFOCFOC")%>',
					invalidarModal: '<%=Url.Action("VisualizarMotivoInvalidacao", "CFOCFOC")%>'
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
			<legend>Nº Bloco</legend>
			<div class="block">
				<div class="coluna18">
					<label>Tipo do documento</label>
					<%=Html.DropDownList("TipoDocumentoBloco", Model.LstTipoDocumento, new { @class = "text ddlTipoDocumento" })%>
				</div>

				<div class="coluna18 prepend1">
					<label>Número</label>
					<%=Html.TextBox("NumeroBloco", string.Empty, new { @class="text maskNumInt txtNumero", @maxlength="10"})%>
				</div>

				<div class="coluna20 prepend1">
					<label>Data inicial da emissão</label>
					<%=Html.TextBox("DataInicialBloco", string.Empty, new { @class="text maskData txtDataInicialEmissao"})%>
				</div>

				<div class="coluna20 prepend1">
					<label>Data final da emissão</label>
					<%=Html.TextBox("DataFinalBloco", string.Empty, new { @class="text maskData txtDataFinalEmissao"})%>
				</div>

				<div class="coluna10 prepend1">
					<button type="button" class="inlineBotao bloco btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block divEsconder">
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
										<a class="btnVisualizarMotivo icone pendencias" title="Visualizar motivo da invalidação"></a>
										<%=Html.Hidden("hiddenObjeto", 0, new {@class="hdnObjetoJson"}) %>
									</td>
								</tr>
							</tbody>
						</table>
					</div>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFO: </label>
					<label class="lblQtdCFOBloco">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblQtdCFOBlocoUtilizado">0</label>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFOC: </label>
					<label class="lblQtdCFOCBloco">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblQtdCFOCBlocoUtilizado">0</label>
				</div>
			</div>
		</fieldset>
		
		<fieldset class="block box bloco">
			<legend>Nº Digital</legend>
			<div class="block">
				<div class="coluna18">
					<label>Tipo do documento</label>
					<%= Html.DropDownList("TipoDocumentoDigital", Model.LstTipoDocumento, new { @class = "text ddlTipoDocumento" })%>
				</div>

				<div class="coluna18 prepend1">
					<label>Número</label>
					<%=Html.TextBox("NumeroDigital", string.Empty, new { @class="text maskNumInt txtNumero", @maxlength="10"})%>
				</div>

				<div class="coluna20 prepend1">
					<label>Data inicial da emissão</label>
					<%=Html.TextBox("DataInicialDigital", string.Empty, new { @class="text maskData txtDataInicialEmissao"})%>
				</div>

				<div class="coluna20 prepend1">
					<label>Data final da emissão</label>
					<%=Html.TextBox("DataFinalDigital", string.Empty, new { @class="text maskData txtDataFinalEmissao"})%>
				</div>

				<div class="coluna10 prepend1">
					<button type="button" class="inlineBotao digital btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block divEsconder">
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
									<td class="tipoDocumento">
									</td>
									<td class="Numero">
									</td>
									<td class="Utilizado">
									</td>
									<td class="Situacao">
									</td>
								
									<td class="acoes">
										<a class="btnVisualizarMotivo icone pendencias" title="Visualizar motivo da invalidação"></a>
										<%=Html.Hidden("hiddenObjeto", 0, new {@class="hdnObjetoJson"}) %>
									</td>
								</tr>	
							</tbody>
						</table>
					</div>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFO: </label>
					<label class="lblQtdCFODigital">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblQtdCFODigitalUtilizado">0</label>
				</div>
			</div>

			<div class="block">
				<div class="coluna30">
					<label>CFOC: </label>
					<label class="lblQtdCFOCDigital">0</label>
				</div>
				<div class="coluna30 prepend2 ultima">
					<label>Utilizado: </label>
					<label class="lblQtdCFOCDigitalUtilizado">0</label>
				</div>
			</div>
		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href='<%=Url.Action("IndexConsultarPraga", "HabilitarEmissaoCFOCFOC") %>'>Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>