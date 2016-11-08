﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloSecurity" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<% EtramitePrincipal user = HttpContext.Current.User as EtramitePrincipal; %>

<% if ((user != null && user.Identity.IsAuthenticated)){ %>
	<!-- INFO DO USUÁRIO  -->
	<div class="usuarioInfoCaixa">
	
		<img alt="seta" src="<%= Url.Content("~/content/_img/seta_user_hover.png") %>" class="setaUser" />

		<div class="usuarioInfo">
			<h4><strong title="<%= Html.Encode( user.EtramiteIdentity.Name )%>"><%= Html.Encode( ViewModelHelper.StringFit(user.EtramiteIdentity.Name, 40) )%></strong></h4>
			<p>Perfil: <strong><%= Html.Encode(user.EtramiteIdentity.FuncionarioTipoTexto)%></strong></p>

			<p>E-Mail: <strong title="<%= Html.Encode( user.EtramiteIdentity.Email )%>" ><%= Html.Encode(ViewModelHelper.StringFit(user.EtramiteIdentity.Email,35))%></strong></p>

			<div class="configCaixa">
				<input type="button" value="Configurar" title="Configurar Usuário" class="floatRight" onclick="document.location.href='<%= Url.Action("AlterarDados", "Credenciado", new {area="", id=user.EtramiteIdentity.FuncionarioId}) %>'" />
			</div>

			<p>Último Acesso</p>
			<p>Data: <strong><%= Html.Encode(user.EtramiteIdentity.DataUltimoLogon)%></strong></p>
			<p>IP: <strong><%= Html.Encode(user.EtramiteIdentity.IpUltimoLogon)%></strong></p>
		</div>
	</div><!--.usuarioInfoCaixa-->
	<!-- =================================== -->
<%} %>