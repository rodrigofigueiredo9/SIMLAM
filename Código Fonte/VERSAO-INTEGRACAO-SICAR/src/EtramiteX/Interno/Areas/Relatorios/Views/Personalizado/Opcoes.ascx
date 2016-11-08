<%@ Import Namespace="Tecnomapas.Blocos.RelatorioPersonalizado.Entities" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<!-- ============================================================================================ -->
<div class="block borderB padding0">
	<div class="coluna35">
		<p>
			<label for="ConfiguracaoRelatorio_FonteDados_Id">Tipo do Relatório</label>
			<%= Html.DropDownList("ConfiguracaoRelatorio.FonteDados.Id", Model.FonteDadosLst, new { @class = "text setarFoco ddlFatos" })%>
		</p>
	</div>
</div>

<!-- ACORDION ================================================================ -->
<% if(Model.ConfiguracaoRelatorio.FonteDados.Id > 0) { %>
<div class="block margemDTop">
	<h5>Selecione os campos da base</h5>
	<div id="accordion" class="coluna100">

			<h3><a href="#"><%: Model.ConfiguracaoRelatorio.FonteDados.Nome %></a> <span class="invertkAll">Inverter Marcação</span><span class="uncheckAll">Desmarcar Todos</span><span class="checkAll">Marcar Todos</span></h3>
			<div class="block coluna100">
				<% foreach (var campo in Model.ConfiguracaoRelatorio.FonteDados.CamposFatoExibicao) { %>
				<% var selecionado = Model.ConfiguracaoRelatorio.CamposSelecionados.SingleOrDefault(x => x.Campo != null && x.Campo.Id == campo.Id); %>
				<span>
					<label class="labelBig"><input type="checkbox" class="cbCampo" <%= selecionado != null ? "checked=\"checked\"" : "" %> /><span><%: campo.Alias %></span></label>
					<% selecionado = selecionado ?? new ConfiguracaoCampo(); %>
					<% selecionado.Campo = campo; %>
					<input type="hidden" class="hdnCampoJSON" value="<%: ViewModelHelper.Json(selecionado) %>" />
				</span>
				<% } %>
			</div>

			<% foreach (var dimensao in Model.ConfiguracaoRelatorio.FonteDados.Dimensoes) { %>
				<h3><a href="#"><%: dimensao.Nome %></a> <span class="invertkAll">Inverter Marcação</span><span class="uncheckAll">Desmarcar Todos</span><span class="checkAll">Marcar Todos</span></h3>
				<div class="block coluna100">
					<% foreach (var campo in dimensao.CamposExibicao) { %>
					<% var selecionado = Model.ConfiguracaoRelatorio.CamposSelecionados.SingleOrDefault(x => x.Campo != null && x.Campo.Id == campo.Id); %>
					<span>
						<label class="labelBig"><input type="checkbox" class="cbCampo" <%= selecionado != null ? "checked=\"checked\"" : "" %> /><span><%: campo.Alias %></span></label>
						<% selecionado = selecionado ?? new ConfiguracaoCampo(); %>
						<% selecionado.Campo = campo; %>
						<input type="hidden" class="hdnCampoJSON" value="<%: ViewModelHelper.Json(selecionado) %>" />
					</span>
					<% } %>
				</div>
			<% } %>
	</div>
</div>
<% } %>
<!-- ========================================================================= -->