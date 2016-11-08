<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalVM>" %>


<div class="box block">
	<div class="coluna21">
		<label>Número *</label>
		<%= Html.TextBox("Numero", (Model.Numero == 0 ? "Gerado Automaticamente" : Model.Numero.ToString()), new { @disabled = "disabled", @maxlength = "15", @class = "text Numero disabled" })%>
	</div>
	<div class="coluna15 prepend2">
		<label>Data de criação *</label>
		<%= Html.TextBox("dataCriacao", (Model.DataCriacao == string.Empty ? DateTime.Now.ToShortDateString() : Model.DataCriacao), new { @disabled = "disabled", @class = "text datepicker dataCriacao maskData disabled" })%>
	</div>
	<div class="coluna25 prepend2">
		<label >Agendamento de vistoria ? *</label>
		<%= Html.DropDownList("AgendamentoVistoriaId", Model.AgendamentoVistoria, new { @disabled = "disabled", @class = "text disabled" })%>
	</div>
	<div class="coluna70 <%= (Model.Setores.Count <= 1 || Model.SetorId > 0 || Model.IsVisualizar) ? "hide" : ""%>">
		<label >Setor *</label>
		<%= Html.DropDownList("SetorId", Model.Setores, new { @class = "text ddlSetores setarFoco" })%>
	</div>
</div>

<div class="divConteudoAtividadeSolicitada associarMultiplo">
	<fieldset class="block box fsAdicionarAtividade">
		<legend>Atividade Solicitada</legend>
	<div class="asmItens">
			<% foreach (var atividade in Model.AtividadesSolicitadasVM) { %>
				<div class="asmItemContainer" style="border:0px">
					<% Html.RenderPartial("AtividadeSolicitadaVisualizar", atividade); %>
				</div>
			<% } %>
	</div>
	</fieldset>
</div>

<fieldset class="block box" id="Roteiro_Container">
		<legend>Roteiro Orientativo</legend>
	<div class="block">
		<div class="dataGrid">
			<table class="tabRoteiros dataGridTable" width="100%" border="0" cellspacing="0"
				cellpadding="0">
				<thead>
					<tr >
						<th width="10%">Número</th>
						<th width="10%">Versão</th>
						<th>Nome</th>
						<th>Atividade Solicitada</th>
						<th width="7%">Ações</th>
					</tr>
					</thead>
					<tbody>
						<% 
						foreach (var item in Model.Roteiros)
						{ %>
						<tr>
							<td>
								<span class="trRoteiroNumero" title="<%= Html.Encode(item.Numero) %>"><%= Html.Encode(item.Numero) %></span>
							</td>
							<td>
								<span class="trRoteiroVersao" title="<%= Html.Encode(item.VersaoAtual) %>"><%= Html.Encode(item.VersaoAtual) %></span>
							</td>
							<td>
								<span class="trRoteiroNome" title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome) %></span>
							</td>
							<td>
								<span class="trRoteiroAtividade" title="<%= Html.Encode(item.AtividadeTexto) %>"><%= Html.Encode(item.AtividadeTexto) %></span>
							</td>
							<td>
								<input type="hidden" class="hdnRoteiroId" value="<%: item.Id %>" />
								<input type="hidden" class="hdnTidRoteiro" value="<%= item.Tid %>" />
								<input title="PDF" type="button" class="icone pdf btnRoteiroPdf" value="" />
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>

	<div class="box block">
		<div class="coluna100">
			<label>Informações Complementares</label>
			<%= Html.TextArea("InformacaoComplementar", Model.InformacaoComplementar, new { @disabled = "disabled", @class = "disabled bloquear", @style = "height: 80px;" })%>
		</div>
	</div>

<table class="hide">
	<tbody>
		<tr class="trRoteiroTemplate">
			<td>
				<span class="trRoteiroNumero">NÚMERO</span>
			</td>
			<td>
				<span class="trRoteiroVersao">VERSÃO</span>
			</td>
			<td>
				<span class="trRoteiroNome">NOME</span>
			</td>
			<td>
				<span class="trRoteiroAtividade">Atividade</span>
			</td>
			<td>
				<input type="hidden" class="hdnTidRoteiro" value="0" />
				<input type="hidden" class="hdnRoteiroId" value="0" />
				<input title="PDF" type="button" class="icone pdf btnRoteiroPdf modoVisualizar" value="" />
			</td>
		</tr>
	</tbody>
</table>
