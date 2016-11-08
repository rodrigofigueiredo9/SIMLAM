/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Motivo = {
    container: null,
    trAtual: null,
    Mensagem: {},
    settings: {
        UrlSalvarMotivo: null,
        UrlAtivarMotivo: null,
        UrlDesativarMotivo: null
    },

    load: function (container) {
        Motivo.container = MasterPage.getContent(container);

        Motivo.container.delegate('.btnEditarMotivo', 'click', Motivo.editar);
        Motivo.container.delegate('.btnCancelarMotivo', 'click', Motivo.cancelar);
        Motivo.container.delegate('.btnAdicionarMotivo', 'click', Motivo.adicionar);
        Motivo.container.delegate('.btnSalvarMotivo', 'click', Motivo.salvar);
        Motivo.container.delegate('.btnDesativarMotivo', 'click', Motivo.desativar);
        Motivo.container.delegate('.btnAtivarMotivo', 'click', Motivo.ativar);

        Motivo.setarBotoes({ btnAdicionarMotivo: true,
                             btnSalvarMotivo: false,
                             btnCancelarMotivo: false,
                             btnEditarMotivo: true,
                             btnAtivarEDesativar: true
                         });

        Aux.setarFoco(container);
    },
    setarBotoes: function (botoes) {
        //Botoes lateral input
        $(".btnAdicionarMotivo", Motivo.container).toggle(botoes.btnAdicionarMotivo);
        $(".btnSalvarMotivo", Motivo.container).toggle(botoes.btnSalvarMotivo);
        $(".btnCancelarMotivo", Motivo.container).toggle(botoes.btnCancelarMotivo);
        //Botoes acoes da grid
        $(".btnEditarMotivo", Motivo.container).toggle(botoes.btnEditarMotivo);
        $(".btnAtivarMotivo", Motivo.container).toggle(botoes.btnAtivarEDesativar);
        $(".btnDesativarMotivo", Motivo.container).toggle(botoes.btnAtivarEDesativar);
    },

    editar: function () {
        Motivo.setarBotoes({ btnAdicionarMotivo: false,
                             btnSalvarMotivo: true,
                             btnCancelarMotivo: true,
                             btnEditarMotivo: false,
                             btnAtivarEDesativar: false
                         });
        Motivo.trAtual = $(this).closest("tr");
        $(".txtMotivo", Motivo.container).val($(".trItemMotivoNome", Motivo.trAtual).text());
    },

    cancelar: function () {
        Motivo.setarBotoes({ btnAdicionarMotivo: true,
                             btnSalvarMotivo: false,
                             btnCancelarMotivo: false,
                             btnEditarMotivo: true,
                             btnAtivarEDesativar: true
                         });
        $(".txtMotivo", Motivo.container).val("");
        Motivo.trAtual = null;
    },

    adicionar: function () {
        var item = Motivo.validar(0, $(".txtMotivo", Motivo.container).val(), true);
        if (!item) { return; }
        Motivo.enviar(item, Motivo.settings.UrlSalvarMotivo);
    },

    salvar: function () {
        var item = Motivo.validar(+$(".hdnItemId", Motivo.trAtual).val(), $(".txtMotivo", Motivo.container).val(), $(".hdnItemAtivo", Motivo.trAtual).val() == "True");
        if (!item) { return; }
        Motivo.enviar(item, Motivo.settings.UrlSalvarMotivo);
    },

    ativar: function () {
        Motivo.trAtual = $(this).closest("tr");
        var item = {
            Id: +($(".hdnItemId", Motivo.trAtual).val()),
            Nome: $(".trItemMotivoNome", Motivo.trAtual).text(),
            IsAtivo: true
        };
        Motivo.enviar(item, Motivo.settings.UrlAtivarMotivo);
    },


    desativar: function () {
        Motivo.trAtual = $(this).closest("tr");
        var item = {
            Id: +($(".hdnItemId", Motivo.trAtual).val()),
            Nome: $(".trItemMotivoNome", Motivo.trAtual).text(),
            IsAtivo: false
        };
        Motivo.enviar(item, Motivo.settings.UrlDesativarMotivo);
    },


    validar: function (id, nome, ativo) {
        if ($.trim(nome) == "") {
            Mensagem.gerar(Motivo.container, new Array(Motivo.Mensagem.NomeObrigatorio));
            return;
        }

        var valido = true;

        $(" .tabItensMotivo tr", Motivo.container).each(function (i, linha) {
            valido = $(" .trItemMotivoNome ", linha).text().toLowerCase() != nome.toLowerCase() && valido;
        });

        if (!valido) {
            Mensagem.gerar(Motivo.container, new Array(Motivo.Mensagem.NomeJaAdicionado));
            return;
        }
        var item = { Id: id, Nome: nome, IsAtivo: ativo };
        return item;
    },

    enviar: function (item, url) {

        MasterPage.carregando(true);
        $.ajax({
            url: url,
            data: JSON.stringify(item),
            type: 'POST',
            typeData: 'json',
            contentType: 'application/json; charset=utf-8',
            cache: false,
            async: false,
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, Motivo.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(Motivo.container, response.Msg);
                }

                if (!response.EhValido) {
                    return;
                }

                if (item.Id <= 0) {

                    var novaLinha = $(".trTemplate", Motivo.container).clone();
                    $(".tabItensMotivo", Motivo.container).append(novaLinha);

                    $(novaLinha).removeClass("trTemplate");
                    var ativoTexto = item.IsAtivo ? "Ativo" : "Desativo";

                    $(".hdnItemId", novaLinha).val(response.Id);
                    $(".hdnItemAtivo", novaLinha).val(item.IsAtivo);

                    $(".trItemMotivoSituacao", novaLinha).text(ativoTexto).attr("title", ativoTexto);
                    $(".trItemMotivoNome", novaLinha).text(item.Nome).attr("title", item.Nome);

                    if (item.IsAtivo) {
                        $('.btnAtivarMotivo', novaLinha).remove();
                    } else {
                        $('.btnDesativarMotivo', novaLinha).remove();
                    }
                } else {
                                    
                    var ativoTexto = item.IsAtivo ? "Ativo" : "Desativo";

                    $(".hdnItemId", Motivo.trAtual).val(item.Id);
                    $(".trItemMotivoSituacao", Motivo.trAtual).text(ativoTexto).attr("title", ativoTexto);
                    $(".trItemMotivoNome", Motivo.trAtual).text(item.Nome).attr("title", item.Nome);

                    if (item.IsAtivo) {
                        $(".btnAtivarMotivo", Motivo.trAtual).replaceWith($(".btnDesativarMotivo", $(".trTemplate", Motivo.container)).clone(true));
                    } else {
                        $(".btnDesativarMotivo", Motivo.trAtual).replaceWith($(".btnAtivarMotivo", $(".trTemplate", Motivo.container)).clone(true));
                    }
                }
                Motivo.cancelar();
            }
        });

        MasterPage.carregando(false);
    }
}