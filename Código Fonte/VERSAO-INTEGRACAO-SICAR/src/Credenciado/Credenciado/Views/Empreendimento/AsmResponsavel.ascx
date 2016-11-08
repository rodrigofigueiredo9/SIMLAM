<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResponsavelVM>" %>

<p class="block"><a title="Remover responsável" class="btnAsmExcluir">Remover responsável</a></p>
<div class="asmConteudoFixo block divResponsavel prepend1" id="Empreendimento.Responsaveis[i]">
	<div class="block">
		<input type="hidden" class="hdnResponsavelId asmItemId" name="Id" value="<%= Html.Encode(Model.Responsavel.Id) %>" />
		<input type="hidden" class="hdnResponsavelTid asmItemTid" name="Tid" value="<%= Html.Encode(Model.Responsavel.Tid) %>" />
		<input type="hidden" class="hdnResponsavelInternoId asmItemInternoId" name="InternoId" value="<%= Html.Encode(Model.Responsavel.InternoId) %>" />
		<%= Html.Hidden("IsCopiado", Model.Responsavel.IsCopiado, new { @class = "hdnIsCopiado" })%>
		<input type="hidden" class="hdnResponsavelIdRelacionamento" name="IdRelacionamento" value="<%= Html.Encode(Model.Responsavel.IdRelacionamento) %>" />

		<% if (!string.IsNullOrEmpty(Model.Responsavel.OrigemTexto)) { %><div class="statusResponsavel"><%: Model.Responsavel.OrigemTexto %></div><% } %>
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
			<a title="Visualizar responsável" class="icone visualizar esquerda inlineBotao btnAsmEditar <%= ((Model.Responsavel.Id.GetValueOrDefault() > 0 || Model.Responsavel.InternoId > 0) ? "" : "hide") %>" href="">Visualizar Responsável</a>
		</div>
	</div>

	<div class="block">
		<div class="coluna35">
			<label for="Empreendimento.Tipo">Tipo *</label>
			<%= Html.DropDownList("Empreendimento.Responsaveis[i].Tipo", Model.TiposResponsavel, new { @class = "text ddlTipoResponsavel" })%>
		</div>
		<div class="coluna22 prepend2 divDatavencimento <%= Model.Responsavel.Tipo != 3 ? "hide" : "" %>">
			<label for="Empreendimento.DataVencimentoTexto">Data de vencimento *</label>
			<input type="text" class="text maskData txtDataVencResponsavel" name="Empreendimento.Responsaveis[i].DataVencimentoTexto" value="<%= Html.Encode(Model.Responsavel.DataVencimentoTexto) %>" />
		</div>

		<div class="coluna42 prepend2 divEspecificar <%= Model.Responsavel.Tipo != 9 ? "hide" : "" %>">
			<label for="Empreendimento.EspecificarTexto">Especificar *</label>
			<input type="text" class="text txtEspecificarTexto" maxlength="100" name="Empreendimento.Responsaveis[i].EspecificarTexto" value="<%= Html.Encode(Model.Responsavel.EspecificarTexto) %>" />
		</div>
	</div>
</div>