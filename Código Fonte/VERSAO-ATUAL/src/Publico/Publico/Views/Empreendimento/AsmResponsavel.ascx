<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResponsavelVM>" %>

<p class="block"><a title="Remover responsável" class="btnAsmExcluir">Remover responsável</a></p>
<div class="asmConteudoFixo block divResponsavel prepend1" id="Empreendimento.Responsaveis[i]">
	<div class="block">
			<input type="hidden" class="hdnResponsavelId asmItemId" name="Id" value="<%= Html.Encode(Model.Responsavel.Id) %>" />
			<input type="hidden" class="hdnResponsavelIdRelacionamento" name="IdRelacionamento" value="<%= Html.Encode(Model.Responsavel.IdRelacionamento) %>" />
		<div class="coluna60">
			<label for="Empreendimento.NomeRazao">Nome/Razão social *</label>
			<input type="text" class="txtNomeResponsavel asmItemTexto text disabled" disabled="disabled" name="Empreendimento.Responsaveis[i].NomeRazao" value="<%= Html.Encode(Model.Responsavel.NomeRazao) %>" />
		</div>
		<div class="coluna17 prepend2">
			<label for="Empreendimento.CpfCnpj">CPF/CNPJ *</label>
			<input type="text" class="text disabled txtCnpjResponsavel" disabled="disabled" name="Empreendimento.Responsaveis[i].CpfCnpj" value="<%= Html.Encode(Model.Responsavel.CpfCnpj) %>" />
		</div>
		<div class="prepend2">
			<button type="button" title="Buscar responsável" class="floatLeft inlineBotao botaoBuscar btnAsmAssociar">Buscar</button>
			<%--<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar <%= (Model.Responsavel.Id.HasValue && Model.Responsavel.Id > 0 ? "" : "hide") %>" href="">Visualizar Responsável</a>--%>
		</div>
	</div>
</div>
<div class="asmConteudoLink">
	<div class="asmConteudoInterno prepend1">
		<div class="block">
			<div class="coluna35">
				<label for="Empreendimento.Tipo">Tipo *</label>
				<%= Html.DropDownList("Empreendimento.Responsaveis[i].Tipo", Model.TiposResponsavel, new { @class = "text ddlTipoResponsavel" })%>
			</div>
			<div class="coluna22 prepend2 divDatavencimento <%= Model.Responsavel.Tipo != 3 ? "hide" : "" %>">
				<label for="Empreendimento.DataVencimentoTexto">Data de vencimento *</label>
				<input type="text" class="text maskData txtDataVencResponsavel" name="Empreendimento.Responsaveis[i].DataVencimentoTexto" value="<%= Html.Encode(Model.Responsavel.DataVencimentoTexto) %>" />
			</div>
		</div>
	</div>
	<a class="linkVejaMaisCampos <%= Model.Responsavel.Tipo.GetValueOrDefault() <= 0 ? "ativo" : string.Empty %>"><span class="asmConteudoInternoExpander prepend1 asmExpansivel">Clique aqui para ocultar detalhes</span></a>
</div>