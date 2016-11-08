<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHome" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<LogVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Log</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<%using (Html.BeginForm("Log", "Home", FormMethod.Post)){ %>


	<div id="central">
		<div class="titTela">Log</div>
		<br />
		<div class="filtroExpansivo">
			<span class="titFiltroSimples">Filtros</span>
			<div class="filtroCorpo filtroSerializarAjax block">

				<div class="coluna98">
					<div class="block">
						<div class="coluna83 fixado">
							<label for="Filtros_Mensagem">Mensagem</label>
							<%= Html.TextBox("Log.Mensagem", null, new { @class = "text txtMensagem" })%>
						</div>
						<div class="coluna10">
							<button type="submit" class="inlineBotao btnBuscar">Buscar</button>
						</div>
					</div>

					<div class="block fixado">
						<div class="coluna10">
							<label for="Filtros_Data">Data de</label>
							<%= Html.TextBox("Log.DataDe", null, new { @class = "text txtDataDe maskData setarFoco", @maxlength = "14" })%>
						</div>
						<div class="coluna10 prepend1">
							<label for="Filtros_Data">Data até</label>
							<%= Html.TextBox("Log.DataAte", null, new { @class = "text txtDataAte maskData", @maxlength = "14" })%>
						</div>
						<div class="coluna58 prepend1">
							<label for="Filtros_Source">Source</label>
							<%= Html.DropDownList("Log.Source", Model.Log.LstSource ,new { @class = "text" })%>
						</div>
					</div>
				</div>

			</div>
		</div>
		<%} %>
		<table>
			<thead>
				<tr>
					<th width="4%"><%:"ID"%></th>
					<th width="10%"><%:"DATA"%></th>
					<th width="15%"><%:"SOURCE"%></th>
					<th><%:"MENSAGEM"%></th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.Log.Resultados)
					{%>
				<tr>
					<td><%: item["ID"]%></td>
					<td><%: item["DATA"]%></td>
					<td><%: item["SOURCE"].ToString().Replace(".",". ") %></td>
					<td class="mensagem"><%: item["MENSAGEM"]%></td>
				</tr>
				<% }  %>
			</tbody>

		</table>
	</div>

	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/masterpage.js") %>" ></script>
	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/jquery-globalize/globalize.js") %>"></script>
	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/jquery-globalize/cultures/globalize.culture.pt-BR.js") %>"></script>
	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/tiny_mce/jquery.tinymce.js") %>"></script>
	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/mask/jquery.maskedinput-1.2.2.js") %>" ></script>
	<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/Lib/mask/jquery.maskMoney-1.4.1.js") %>" ></script>	
	<script type="text/javascript">
		$(function () {
			var container = $('#central');
			container.delegate('.filtroSerializarAjax', 'keyup', function (e) {
				if (e.keyCode == 13) $('.btnBuscar', container).click();
			});

			$('.txtMensagem', container).focus();
			$(container).css('min-height', '700px');
		});

		if ($('.txtMensagem') && $('.txtMensagem').val())
		{
			var replace = $('.mensagem').html().toString().replace($('.txtMensagem').val(), "<span style='background-color:yellow;'>" + $('.txtMensagem').val() + "</span>");
			$('.mensagem').html(replace);
		}
	</script>
</asp:Content>