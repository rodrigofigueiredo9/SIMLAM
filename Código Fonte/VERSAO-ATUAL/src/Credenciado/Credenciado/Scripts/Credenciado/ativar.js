/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

CredenciadoAtivar = {
	settings: {
		urls: {
			verificar: null,
			salvar: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(CredenciadoAtivar.settings, options); }
		CredenciadoAtivar.container = container;

		CredenciadoAtivar.container.delegate('.btnValidar', 'click', CredenciadoAtivar.verificar);
		CredenciadoAtivar.container.delegate('.btnSalvar', 'click', CredenciadoAtivar.salvar);
		Aux.setarFoco(CredenciadoAtivar.container);
	},

	obter: function () {
		var objeto = {
			Chave: $('.txtChave', CredenciadoAtivar.container).val(),
			Login: $('.txtLogin', CredenciadoAtivar.container).val(),
			Senha: $('.txtSenha', CredenciadoAtivar.container).val(),
			ConfirmarSenha: $('.txtConfirmarSenha', CredenciadoAtivar.container).val()
		};

		return objeto;
	},

	verificar: function () {
		var chave = $('.txtChave', CredenciadoAtivar.container).val();
		var retorno = MasterPage.executarAjax(CredenciadoAtivar.settings.urls.verificar, { Chave: chave }, CredenciadoAtivar.container, false);

		if (retorno.EhValido) {
			$('.containerDados', CredenciadoAtivar.container).empty();
			$('.containerDados', CredenciadoAtivar.container).html(retorno.Html);
			$('.txtChave', CredenciadoAtivar.container).attr('disabled', 'disabled').addClass('disabled');
			$('.btnValidar', CredenciadoAtivar.container).remove();

			$('.btnSalvar', CredenciadoAtivar.container).removeClass('hide');
			$('.btnModalOu', CredenciadoAtivar.container).removeClass('hide');

			return;
		}

		$('.btnSalvar', CredenciadoAtivar.container).addClass('hide');
		$('.btnModalOu', CredenciadoAtivar.container).addClass('hide');
	},

	salvar: function () {
		var objeto = CredenciadoAtivar.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: CredenciadoAtivar.settings.urls.salvar,
			data: JSON.stringify(objeto),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CredenciadoAtivar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CredenciadoAtivar.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}