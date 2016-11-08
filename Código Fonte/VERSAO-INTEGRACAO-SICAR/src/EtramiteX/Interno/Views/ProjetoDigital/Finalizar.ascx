<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalVM>" %>

<div class="modalFinalizar">
	<fieldset class="block box">
		<div class="coluna21">
			<label>Número *</label>
			<%= Html.TextBox("Requerimento.Numero", Model.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna15 prepend2">
			<label>Data de criação *</label>
			<%= Html.TextBox("Requerimento.DataCadastro", Model.DataCriacao, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna25 prepend2">
			<label>Situação do requerimento *</label>
			<input type="hidden" class="requerimentoSituacao" value="<%= Model.SituacaoId %>" />
			<%= Html.TextBox("Requerimento.SituacaoTexto", Model.SituacaoTexto, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna22 prepend2">
			<label >Agendamento de vistoria ? *</label>
			<%= Html.DropDownList("AgendamentoVistoriaId", Model.AgendamentoVistoria, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
	</fieldset>

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
				<table class="tabRoteiros dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
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

	<fieldset class="block box">
		<legend>Interessado</legend>
		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.Interessado.Id) %>" />
		<div class="coluna75">
			<label>Nome/Razão Social</label>
			<%= Html.TextBox("Requerimento.Pessoa.NomeRazaoSocial", Model.Interessado.NomeRazaoSocial, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna20 prepend2">
			<label>CPF/CNPJ</label>
			<%= Html.TextBox("Requerimento.Pessoa.CPFCNPJ", Model.Interessado.CPFCNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Responsável Técnico</legend>
		<% Html.RenderPartial("ResponsavelTecnico"); %>
	</fieldset>

	<fieldset class="block box">
		<legend>Empreendimento</legend>

		<% if(Model.Empreendimento.Id > 0) { %>
		<div class="coluna75">
			<label class="lblDenominador">Denominação</label>
			<%= Html.TextBox("Requerimento.Empreendimento.Denominador", Model.Empreendimento.Denominador, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna20 prepend2">
			<label>CNPJ</label>
			<%= Html.TextBox("Requerimento.Empreendimento.CNPJ", Model.Empreendimento.CNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<% } else { %>
		<label>Não existe empreendimento associado a este requerimento.</label>
		<% } %>
	</fieldset>
</div>