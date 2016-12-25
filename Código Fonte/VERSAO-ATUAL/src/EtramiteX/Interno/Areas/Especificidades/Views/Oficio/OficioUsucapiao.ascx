<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Oficio" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OficioUsucapiaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Oficio/oficioUsucapiao.js") %>"></script>
<script>
	OficioUsucapiao.urlObterDadosOficioUsucapiao = '<%= Url.Action("ObterDadosOficioUsucapiao", "Oficio", new {area="Especificidades"}) %>';
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>
	<br />

	<div class="block">
		<div class="coluna28 append2">
			<label for="Oficio_Destinatario">Dimensão da área requerida (m²) *</label>
			<%= Html.TextBox("Oficio.Dimensao", Model.Oficio.Dimensao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDimensao maskDecimalPonto", @maxlength = "12" }))%>
		</div>

		<div class="coluna31">
			<label for="Oficio_EmpreendimentoTipo">Tipo do empreendimento</label><br />
			<label><%= Html.RadioButton("Oficio.EmpreendimentoTipo", (int)eZonaLocalizacao.Rural, (Model.Oficio.EmpreendimentoTipo == (int)eZonaLocalizacao.Rural), ViewModelHelper.SetaDisabled(true, new { @class = "rdbEmpreendimentoTipo rdbEmpreendimentoTipoRural" }))%>Rural</label>
			<label><%= Html.RadioButton("Oficio.EmpreendimentoTipo", (int)eZonaLocalizacao.Urbana, (Model.Oficio.EmpreendimentoTipo == (int)eZonaLocalizacao.Urbana), ViewModelHelper.SetaDisabled(true, new { @class = "rdbEmpreendimentoTipo rdbEmpreendimentoTipoUrbano" }))%>Urbano</label>
		</div>
	</div>

	<div class="block">
		<div class="coluna75 append2">
			<label for="Oficio_DestinatarioPGE">Destinatário PGE *</label>
			<%= Html.TextArea("Oficio.DestinatarioPGE", (Model.Oficio.Id.GetValueOrDefault() <= 0) ? Model.Oficio.DestinatarioTextoPadrao : Model.Oficio.Destinatario, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media txtDestinatarioPGE" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna75 append2">
			<label for="Oficio_DescricaoOficio">Descrição do ofício *</label>
			<%= Html.TextArea("Oficio.DescricaoOficio", (Model.Oficio.Id.GetValueOrDefault() <= 0) ? Model.Oficio.DescricaoTextoPadrao : Model.Oficio.Descricao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text txtDescricaoOficio" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<legend>Arquivo</legend>
	<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>