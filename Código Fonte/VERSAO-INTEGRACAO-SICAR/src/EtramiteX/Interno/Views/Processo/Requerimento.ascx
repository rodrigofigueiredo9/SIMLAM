<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RequerimentoVM>" %>

<div class="requerimentoPartial">
	<input type="hidden" class="hdnProtocoloId" value="<%= Model.ProtocoloId %>" />
	<input type="hidden" class="hdnProtocoloTipo" value="<%= Model.ProtocoloTipo %>" />
	<div class="divConteudoAtividadeSolicitada associarMultiplo">
		<fieldset class="block box fsAdicionarAtividade">
			<legend>Atividade Solicitada</legend>

			<div class="asmItens">
				<% foreach (var atividade in Model.AtividadesSolicitadasVM) { %>
					<div class="asmItemContainer" style="border:0px">
						<% atividade.IsRequerimento = !Model.IsRequerimentoProcesso; %>
						<% atividade.IsRequerimentoVisualizar = !Model.IsEditar; %>
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
			<%= Html.TextBox("Interessado.NomeRazaoSocial", Model.Interessado.NomeRazaoSocial, new { @class = "text disabled txtIntNome", @disabled = "disabled" })%>
		</div>
		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Interessado.CPFCNPJ", Model.Interessado.CPFCNPJ, new { @class = "text disabled txtIntCnpj", @disabled = "disabled" })%>
		</div>
		<div class="prepend2">
			<% if (Model.IsEditar) { %>
			<button type="button" title="Buscar interessado" class="floatLeft inlineBotao botaoBuscar btnAssociarInteressado">Buscar</button>
			<% } %>
			<button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Responsável Técnico</legend>
		<% if(Model.IsEditar) { %>
			<% Html.RenderPartial("CriarRespTecnico"); %>
		<% } else { %>
			<% Html.RenderPartial("CriarRespTecnicoVisualizar"); %>
		<% } %>
	</fieldset>

	<fieldset class="block box">
		<legend>Empreendimento</legend>
		<input type="hidden" class="hdnEmpreendimentoId" value="<%= Html.Encode(Model.Empreendimento.Id) %>" />
		<div class="coluna60">
			<label class="lblDenominador">Denominação</label>
			<%= Html.TextBox("Empreendimento.Denominador", Model.Empreendimento.Denominador, new { @class = "text disabled txtEmpDenominador", @disabled = "disabled" })%>
		</div>
		<div class="coluna16 prepend2">
			<label>CNPJ</label>
			<%= Html.TextBox("Empreendimento.CNPJ", Model.Empreendimento.CNPJ, new { @class = "text disabled tctEmpCnpj", @disabled = "disabled" })%>
		</div>
		<div class="prepend2">
			<% if (Model.IsEditar) { %>
			<span class="spanBtnAssociarEmp <%= (Model.Empreendimento.Id > 0) ? "hide" : "" %>">
				<button type="button" title="Buscar empreendimento" class="floatLeft inlineBotao botaoBuscar btnAssociarEmp">Buscar</button>
			</span>
			<% } %>
			<span class="spanBtnEditarEmp <%= (Model.Empreendimento.Id > 0) ? "" : "hide" %>">
				<% if (Model.IsEditar) { %><button type="button" title="Limpar empreendimento" class="floatLeft inlineBotao btnLimparEmp">Limpar</button><% } %>
				<button type="button" class="icone visualizar esquerda inlineBotao btnEditarEmp" title="Visualizar empreendimento"></button>
			</span>
		</div>
	</fieldset>
</div>