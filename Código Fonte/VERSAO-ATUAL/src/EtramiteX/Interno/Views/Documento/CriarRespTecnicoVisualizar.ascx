<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>

<div class="divConteudoResponsavelTec associarMultiplo">
	<div class="asmItens">
	<% if (Model.Responsaveis.Count > 0) { %>
	<% for (int count = 0; count < Model.Responsaveis.Count; count++) { %>
		<div class="asmItemContainer boxBranca borders">
			<div class="conteudoResponsaveis block prepend1">

				<div class="asmConteudoFixo block">
					<div class="coluna60">
						<label>Nome/Razão social</label>
						<input type="hidden" class="hdnIdRelacionamento" value="<%= Model.Responsaveis[count].IdRelacionamento %>" />
						<input type="hidden" class="hdnResponsavelId asmItemId" value="<%= Model.Responsaveis[count].Id %>" />
						<input type="text" class="text nomeRazao asmItemTexto disabled" name="resp[<%= count %>].NomeRazao" value="<%= Model.Responsaveis[count].NomeRazao %>" disabled="disabled" />
					</div>
					<div class="coluna17 prepend2">
						<label>CPF/CNPJ</label>
						<input type="text" class="text cpfCnpj disabled" name="resp[<%= count %>].CpfCnpj" value="<%= Model.Responsaveis[count].CpfCnpj %>" disabled="disabled" />
					</div>
					<div class="prepend2">
						<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar modoVisualizar" href="">Visualizar responsável</a>
					</div>
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
								<input type="text" class="text art disabled" name="resp[<%= count %>].Art" value="<%= Model.Responsaveis[count].NumeroArt %>" disabled="disabled" />
							</div>
						</div>
					</div>
					<a class="linkVejaMaisCampos"><span class="asmConteudoInternoExpander asmExpansivel">Clique aqui para ver mais detalhes</span></a>
				</div>
			</div>
		</div>
	<% } %>
	<% } else { %>
		<div class="asmItemContainer boxBranca borders">
			<div class="conteudoResponsaveis block prepend1">

				<div class="asmConteudoFixo block">
					<div class="coluna60">
						<label>Nome/Razão social</label>
						<input type="hidden" class="hdnIdRelacionamento" value="0" />
						<input type="hidden" class="hdnResponsavelId asmItemId" value="0" />
						<input type="text" class="text nomeRazao disabled" name="resp[0].NomeRazao" value="" disabled="disabled" />
					</div>
					<div class="coluna17 prepend2">
						<label>CPF/CNPJ</label>
						<input type="text" class="text cpfCnpj disabled" name="resp[0].CpfCnpj" value="" disabled="disabled" />
					</div>
				</div>

				<div class="asmConteudoLink hide">
					<div class="asmConteudoInterno">
						<div class="divInternoResponsavel block">
							<div class="coluna25">
								<label>Função *</label>
								<%= Html.DropDownList("resp[0].funcao", ViewModelHelper.SelecionarSelectList(Model.ResponsavelFuncoes, 0), new { @class = "text funcao disabled", @disabled = "disabled" })%>
							</div>
							<div class="coluna15 prepend2">
								<label>Número da ART *</label>
								<input type="text" class="text art disabled" name="resp[0].Art" value="" disabled="disabled" />
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