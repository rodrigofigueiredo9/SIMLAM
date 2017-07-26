<%@ Import Namespace="Tecnomapas.Blocos.Etx.ModuloValidacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LogonVM>" %>

<% if (HttpContext.Current.User.Identity.IsAuthenticated) { %>
		<div class="divAcessoNegado divAcessoNegadoContainer prepend2 fonteBrancaLabel">

			<h2 class="fonteBrancaLabel">Acesso negado</h2>

			<p>Você não tem permissão para acessar essa funcionalidade.</p>

			<% if (!String.IsNullOrEmpty(Request.Params["msg"]))
			{
				Validacao.QueryParamDeserializer(Request.Params["msg"]);
			}%>

			<% foreach (var item in Validacao.Erros)
				{%>
					<p><%= Html.Encode(item.Texto)%></p>
			<%} %>

			<p class="acessoNegadoInicio"><%= Html.ActionLink("Início", "Index", "Home") %></p>
		</div>

<% } else { %>

	<% if (Model.IsAjaxRequest ){ %>
		<div class="block box coluna95 ">
	<% } %>

	<br />

	<% using (Html.BeginForm("LogOn", "Autenticacao", FormMethod.Post, new { @Class="formLogon"})) { %>

	<% if (!String.IsNullOrEmpty(Request.Params["ReturnUrl"])) { %>
		<%= Html.Hidden("returnUrl", HttpUtility.UrlEncode(Request.Params["ReturnUrl"]))%>
	<% } %>

	<div id="loginCaixa">

			<div class="corpoCaixaLogin">
				<div class="holderFundo">
					<div class="block">

						<div class="logo"></div>

						<div class="prepend10">
                            <% if (Model.EsqueciSenha!=true){ %>
							<p style="float:left">
								<label for="login" class="fonteBrancaLabel">Login:</label><br/>
								<%= Html.TextBox("login", null, new { @class = "loginTxt", @maxlength = "30" })%>
								<%= Html.ValidationMessage("login")%>
							</p>
							<p style="float:left; margin-left:20px">
								<label for="senha" class="fonteBrancaLabel">Senha:</label><br/>
								<%= Html.Password("senha", null, new { @class = "loginTxtSenha", type = "password", @maxlength = "20" })%>
								<%= Html.ValidationMessage("senha")%>
							</p>
							<% if (!(Model != null && Model.AlterarSenha)) { %>
							<p>
                                <% if (!Model.IsAjaxRequest && Model.EsqueciSenha!=true) { %>
                                    <input type="button" value="&nbsp;&nbsp;Esqueci minha senha&nbsp;&nbsp;" class="btnEsqueciSenha" />
                                    <%= Html.Hidden("esqueciSenha", false)%>
                                <% } %>
								<input type="button" value="&nbsp;&nbsp;Entrar&nbsp;&nbsp;" class="btnEntrarLogin" />
							</p>
							<%} %>
                            <%} %>
                            <% if (!Model.IsAjaxRequest && Model.EsqueciSenha==true){ %>
                                <%= Html.Hidden("verificarTrocarSenha", false)%>
                                <div class="msgSis">
								    <p>
									    <%= Model.AlterarSenhaMsg %>
								    </p>
							    </div>
                                <p style="float:left">
								    <label for="cpf" class="fonteBrancaLabel">CPF:</label><br/>
								    <%= Html.TextBox("CPF", null, new { @class = "cpfTxt maskCpf"})%>
								    <%= Html.ValidationMessage("CPF")%>
							    </p>
                                <p style="float:left; margin-left:20px">
                                    <label for="email" class="fonteBrancaLabel">E-mail:</label><br/>
								    <%= Html.TextBox("Email", null, new { @class = "emailTxt"})%>
								    <%= Html.ValidationMessage("email")%>
                                </p>
                                <p>
                                    <input type="button" value="&nbsp;&nbsp;Recuperar Senha&nbsp;&nbsp;" class="btnRecuperarSenha" />
                                </p>
                            <% } %>

							<% if (Model != null && Model.AlterarSenha) { %>

							<%= Html.Hidden("alterarSenha", Model.AlterarSenha)%>

							<div class="msgSis">
								<p>
									<%= Model.AlterarSenhaMsg %>
								</p>
							</div>
							<p style="float:left">
								<label for="novaSenha" class="fonteBrancaLabel">Nova senha</label><br/>
								<%= Html.TextBox("NovaSenha", null, new { @class = "loginTxtSenha", type = "password", @maxlength = "20" })%>
							</p>
							<p style="float:left; margin-left:20px">
								<label for="confirmarNovaSenha" class="fonteBrancaLabel">Confirmar nova senha</label><br/>
								<%= Html.TextBox("ConfirmarNovaSenha", null, new { @class = "loginTxtSenha", type = "password", @maxlength = "20" })%>
							</p>
							<p>
								<input type="button" value="Alterar a senha" class="btnEntrarLogin" />
							</p>
							<%} %>
						</div>
					</div>
				</div>
			</div>
		</div>

		<% if (Model.IsAjaxRequest ){ %>
		</div>
		<% } %>

	<% } %>

	<script type="text/javascript" >
	    $(".maskCpf").unmask().mask("999.999.999-99");

	    $('#login').focus();

			$('#loginCaixa').keypress(function (e) {
				if (e.keyCode != 13) return;
				$('.btnEntrarLogin').click();
			});

			<% if (Model.IsAjaxRequest ){ %>
				$('object').parents('.fundoModal').addClass('hide');

				<% if (Validacao.Erros.Count > 0){ %>
					Mensagem.gerar(
						MasterPage.getContent($('.formLogon').parents('.modalContent')),
						JSON.parse('<%= ViewModelHelper.Json(Validacao.Erros).ToString().Replace("\\", "\\\\") %>')
						);
				<% } %>
			<% } %>

			$('.btnEntrarLogin').click(function () {
				$('.camposExtrasLogin').slideDown('normal');

				<% if ( Model == null || !Model.IsAjaxRequest ){ %>
					$('.formLogon').submit();
			    <%} else { %>

					Modal.carregando($('.formLogon'), true);

					var modalContent = $('.formLogon').parents('.modalContent');

					$.ajax({ url: MasterPage.urlLogin, data: $('.formLogon').serialize(), type: 'POST', cache: false, async: false, modalLogin: false,
						error: function (XMLHttpRequest, textStatus, errorThrown) {
							Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(modalContent));
						},
						success: function (data, textStatus, XMLHttpRequest) {
							Modal.carregando($('.formLogon'), false);
							if (data != null && String(data).indexOf('formLogon') >= 0) {
								modalContent.empty();
								modalContent.append(data);
								return;
							} else {
								Modal.fechar($('.formLogon'));
								$('object').parents('.fundoModal').removeClass('hide');
								Modal.alinha();
							}
						}
					});
				<% } %>
			});






	    $('.btnEsqueciSenha').click(function () {
	        //alert('INÍCIO ESQUECI SENHA');

	        $('.camposExtrasLogin').slideDown('normal');

	        <% if ( Model == null || !Model.IsAjaxRequest ){ %>
	            //alert($('#esqueciSenha').val());
	            $('#esqueciSenha').val('True');
	        //alert($('#esqueciSenha').val());

	            $('.formLogon').submit();
	        <% } %>

	        //alert('FIM ESQUECI SENHA')
	    });

	    $('.btnRecuperarSenha').click(function () {
	        //alert('INÍCIO RECUPERAR SENHA');

	        $('.camposExtrasLogin').slideDown('normal');

	        <% if ( Model == null || !Model.IsAjaxRequest ){ %>
	        $('#esqueciSenha').val('False');
	        $('#verificarTrocarSenha').val('True');

            $('.formLogon').submit();
	        <% } %>

	        //alert('FIM RECUPERAR SENHA')
        });








			if ($('input[type=hidden]#alterarSenha').val() === 'True') {
				$('#login').addClass('disabled').attr('readOnly', 'true');
				$('#senha').focus();
			}
	</script>
<% } %>