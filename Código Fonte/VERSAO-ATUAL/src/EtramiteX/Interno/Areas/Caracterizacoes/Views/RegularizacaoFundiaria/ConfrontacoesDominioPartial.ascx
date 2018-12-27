<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>

<fieldset class="block box fsDominialidade filtroExpansivoAberto">
	<legend class="titFiltros">Confrontações do dominio</legend>

	<!-- Confrontações do dominio -->
	<div class="filtroCorpo">
		<div class="block">
			<div class="coluna100 divRadioFaixaDivisa ">
				<label for="">Norte *</label>
				<%= Html.TextArea("Caracterizacao.Posse.ConfrontacoesNorte", Model.Caracterizacao.Posse.ConfrontacoesNorte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacoesNorte", @maxlength = "250" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna100 divRadioFaixaDivisa ">
				<label for="">Sul *</label>
				<%= Html.TextArea("Caracterizacao.Posse.ConfrontacoesSul", Model.Caracterizacao.Posse.ConfrontacoesSul, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacoesSul", @maxlength = "250" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna100 divRadioFaixaDivisa ">
				<label for="">Leste *</label>
				<%= Html.TextArea("Caracterizacao.Posse.ConfrontacoesLeste", Model.Caracterizacao.Posse.ConfrontacoesLeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacoesLeste", @maxlength = "250" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna100 divRadioFaixaDivisa ">
				<label for="">Oeste *</label>
				<%= Html.TextArea("Caracterizacao.Posse.ConfrontacoesOeste", Model.Caracterizacao.Posse.ConfrontacoesOeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontacoesOeste", @maxlength = "250" }))%>
			</div>
		</div>
	</div>