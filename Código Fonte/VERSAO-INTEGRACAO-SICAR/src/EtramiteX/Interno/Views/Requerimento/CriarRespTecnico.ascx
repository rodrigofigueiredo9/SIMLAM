<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>
<div>
	<div class="divConteudoResponsavelTec associarMultiplo <%= Model.IsAbaFinalizar ? "" : "block box" %>">
		<input type="hidden" class="hdnResonsavelVisualizar" value="false" />
		<input type="hidden" class="hdnQuantidadeItem" value="<%= Model.Responsaveis.Count %>" />
		<div class="asmItens">
			<% if (Model.Responsaveis.Count == 0 && !Model.IsAbaFinalizar) { %>
			<div class="asmItemContainer boxBranca borders">
				<div class="conteudoResponsaveis block prepend1">
					<p class="block"><a title="Remover responsável" class="btnAsmExcluir modoVisualizar">Remover responsável</a></p>

					<div class="asmConteudoFixo block">
						<div class="coluna60">
							<label>Nome/Razão social </label>
							<input type="hidden" class="hdnIdRelacionamento" value="" />
							<input type="hidden" class="hdnResponsavelId asmItemId" value="" />
							<input type="text" class="text nomeRazao disabled asmItemTexto" name="resp[0].NomeRazao" value="" maxlength="80" disabled="disabled" />
						</div>
						<div class="coluna17 prepend2">
							<label>CPF/CNPJ </label>
							<input type="text" class="text cpfCnpj disabled" name="resp[0].CpfCnpj" value="" maxlength="10" disabled="disabled" />
						</div>
						<div class="prepend2">
							<button type="button" title="Buscar responsável técnico" class="floatLeft inlineBotao btnAsmAssociar">Buscar</button>
							<button type="button" title="Limpar" class="floatLeft inlineBotao btnAsmLimpar hide">Limpar</button>
							<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar hide" href="" style="display: none;">Visualizar Responsável</a>
						</div>
					</div>
					
					<div class="asmConteudoLink hide">
						<div class="asmConteudoInterno">
							<div class="divInternoResponsavel block">
								<div class="coluna25">
									<label>Função *</label>
									<%= Html.DropDownList("resp[count].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, 0), new { @class = "text funcao" })%>
								</div>
								<div class="coluna15 prepend2">
									<label>Número da ART *</label>
									<input type="text" class="text art" name="resp[0].Art" value="" maxlength="20" />
								</div>
							</div>
						</div>
						<a class="linkVejaMaisCampos ativo"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ocultar detalhes</span></a>
					</div>
				</div>
			</div>
			<% } %>

			<% for (int count = 0; count < Model.Responsaveis.Count; count++) { %>
			<div class="asmItemContainer boxBranca borders">
				<div class="conteudoResponsaveis block prepend1">
					<% if(!Model.IsAbaFinalizar) { %>
					<p class="block"><a title="Remover responsável" class="btnAsmExcluir modoVisualizar">Remover Responsável</a></p>
					<% } %>

					<div class="asmConteudoFixo block">
						<div class="coluna60">
							<label>Nome/Razão social </label>
							<input type="hidden" class="hdnIdRelacionamento" value="<%= Model.Responsaveis[count].IdRelacionamento %>" />
							<input type="hidden" class="hdnResponsavelId asmItemId" value="<%= Model.Responsaveis[count].Id %>" />
							<input type="text" class="text nomeRazao disabled asmItemTexto" name="resp[<%= count %>].NomeRazao" value="<%= Model.Responsaveis[count].NomeRazao %>" maxlength="80" disabled="disabled" />
						</div>
						<div class="coluna17 prepend2">
							<label>CPF/CNPJ </label>
							<input type="text" class="text cpfCnpj disabled" name="resp[<%= count %>].CpfCnpj" value="<%= Model.Responsaveis[count].CpfCnpj %>" maxlength="10" disabled="disabled" />
						</div>
					<% if(!Model.IsAbaFinalizar) { %>
						<div class="prepend2">
							<button type="button" title="Limpar" class="floatLeft inlineBotao modoVisualizar btnAsmLimpar">Limpar</button>
							<button type="button" class="floatLeft inlineBotao btnAsmAssociar modoVisualizar hide">Buscar</button>
							<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar modoVisualizar" href="">Visualizar Responsável</a>
						</div>
					<% } %>
					</div>
					<div class="asmConteudoLink">
						<div class="asmConteudoInterno hide">
							<div class="divInternoResponsavel block">
							<% if(!Model.IsAbaFinalizar) { %>
								<div class="coluna25">
									<label>Função *</label>
									<%= Html.DropDownList("resp[count].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, Model.Responsaveis[count].Funcao), new { @class = "text funcao bloquear" })%>
								</div>
								<div class="coluna15 prepend2">
									<label>Número da ART *</label>
									<input type="text" class="text art bloquear" name="resp[<%= count %>].Art" value="<%= Model.Responsaveis[count].NumeroArt %>" maxlength="20" />
								</div>
							<% } else { %>
								<div class="coluna25">
									<label>Função *</label>
									<%= Html.DropDownList("resp[count].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, Model.Responsaveis[count].Funcao), new { @class = "text funcao disabled", @disabled = "disabled" })%>
								</div>
								<div class="coluna15 prepend2">
									<label>Número da ART *</label>
									<input type="text" class="text art disabled" disabled="disabled" name="resp[<%= count %>].Art" value="<%= Model.Responsaveis[count].NumeroArt %>" maxlength="20" />
								</div>
							<% } %>
							</div>
						</div>
						<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
					</div>
				</div>
			</div>
			<% } %>
		</div>

		<% if (!Model.IsAbaFinalizar) { %>
		<div class="block fsAssociarMultiplo negtTop15"><br /><button title="Adicionar responsável técnico" class="btnAsmAdicionar direita modoVisualizar">Responsável técnico</button></div>

		<div class="hide">
			<div class="asmItemTemplateContainer asmItemContainer boxBranca borders">
				<div class="conteudoResponsaveis block prepend1">
					<p class="block"><a title="Remover responsável" class="btnAsmExcluir">Remover responsável</a></p>

					<div class="asmConteudoFixo block">
						<div class="coluna60">
							<label>Nome/Razão social </label>
							<input type="hidden" class="hdnIdRelacionamento" value="0" />
							<input type="hidden" class="hdnResponsavelId asmItemId" value="0" />
							<input type="text" class="text nomeRazao disabled asmItemTexto" name="resp[count].NomeRazao" value="" maxlength="80" disabled="disabled" />
						</div>
						<div class="coluna17 prepend2">
							<label>CPF/CNPJ </label>
							<input type="text" class="text cpfCnpj disabled" name="resp[count].CpfCnpj" value="" maxlength="10" disabled="disabled" />
						</div>
						<div class="prepend2">
							<button type="button" title="Limpar" class="floatLeft inlineBotao btnAsmLimpar hide">Limpar</button>
							<button type="button" class="floatLeft inlineBotao btnAsmAssociar">Buscar</button>
							<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar hide" href="">Visualizar Responsável</a>
						</div>
					</div>
					<div class="asmConteudoLink">
						<div class="asmConteudoInterno hide">
							<div class="divInternoResponsavel block">
								<div class="coluna25">
									<label>Função *</label>
									<%= Html.DropDownList("resp[count].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, 0), new { @class = "text funcao" })%>
								</div>
								<div class="coluna15 prepend2">
									<label>Número da ART *</label>
									<input type="text" class="text art" name="resp[count].Art" value="" maxlength="20" />
								</div>
							</div>
						</div>
						<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
					</div>
				</div>
		<% } else if(Model.Responsaveis.Count == 0) { %>
		<label>Não existe responsável técnico associado a este requerimento.</label>
		<% } %>
	</div>
</div>