<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TituloModeloVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Modelo Título</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Titulo/tituloModeloVisualizar.js") %>" ></script>
	<script>
		$(function () {
			TituloModeloVisualizar.urlObterAssinantes = '<%= Url.Action("ObterFuncionariosSetor", "Titulo") %>';
			TituloModeloVisualizar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Modelo Título</h1>
		<br />
		<input type="hidden" class="hdnEditarModeloId"  value="<%=  Model.ModeloId %>" />
		<div class="box">
			<div class="block">
				<div class="coluna20">
					<label>
						Tipo *</label>
					<%= Html.DropDownList("Modelo.Tipo", Model.Tipos, new { @class = "text ddlTipo disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna48 prepend1">
					<label>
						SubTipo</label>
					<%= Html.TextBox("Modelo.Subtipo", null, new { @class = "text txtSubtipo disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>
						Data de Criação *</label>
					<%= Html.TextBox("Modelo.DataCriacao", Model.Modelo.DataCriacao.DataTexto, new { @class = "text maskData txDataCriacao disabled", @disabled = "disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna70">
					<label>
						Nome *</label>
					<%= Html.TextBox("Modelo.Nome", null, new { @class = "text txtNome disabled", @disabled = "disabled", @maxlength = "200" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>
						Sigla *</label>
					<%= Html.TextBox("Modelo.Sigla", null, new { @class = "text txtSigla disabled", @disabled = "disabled", @maxlength = "100" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna20">
					<label>Tipo de documento *</label>
					<%= Html.DropDownList("Modelo.TipoDocumento", Model.TiposDocumento, new { @class = "text ddlTipoDoc disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna48 prepend1">
					<label>Tipo de protocolo *</label>
					<%= Html.DropDownList("Modelo.TipoProtocolo", Model.TiposProtocolos, new { @class = "text ddlTipoProtocolo disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</div>

		<div class="block dataGrid">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
				<tbody>
					<tr>
						<td>
							<span title="Numeração automática?">Numeração automática?</span>
						</td>
						<td width="16%">
							<input type="hidden" class="hdnNumeracaoAutoId" value="<%= Model.NumeracaoAutomatica.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("NumeracaoAutomatica", 1, Convert.ToBoolean(Model.NumeracaoAutomatica.Valor), new { @class = "radio radNumeracaoAutomaticaS disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeracaoAutomatica", 0, !Convert.ToBoolean(Model.NumeracaoAutomatica.Valor), new { @class = "radio radNumeracaoAutomaticaN disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="Numeração reiniciada por ano?">Numeração reiniciada por ano?</span>
						</td>
						<td width="16%">
							<input type="hidden" class="hdnNumeracaoReiniciadaId" value="<%= Model.NumeracaoReiniciada.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("NumeracaoReiniciadaAno", 1, Convert.ToBoolean(Model.NumeracaoReiniciada.Valor), new { @class = "radio radNumeracaoReiniciadaAno disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeracaoReiniciadaAno", 0, !Convert.ToBoolean(Model.NumeracaoReiniciada.Valor), new { @class = "radio disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui protocolo obrigatório?">Possui protocolo obrigatório?</span>
						</td>
						<td>
							<input type="hidden" class="hdnProtocoloObrId" value="<%= Model.ProtocoloObrigatorio.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("ProtocoloObrigatorio", 1, Convert.ToBoolean(Model.ProtocoloObrigatorio.Valor), new { @class = "radio radProtocoloObrigatorio disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("ProtocoloObrigatorio", 0, !Convert.ToBoolean(Model.ProtocoloObrigatorio.Valor), new { @class = "radio disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="Possui prazo?">Possui prazo?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPrazoId" value="<%= Model.Prazo.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PossuiPrazo", 1, Convert.ToBoolean(Model.Prazo.Valor), new { @class = "radio radPossuiPrazoS disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PossuiPrazo", 0, !Convert.ToBoolean(Model.Prazo.Valor), new { @class = "radio radPossuiPrazoN disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui condicionantes?">Possui condicionantes?</span>
						</td>
						<td>
							<input type="hidden" class="hdnCondicionanteId" value="<%= Model.Condicionantes.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PossuiCondicionantes", 1, Convert.ToBoolean(Model.Condicionantes.Valor), new { @class = "radio radPossuiCondicionantes disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PossuiCondicionantes", 0, !Convert.ToBoolean(Model.Condicionantes.Valor), new { @class = "radio disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="É passível de renovação?">É passível de renovação?</span>
						</td>
						<td>
							<input type="hidden" class="hdnRenovacaoId" value="<%= Model.Renovacao.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PassivelRenovacao", 1, Convert.ToBoolean(Model.Renovacao.Valor), new { @class = "radio radPassivelRenovacaoS disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PassivelRenovacao", 0, !Convert.ToBoolean(Model.Renovacao.Valor), new { @class = "radio radPassivelRenovacaoN disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui fase anterior?" >Possui fase anterior?</span>
						</td>
						<td>
							<input type="hidden" class="hdnFaseAnteriorId" value="<%= Model.FaseAnterior.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("FaseAnterior", 1, Convert.ToBoolean(Model.FaseAnterior.Valor), new { @class = "radio radFaseAnteriorS disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("FaseAnterior", 0, !Convert.ToBoolean(Model.FaseAnterior.Valor), new { @class = "radio radFaseAnteriorN disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="Permitir enviar e-mail?">Permitir enviar e-mail?</span>
						</td>
						<td>
							<input type="hidden" class="hdnEnviarEmailId" value="<%= Model.EnviarEmail.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("EnviarEmail", 1, Convert.ToBoolean(Model.EnviarEmail.Valor), new { @class = "radio radEnviarEmailS disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("EnviarEmail", 0, !Convert.ToBoolean(Model.EnviarEmail.Valor), new { @class = "radio radEnviarEmailN disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="É solicitado pelo público externo?">É solicitado pelo público externo?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPublicoExternoId" value="<%= Model.PublicoExterno.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PublicoExterno", 1, Convert.ToBoolean(Model.PublicoExterno.Valor), new { @class = "radio radPublicoExterno disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PublicoExterno", 0, !Convert.ToBoolean(Model.PublicoExterno.Valor), new { @class = "radio radPublicoExternoN disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="O PDF do modelo será gerado pelo sistema?">O PDF do modelo será gerado pelo sistema?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPdfGeradoSistemaId" value="<%= Model.PdfGeradoSistema.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PdfGeradoSistema", 1, Convert.ToBoolean(Model.PdfGeradoSistema.Valor), new { @class = "radio radPdfGeradoSistema disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PdfGeradoSistema", 0, !Convert.ToBoolean(Model.PdfGeradoSistema.Valor), new { @class = "radio radPdfGeradoSistemaN disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
                    <!-- ## -->
					<tr>
						<td>
							<span title="É solicitado pelo público externo?">É emitido pelo credenciado?</span>
						</td>
						<td>
							<input type="hidden" class="hdnCredenciadoEmiteId" value="<%= Model.CredenciadoEmite.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("CredenciadoEmite", 1, Convert.ToBoolean(Model.CredenciadoEmite.Valor), new { @class = "radio radCredenciadoEmite disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("CredenciadoEmite", 0, !Convert.ToBoolean(Model.CredenciadoEmite.Valor), new { @class = "radio radCredenciadoEmiteN disabled", @disabled = "disabled" })%> Não</label>
						</td>
						<td>
							<span title="O PDF do modelo será gerado pelo sistema?">Numeração sincronizada com institucional?</span>
						</td>
						<td>
							<input type="hidden" class="hdnNumeroSincronizadoId" value="<%= Model.NumeroSincronizado.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("NumeroSincronizado", 1, Convert.ToBoolean(Model.NumeroSincronizado.Valor), new { @class = "radio radNumeroSincronizado disabled", @disabled = "disabled" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeroSincronizado", 0, !Convert.ToBoolean(Model.NumeroSincronizado.Valor), new { @class = "radio radNumeroSincronizadoN disabled", @disabled = "disabled" })%> Não</label>
						</td>
					</tr>
                    <!-- ## -->
				</tbody>
			</table>
		</div>

		<br />
		<div class="block divPossuiPrazo hide">
			<fieldset class="block box">
				<legend>Configurar Prazo</legend>
				<div class="coluna20">
					<label>Início do prazo *</label>
					<input type="hidden" class="hdnInicioPrazoId" value="<%= Model.InicioPrazo.Id %>" />
					<%= Html.DropDownList("InicioPrazo.Valor", Model.IniciosPrazo, new { @class = "text ddlInicioPrazo disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>Tipo do prazo *</label>
					<input type="hidden" class="hdnTipoPrazoId" value="<%= Model.TipoPrazo.Id %>" />
					<%= Html.DropDownList("TipoPrazo.Valor", Model.TiposPrazo, new { @class = "text ddlTipoPrazo disabled", @disabled = "disabled" })%>
				</div>
			</fieldset>
		</div>

		<div class="block divFaseAnteriror">
			<fieldset class="block box">
				<legend>Configurar Título da Fase Anterior</legend>
				<div class="block dataGrid coluna100">
					<table class="dataGridTable tabModelos <%= Model.Modelo.Modelos.Count == 0 ?"hide" : ""%>" width="100%" border="0" cellspacing="0" cellpadding="0">
						<tbody>
						<%for (int i = 0; i < Model.Modelo.Modelos.Count; i++) { %>
							<tr>
								<td>
									<input type="hidden" class="hdnModeloIdRelacionamento" value="<%= Model.Modelo.Modelos[i].IdRelacionamento %>" />
									<input type="hidden" class="hdnModeloId" value="<%= Model.Modelo.Modelos[i].Id %>" />
									<span class="modeloTexto"><%= Model.Modelo.Modelos[i].Nome%></span>
								</td>
							</tr>
						<% } %>
						</tbody>
					</table>
				</div>
			</fieldset>
		</div>

		<div class="block divEnviarEmail hide">
			<fieldset class="block box">
				<legend>Configurar Envio de E-mail</legend>
				<div class="block">
					<div class="coluna99">
						<label>Texto do e-mail *</label>
						<input type="hidden" class="hdnTextoEmailId" value="<%= Model.TextoEmail.Id %>" />
						<%= Html.TextArea("TextoEmail.Valor", Convert.ToString(Model.TextoEmail.Valor), new { @class = "textareaPequeno txtEmail disabled", @disabled = "disabled" })%>
						<label style="font-style:italic; color:Silver">* As opções entre colchetes serão preenchidas automaticamente pelo sistema no envio do e-mail.
						Caso algumas dessas formatações sejam modificadas ou removidas a informação não será preenchida.</label>
					</div>
				</div>
				<div class="block coluna50">
					<label>Anexar PDF do título?</label>
					<input type="hidden" class="hdnAnexarPDFId" value="<%= Model.AnexarPDFTitulo.Id %>" />
					<label class="prepend6"><%= Html.RadioButton("AnexarPDF", 1, Convert.ToBoolean(Model.AnexarPDFTitulo.Valor), new { @class = "radio radAnexarPDFS disabled", @disabled = "disabled" })%> Sim</label>
					<label class="prepend6"><%= Html.RadioButton("AnexarPDF", 0, !Convert.ToBoolean(Model.AnexarPDFTitulo.Valor), new { @class = "radio disabled", @disabled = "disabled" })%> Não</label>
				</div>
			</fieldset>
		</div>

		<fieldset class="block box filtroExpansivoAberto">
			<legend class="titFiltros">Setor de Cadastro</legend>
			<div class="block filtroCorpo">
				<div class="block dataGrid">
					<table class="dataGridTable tabSetores <%= Model.Modelo.Setores.Count == 0 ?"hide" : ""%>" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Setor</th>
								<th>Hierarquia do cabeçalho</th>
							</tr>
						</thead>
						<tbody>
						<% for (int i = 0; i < Model.Modelo.Setores.Count; i++) { %>
							<tr>
								<td>
									<input type="hidden" class="hdnSetorId" value="<%= Model.Modelo.Setores[i].Id %>" />
									<span class="setorTexto"><%= Model.Modelo.Setores[i].Texto %></span>
								</td>
								<td>
									<span class="hierarquiaCab"><%= Model.Modelo.Setores[i].HierarquiaCabecalho %></span>
								</td>
							</tr>
						<% } %>
						</tbody>
					</table>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box filtroExpansivoAberto">
			<legend class="titFiltros">Assinante</legend>
				<div class="block filtroCorpo">
					<table class="dataGridTable tabAssinantes" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
						<tr>
							<th>Setor</th>
							<th>Assinante</th>
						</tr>
						</thead>
						<tbody>
							<%for (int i = 0; i < Model.Modelo.Assinantes.Count; i++) {%>
							<tr>
								<td>
									<span class="setorTexto"><%= Model.Modelo.Assinantes[i].SetorTexto %></span>
								</td>
								<td>
									<span class="assinanteTexto"><%= Model.Modelo.Assinantes[i].TipoTexto%></span>
								</td>
							</tr>
						<% } %>
						</tbody>
					</table>
				</div>
		</fieldset>

		<% if (Convert.ToBoolean(Model.PdfGeradoSistema.Valor)) { %>
		<fieldset class="block box">
			<legend>PDF do Título</legend>
			<div class="block">
				<div class="coluna30 inputFileDiv">
					<label>Arquivo *</label>
					<div class="block">
						<% if (Model.Modelo.Arquivo.Id.HasValue) {%>
							<%= Html.ActionLink(Model.Modelo.Arquivo.Nome, "Baixar", "Arquivo", new { @id = Model.Modelo.Arquivo.Id }, new { @Class =" txtArquivoNome" })%>
						<%} %>
					</div>
					<input type="hidden" class="hdnArquivoId" value="<%= Html.Encode(Model.Modelo.Arquivo.Id) %>" />
					<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
				</div>
			</div>
		</fieldset>
		<% } %>

		<div class="block box btnTituloContainer">
			<span class="cancelarCaixa"><span class="btnModalOu"></span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("TituloModeloListar") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>