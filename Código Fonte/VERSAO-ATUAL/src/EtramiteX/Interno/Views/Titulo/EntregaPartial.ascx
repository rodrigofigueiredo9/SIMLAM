<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EntregaVM>" %>

<fieldset class="block box">
	<legend>Processo/Documento</legend>
	<div class="coluna15">
		<label>Nº de registro *</label>
		<%= Html.TextBox("Entrega.ProcessoNumero", null, new { @maxlength = "12", @class = "text setarFoco txtProcessoNumero" })%>
	</div>

	<span class="containerVerificarProc">
		<button type="button" class="inlineBotao btnVerificarProc">Verificar</button>
	</span>

	<span class="containerLimparProc hide">
		<button type="button" class="inlineBotao btnLimparProc">Limpar</button>
	</span>
</fieldset>

<div class="divConteudoEntrega hide">
	<fieldset class="block box">
		<legend>Titulo</legend>

		<input type="hidden" class="hdnProcDocId" value="0" />
		<input type="hidden" class="hdnProcDocIsProcesso" value="" />

		<div class="block dataGrid">
			<table class="dataGridTable tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%"><input type="checkbox" class="chkMarcarTodos"/>Número</th>
						<th>Modelo</th>
						<th width="15%">Situação</th>
						<th width="8%">Ações</th>
					</tr>
				</thead>
				<tbody>
				</tbody>
			</table>
			<table style="display: none">
				<tbody>
					<tr class="trItem trItemTemplate">
						<td>
							<input type="checkbox" class="chkTitulo"/>
							<span class="tituloNumero"></span>
						</td>
						<td><span class="tituloModelo"></span></td>
						<td><span class="tituloSituacao"></span></td>
						<td>
							<input type="hidden" class="hdnTituloId" />
							<input title="PDF" class="icone pdf btnPdfTitulo" type="button" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Dados da Entrega</legend>
		<div class="block">
			<div class="coluna15">
				<label for="Entrega_DataEntrega">Data de entrega *</label>
				<%= Html.TextBox("Entrega.DataEntrega", Model.Entrega.DataEntrega.DataTexto, new { @class = "text setarFoco txtDataEntrega maskData" })%>
			</div>
		</div>

		<div class="block">
			<input type="hidden" class="hdnPessoaId" />

			<div class="coluna15 preprnd2">
				<label for="Entrega_CPF">CPF *</label>
				<%= Html.TextBox("Entrega.CPF", Model.Entrega.CPF, new { @class = "text setarFoco txtCpf maskCpf" })%>
			</div>

			<span class="containerVerificar"><button type="button" class="inlineBotao btnVerificarCpf">Verificar</button></span>
			<span class="containerLimpar hide"><button type="button" class="inlineBotao btnLimparCpf">Limpar</button></span>
		</div>

		<div class="block">
			<div class="ultima">
				<label for="Entrega_Nome">Nome *</label>
				<%= Html.TextBox("Entrega.Nome", Model.Entrega.Nome, new { @class = "text txtNome disabled", @disabled = "disabled", @maxlength = "80" })%>
			</div>
		</div>
	</fieldset>
</div>