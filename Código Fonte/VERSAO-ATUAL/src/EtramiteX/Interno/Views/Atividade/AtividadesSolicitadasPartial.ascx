<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProcesso" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarAtividadesSolicitadasVM>" %>

<script>
	$(function () {
		AtividadesSolicitadas.urlEncerrarAtividade = '<%= Url.Action("EncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlMotivoEncerrarAtividade = '<%= Url.Action("MotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlSalvarMotivoEncerrarAtividade = '<%= Url.Action("SalvarMotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlVisualizarEncerrarAtividade = '<%= Url.Action("VisualizarMotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlVisualizarPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
	});
</script>

<div class="modalAtividadesSolicitadas">
	<h1 class="titTela">Atividade Solicitada</h1>
	<br />

	<fieldset class="block box coluna95">
		<legend><%= Model.TituloTela %></legend>
		<div class="block">
			<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(Model.ProtocoloId) %>" />
			<input type="hidden" class="hdnProtocoloTipo" value="<%= Html.Encode(Model.IsProcesso) %>" />

			<div class="coluna25">
				<label for="Processo.Numero">Nº de Registro do <%= Model.IsProcesso ? "Processo" : "Documento" %> *</label>
				<%= Html.TextBox("Numero", Model.Numero, new { @class = "text txtProcessoNumero disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend2">
				<label for="Processo.Tipo.Id">Tipo *</label>
				<%= Html.DropDownList("Tipo.Id", Model.Tipos , new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box coluna95">
		<legend>Requerimento Padrão</legend>
		<div class="asmItens">
			<% if (Model.Protocolo != null) { %>
				<% if (Model.Protocolo.Requerimento.Id > 0) { %>
					<div class="asmItemContainer boxBranca borders">
						<% Html.RenderPartial("~/Views/Atividade/AsmAtividadesSolicitadas.ascx", new ListarAtividadesSolicitadasVME(Model.Protocolo, Model.IsEncerrar)); %>
					</div>
				<% } %>

				<% if (Model.Protocolo.IsProcesso) { %>
					<% foreach (Processo processo in ((Processo)Model.Protocolo).Processos) { %>
						<div class="asmItemContainer boxBranca borders">
							<% Html.RenderPartial("~/Views/Atividade/AsmAtividadesSolicitadas.ascx", new ListarAtividadesSolicitadasVME(processo, Model.IsEncerrar)); %>
						</div>
					<% } %>

					<% foreach (Documento documento in ((Processo)Model.Protocolo).Documentos) { %>
						<div class="asmItemContainer boxBranca borders">
							<% Html.RenderPartial("~/Views/Atividade/AsmAtividadesSolicitadas.ascx", new ListarAtividadesSolicitadasVME(documento, Model.IsEncerrar)); %>
						</div>
					<% } %>
				<% } %>
			<% } %>
		</div>
	</fieldset>
</div>