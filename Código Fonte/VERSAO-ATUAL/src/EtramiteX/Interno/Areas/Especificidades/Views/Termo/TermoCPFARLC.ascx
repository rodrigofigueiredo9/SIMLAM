<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TermoCPFARLCVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/termoCPFARLC.js") %>"></script>
<script>
	TermoCPFARLC.settings.urls.obterDadosTermoCPFARLC = '<%= Url.Action("ObterDadosTermoCPFARLC", "Termo", new { area = "Especificidades" }) %>';
	TermoCPFARLC.settings.urls.obterARL = '<%= Url.Action("ObterArl", "Termo", new { area = "Especificidades" }) %>';
	TermoCPFARLC.settings.urls.obterDadosEmpreendimentoReceptor = '<%= Url.Action("ObterDadosEmpreendimentoReceptor", "Termo", new { area = "Especificidades" }) %>';
	TermoCPFARLC.settings.urls.validarAdicionarARL = '<%= Url.Action("ValidarAdicionarARL", "Termo", new { area = "Especificidades" }) %>';

	TermoCPFARLC.settings.Mensagens = <%= Model.Mensagens %>;
	TermoCPFARLC.settings.dominialidadeID = <%= Model.Especificidade.DominialidadeID %>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>

	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<fieldset class="block boxBranca">
		<legend>Cedente</legend>

		<div class="block">
			<div class="coluna75">
				<label for="CedenteDominioID">Matrícula/Posse do cedente *</label>
				<%= Html.DropDownList("CedenteDominio", Model.CedenteDominios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.CedenteDominios.Count == 1, new { @class = "text ddlCedenteDominio" })) %>
			</div>
		</div>

		<%if (!Model.IsVisualizar)
	{ %>
		<div class="block">
			<div class="coluna75">
				<label for="CedenteARLCompensacao">ARL para compensação cedente *</label>
				<%= Html.DropDownList("CedenteARLCompensacao", Model.CedenteReservas, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.CedenteReservas.Count == 1, new { @class = "text ddlCedenteARLCompensacao" })) %>
			</div>
			<div class="coluna10">
				<button type="button" class="inlineBotao botaoAdicionarIcone btnAddCedenteARLCompensacao btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna75">
				<table class="dataGridTable dgReservasLegal" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th>Identificação da ARL</th>
							<th width="30%">Área Croqui (m²)</th>
							<%if (!Model.IsVisualizar)
		 { %><th width="9%">Ação</th>
							<% } %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.Especificidade.CedenteARLCompensacao)
		 { %>
						<tr>
							<td>
								<span class="Nome" title="<%:item.Identificacao%>"><%:item.Identificacao%></span>
							</td>
							<td>
								<span class="Area" title="<%:item.ARLCroqui.ToStringTrunc() %>"><%:item.ARLCroqui.ToStringTrunc() %></span>
							</td>
							<%if (!Model.IsVisualizar)
		 { %>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
							<% } %>
						</tr>
						<% } %>
						<% if (!Model.IsVisualizar)
		 { %>
						<tr class="trTemplateRow hide">
							<td><span class="Nome"></span></td>
							<td><span class="Area"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>

		<%if (!Model.IsVisualizar)
	{ %>
		<div class="block">
			<div class="coluna75">
				<label for="CedenteResponsaveisEmpreendimento">Responsável do empreendimento cedente *</label>
				<%= Html.DropDownList("CedenteResponsaveisEmpreendimento", Model.CedenteResponsaveisEmpreendimento, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.CedenteResponsaveisEmpreendimento.Count == 1, new { @class = "text ddlCedenteResposaveisEmp ddlResposaveisEmp" })) %>
			</div>
			<div class="coluna10">
				<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAddItem btnAddResponsavelEmp" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna75">
				<table class="dataGridTable dgCedenteResponsaveis dgResponsaveis" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th>Responsável do empreendimento cedente</th>
							<%if (!Model.IsVisualizar)
		 { %><th width="9%">Ação</th>
							<%} %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.Especificidade.CedenteResponsaveisEmpreendimento)
		 { %>
						<tr>
							<td>
								<span class="Nome" title="<%:item.NomeRazao%>"><%:item.NomeRazao%></span>
							</td>
							<%if (!Model.IsVisualizar)
		 { %>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
							<%} %>
						</tr>
						<% } %>
						<% if (!Model.IsVisualizar)
		 { %>
						<tr class="trTemplateRow hide">
							<td><span class="Nome"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>

	<fieldset class="block boxBranca">
		<legend>Receptor</legend>

		<div class="block">
			<div class="coluna75">
				<label for="ReceptorEmpreendimento">Empreendimento receptor *</label>
				<%= Html.DropDownList("ReceptorEmpreendimento", Model.ReceptorEmpreendimentos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ReceptorEmpreendimentos.Count == 1, new { @class = "text ddlReceptorEmpreendimentos" })) %>
			</div>
		</div>

		<div class="block">
			<div class="coluna75">
				<label for="ReceptorDominio">Matrícula/Posse receptora *</label>
				<%= Html.DropDownList("ReceptorDominio", Model.ReceptorDominios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ReceptorDominios.Count == 1, new { @class = "text ddlReceptorDominio" })) %>
			</div>
		</div>

		<%if (!Model.IsVisualizar)
	{ %>
		<div class="block">
			<div class="coluna75">
				<label for="ReceptorResponsaveisEmpreendimento">Responsável do empreendimento receptor *</label>
				<%= Html.DropDownList("ReceptorResponsaveisEmpreendimento", Model.ReceptorResponsaveisEmpreendimento, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ReceptorResponsaveisEmpreendimento.Count == 1, new { @class = "text ddlReceptorResposaveisEmp ddlResposaveisEmp" })) %>
			</div>
			<div class="coluna10">
				<button type="button" class="inlineBotao botaoAdicionarIcone btnAddItem btnAddResponsavelEmp" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna75">
				<table class="dataGridTable dgReceptorResponsaveis dgResponsaveis" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th>Responsável do empreendimento receptor</th>
							<%if (!Model.IsVisualizar)
		 { %><th width="9%">Ação</th>
							<% } %>
						</tr>
					</thead>
					<tbody>
						<% foreach (var item in Model.Especificidade.ReceptorResponsaveisEmpreendimento)
		 { %>
						<tr>
							<td>
								<span class="Nome" title="<%:item.NomeRazao%>"><%:item.NomeRazao%></span>
							</td>
							<%if (!Model.IsVisualizar)
		 {%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
							<%} %>
						</tr>
						<% } %>
						<% if (!Model.IsVisualizar)
		 { %>
						<tr class="trTemplateRow hide">
							<td><span class="Nome"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" />
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</fieldset>

<% if (Model.IsCondicionantes)
   { %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>