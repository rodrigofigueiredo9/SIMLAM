<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMSimuladorGeo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SimuladorGeoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>


<% Html.RenderPartial("Mensagem"); %>

<h1 class="titTela">Simulador de Importação de Shape - Dominialidade</h1>
<br />

<div>
	<fieldset class="block box" >
		<legend>Identificador do Usuário da Verificação</legend>
		<div class="coluna30">
			<div>
				<label>CPF *</label>
				<%= Html.TextBox("txtCpf", null, new { @class = "text maskCpf txtCpf" })%>
			</div>
		</div>
		<div class="coluna30">
			<button type="button" class="inlineBotao btnVerificarCpf">Verificar</button>
			<button type="button" class="inlineBotao btnLimparCpf hide">Limpar</button>
		</div>
	</fieldset>
	
	<div class="SimuladorGeoContent hide">

		<input type="hidden" class="hdnSimuladorGeoId" value="<%= Model.SimuladorGeo.Id %>" />
		<input type="hidden" class="hdnSimuladorGeoMecanismoElaboracaoId" value="<%= Model.SimuladorGeo.MecanismoElaboracaoId %>" />
		<input type="hidden" class="hdnSimuladorGeoSituacaoId" value="<%= Model.SimuladorGeo.SituacaoId %>" />
		
		<fieldset class="block box" >
			<legend>Ponto de Coordenada Geográfica do Empreendimento</legend>
			<div class="coluna20">
				<label>Sistema de Coordenada</label>
				<%= Html.DropDownList("ddlSistemaCoordenadas", Model.SistemaCoordenada, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlSistemaCoord" }))%>
			</div>
			<div class="coluna15">
				<label>Datum</label>
				<%= Html.DropDownList("ddlDatuns", Model.Datuns, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlDatum" }))%>
			</div>
			<div class="coluna15">
				<label>Fuso</label>
				<%= Html.DropDownList("ddlFusos", Model.Fusos, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlFuso" }))%>
			</div>
			<div class="coluna15">
				<label>Easting*</label>
				<%= Html.TextBox("txtEasting", Model.Easting, new { maxLength = 13, @class = "text maskDecimal4 txtEasting" })%>
			</div>
			<div class="coluna15">
				<label>Northing*</label>
				<%= Html.TextBox("txtNorthing", Model.Northing, new { maxLength = 13, @class = "text maskDecimal4 txtNorthing" })%>
			</div>
		</fieldset>

		<% Html.RenderPartial("BaseReferenciaPartial"); %>

		<% Html.RenderPartial("EnviarPartial"); %>
	</div>

</div>