<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTitulo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteSituacaoAlterarVM>" %>

<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteVisualizar.js") %>" ></script>
<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteProrrogar.js") %>" ></script>
<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteAtender.js") %>" ></script>

<script>
	$(function () {
		CondicionanteSituacaoAlterar.load($('#central'), {
			urls: {
				visualizar: '<%= Url.Action("CondicionanteVisualizar", "Titulo") %>',
				prorrogar: '<%= Url.Action("CondicionanteProrrogar", "Titulo") %>',
				atender: '<%= Url.Action("CondicionanteAtender", "Titulo") %>'
			}
		});
	});
</script>

<h2 class="titTela">Alterar Situação da Condicionante</h2>
<br />

<div class="condicionantesPartial">
	<input type="hidden" class="hdnTituloId" value="<%= Model.Titulo.Id %>" />

	<fieldset class="block box">
		<legend>Título</legend>
		<div class="block">
			<div class="coluna30">
				<label>Número *</label>
				<%= Html.TextBox("Titulo.Numero.Texto", null, new { @class = "text disabled txtTituloNumero", @disabled = "disabled" })%>
			</div>
			<div class="coluna60 prepend2">
				<label>Modelo *</label>
				<%= Html.TextBox("Titulo.Modelo.Nome", null, new { @class = "text disabled txtTituloModelo", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Condicionantes</legend>

		<table class="dataGridTable tabItens" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th> Descrição</th>
					<th width="10%">Situação</th>
					<th width="10%">Vencimento</th>
					<th width="13%">Ações</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (TituloCondicionante item in Model.Titulo.Condicionantes) { %>

					<% if (item.Periodicidades == null || item.Periodicidades.Count == 0 ) { %>
						<tr class="trItem">
							<td><span title="<%= Html.Encode(item.Descricao) %>" class="CondDesc"> <%= Html.Encode(item.Descricao)%></span></td>
							<td><span title="<%= Html.Encode(item.Situacao.Texto) %>" class="CondSituacao"> <%= Html.Encode(item.Situacao.Texto)%></span></td>
							<td><span title="<%= Html.Encode(item.DataVencimento.DataTexto) %>" class="CondVencimento"> <%= Html.Encode(item.DataVencimento.DataTexto)%></span></td>
							<td>
								<input type="hidden" class="hdnCondicionanteId" value= "<%= Html.Encode(item.Id) %>" />
								<input type="hidden" class="hdnCondicionantePeriodicidadeId" value= "0" />
								<input title="Visualizar" type="button" class="icone visualizar btnVisualizarCond" />
								<input title="Atender" type="button" class="icone recebido bntAtenderCond" />
								<input title="Prorrogar" type="button" class="icone historico btnProrrogarCond" />
							</td>
						</tr>
					<%  } %>

					<% foreach (TituloCondicionantePeriodicidade period in item.Periodicidades.OrderBy(x => x.DataVencimento.Data)) { %>

					<tr class="trItem">
						<td><span title="<%= Html.Encode(item.Descricao) %>" class="CondDesc"> <%= Html.Encode(item.Descricao)%></span></td>
						<td><span title="<%= Html.Encode(period.Situacao.Texto) %>" class="CondSituacao"> <%= Html.Encode(period.Situacao.Texto)%></span></td>
						<td><span title="<%= Html.Encode(period.DataVencimento.DataTexto) %>" class="CondVencimento"> <%= Html.Encode(period.DataVencimento.DataTexto)%></span></td>
						<td>
							<input type="hidden" class="hdnCondicionanteId" value= "<%= Html.Encode(item.Id) %>" />
							<input type="hidden" class="hdnCondicionantePeriodicidadeId" value= "<%= Html.Encode(period.Id) %>" />
							<input title="Visualizar" type="button" class="icone visualizar btnVisualizarCond" />
							<input title="Atender" type="button" class="icone recebido bntAtenderCond" />
							<input title="Prorrogar" type="button" class="icone historico btnProrrogarCond" />
						</td>
					</tr>

					<%  } %>

				<%  } %>
			</tbody>
		</table>

	</fieldset>
</div>

<div class="block box btnContainer hide">
	<input class="btnSalvar floatLeft" type="button" value="Salvar" />
	<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
</div>