///<reference path="../masterpage.js" />
///<reference path="../jquery.json-2.2.min.js" />
/// <reference path="../mensagem.js" />

OrgaoParceiroAlterarSituacao = {
	settings: {
		urls: {
			salvar: null
		},
		Mensagens: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(OrgaoParceiroAlterarSituacao.settings, options);
		}

		OrgaoParceiroAlterarSituacao.container = MasterPage.getContent(container);
		OrgaoParceiroAlterarSituacao.container.delegate('.ddlSituacaoNova', 'change', OrgaoParceiroAlterarSituacao.onSituacaoChange);
		OrgaoParceiroAlterarSituacao.container.delegate('.btnSalvar', 'click', OrgaoParceiroAlterarSituacao.onSalvar);
	},

	onSituacaoChange: function () {

		if ($(".ddlSituacaoNova", OrgaoParceiroAlterarSituacao.container).val() == "2") {
			$(".divMotivo", OrgaoParceiroAlterarSituacao.container).removeClass("hide");
			$(".divMotivo txtSituacaoMotivo", OrgaoParceiroAlterarSituacao.container).focus();
			return;
		}

		if (!$(".divMotivo", OrgaoParceiroAlterarSituacao.container).hasClass("hide")) {
			$(".divMotivo", OrgaoParceiroAlterarSituacao.container).addClass("hide");

			if ($('.txtSituacaoAtual', OrgaoParceiroAlterarSituacao.container).val() == 'Ativo') {
				$('.txtSituacaoMotivo', OrgaoParceiroAlterarSituacao.container).val('');
			}
			return;
		}

		if ($('.txtSituacaoAtual', OrgaoParceiroAlterarSituacao.container).val() == 'Bloqueado' &&
			$('.ddlSituacaoNova', OrgaoParceiroAlterarSituacao.container).val() == '0') {

			$(".divMotivo", OrgaoParceiroAlterarSituacao.container).removeClass("hide");
			$(".divMotivo txtSituacaoMotivo", OrgaoParceiroAlterarSituacao.container).focus();
			return;
		}

		MasterPage.redimensionar();
	},

	obterObjeto: function () {
		var Orgao = {};

		Orgao.Id = $('.hdnOrgaoParceiroId', OrgaoParceiroAlterarSituacao.container).val();
		Orgao.Sigla = $('.txtSigla', OrgaoParceiroAlterarSituacao.container).val();
		Orgao.Nome = $('.txtNome', OrgaoParceiroAlterarSituacao.container).val();
		Orgao.SituacaoMotivo = $('.txtSituacaoMotivo', OrgaoParceiroAlterarSituacao.container).val();
		Orgao.SituacaoId = $('.ddlSituacaoNova', OrgaoParceiroAlterarSituacao.container).val();
		Orgao.SituacaoTexto = $('.ddlSituacaoNova :selected', OrgaoParceiroAlterarSituacao.container).text();
		Orgao.SituacaoData = { DataTexto: $('.txtDataSituacaoNova', OrgaoParceiroAlterarSituacao.container).val() };

		return Orgao;
	},

	salvar: function () {
		$.ajax({
			url: OrgaoParceiroAlterarSituacao.settings.urls.salvar,
			data: JSON.stringify(OrgaoParceiroAlterarSituacao.obterObjeto()),
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaoParceiroAlterarSituacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(OrgaoParceiroAlterarSituacao.container, response.Msg);
				}
			}
		});
	},

	onSalvar: function () {
		orgao = OrgaoParceiroAlterarSituacao.obterObjeto();
		var msg = { Titulo: '', Texto: '' };

		if ($('.ddlSituacaoNova', OrgaoParceiroAlterarSituacao.container).val() == '1') {
			msg.Texto = Mensagem.replace(OrgaoParceiroAlterarSituacao.settings.Mensagens.ConfirmAtivar, '#siglaNome#', orgao.Sigla + ' - ' + orgao.Nome).Texto;
			msg.Titulo = OrgaoParceiroAlterarSituacao.settings.Mensagens.TituloConfirmAtivar.Texto;
		} else {
			msg.Texto = Mensagem.replace(OrgaoParceiroAlterarSituacao.settings.Mensagens.ConfirmBloquear, '#siglaNome#', orgao.Sigla + ' - ' + orgao.Nome).Texto;
			msg.Titulo = OrgaoParceiroAlterarSituacao.settings.Mensagens.TituloConfirmBloquear.Texto;
		}

		Modal.confirma({
			btnOkCallback: function (modalContent) {
				OrgaoParceiroAlterarSituacao.salvar();
				Modal.fechar(modalContent);
			},
			conteudo: msg.Texto,
			titulo: msg.Titulo
		});
	}
}