<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>

<div class="requerimentoPartial">

	<div class="divConteudoAtividadeSolicitada associarMultiplo">
		<fieldset class="block box fsAdicionarAtividade">
			<legend>Atividades Solicitada</legend>

			<div class="asmItens">
				<% foreach (var atividade in Model.AtividadesSolicitadasVM) { %>
					<div class="asmItemContainer" style="border:0px">
						<% atividade.IsRequerimento = !Model.IsRequerimentoProcesso; %>
						<% atividade.IsRequerimentoVisualizar = Model.IsVisualizar; %>
						<% Html.RenderPartial("~/Views/Shared/AtividadeSolicitada.ascx", atividade); %>
					</div>
				<% } %>
			</div>
		</fieldset>
	</div>

	<fieldset class="block box">
		<legend>Interessado</legend>
		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.Interessado.Id) %>" />
		<div class="coluna60">
			<label>Nome/Razão social *</label>
			<%= Html.TextBox("Processo.Interessado.NomeRazaoSocial", Model.Interessado.NomeRazaoSocial, new { @class = "text disabled txtIntNome", @disabled = "disabled" })%>
		</div>
		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Processo.Interessado.CPFCNPJ", Model.Interessado.CPFCNPJ, new { @class = "text disabled txtIntCnpj", @disabled = "disabled" })%>
		</div>
		<%--<div class="prepend2">
			<button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button>
		</div>--%>
	</fieldset>

	<fieldset class="block box">
		<legend>Responsável Técnico</legend>
		<% Html.RenderPartial("CriarRespTecnicoVisualizar"); %>
	</fieldset>

	<fieldset class="block box">
		<legend>Empreendimento</legend>
		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(Model.Empreendimento.Id) %>" />
		<div class="coluna60">
			<label class="lblDenominador">Denominação</label>
			<%= Html.TextBox("Processo.Empreendimento.Denominador", Model.Empreendimento.Denominador, new { @class = "text disabled txtEmpDenominador", @disabled = "disabled" })%>
		</div>
		<div class="coluna16 prepend2">
			<label>CNPJ</label>
			<%= Html.TextBox("Processo.Empreendimento.CNPJ", Model.Empreendimento.CNPJ, new { @class = "text disabled tctEmpCnpj", @disabled = "disabled" })%>
		</div>
		<%--<div class="prepend2">
			<span class="spanBtnEditarEmp <%= (Model.Empreendimento.Id > 0) ? "" : "hide" %>">
				<button type="button" class="icone visualizar esquerda inlineBotao btnEditarEmp" title="Visualizar empreendimento"></button>
			</span>
		</div>--%>
	</fieldset>
</div>