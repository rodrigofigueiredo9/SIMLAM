<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloSecurity" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<% EtramitePrincipal user = HttpContext.Current.User as EtramitePrincipal; %>
<% if ((user != null && user.Identity.IsAuthenticated)){ %>
<!-- INFO DO USUÁRIO  -->
<div class="usuarioInfoCaixa">
	
	<img src="<%= Url.Content("~/content/_img/seta_user_hover.png") %>" class="setaUser">
	<div class="usuarioInfo">
		<h4><strong title="<%= Html.Encode( user.EtramiteIdentity.Name )%>"><%= Html.Encode( ViewModelHelper.StringFit(user.EtramiteIdentity.Name, 40) )%></strong></h4>
		<p>Perfil: <strong><%= Html.Encode(user.EtramiteIdentity.FuncionarioTipoTexto)%></strong></p>

		<p>E-Mail: <strong title="<%= Html.Encode( user.EtramiteIdentity.Email )%>" ><%= Html.Encode(ViewModelHelper.StringFit(user.EtramiteIdentity.Email,35))%></strong></p>
		<div class="configCaixa">
		<% if ((user as EtramitePrincipal).EtramiteIdentity.FuncionarioTipo == 2) { %>
			<input type="button" value="Configurar" title="Configurar Administrador" class="floatRight" onclick="document.location.href='<%= Url.Action("AlterarAdministrador", "Administrador", new {area="", id=user.EtramiteIdentity.FuncionarioId}) %>'">
		<% } else { %>
			<input type="button" value="Configurar" title="Configurar Usuário" class="floatRight" onclick="document.location.href='<%= Url.Action("AlterarFuncionario", "Funcionario", new {area="", id=user.EtramiteIdentity.FuncionarioId}) %>'">
		<% } %>
		</div>
		<p>Último Acesso</p>
		<p>Data: <strong><%= Html.Encode(user.EtramiteIdentity.DataUltimoLogon)%></strong></p>
		<p>IP: <strong><%= Html.Encode(user.EtramiteIdentity.IpUltimoLogon)%></strong></p>
	</div>		
</div><!--.usuarioInfoCaixa-->
<!-- =================================== -->
<%} %>

