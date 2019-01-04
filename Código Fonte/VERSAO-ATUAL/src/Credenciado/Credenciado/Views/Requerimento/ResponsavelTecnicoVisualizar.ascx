<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>
<div>
	<div class="divConteudoResponsavelTec associarMultiplo <%= Model.IsAbaFinalizar ? "" : "block box" %>">
		<input type="hidden" class="hdnResonsavelVisualizar" value="true" />
		<input type="hidden" class="hdnQuantidadeItem" value="<%= Model.Responsaveis.Count %>" />
		<input type="hidden" class="hdnBarragemDeclataroria" value="<%= Model.contemBarragemDeclaratoria %>" />
		<div class="asmItens">
			<% for (int count = 0; count < Model.Responsaveis.Count; count++) { %>
			<div class="asmItemContainer boxBranca borders">
				<div class="conteudoResponsaveis block prepend1">
					<div class="asmConteudoFixo block">
						<div class="coluna60">
							<label>Nome/Razão social </label>
							<input type="hidden" class="hdnIdRelacionamento" value="<%= Model.Responsaveis[count].IdRelacionamento %>" />
							<input type="hidden" class="hdnResponsavelId asmItemId" value="<%= Model.Responsaveis[count].Id %>" />
							<input type="text" class="text nomeRazao disabled asmItemTexto" name="resp[<%= count %>].NomeRazao" value="<%= Model.Responsaveis[count].NomeRazao %>" maxlength="80" disabled="disabled" />
						</div>
						<div class="coluna17 prepend2">
							<label>CPF/CNPJ </label>
							<input type="text" class="text cpfCnpj disabled" name="resp[<%= count %>].CpfCnpj" value="<%= Model.Responsaveis[count].CpfCnpj %>" maxlength="15" disabled="disabled" />
						</div>
					<% if(!Model.IsAbaFinalizar) { %>
						<div class="prepend2">
							<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar" href="">Visualizar Responsável</a>
						</div>
					<% } %>
					</div>
					<div class="asmConteudoLink">
						<div class="asmConteudoInterno hide">
							<div class="divInternoResponsavel block">
								<div class="coluna25">
									<label>Função *</label>
									<%= Html.DropDownList("resp[count].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, Model.Responsaveis[count].Funcao), new { @class = "text funcao disabled", @disabled = "disabled" })%>
								</div>
								<div class="coluna15 prepend2">
									<label>Número da ART *</label>
									<input type="text" class="text art disabled" disabled="disabled" name="resp[<%= count %>].Art" value="<%= Model.Responsaveis[count].NumeroArt %>" maxlength="20" />
								</div>
							</div>
						</div>
						<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
					</div>
				</div>
			</div>
			<% } %>
		</div>
	</div>
</div>