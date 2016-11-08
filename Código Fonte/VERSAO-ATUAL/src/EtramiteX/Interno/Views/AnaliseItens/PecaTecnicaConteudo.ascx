<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PecaTecnicaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>



<input type="hidden" class="hdnPecaTecnicaId" value="<%: Model.PecaTecnica.Id %>" />


<fieldset class="block box fsResponsaveis">
	<legend>Dados do Imóvel</legend>
	<div class="block">
		<div class="coluna90">
			<label>Empreendimento</label>
			<%: Html.TextBox("EmpreendimentoNome", Model.PecaTecnica.Protocolo.Empreendimento.Denominador, new { @class = "text txtEmpreendimentoNome campo disabled", @disabled = "disabled" })%>
			<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.PecaTecnica.Protocolo.Empreendimento.Id %>" />
		</div>
		<div class="coluna70">
			<label>Responsável do empreendimento *</label>
			<%: Html.DropDownList("PecaTecnica.RespEmpreendimento", Model.RespEmpreendimento, ViewModelHelper.SetaDisabled(false, new { @class = "text campo ddlRespEmpreendimento" }))%>
		</div>

		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarResponsavel btnAddItem" title="Adicionar">+</button>
		</div>
	</div>

	<div class="block coluna70 dataGrid">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Responsável do empreendimento</th>
					<th width="9%">Ação</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var responsavel in Model.PecaTecnica.ResponsaveisEmpreendimento){ %>
				<tr>
					<td>
						<span class="ResponsavelNome" title="<%:responsavel.NomeRazao%>"><%:responsavel.NomeRazao%></span>
					</td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(responsavel)%>' />
						<input title="Excluir" type="button" class="icone excluir btnExcluirResponsavel" value="" />
					</td>
				</tr>
				<% } %>
				<tr class="trTemplateRow hide">
					<td><span class="ResponsavelNome"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input title="Excluir" type="button" class="icone excluir btnExcluirResponsavel" value="" />
					</td>
				</tr>
			</tbody>
		</table>


</fieldset>

<fieldset class="block box">
	<legend>Elaborador</legend>
	<div class="block">
		<div class="coluna35 radioResponsavel"> 
			<% string checado = Model.PecaTecnica.ElaboradorTipoEnum == eElaboradorTipo.TecnicoIdaf ? "checked='checked'" : "";%>
			<label><input type="radio" name="PecaTecnica.ElaboradorTipo" value = "<%= (int)eElaboradorTipo.TecnicoIdaf %>" class="radioResponsavelElaborador campo tecIDAF" <%:checado%>/> Técnico IDAF </label> 

			<% checado = Model.PecaTecnica.ElaboradorTipoEnum == eElaboradorTipo.TecnicoTerceirizado ? "checked='checked'" : "";%>
			<label> <input type="radio" name="PecaTecnica.ElaboradorTipo" value = "<%= (int)eElaboradorTipo.TecnicoTerceirizado %>" class="radioResponsavelElaborador campo tecTerceirizado" <%:checado%> /> Técnico Terceirizado </label>
		</div>

		<div class="coluna54 divSetores <%: Model.PecaTecnica.ElaboradorTipoEnum == eElaboradorTipo.TecnicoIdaf ? "" : "hide" %>">
			<label>Setor de cadastro *</label>
			<%: Html.DropDownList("PecaTecnica.SetorCadastro", Model.Setores, ViewModelHelper.SetaDisabled(Model.Setores.Count == 1, new { @class = "text campo ddlSetores " }))%>
		</div>
	</div>	

	<div class="block">
		<div class="coluna90">
			<label>Nome</label>
			<%: Html.DropDownList("PecaTecnica.Elaborador", Model.Elaboradores, ViewModelHelper.SetaDisabled(Model.Elaboradores.Count == 1, new { @class = "text campo ddlElaborador" }))%>
		</div>
	</div>
</fieldset>
