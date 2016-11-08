<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTitulo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TituloModeloVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Modelo Título</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/tituloModelo.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			TituloModelo.urlObterUltimoNumero = '<%= Url.Action("ObterUltimoNumeroGerado", "Titulo") %>';
			TituloModelo.urlObterAssinantes = '<%= Url.Action("ObterFuncionariosSetor", "Titulo") %>';
			TituloModelo.urlEnviarArquivo= '<%= Url.Action("Arquivo", "Arquivo") %>';
			TituloModelo.urlObterTextoPadraoEmail= '<%= Url.Action("ObterTextoPadraoEmail", "Titulo") %>';
			TituloModelo.urlEditar = '<%= Url.Action("TituloModeloEditar", "Titulo") %>';
			TituloModelo.Mensagens = <%= Model.Mensagens  %>;
			TituloModelo.TiposArquivo = <%= Model.TiposArquivoValido %>;
			TituloModelo.urlVerificarPublicoExternoAtividade = '<%= Url.Action("VerificarPublicoExternoAtividade", "Titulo") %>';
			TituloModelo.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Modelo Título</h1>
		<br />
		<input type="hidden" class="hdnEditarModeloId"  value="<%=  Model.ModeloId %>" />

		<div class="box">
			<div class="block">
				<div class="coluna20">
					<label>Tipo *</label>
					<%= Html.DropDownList("Modelo.Tipo", Model.Tipos, new { @class = "text ddlTipo disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna48 prepend1">
					<label>Subtipo</label>
					<%= Html.TextBox("Modelo.Subtipo", null, new { @class = "text txtSubtipo", @maxlength = "100" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>Data de criação *</label>
					<%= Html.TextBox("Modelo.DataCriacao", Model.Modelo.DataCriacao.DataTexto, new { @class = "text maskData txDataCriacao disabled", @disabled = "disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna70">
					<label>Nome *</label>
					<%= Html.TextBox("Modelo.Nome", null, new { @class = "text txtNome", @maxlength = "200" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>Sigla *</label>
					<%= Html.TextBox("Modelo.Sigla", null, new { @class = "text txtSigla", @maxlength = "100" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna20">
					<label>Tipo de documento *</label>
					<%= Html.DropDownList("Modelo.TipoDocumento", Model.TiposDocumento, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlTipoDoc" }))%>
				</div>
				<div class="coluna48 prepend1">
					<label>Tipo de protocolo *</label>
					<%= Html.DropDownList("Modelo.TipoProtocolo", Model.TiposProtocolos, new { @class = "text ddlTipoProtocolo" })%>
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
							<label class="prepend6"><%= Html.RadioButton("NumeracaoAutomatica", 1, Convert.ToBoolean(Model.NumeracaoAutomatica.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radNumeracaoAutomaticaS" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeracaoAutomatica", 0, !Convert.ToBoolean(Model.NumeracaoAutomatica.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radNumeracaoAutomaticaN" }))%> Não</label>
						</td>
						<td>
							<span title="Numeração reiniciada por ano?">Numeração reiniciada por ano?</span>
						</td>
						<td width="16%">
							<input type="hidden" class="hdnNumeracaoReiniciadaId" value="<%= Model.NumeracaoReiniciada.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("NumeracaoReiniciadaAno", 1, Convert.ToBoolean(Model.NumeracaoReiniciada.Valor), new { @class = "radio radNumeracaoReiniciadaAno" })%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeracaoReiniciadaAno", 0, !Convert.ToBoolean(Model.NumeracaoReiniciada.Valor), new { @class = "radio" })%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui protocolo obrigatório?">Possui protocolo obrigatório? </span>
						</td>
						<td>
							<input type="hidden" class="hdnProtocoloObrId" value="<%= Model.ProtocoloObrigatorio.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("ProtocoloObrigatorio", 1, Convert.ToBoolean(Model.ProtocoloObrigatorio.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radProtocoloObrigatorio" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("ProtocoloObrigatorio", 0, !Convert.ToBoolean(Model.ProtocoloObrigatorio.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio" }))%> Não</label>
						</td>
						<td>
							<span title="Possui prazo?">Possui prazo?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPrazoId" value="<%= Model.Prazo.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PossuiPrazo", 1, Convert.ToBoolean(Model.Prazo.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPossuiPrazoS" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PossuiPrazo", 0, !Convert.ToBoolean(Model.Prazo.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPossuiPrazoN" }))%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui condicionantes?">Possui condicionantes?</span>
						</td>
						<td>
							<input type="hidden" class="hdnCondicionanteId" value="<%= Model.Condicionantes.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PossuiCondicionantes", 1, Convert.ToBoolean(Model.Condicionantes.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPossuiCondicionantes" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PossuiCondicionantes", 0, !Convert.ToBoolean(Model.Condicionantes.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio" }))%> Não</label>
						</td>
						<td>
							<span title="É passível de renovação?">É passível de renovação?</span>
						</td>
						<td>
							<input type="hidden" class="hdnRenovacaoId" value="<%= Model.Renovacao.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PassivelRenovacao", 1, Convert.ToBoolean(Model.Renovacao.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPassivelRenovacaoS" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PassivelRenovacao", 0, !Convert.ToBoolean(Model.Renovacao.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPassivelRenovacaoN" }))%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="Possui fase anterior?" >Possui fase anterior? </span>
						</td>
						<td>
							<input type="hidden" class="hdnFaseAnteriorId" value="<%= Model.FaseAnterior.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("FaseAnterior", 1, Convert.ToBoolean(Model.FaseAnterior.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radFaseAnteriorS" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("FaseAnterior", 0, !Convert.ToBoolean(Model.FaseAnterior.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radFaseAnteriorN" }))%> Não</label>
						</td>
						<td>
							<span title="Permitir enviar e-mail?">Permitir enviar e-mail?</span>
						</td>
						<td>
							<input type="hidden" class="hdnEnviarEmailId" value="<%= Model.EnviarEmail.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("EnviarEmail", 1, Convert.ToBoolean(Model.EnviarEmail.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radEnviarEmailS" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("EnviarEmail", 0, !Convert.ToBoolean(Model.EnviarEmail.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radEnviarEmailN" }))%> Não</label>
						</td>
					</tr>
					<tr>
						<td>
							<span title="É solicitado pelo público externo?">É solicitado pelo público externo?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPublicoExternoId" value="<%= Model.PublicoExterno.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PublicoExterno", 1, Convert.ToBoolean(Model.PublicoExterno.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPublicoExterno" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PublicoExterno", 0, !Convert.ToBoolean(Model.PublicoExterno.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPublicoExternoN" }))%> Não</label>
						</td>
						<td>
							<span title="O PDF do modelo será gerado pelo sistema?">O PDF do modelo será gerado pelo sistema?</span>
						</td>
						<td>
							<input type="hidden" class="hdnPdfGeradoSistemaId" value="<%= Model.PdfGeradoSistema.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("PdfGeradoSistema", 1, Convert.ToBoolean(Model.PdfGeradoSistema.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPdfGeradoSistema" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("PdfGeradoSistema", 0, !Convert.ToBoolean(Model.PdfGeradoSistema.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radPdfGeradoSistemaN" }))%> Não</label>
						</td>
					</tr>
                    <!-- ## -->
					<tr>
						<td>
							<span title="É solicitado pelo público externo?">É emitido pelo credenciado?</span>
						</td>
						<td>
							<input type="hidden" class="hdnCredenciadoEmiteId" value="<%= Model.CredenciadoEmite.Id %>" />
							<label class="prepend6"><%= Html.RadioButton("CredenciadoEmite", 1, Convert.ToBoolean(Model.CredenciadoEmite.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radCredenciadoEmite" }))%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("CredenciadoEmite", 0, !Convert.ToBoolean(Model.CredenciadoEmite.Valor), ViewModelHelper.SetaDisabled(Model.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio, new { @class = "radio radCredenciadoEmiteN" }))%> Não</label>
						</td>
						<td>
							<span title="O PDF do modelo será gerado pelo sistema?">Numeração sincronizada com institucional?</span>
						</td>
						<td>
							<input type="hidden" class="hdnNumeroSincronizadoId" value="<%= Model.NumeroSincronizado.Id %>" />
                            <% 
                               object objAttr = null;
                               object objAttrN = null;
                               
                               if (Convert.ToBoolean(Model.CredenciadoEmite.Valor))
	                           {
		                           objAttr =  new { @class = "radio radNumeroSincronizado" };
                                   objAttrN = new { @class = "radio radNumeroSincronizadoN" };
	                           }
                               else
                               {
                                   objAttr = new { @class = "radio radNumeroSincronizado disabled", @disabled = "disabled" };
                                   objAttrN = new { @class = "radio radNumeroSincronizadoN disabled", @disabled = "disabled" };
                               }                               
                            %>
							<label class="prepend6"><%= Html.RadioButton("NumeroSincronizado", 1, Convert.ToBoolean(Model.NumeroSincronizado.Valor), objAttr)%> Sim</label>
							<label class="prepend6"><%= Html.RadioButton("NumeroSincronizado", 0, !Convert.ToBoolean(Model.NumeroSincronizado.Valor), objAttrN)%> Não</label>
						</td>
					</tr>
                    <!-- ## -->
				</tbody>
			</table>
		</div>
		
		<div class="block divPossuiPrazo hide">
			<fieldset class="block box">
				<legend>Configurar Prazo</legend>
				<div class="coluna20">
					<label>Início do prazo *</label>
					<input type="hidden" class="hdnInicioPrazoId" value="<%= Model.InicioPrazo.Id %>" />
					<%= Html.DropDownList("InicioPrazo.Valor", Model.IniciosPrazo, new { @class = "text ddlInicioPrazo" })%>
				</div>
				<div class="coluna20 prepend1">
					<label>Tipo do prazo *</label>
					<input type="hidden" class="hdnTipoPrazoId" value="<%= Model.TipoPrazo.Id %>" />
					<%= Html.DropDownList("TipoPrazo.Valor", Model.TiposPrazo, new { @class = "text ddlTipoPrazo" })%>
				</div>
			</fieldset>
		</div>
		
		<div class="block divFaseAnteriror">
			<fieldset class="block box">
				<legend>Configurar Título da Fase Anterior</legend>
				<div class="block">
					<div class="coluna99">
						<label>Título anterior obrigatorio? *</label>
						<input type="hidden" class="hdnTituloAnteriroObrigatorioId" value="<%= Model.TituloAnteriroObrigatorio.Id %>" />
						<label class="prepend1"><%= Html.RadioButton("TituloAnteriroObr", 1, Convert.ToBoolean(Model.TituloAnteriroObrigatorio.Valor), new { @class = "radio radTituloAnteriroObr" })%> Sim</label>
						<label class="prepend1"><%= Html.RadioButton("TituloAnteriroObr", 0, !Convert.ToBoolean(Model.TituloAnteriroObrigatorio.Valor), new { @class = "radio radTituloAnteriroObrN" })%> Não</label>
					</div>
				</div>
				<div class="block">
					<div class="coluna47">
						<label>Modelo *</label>
						<%= Html.DropDownList("Modelo", Model.Modelos, new { @class = "text ddlModelo" })%>
					</div>
					<div class="coluna15 prepend1">
						<button type="button" style="width:35px" class="btnAddModelo inlineBotao botaoAdicionarIcone" title="Adicionar Modelo">Adicionar</button>
					</div>
				</div>
				<div class="block dataGrid coluna100">
					<table class="dataGridTable tabModelos <%= Model.Modelo.Modelos.Count == 0 ?"hide" : ""%>" width="100%" border="0" cellspacing="0" cellpadding="0">
						<tbody>
						<%for (int i = 0; i < Model.Modelo.Modelos.Count; i++){
						%>
						<tr>
							<td>
								<input type="hidden" class="hdnModeloIdRelacionamento" value="<%= Model.Modelo.Modelos[i].IdRelacionamento %>" />
								<input type="hidden" class="hdnModeloId" value="<%= Model.Modelo.Modelos[i].Id %>" />
								<span class="modeloTexto"><%= Model.Modelo.Modelos[i].Nome%></span>
							</td>
							<td  width="5%">
								<button title="Excluir" class="icone excluir btnExcluirModelo" type="button"></button>
							</td>
						</tr>
					<% } %>
					</tbody>
				</table>
				<table style="display:none">
					<tbody>
						<tr class="trItemTemplateTitulo">
							<td>
								<input type="hidden" class="hdnModeloId" value="0" />
								<input type="hidden" class="hdnModeloIdRelacionamento" value="0" />
								<span class="modeloTexto"></span>
							</td>
							<td  width="5%">
								<button title="Excluir" class="icone excluir btnExcluirModelo" value="" type="button"></button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
			</fieldset>
		</div>
		
		<div class="block divEnviarEmail hide">
			<fieldset class="block box">
				<legend>Configurar Envio de E-mail</legend>
				<div class="block">
					<div class="block floatRight">
						<label>&nbsp;</label>
						<input type="button"  title="Carregar texto original" class="icone refresh btnEmailOriginal" />
					</div>
					<div class="coluna99">
						<label>Texto do e-mail *</label>
						<input type="hidden" class="hdnTextoEmailId" value="<%= Model.TextoEmail.Id %>" />
						<%= Html.TextArea("TextoEmail.Valor", Convert.ToString(Model.TextoEmail.Valor), new { @class = "textareaPequeno txtEmail", @Maxlength = "500" })%>
						<label style="font-style:italic; color:Silver">* As opções entre colchetes serão preenchidas automaticamente pelo sistema no envio do e-mail. 
						Caso algumas dessas formatações sejam modificadas ou removidas a informação não será preenchida.</label>
					</div>
				</div>
				<div class="block coluna50">
					<label>Anexar PDF do título?</label>
					<input type="hidden" class="hdnAnexarPDFId" value="<%= Model.AnexarPDFTitulo.Id %>" />
					<label class="prepend1"><%= Html.RadioButton("AnexarPDF", 1, Convert.ToBoolean(Model.AnexarPDFTitulo.Valor), new { @class = "radio radAnexarPDFS" })%> Sim</label>
					<label class="prepend1"><%= Html.RadioButton("AnexarPDF", 0, !Convert.ToBoolean(Model.AnexarPDFTitulo.Valor), new { @class = "radio" })%> Não</label>
				</div>
			</fieldset>
		</div>
		
		<fieldset class="block box filtroExpansivoAberto">
			<legend class="titFiltros">Setor de Cadastro</legend>
			<div class="block filtroCorpo">
				<div class="block">
					<div class="coluna50">
						<label>Setor *</label>
						<%= Html.DropDownList("Setor", Model.Setores, new { @class = "text ddlSetor" })%>
					</div>
					<div class="coluna30">
						<label>Hierarquia do cabeçalho </label>
						<%= Html.TextBox("Hierarquia", null, new { @class = "text txtHierarquiaCab", @Maxlength="100" })%>
					</div>
					<div class="coluna15 prepend1">
						<button type="button" style="width:35px" class="btnAddSetor inlineBotao botaoAdicionarIcone" title="Adicionar Setor">Adicionar</button>
					</div>
				</div>
			
				<div class="block dataGrid">
					<table class="dataGridTable tabSetores <%= Model.Modelo.Setores.Count == 0 ?"hide" : ""%>" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Setor</th>
								<th>Hierarquia do cabeçalho</th>
								<th width="5%"></th>
							</tr>
						</thead>
						<tbody>
						<%for (int i = 0; i < Model.Modelo.Setores.Count; i++) { %>
						<tr>
							<td>
								<input type="hidden" class="hdnSetorId" value="<%= Model.Modelo.Setores[i].Id %>" />
								<input type="hidden" class="hdnSetorIdRelacionamento" value="<%= Model.Modelo.Setores[i].IdRelacao %>" />
								<span class="setorTexto" title="<%= Model.Modelo.Setores[i].Texto%>"><%= Model.Modelo.Setores[i].Texto%></span>
							</td>
							<td>
								<span class="hierarquiaCab" title="<%= Model.Modelo.Setores[i].HierarquiaCabecalho %>"><%= Model.Modelo.Setores[i].HierarquiaCabecalho %></span>
							</td>
							<td width="5%">
								<button title="Excluir" class="icone excluir btnExcluirSetor" value="" type="button"></button>
							</td>
						</tr>
					<% } %>
					</tbody>
					</table>
					<table style="display:none">
						<tbody>
							<tr class="trItemTemplateSetor">
								<td>
									<input type="hidden" class="hdnSetorId" value="0" />
									<input type="hidden" class="hdnSetorIdRelacionamento" value="0" />
									<span class="setorTexto"></span>
								</td>
								<td>
									<span class="hierarquiaCab"></span>
								</td>
								<td  width="5%">
									<button title="Excluir" class="icone excluir btnExcluirSetor" value="" type="button"></button>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</fieldset>
		
		
		<fieldset class="block box filtroExpansivoAberto">
			<legend class="titFiltros">Assinante</legend>
				<div class="block filtroCorpo">
					<div class="block">
						<div class="coluna50">
							<label>Setor *</label>
							<%= Html.DropDownList("SetorAssinante", Model.Setores, new { @class = "text ddlSetorAssinante" })%>
						</div>
						<div class="coluna30">
							<label>Assinante *</label>
							<%= Html.DropDownList("Assinante", Model.Assinantes, new { @class = "text ddlAssinante" })%>
						</div>
						<div class="coluna15 botoesPalavraChavesDiv">
							<button type="button" style="width:35px" class="btnAddAssinante inlineBotao botaoAdicionarIcone" title="Adicionar Assinante">Adicionar</button>
						</div>
					</div>
				
					<div class="block dataGrid">
						<table class="dataGridTable tabAssinantes" width="100%" border="0" cellspacing="0" cellpadding="0">
							<thead>
								<tr>
									<th>Setor</th>
									<th>Assinante</th>
									<th width="5%"></th>
								</tr>
							</thead>
							<tbody>
							<%for (int i = 0; i < Model.Modelo.Assinantes.Count; i++){
							%>
							<tr>
								<td>
									<input type="hidden" class="hdnAssinanteId" value="<%= Model.Modelo.Assinantes[i].Id %>" />
									<input type="hidden" class="hdnSetorId" value="<%= Model.Modelo.Assinantes[i].SetorId %>" />
									<span class="setorTexto" title="<%= Model.Modelo.Assinantes[i].SetorTexto %>"><%= Model.Modelo.Assinantes[i].SetorTexto %></span>
								</td>
								<td>
									<input type="hidden" class="hdnAssinanteTipoId" value="<%= Model.Modelo.Assinantes[i].TipoId %>" />
									<input type="hidden" class="hdnAssinanteIdRelacionamento" value="<%= Model.Modelo.Assinantes[i].IdRelacionamento %>" />
									<span class="assinanteTexto" title="<%= Model.Modelo.Assinantes[i].TipoTexto%>"><%= Model.Modelo.Assinantes[i].TipoTexto%></span>
								</td>
								<td  width="5%">
									<button title="Excluir" class="icone excluir btnExcluirAssinante" value="" type="button"></button>
								</td>
							</tr>
						<% } %>
						</tbody>
					</table>
					<table style="display:none">
						<tbody>
							<tr class="trItemTemplateAssinante">
								<td>
									<input type="hidden" class="hdnAssinanteId" value="0" />
									<input type="hidden" class="hdnSetorId" value="0" />
									<span class="setorTexto"></span>
								</td>
								<td>
									<input type="hidden" class="hdnAssinanteTipoId" value="0" />
									<input type="hidden" class="hdnAssinanteIdRelacionamento" value="0" />
									<span class="assinanteTexto"></span>
								</td>
								<td  width="5%">
									<button title="Excluir" class="icone excluir btnExcluirAssinante" value="" type="button"></button>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
				</div>
		</fieldset>

		<% if (Convert.ToBoolean(Model.PdfGeradoSistema.Valor)) { %>
		<fieldset class="block box">
			<legend>PDF do Título</legend>
			<div class="block">
				<div class="coluna40 inputFileDiv">
					<label>Arquivo *</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.Modelo.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.Modelo.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome"><%= Html.Encode(Model.Modelo.Arquivo.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.Modelo.Arquivo.Nome) ? "" : "hide" %>">
						<input type="file" id="file" class="inputFile" style="display: block" name="file" />
					</span>
				</div>
				<div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.Modelo.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.Modelo.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
			</div>
		</fieldset>
		<% } %>

		<div class="block box btnTituloContainer">
			<input class="btnModeloTituloSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("TituloModeloListar") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
