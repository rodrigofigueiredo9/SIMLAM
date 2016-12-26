<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TermoAprovacaoMedicaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/termoAprovacaoMedicao.js") %>"></script>

<script>
	TermoAprovacaoMedicao.settings.urls.urlObterDadosTermoAprovacaoMedicao = '<%= Url.Action("ObterDadosTermoAprovacaoMedicao", "Termo", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box fdsEspecificidade">
	<legend>Especificidade</legend>

	<div class="block">
		<div class="coluna75">
			<label for="Termo_Destinatario">Destinatário *</label>
			<%= Html.DropDownList("Termo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text ddlDestinatarios", @id = "ddlDestinatarios" }))%>
		</div>
		<div class="coluna15 prepend2">
			<label for="Termo_DataMedicao_DataTexto">Data de Medição *</label><br />
			<%= Html.TextBox("Termo.DataMedicao.DataTexto", Model.Termo.DataMedicao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "textarea text maskData txtDataMedicao", @id = "txtDataMedicao" }))%>
		</div>
	</div>

	<fieldset class="block box">
		<legend>Responsável pela Medição</legend>
		<div class="block">
			<div class="coluna30 divTipoResp">
				<label><%= Html.RadioButton("Tipo", 1, Model.Termo.TipoResponsavel == 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "radio rdbTecIDAF" }))%> Técnico IDAF</label>
				<label class="append5"><%= Html.RadioButton("Tipo", 2, Model.Termo.TipoResponsavel == 2, ViewModelHelper.SetaDisabled(Model.IsVisualizar,new { @class = "radio rdbTecTercerizado" }))%> Técnico tercerizado</label>
			</div>
			<div class="coluna50">
				<span class="<%= Model.Termo.TipoResponsavel == 2 ? "" : "hide " %>spanTecnico"><%= Html.DropDownList("Termo.Termo.ResponsavelMedicao", Model.Tecnicos, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Tecnicos.Count <= 2, new { @class = "text ddlTecnicos" }))%></span>
				<span class="<%= Model.Termo.TipoResponsavel == 1 ? "" : "hide " %>spanFuncionario"><%= Html.DropDownList("Termo.Termo.Funcionario", Model.Funcionario, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlFuncionario" }))%></span>
			</div>
		</div>
		<div class="block divSetorCadastro <%= Model.Termo.TipoResponsavel == 1 ? "" : "hide " %>">
			<div class="coluna55">
				<label for="Termo_SetorCadastro">Setor de Cadastro *</label>
				<%= Html.DropDownList("Termo.SetorCadastro", Model.Setores, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Setores.Count <= 2, new { @class = "text ddlSetoresUsuario", @id = "ddlSetoresUsuario" }))%>
			</div>
		</div>
	</fieldset>

</fieldset>