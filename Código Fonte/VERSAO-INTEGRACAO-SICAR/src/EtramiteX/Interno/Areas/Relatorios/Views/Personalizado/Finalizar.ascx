<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<h5>Salvar Relatório</h5>

<div class="block">
	<div class="coluna95">
		<p>
			<label for="Nome">Nome *</label>
			<%= Html.TextBox("Nome", Model.ConfiguracaoRelatorio.Nome, new { @class = "text txtNome", maxlength = "80" })%>
		</p>

		<p>
			<label for="Descricao">Descrição *</label><br />
			<%= Html.TextArea("Descricao", Model.ConfiguracaoRelatorio.Descricao, new { @class = "text txtDescricao", maxlength = "500" })%>
		</p>
	</div>
</div>