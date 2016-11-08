/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="Lib/mask/jquery.maskMoney-1.4.1.js" />
/// <reference path="Lib/json2.js" />
/// <reference path="Lib/jquery-globalize/globalize.js" />

// Desabilida cache nas operações ajax
$.ajaxSetup({ cache: false });

$(window).load(function () {
	$(".carregandoMaster").addClass("hide");

	
});

function object(o) {
    function F() { };
    F.prototype = o;
    return new F();
}

$(function () {
	$(".container").removeClass("hide");

	Usuario.loadSessao();
	
	/*==================================*/
	//Evento de redimendionar com uma verificação se realmente houve uma mudança no tamanho da janela
	//Verificação necessária para prevenir multiplos eventos no IE8.
	var winWidth = $(window).width();
	var winHeight = $(window).height();

	$(window).resize(function () {

		function redimencionando() {
			MasterPage.redimensionar();
			Modal.alinha();
		}
		var winNewWidth = $(window).width();
		var winNewHeight = $(window).height();

		if (winWidth != winNewWidth || winHeight != winNewHeight) {
			//window.clearTimeout(resizeTimeout);
			resizeTimeout = window.setTimeout(redimencionando, 10);
		}

		winWidth = winNewWidth;
		winHeight = winNewHeight;

	});
	/*==================================*/

	/*$(window).resize(function () {
	MasterPage.redimensionar();
	Modal.alinha();
	});*/

    //Culture
    Globalize.culture("pt-BR");

	MasterPage.controleNavegacao();
	MasterPage.redimensionar();
	Menu.load();
	Mascara.load();
	MasterPage.load();
	Usuario.loadInfo();
	Modal.load();

	Listar.load();

	$("#central").ajaxComplete(MasterPage.validarAcesso);

	Listar.configurarListar();
	Modal.alinha();
		
});

Usuario = {
    loadSessao: function () {
        ///Script para mostrar e esconder o BOTÃO DE FECHAR SESSÃO.
        $('.sessao').mouseenter(function () {
            if ($('.fecharSessao').is(':animated')) {
                $('.fecharSessao').stop(true, true);
            } else {
                //$('.fecharSessao').fadeIn('fast');
                $('.fecharSessao').show();
            }
        });

        $('.sessao').mouseleave(function () {
            //$('.fecharSessao').fadeOut('fast');
            $('.fecharSessao').hide();
        });
    },

    loadInfo: function () {
        /////Script para abrir e fechar info do usuário
        $('.usuarioNome').click(function () {
            if ($('.usuarioInfoCaixa').is(':animated')) {
                $('.usuarioInfoCaixa').stop(true, true);
            } else {
                $('.usuarioInfoCaixa').slideToggle('fast');
                $(this).toggleClass('ativo');
                $('.usuarioCaixa').toggleClass('ativo');
            }
        });
    }
}

MasterPage = {

    loading: false,
	urlDatePicker: "../src/etramiteX/interno/content/_img/dot.png",
	urlLogin: "",
	urlEnderecoMunicipio: "",
	urlManual: "",
	urlSobre: "",
	urlSobreItens: "",
	urlObterSobreItens: "",
	loginTimeOutMinutes: 0,
	clockStarted: 0,
	clockTimeReset: null, //{ min: 0, seg: 0 },
	keyENTER: 13,

	validarAcesso: function (e, xhr, settings) {

		var response = (xhr.responseHTML) ? xhr.responseHTML : xhr.responseText;

		if (response != null &&
			((typeof (response) === "string" && response.indexOf("MsgPermissoes") >= 0) ||
			(typeof (response) === "object" && response.MsgPermissoes.length > 0))) {

			var jsonObj = (typeof (response) === "object") ? response : $.parseJSON(response);

			var container = $(".dialogContainer .fundoModal:last-child .modalContent");
			if (container.length === 0) {
				container = $("#central");
			}

			container.empty();

			Mensagem.gerar(container, jsonObj.MsgPermissoes);

			if (container.hasClass('modalContent')) {
				Modal.carregando(container, false);
				Modal.defaultButtons(modalContent);
			} else {
				MasterPage.carregando(false);
			}
			return;
		}

		if (response != null && typeof (response) === "string" && response.indexOf("divAcessoNegadoContainer") >= 0) {
			var container = $(".dialogContainer .fundoModal:last-child .modalContent");
			if (container.length === 0) {
				container = $("#central");
			}

			container.empty();
			container.append($(response).find(".divAcessoNegadoContainer"));

			if (container.hasClass('modalContent')) {
				Modal.carregando(container, false);
				Modal.defaultButtons(container);
			} else {
				MasterPage.carregando(false);
			}

			return;
		}

	},

	getContent: function (contentChild) {
		var container = Modal.getModalContent(contentChild);
		if (container.length > 0) return $(container);
		return $(contentChild).closest('#central');
	},

	carregando: function (isCarregando) {

	    MasterPage.loading = isCarregando;

	    if (isCarregando) {
	        setTimeout(function () {
	            if (MasterPage.loading) {
	                $(".carregandoMaster").removeClass("hide");
	            }
	        }, 250);//## Isso faz que o progress nao apareça pra requisições muito rapidas, que sejam mais rapidas que 250ms.
		}
		else {
			$(".carregandoMaster").addClass("hide");
		}
	},

	redireciona: function (url) {
		document.location.href = url;
	},

	jsonDate2Dma: function (dataJson) {
		return eval(dataJson.replace(/\/Date\((\d+)\)\//gi, "new Date($1)")).toString("dd/MM/yyyy");
	},

	fixJsonWithDates: function (jsonWithDates) {
		return jsonWithDates.replace(/\/Date\(([+-]{0,1}\d+[+-]{0,1}\d*)\)\//gi, '\\/Date($1)\\/');
	},

	clockStart: function () {
		var min = MasterPage.loginTimeOutMinutes;
		var seg = 0;

		function clockTick() {

			if (typeof MasterPage.clockTimeReset != "undefined" && MasterPage.clockTimeReset != null) {
				min = MasterPage.clockTimeReset.min;
				seg = MasterPage.clockTimeReset.seg;
				MasterPage.clockTimeReset = null;
			}

			if (seg <= 0) {
				seg = 59;
				min = min - 1;
			}
			else {
				seg = seg - 1;
			}

			if (min >= 0 && seg >= 0) {
				$(".timer").html(min + ":" + (seg < 10 ? "0" + seg : seg));
				setTimeout(clockTick, 1000);
			}
		}

		clockTick();
	},

	load: function () {
		MasterPage.botoes();
		MasterPage.grid();
		MasterPage.manuais();
		MasterPage.sobre();

		$(".timer").ajaxComplete(function (request, settings) {
			MasterPage.clockTimeReset = { min: MasterPage.loginTimeOutMinutes, seg: 0 };
			if (MasterPage.loginTimeOutMinutes > 0 && !MasterPage.clockStarted) {
				MasterPage.clockStarted = 1;
				MasterPage.clockStart();
			}
		});

		if (MasterPage.loginTimeOutMinutes > 0 && !MasterPage.clockStarted) {
			MasterPage.clockStarted = 1;
			MasterPage.clockStart();
		}

		//Animate FieldSet Expansivo
		$('#central').delegate(".fieldExpansivo legend", 'click', function () {
			var container = $(this).closest('.fieldExpansivo').find('.expandirRetrair');
			$(container).slideToggle(100);
			$(this).toggleClass('fAberto');
		});
	},

	grid: function (container) {
		container = (typeof container == 'undefined' || container == null) ? $("body") : container;
		///ESTILOS DA DATAGRID
		$('table.dataGridTable tbody tr:visible:even', container).addClass('impar');
		$('table.dataGridTable tbody tr:visible:odd', container).addClass('par');
		$('table.dataGridTable tbody tr:not(.semHover)', container)
			.live('mouseenter', function () { $(this).addClass('selecionado'); })
			.live('mouseleave', function () { $(this).removeClass('selecionado'); }
		);
	},

	botoes: function (container) {

		container = (typeof container == 'undefined' || container == null) ? $("body") : container;

		$('button, input[type="button"], input[type="submit"]', container).button();

		$('.botaoAdicionar', container).button({
			icons: { primary: 'ui-icon-plusthick' }
		});

		$('.botaoBuscar', container).button();

	$('.botaoVisualizar', container).button({
		icons: {
			primary: 'ui-icon-visualizar'
		}
	});

	$('.botaoLocalizar', container).button({
		icons: {
			primary: 'ui-icon-localizar'
		}
	});

	$('.botaoGerarPDF', container).button({
		icons: {
			primary: 'ui-icon-pdf'
		}
	});

	$('.botaoGerarXLS', container).button({
		icons: {
			primary: 'ui-icon-xls'
		}
	});

	$('.botaoInserirLista, .botaoAdicionarIcone', container).button({
		icons: { primary: 'ui-icon-plusthick' },
		text: false
	});

	$('.botaoSalvarIcone', container).button({
		icons: { primary: 'ui-icon-salvarIcone' },
		text: false
	});

	$('.botaoCancelarIcone', container).button({
		icons: { primary: 'ui-icon-cancelarIcone' },
		text: false
	});
},

controleNavegacao: function () {

	window.onunload = function () { $(".carregandoMaster").show(); return true; };
	/*$("input[type='submit']").live("click", function () { $(this).attr("disabled", true); });

	$("a,button").live("click", function () {
	var button = $(this);
	button.attr("disabled", true);

	function buttonclockTick() {
	button.removeAttr("disabled");
	}

	setTimeout(buttonclockTick, 1200);

	return true;
	});*/

},

countRed: 0,

redimensionar: function () {

	if (MasterPage.countRed == 0) {
		MasterPage.countRed = MasterPage.countRed + 1;
		MasterPage.redimensionarAcao();
		MasterPage.countRed = 0;
	}
},

///Script para redimensionar a altura do MENU DE NAVEGAÇÃO e A SEÇÃO CENTRAL de acordo com o tamanho da janela.
redimensionarAcao: function () {

	$('#central').css('position', '');
	$('.menuSecCaixa').css('position', '');
	$('#central').css('min-height', ''); ////Hack para "zerar" a altura mínima para podermos dimensionar sem problemas na frente.

	if ($('nav').height() > $(window).height() - 46) {
		$('.container').css('height', $('nav').height() + 'px');

	} else {
		$('.container').css('height', $(window).height() - 46 + 'px');
	}


	var top = $('#central').offset().top;
	var maxBottom = 0;
	$('.container').each(function () {
		var pos = $(this).offset();
		var height = $(this).outerHeight();
		var bottom = pos.top + height;
		if (bottom > maxBottom) {
			maxBottom = bottom;
		}
	});
	$('#central').css({ 'min-height': (maxBottom - top) - 40 + 'px' });
},

abasNavagacao: function () {
	///Comando para iniciar a funcionalidade das abas de navegação
	$('#abasNav').tabs({ fx: { opacity: 'toggle'} }).tabs();
},

validarAjax: function (url, params, container, async) {
	var objResponse = null;
	var arrayMensagem = new Array();
	var ehValido = false;
	var ehModal = MasterPage.getContent(container).hasClass('modalContent');

	if (!ehModal) {
		MasterPage.carregando(true);
	}

	$.ajax({ url: url, data: JSON.stringify(params), cache: false, async: async,
		type: 'POST', dataType: 'json', contentType: 'application/json; charset=utf-8',
		crossDomain: true,
		error: function (XMLHttpRequest, textStatus, erroThrown) {
			arrayMensagem = Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
		},
		success: function (response, textStatus, XMLHttpRequest) {
			Mensagem.limpar(MasterPage.getContent(container));
			objResponse = response;

			if (response.EhValido) {
				ehValido = true;
			}

			if (response.Msg && response.Msg.length > 0) {
				arrayMensagem = response.Msg;

				if (container) {
					var context = MasterPage.getContent(container);
					Mensagem.gerar(context, response.Msg);
				}
			}
		}
	});

	if (!ehModal) {
		MasterPage.carregando(false);
	}

	return { EhValido: ehValido, Msg: arrayMensagem, ObjResponse: objResponse };
},

json: function (ctr) {

	ctr.wrap("<form></form>");
	var st = ctr.parent().serializeArray();
	ctr.unwrap();

	var obj = {};

	$.each(st, function (idx, item) {
		if (typeof (item.name) !== 'undefined') {
			obj[item.name] = item.value;
		}
	});

	return obj;
},

manuais: function () {
	$(".iconeAjuda").click(function () {
		Modal.abrir(MasterPage.urlManual, null, null, Modal.tamanhoModalPequena);
	});
},

sobre: function () {
	$(".iconeSobre").click(function () {
		Modal.abrir(MasterPage.urlSobre, null, function (context) {
			$(".btnHistorico", context).click(function () {
				Modal.abrir(MasterPage.urlSobreItens, null, function (contextItens) {

					$("p", contextItens).click(function () {
						var context = $(this).closest('.itemVersao');
						var temItens = $('ul', context).find('li').length > 0;
						
						$(this).toggleClass("aberto");
						$('ul', context).toggleClass("hide");

						if (temItens) {
							return;
						}

						$.ajax({ url: MasterPage.urlObterSobreItens,
							data: JSON.stringify({ versaoId: $('.versaoId', context).val() }),
							cache: false,
							async: false,
							type: 'POST',
							dataType: 'json',
							contentType: 'application/json; charset=utf-8',
							error: function (XMLHttpRequest, textStatus, erroThrown) {
								Aux.error(XMLHttpRequest, textStatus, erroThrown, context);
							},
							success: function (response, textStatus, XMLHttpRequest) {
								if (response.Itens.length == 0) {
									var item = $('.liTemplate', contextItens).clone().removeAttr('class').removeAttr('style');
									$('span:last', item).text('Não existe itens para esta versão.');
									$('ul', context).append(item);
									return;
								}

								$.each(response.Itens, function (i, obj) {
									var item = $('.liTemplate', contextItens).clone().removeAttr('class').removeAttr('style');
									$('span:first', item).text(obj.Tipo);
									$('span:last', item).text("#" + obj.NumeroTP + " - " + obj.Descricao);
									$('ul', context).append(item);
								});
							}
						});
					});
				}, Modal.tamanhoModalPequena);
			});
		}, Modal.tamanhoModalPequena);
	});

},

tabIndex: function (options) {

	var settings = { container: null, ligado: true };
	settings = $.extend(settings, options);

	settings.container = settings.container || $('.container');

	if (!settings.ligado) {
		$(settings.container).find('*').each(function (idx, item) {
			if (item.tabIndex && item.tabIndex > -1) {
				$(item).data('tabIndex', $(item).attr('tabIndex'));
			}
			item.tabIndex = -1;
		});
	} else {
		$(settings.container).find('*').each(function (idx, item) {
			if ($(item).data('tabIndex')) {
				$(item).attr('tabIndex', $(item).data('tabIndex'));
			} else {
				$(item).removeAttr('tabIndex');
			}
		});
	}
},

	log: function (strMsg, isRed) {

		if ($('#divDevDebugLog').length <= 0) {
			$('body').append('<div id="divDevDebugLog" onclick="$(this).html(\'\');" style="display: block; z-index: 100000000; margin-left: 1%; margin-right: 1%; text-align: left; overflow:auto; position: fixed; bottom: 0px; left: 0px; background-color: White; height: 25%; width: 98%; border-style: dashed; border-width: 2px; border-color: Gray;"></div>');
			$('#divDevDebugLog').dblclick(function () { $(this).remove(); });
		}
		data = new Date();
		var domDiv = $('#divDevDebugLog');
		domDiv.append("<span style='color: " + (isRed ? 'Red' : 'Green') + "; font-weight: bold; padding: 5px; border-bottom: 1px solid gray; display: inline-block;'><span style='border-right: 1px solid gray; display: inline-block; width: 85px;'>" + data.getHours().toString() + ":" + data.getMinutes().toString() + ":" + data.getSeconds().toString() + ":" + data.getMilliseconds() + "</span> - " + strMsg + "</span><br/>");
		//domDiv.html("<span style='color: " + (isRed ? 'Red' : 'Green') + "; font-weight: bold;'>" + data.getHours().toString() + ":" + data.getMinutes().toString() + ":" + data.getSeconds().toString() + " - " + strMsg + "</span><br/>");
	},

	redirecionaPost: function (url, data) {
		if (url && data) {
			data = JSON.stringify(data);
			var inputParams = $('<input type="hidden" name="paramsManter" />').val(data);
			var form = $('<form action="' + url + '" method="post"></form>');
			form.appendTo('body');
			form.append(inputParams);
			form.submit();
		};
	}
}

Menu = {
    isOpen: false,
    load: function () {
        ///Script para recolher e expandir o MENU DE NAVEGAÇÃO
        $('#menuCmd > a.exp').click(function () {
            if ($('nav').width() == 170) {
                $('nav').animate({ width: "45" }, { duration: 300 });
                $('.centralAlinha').animate({ marginLeft: "45" }, { duration: 300, step: function () { MasterPage.redimensionar(); } });
                if ($.browser.msie) {
                    $('body').animate({ backgroundPositionX: "-130px" }, { duration: 300 });
                } else {
                    $('body').animate({ backgroundPosition: "-130px top" }, { duration: 300 });
                }
                Cookie.set("StatusMenu", true, 5);
            } else {
                $('nav').animate({ width: "170" }, { duration: 300 });
                $('.centralAlinha').animate({ marginLeft: "175" }, { duration: 300, step: function () { MasterPage.redimensionar(); } });
                if ($.browser.msie) {
                    $('body').animate({ backgroundPositionX: "0px" }, { duration: 300 });
                } else {
                    $('body').animate({ backgroundPosition: "0px top" }, { duration: 300 });
                }
                Cookie.set("StatusMenu", false, 5);
            }
            $(this).toggleClass('rec');
            $('nav').toggleClass('navRec');
            $('.linkMenu').toggleClass('linkRec');
            $('.itemMenu').toggleClass('itemRec');
            $('.centralAlinha').toggleClass('rec');
            $('body').toggleClass('bgFauxRec');
        });

        //Guarda se o menu Esta Expandido no Cookie
        Menu.isOpen = Cookie.get("StatusMenu");

        if ((Menu.isOpen === 'true')) {
            $('#menuCmd > a.exp').toggleClass('rec');
            $('nav').toggleClass('navRec');
            $('.linkMenu').toggleClass('linkRec');
            $('.itemMenu').toggleClass('itemRec');
            $('.centralAlinha').toggleClass('rec');
            $('body').toggleClass('bgFauxRec');
            MasterPage.redimensionar();
        }
    }
}

Mascara = {

	urlTinymce: null,

	TinyMCEDefault: function () {
		return {
			// Location of TinyMCE script
			//script_url: Mascara.urlTinymce + 'tiny_mce.js',
			script_url: Mascara.urlTinymce + 'tiny_mce_gzip.ashx',

			// General options
			theme: "advanced",
			plugins: "autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,advlist",

			mode: "specific_textareas",
			editor_selector: "tinymce",
			editor_deselector: "tinymceOff",

			// Theme options
			theme_advanced_buttons1: "newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,formatselect,fontselect,fontsizeselect",
			theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,cleanup,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
			theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,ltr,rtl,|,styleprops,visualchars,|,fullscreen",
			//theme_advanced_buttons4: "styleprops,visualchars,nonbreaking",
			theme_advanced_toolbar_location: "top",
			theme_advanced_toolbar_align: "left",
			theme_advanced_statusbar_location: "bottom",

			theme_advanced_resizing: true,
			theme_advanced_resizing_max_width: $(document).width() - 284//,
			//theme_advanced_resizing_use_cookie: false//,
			//theme_advanced_resize_horizontal: false

			// Example content CSS (should be your site CSS)
			//content_css: "css/content.css",

			// Drop lists for link/image/media/template dialogs
			//template_external_list_url: "lists/template_list.js",
			//external_link_list_url: "lists/link_list.js",
			//external_image_list_url: "lists/image_list.js",
			//media_external_list_url: "lists/media_list.js",

			// Replace values for the template plugin
			/*template_replace_values: {
			username: "Some User",
			staffid: "991234"
			}*/
		}
	},

	load: function (container) {

		// Cheat do TextArea
		$('textarea[maxlength]').keyup(function () {
			var max = parseInt($(this).attr('maxlength'));
			if ($(this).val().length > max) {
				$(this).val($(this).val().substr(0, $(this).attr('maxlength')));
			}
		});

		//tiny_mce
		$('.tinymce', container).tinymce(Mascara.TinyMCEDefault());

		$(".maskNumAno").unbind(".mask").mask(/^([0-9]{1,7}\/[0-9]{0,4}$|^[0-9]{1,7}\/$|^[0-9]{1,7})?$/, []);

		$(".maskDataAno").unbind(".mask").mask(/^([0-9/]{0,10})$/, []);

		/*-----------------------------INICIO - Mascaras------------------------------*/
		//Coordenada GMS
		$.mask.definitions['~'] = '[-+]';
		$(".maskCoordGms", container).unmask().mask("~99:99:99,99");

		//Coordenada GDEC
		$(".maskCoordGdec", container).unmask().mask("~99,9999999", { placeholder: "0" });

		//Coordenada UTM
		$(".maskUtm", container).unmask().mask("9999999,99999");

		// CEP
		$(".maskCep", container).unmask().mask("99.999-999");

		//CCIR - Certificado de Cadastro de Imovel Rural
		$(".maskCcir", container).unmask().mask("999.999.999.999-9");

		//NIRF - Número do Imóvel na Receita Federal
		$(".maskNirf", container).unmask().mask("9.999.999-9");

		//FONE - Número Telefonico
		$(".maskFone", container).unmask().mask("(99) 9999-9999");

		//CPF - Cadastro de Pessoa Física
		$(".maskCpf", container).unmask().mask("999.999.999-99");

		//CPF - Cadastro de Pessoa Física
		$(".maskCpfParcial", container).unmask().mask("999.999.999-99");

		//CNPJ - Cadastro Nacional de Pessoa Juridica
		$(".maskCnpj", container).unmask().mask("99.999.999/9999-99");

		//CNPJ - Cadastro Nacional de Pessoa Juridica
		$(".maskCnpjParcial", container).unmask().mask("99.999.999/9999-99");

		//DATA - Campo de data padrão
		$(".maskData", container).unmask().mask("39/19/9999");

		//Número da caixa do arquivo (3 dig.)
		$(".maskNum3", container).unmask().mask("999");

		//Número da caixa do arquivo (4 dig.)
		$(".maskNum4", container).unmask().mask("9999");

		$(".maskNum15", container).unmask().mask("999999999999999");

		//Número da caixa do arquivo (38 dig.)
		$(".maskNum38", container).unmask().mask("99999999999999999999999999999999999999");

		//Número do Endereço
		$(".maskNumEndereco", container).unmask().mask("999999");

		//Área Abrangencia
		$(".maskAreaAbrangencia", container).unmask().mask("9999999,99");

		//Documento Anterior
		$(".maskDocumentoAnterior", container).unmask().mask("9999999/9999");

		//Máscara de processo
		$(".maskProc", container).unmask().mask("9999999/9999");

		//Quantidade Volumes Processo
		$(".maskQuantidadeVolumes", container).unmask().mask("99");

		// Mascara 4,4 para campos de área
		$(".maskArea44", container).unmask().maskMoney({ precision: 4 });

		// Mascara 5,4 para campos de área
		$(".maskArea54", container).unmask().mask("99999,9999");

		// Mascara 7,4 para campos de área
		$(".maskArea74", container).unmask().mask("9999999,9999");

		// Mascara 7,2 para campos de área
		$(".maskArea72", container).unmask().mask("9999999,99");

		// Mascara de hora
		$(".maskHora", container).unmask().mask("99:99:99");

	    // Mascara de hora e minuto
		$(".maskHoraMinuto", container).unmask().mask("99:99");


	    // Mascara de mes/ano
		$(".maskMesAno", container).unmask().mask("99/9999");

		$(".maskInteger", container).unmaskMoney().maskMoney({ precision: 0, thousands: '.', allowZero: true, defaultZero: false });
		$(".maskIntegerObrigatorio", container).unmaskMoney().maskMoney({ precision: 0, thousands: '.', defaultZero: false });

		$(".maskDecimal", container).unmaskMoney().maskMoney();
		$(".maskDecimal4", container).unmaskMoney().maskMoney({ precision: 4 });
		$(".maskDecimal3", container).unmaskMoney().maskMoney({ precision: 3 });
		$(".maskDecimalPonto", container).unmaskMoney().maskMoney({ decimal: ',', thousands: '.' });
		$(".maskDecimalPonto4", container).unmaskMoney().maskMoney({ decimal: ',', thousands: '.', precision: 4 });
		$(".maskDecimalPonto3", container).unmaskMoney().maskMoney({ decimal: ',', thousands: '.', precision: 3 });

		/*-->maskMoney
		{
		symbol:'US$', // Simbolo
		decimal:'.', // Separador do decimal
		precision:2, // Precisão
		thousands:',', // Separador para os milhares
		allowZero:false, // Permite que o digito 0 seja o primeiro caractere
		showSymbol:false // Exibe/Oculta o símbolo
		}		
		*/
		$(".maskNumInt", container).each(function (idx, item) {
			$(item).unmask().mask(Mascara.criarMascara($(item).attr('maxlength'), 2));
		});

		$(".maskString", container).each(function (idx, item) {
			$(item).unmask().mask(Mascara.criarMascara($(item).attr('maxlength'), 1));
		});

		$(".datepicker", container).datepicker('destroy').datepicker({
			dateFormat: 'dd/mm/yy',
			dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado', 'Domingo'],
			dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
			dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
			monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
			monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
			nextText: 'Próximo',
			prevText: 'Anterior'
		});

		//		$(".maskNumAno", container).keypress(function (e) {
		//			var tecla = e.charCode || e.keyCode || e.which;
		//			var texto = $(this).val();
		//			var pos = $(this).caret();

		//			var ignore = (tecla < 16 || (tecla > 16 && tecla < 32) || (tecla > 32 && tecla < 41));

		//			//delete selection before proceeding
		//			if (!ignore || tecla == 8 || tecla == 46)
		//				return true;

		//			if (!Mascara.validarExpressaoRegular('^[0-9]{1,7}\/[0-9]{0,4}$|^[0-9]{1,7}\/$|^[0-9]{1,7}$', texto.substr(0, pos.begin) + String.fromCharCode(tecla) + texto.substr(pos.end, texto.length - 1))) {
		//				return false;
		//			}
		//			//paste, input
		//		});

		/*-----------------------------FIM - Mascaras---------------------------------*/
	},

	validarExpressaoRegular: function (expressao, valor) {
		var re = new RegExp(expressao, 'i'); //ignorar case
		return re.test(valor);
	},

	criarMascara: function (quantidadeCaracter, tipo) {
		var mascara = "";
		for (i = 0; i < quantidadeCaracter; i++) {
			mascara += (tipo === 1) ? "a" : "9";
		}
		return mascara;
	},

	getFloatMask: function (strValue) {
		//return strValue ? parseFloat(strValue.trim().replace(/\./g, '').replace(',', '.')) : 0;
	    return strValue ? Globalize.parseFloat(strValue) : strValue;
	},

	getIntMask: function (strValue) {
		//return strValue ? parseFloat(strValue.trim().replace(/\./g, '').replace(',', '.')) : 0;
		return strValue ? Globalize.parseInt(strValue) : strValue;
	},

	getInt64Mask: function (strValue) {
		//return strValue ? parseFloat(strValue.trim().replace(/\./g, '').replace(',', '.')) : 0;
		return strValue ? new Number(strValue) : strValue;
	},

	getStringMask: function (number, format) {
		return number ? Globalize.format(number, format ? format : 'n2') : number;
	}
}

Listar = {

    QuantidadePaginaTimeOut: 1440,

    load: function () {
        ///Expandir e recolher a aba de Filtro
    	$('body').delegate(".titFiltro", "click", Listar.expadirFiltro);

    },

    atualizarEstiloOrdenar: function (index, controle) {
        $('td, th', controle).removeClass('selecionado');
        $('td:nth-child(' + (index) + '), th:nth-child(' + (index) + ')', controle).addClass('selecionado');
    },

    expadirFiltro: function () {
    	$(this).toggleClass('fAberto');

    	if ($(this).parent().find('.fixado').length == 0) {
    		if ($(this).closest('.filtroExpansivo').find('.filtroCorpo').is(':animated')) {
    			$(this).closest('.filtroExpansivo').find('.filtroCorpo').stop(true, true);
    			$(this).toggleClass('fAberto');
    		} else {
    			$(this).slideToggle('normal');
    		}
    	} else {
    		if ($(this).closest('.filtroExpansivo').find('.filtroCorpo > div').children().not('.fixado').is(':animated')) {
    			$(this).closest('.filtroExpansivo').find('.filtroCorpo > div').children().not('.fixado').stop(true, true);
    			$(this).toggleClass('fAberto');
    		} else {
    			$(this).closest('.filtroExpansivo').find('.filtroCorpo > div').children().not('.fixado').slideToggle('normal');
    		}
    	}
        Cookie.set("isFiltroExpandido", $('.titFiltro').hasClass('fAberto'), 5);
    },

    configurarListar: function () {
        var isFiltroExpandido = Cookie.get("isFiltroExpandido");
        if (isFiltroExpandido === "true") {
            $('.titFiltro').addClass('fAberto');
            $('.filtroCorpo > div').children().removeClass('hide');
        } else {
            $('.titFiltro').removeClass('fAberto');
        }

        var i = parseInt($('.listarHdnOrdenarPor').val());
        if (i >= 0) {
            Listar.atualizarEstiloOrdenar(i);
        }
    },

    atualizarEstiloTable: function (controle) {
        $('tbody tr', controle).removeClass('impar par');
        $('tbody tr:visible:even', controle).addClass('impar');
        $('tbody tr:visible:odd', controle).addClass('par');
    }
}

Cookie = {
    //Script gerenciar Cookie
    get: function (c_name) {
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=");
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) c_end = document.cookie.length;
                return unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return "";
    },

    set: function (c_name, value, expireMin) {
        var exdate = new Date();
        exdate.setTime(exdate.getTime() + (expireMin * 60 * 1000));

        document.cookie = c_name + "=" + escape(value) +
		((expireMin == null) ? "" : ";expires=" + exdate.toGMTString() + ';path=/');
    }
}

//Script gerenciar Eventos de Checkbox em tabelas
function SetClickCheckBox(event, controlePai) {

    var tr = $(controlePai).closest('tr');

    if (event.target.type != 'checkbox') {
        event.stopPropagation();
        tr.find('input:checkbox').attr('checked', !tr.find('input:checkbox').attr('checked'));
    }

    if (tr.find('input:checkbox').attr('checked')) {
        tr.addClass('linhaSelecionada');
    }
    else {
        tr.removeClass('linhaSelecionada');
    }
}

//Modal	------------------------------------------------------------------------------
Modal = {

    /////Script para fechar o Modal
	load: function () {
		$('.fMdl').live("click", function () {
			Modal.fechar($(this));
		});
	},

	scrollTop: function (modalRef) {
		$(modalRef).parents('.boxModal').animate({ scrollTop: 0 }, 'slow');
	},

	///Função para alinhar o MODAL
	alinha: function () {
		$(".fundoModal").each(function () {

			var maxHeight = $(this).height() - 250;
			var top = parseFloat($(this).find('.modalBranco').css('top'));

			if (typeof ($(this).data['modalTamanho']) !== 'undefined') {
				maxHeight = $(this).height() - $(".fundoModal").data['modalTamanho'].heightPadding;
				top = Math.abs((($(".fundoModal").data['modalTamanho'].heightPadding / 2) - 75));
			}

			$(this).find('.boxModal').css('max-height', maxHeight);
			$(this).find('.modalBranco').css('margin-left', ($(this).find('.modalBranco').width() / 2) * -1);
			$(this).find('.modalBranco').css('top', top + 'px');
		});

		$(Modal.arrayResize).each(function (item) {
			if (item) {
				item();
			}
		});
	},

	arrayResize: new Array(),

	resize: function (acao) {
		Modal.arrayResize.push(acao);
	},

	tamanhoModalPequena: { width: "400px", minWidthPerc: "40%" },
	tamanhoModalMedia: { width: "600px", minWidthPerc: "60%" },
	tamanhoModalGrande: { width: "800px", minWidthPerc: "90%" },
	tamanhoModalFull: { width: "875px", minWidthPerc: "95%", heightPadding: 100 },

	dimensionar: function (modalRef, width, minWidthPerc) {
		var modalBranco = modalRef.find('.modalBranco');
		modalBranco.css("width", width);
		modalBranco.css("min-width", minWidthPerc);
		modalBranco.css('margin-left', (modalBranco.width() / 2) * -1);
	},

	confirma: function (options) {
		var defaults = {
			btnOkCallback: null,       // função que será chamada quando o botão [Ok] for clicado, se esta função retornar false o diálogo não é fechado. Para esta função é passado por parâmetro a div do diálogo.
			btnOkData: null,           // Dados a serem passados para a funcao [btnOkCallback]
			removerFechar: false,      // Remove o botão de fechar da modal
			btnCancelCallback: null,   // função que será chamada quando o botão [Cancelar] for clicado, se esta função retornar false o diálogo não é fechado. Para esta função é passado por parâmetro a div do diálogo.
			btnCancelData: null,       // Dados a serem passados para a funcao [btnCancelCallback]
			btnOkLabel: 'Sim',         // Label do botão [Ok] ("Ok", "Excluir" , "Confirmar", "Sim", etc..)
			btCancelLabel: 'Cancelar', // Label do botão [Cancelar] ("Cancelar", "Voltar", "Não" , etc...)
			url: '',                   // Url a ser carregada na modal. Pode-se usar "conteudo" ao invés da "url" para preencher a modal
			conteudo: '', 		   // Conteúdo para preencher a modal. Pode-se usar "url" ao invés de "conteudo" para preencher a modal
			urlData: null,             // Dados a serem passados para a "url"
			onLoadCallbackName: '',    // Funcao a ser chamada depois que o modal carregar, pode ser uma string ("Processo.load") ou uma funcao (Processo.load)
			titulo: null, 		   // Titulo da Modal de Excluir
			tamanhoModal: Modal.tamanhoModalPequena
		};
		var settings = $.extend({}, defaults, options);

		var _onModalLoad = function (modal) {

			Modal.defaultButtons(modal,
				settings.btnOkCallback,
				settings.btnOkLabel,
				settings.btnOkData,
				settings.btnCancelCallback,
				settings.btCancelLabel,
				settings.btnCancelData,
				settings.removerFechar);

			var modalLoadFunction = null;

			if (typeof settings.onLoadCallbackName != 'undefined') {
				if (typeof settings.onLoadCallbackName == 'string') {
					modalLoadFunction = eval(settings.onLoadCallbackName);
				} else {
					modalLoadFunction = settings.onLoadCallbackName;
				}
				$('input:first', modal).focus();
				if (typeof modalLoadFunction == 'function') {
					modalLoadFunction(modal);
				}
			}
		};

		if (settings.url) {
			Modal.abrir(settings.url, settings.urlData, _onModalLoad, settings.tamanhoModal);
		} else {
			Modal.abrirHtml(settings.conteudo, { 'tamanho': settings.tamanhoModal, 'onLoadCallbackName': _onModalLoad, 'titulo': settings.titulo });
		}
	},

	customButtons: function (modal, buttonContainer) {
		var btnModalContainer = $('.modelBotoes', modal);
		btnModalContainer.empty();
		btnModalContainer.append(buttonContainer.contents())
		btnModalContainer.removeClass('hide');

		$('.linkCancelar', btnModalContainer).click(function () {
			Modal.fechar(modal);
		});

		buttonContainer.remove();
	},

	defaultButtons: function (modal, okFunction, okLabel, okData, cancelFunction, cancelLabel, cancelData, removerFechar) {
		if (!modal.hasClass('fundoModal')) {
			modal = modal.closest('.fundoModal')
		}
		var buttonsContainer = modal.find('.modelBotoes');
		buttonsContainer.removeClass("hide");

		var btnOk = buttonsContainer.find(".btnModalOk");
		if (typeof okLabel != 'undefined' && okLabel != null) {
			btnOk.click(function () {
				okFunction(modal, okData);
			});
			if (okLabel && typeof okLabel == 'string') {
				btnOk.val(okLabel);
			}
			$('.btnModalOu', buttonsContainer).show();
			$('.btnModalOk', buttonsContainer).show();
		}
		else {
			$('.btnModalOu', buttonsContainer).hide();
			$('.btnModalOk', buttonsContainer).hide();
		}

		var btnCancel = buttonsContainer.find(".linkCancelar");
		if (typeof cancelFunction == 'function') {
			btnCancel.click(function () {
				if (cancelFunction(modal, cancelData) !== false) {
					Modal.fechar(modal);
				}
			});
		}
		else {
			btnCancel.click(function () {
				Modal.fechar(modal);
			});
		}

		if (btnCancel && btnCancel.length > 0 &&
			 typeof cancelLabel == 'string') {
			btnCancel.text(cancelLabel);
		}

		if (removerFechar) {
			modal.find(".fMdl").remove();
		}
	},

	buttons: function (modal, controle) {
		if (!modal.hasClass('fundoModal')) {
			modal = modal.closest('.fundoModal')
		}
		var buttonsContainer = modal.find('.modelBotoes');

		buttonsContainer.find(".btnModalOk").remove();
		buttonsContainer.find(".linkCancelar").click(function () {
			Modal.fechar(modal);
		});

		controle.removeClass("hide");
		buttonsContainer.prepend(controle);
		buttonsContainer.removeClass("hide");
	},

	limparButtons: function (modal) {
		if (!modal.hasClass('fundoModal')) {
			modal = modal.closest('.fundoModal')
		}
		var buttonsContainer = modal.find('.modelBotoes');
		buttonsContainer.empty();
	},

	// onLoadCallback (opcional)
	abrir: function (url, data, onLoadCallbackName, tamanho, tituloTela) {
		var modal = $(".modalTemplate").clone();
		modal.removeClass("hide");
		modal.removeClass("modalTemplate");

		Mensagem.limpar();

		$(".dialogContainer").append(modal);

		if (typeof tamanho !== "undefined") {
			Modal.dimensionar(modal, tamanho.width, tamanho.minWidthPerc);
			//$('.fundoModal', modal).addClass('.modalHeight');
			if (tamanho == Modal.tamanhoModalFull) {
				$('.fundoModal', modal).data['modalTamanho'] = tamanho;
			}
		}
		else {
			Modal.alinha();
		}

		var modalContent = modal.find(".modalContent");
		modal.attr('tabindex', 5000).focus();
		var retorno = null;

		var erroCallBack = function (jqXHR, status, errorThrown) {

			if (jqXHR.status == 401) {
				modal.remove();
			}

			Aux.error(jqXHR, status, errorThrown);
		};

		if (typeof data == 'object' && data != null) {
			$.ajax({
				type: 'POST',
				dataType: 'html',
				contentType: 'application/json; charset=utf-8',
				url: url,
				data: JSON.stringify(data),
				cache: false,
				async: false,
				modalLogin: false,
				error: erroCallBack,
				success: function (response, status, xhr) {
					retorno = response;
				}
			});
		} else {
			$.ajax({
				type: 'GET',
				dataType: 'html',
				url: url,
				data: data,
				cache: false,
				async: false,
				modalLogin: false,
				error: erroCallBack,
				success: function (response, status, xhr) {
					retorno = response;
				}
			});
		}

		if (retorno == null) {
			return;
		}

		if (url != MasterPage.urlLogin && retorno.indexOf("loginCaixa") >= 0) {
			retorno = '<div class=\'block box coluna95 \'>' + retorno + '</div>'; ;
		}

		modalContent.prepend(retorno);

		if (tituloTela) {
			$('.titTela', modalContent).text(tituloTela);
		}

		modal.find(".modalCarregando").addClass("hide");
		modalContent.removeClass("hide");
		Modal.alinha();

		//Reload de Datepiker e estilo de tabela e mascaras
		MasterPage.botoes(modalContent);
		MasterPage.grid(modalContent);

		MasterPage.tabIndex({ ligado: false });
		MasterPage.tabIndex({ container: $(".dialogContainer .fundoModal:not(:last)"), ligado: false });

		Mascara.load(modalContent);

		var modalLoadFunction = null;

		if (typeof onLoadCallbackName != 'undefined') {
			if (typeof onLoadCallbackName == 'string') {
				modalLoadFunction = eval(onLoadCallbackName);
			} else {
				modalLoadFunction = onLoadCallbackName;
			}
			setTimeout(function () {
				$('input::enabled:first', modal).focus();
				if (typeof modalLoadFunction == 'function') {
					modalLoadFunction(modal);
				}
			}, 100);
		}
	},

	abrirHtml: function (content, options) {

		var settings = $.extend({}, {
			'tamanho': Modal.tamanhoModalPequena,
			'onLoadCallbackName': '',
			'titulo': null,
			'html': false
		}, options);

		var conteudo = '';

		if (settings.titulo != null) {
			conteudo = '<h2 class=\'titTela\'>' + settings.titulo + '</h2><br />';
		}

		if (!settings.html) {
			var conteudoTexto = '<div class=\'block box coluna95 \'>' + content + '</div>';
			conteudo = settings.titulo == null ? conteudoTexto : conteudo + conteudoTexto;
		} else {
			conteudo += content;
		}

		Mensagem.limpar();

		var modal = $('.modalTemplate').clone();
		modal.removeClass('hide');
		modal.removeClass('modalTemplate');

		$('.dialogContainer').append(modal);

		var modalContent = modal.find('.modalContent');
		modalContent.html(conteudo);

		modal.find('.modalCarregando').addClass('hide');
		modalContent.removeClass('hide');

		if (typeof settings.tamanho !== 'undefined') {
			Modal.dimensionar(modal, settings.tamanho.width, settings.tamanho.minWidthPerc);
		}
		else {
			Modal.alinha();
		}

		if (typeof settings.onLoadCallbackName != 'undefined') {
			if (typeof settings.onLoadCallbackName == 'string') {
				settings.onLoadCallbackName = eval(settings.onLoadCallbackName);
			}
			setTimeout(function () {
				$('input::enabled:first', modal).focus();
				if (typeof settings.onLoadCallbackName == 'function') {
					settings.onLoadCallbackName(modal);
				}
			}, 100);
		}
	},

	update: function (modalRef, url, data, callBack) {
		var modal = modalRef;
		if (!modal.hasClass('fundoModal')) {
			modal = modalRef.closest(".fundoModal");
		}
		var modalContent = modal.find(".modalContent");

		modal.find(".modalCarregando").removeClass("hide");
		modalContent.addClass("hide");

		$.ajax({
			type: (typeof data == "object" && data != null) ? "POST" : "GET",
			url: url,
			data: data,
			cache: false,
			async: false,
			dataType: "html",
			error: Aux.error,
			success: function (response, status, xhr) {

				if (response != null) {
					if (response.indexOf("logOnTela") >= 0) {
						Modal.abrir(MasterPage.urlLogin, null, null, Modal.tamanhoModalPequena);
						Modal.fechar(modalContent);
						return;
					}
				}

				modalContent.empty();
				modalContent.prepend(response);

				if (typeof callBack == 'function') {
					callBack(modalContent, response, status, xhr);
				}

				modal.find(".modalCarregando").addClass("hide");
				modalContent.removeClass("hide");
				Modal.alinha();

				//Reload de Datepiker e estilo de tabela e mascaras
				//MasterPage.load();
				MasterPage.botoes(modalContent);
				MasterPage.grid(modalContent);

				Mascara.load(modalContent);

			}
		});
	},

	json: function (ctr) {
		var modal = Modal.getFundoModal(ctr);
		modal.wrap("<form></form>");
		var st = modal.parent().serializeArray();
		modal.unwrap();

		var obj = {};

		$.each(st, function (idx, item) {
			if (typeof (item.name) !== 'undefined') {
				obj[item.name] = item.value;
			}
		});

		return obj;
	},

	serialize: function (ctr) {
		ctr.wrap("<form></form>");
		var st = ctr.parent().serialize();
		ctr.unwrap();
		return st;
	},

	fechar: function (ctr) {
		var fundoModal = Modal.getFundoModal(ctr);
		var modalContent = MasterPage.getContent(fundoModal);
		var fazendoUploadNestaModal = fundoModal.data('uploademandamento');

		//Mensagem.limpar(modalContent);

		if (fazendoUploadNestaModal) {
			Mensagem.gerar(modalContent, new Array({ Tipo: 3, Texto: "Tranferência de arquivo em andamento..." }));
			return false;
		} else {
			fundoModal.find('.modalBranco, .modalSobre').slideUp('fast', function () {
				fundoModal.hide().remove();

				var tabIndexContainer = null;
				if ($(".dialogContainer .fundoModal").length > 0) {
					tabIndexContainer = $(".dialogContainer .fundoModal:last");
				}

				MasterPage.tabIndex({ container: tabIndexContainer, ligado: true });

			});

			return true;
		}
	},

	getModalContent: function (modalChild) {
		if ($(modalChild).hasClass('.modalContent')) return $(modalChild);

		var modalContent = $(modalChild).find('.modalContent');
		if (modalContent.length > 0) return modalContent;

		modalContent = $(modalChild).closest('.modalContent');
		return modalContent;
	},

	getFundoModal: function (meuThis) {
		if ($(meuThis).hasClass('.fundoModal')) return $(meuThis);

		var modal = $(meuThis).find('.fundoModal');
		if (modal.length > 0) return modal;

		modal = $(meuThis).closest('.fundoModal');
		return modal;
	},

	carregando: function myfunction(ctr, flag) {
		var modal = Modal.getFundoModal(ctr);
		var modalContent = modal.find(".modalContent");

		if (flag) {
			modal.find(".modalCarregando").removeClass("hide");
			modalContent.addClass("hide");
		}
		else {
			modal.find(".modalCarregando").addClass("hide");
			modalContent.removeClass("hide");
		}
	},

	excluir: function (options) {

		var settings = {
			'urlConfirm': null,
			'urlAcao': null,
			'id': null,
			'callBack': null,
			'btnExcluir': null,
			'btnTexto': 'Excluir',
			'tamanho': Modal.tamanhoModalMedia,
			'naoExecutarUltimaBusca': false,
			'containerListar': null,
			'quantidade': null,
			'totalRegistros': null
		};

		$.extend(settings, options);

		settings.containerListar = MasterPage.getContent(settings.btnExcluir);
		settings.quantidade = $('.dataGridTable tbody tr', settings.containerListar).length;
		settings.totalRegistros = $('#totalRegistros strong', settings.containerListar).text();

		Modal.confirma({
			btnOkLabel: settings.btnTexto,
			url: settings.urlConfirm + '/' + settings.id,
			tamanhoModal: settings.tamanho,
			btnOkCallback: function (modalContent) {
				$.ajax({ url: settings.urlAcao,
					data: { id: settings.id },
					type: "POST",
					cache: false,
					async: false,
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(modalContent));
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							if (settings.quantidade == 1) {
								if (settings.quantidade == settings.totalRegistros) {
									$('.gridContainer', settings.containerListar).empty();
								} else {
									if (!settings.naoExecutarUltimaBusca) {
										settings.containerListar.listarAjax('ultimaBuscaVoltarPagina');
									}
								}
							} else {
								if (!settings.naoExecutarUltimaBusca) {
									settings.containerListar.listarAjax('ultimaBusca');
								}
							}
							Mensagem.gerar(settings.containerListar, response.Msg);
							Modal.fechar(modalContent);
							if (settings.callBack) {
								settings.callBack(response, settings.btnExcluir);
							}
						} else {
							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(Modal.getModalContent(modalContent), response.Msg);
							}
						}
					}
				});
			}
		});
	},

	executar: function (options) {

		var settings = {
			'urlConfirm': null,
			'urlAcao': null,
			'id': null,
			'dataPost': null,
			'callBack': null,
			'btnAcao': null,
			'btnTexto': 'Sim',
			'tamanho': Modal.tamanhoModalMedia,
			'container': null
		};

		$.extend(settings, options);
		settings.container = MasterPage.getContent(settings.btnAcao);

		if (!settings.dataPost) {
			settings.dataPost = { id: settings.id };
		}

		Modal.confirma({
			btnOkLabel: settings.btnTexto,
			url: settings.urlConfirm + '/' + settings.id,
			tamanhoModal: settings.tamanho,
			btnOkCallback: function (modalContent) {
				$.ajax({
					url: settings.urlAcao,
					data: JSON.stringify(settings.dataPost),
					type: 'POST',
					typeData: 'json',
					contentType: 'application/json; charset=utf-8',
					cache: false,
					async: false,
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, settings.container);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						modalContent = Modal.getModalContent(modalContent);

						if (response.EhValido) {
							Modal.fechar(modalContent);
							settings.container.listarAjax('ultimaBusca');
							Mensagem.gerar(settings.container, response.Msg);
						} else {
							Mensagem.gerar(modalContent, response.Msg);
						}
					}
				});
			}
		});
	}
}

//Modal	Listar ------------------------------------------------------------------------

ModalListar = {
    load: function (modalElement, url, updateCallback) {

        var modal = modalElement.find(".modalContent");
        modalElement.delegate('.quantPaginacao', 'change', function () {
            modalElement.find('.paginaAtual').attr('value', 1);
            var dataFiltro = Modal.json(modal);

            var indice = parseInt(modalElement.find('.quantPaginacao').attr('value'));
            Cookie.set('QuantidadePorPagina', indice, Listar.QuantidadePaginaTimeOut);

            Modal.update(modalElement, url, dataFiltro, updateCallback);
        });

        //link das páginas do grid
        modalElement.delegate(".pag", "click", function () {
            var pag = $(this).attr('id');
            pag = parseInt(pag.substring(7));

            var pagAtual = parseInt(modal.find('.paginaAtual').val());

            if (pag != pagAtual) {

                modalElement.find('.pag').removeClass('ativo');
                $(this).addClass('ativo');

                modal.find('.paginaAtual').attr('value', pag);


                var dataFiltro = Modal.json(modal);
                Modal.update(modalElement, url, dataFiltro, updateCallback);
            }
        });


        modalElement.delegate(".anterior", "click", function () {

            var pag = parseInt(modal.find('.paginaAtual').val());

            if (pag > 1) {
                pagAnterior = pag - 1;
                modal.find('.pag').removeClass('ativo');
                modal.find('.numPag_' + pagAnterior).addClass('ativo');
                modal.find('.paginaAtual').attr('value', pagAnterior);

                var dataFiltro = Modal.json(modal);
                Modal.update(modalElement, url, dataFiltro, updateCallback);
            }
        });

        modalElement.delegate(".proxima", "click", function () {

            var pag = parseInt(modal.find('.paginaAtual').val());
            var pagFinal = parseInt(modal.find('.paginaFinal').val());

            if (pag < pagFinal) {
                proximaPag = pag + 1;
                modal.find('.pag').removeClass('ativo');
                modal.find('.numPag_' + proximaPag).addClass('ativo');
                modal.find('.paginaAtual').attr('value', proximaPag);

                var dataFiltro = Modal.json(modal);
                Modal.update(modalElement, url, dataFiltro, updateCallback);
            }
        });

        modalElement.delegate(".comeco", "click", function () {
            var pagAtual = parseInt(modal.find('.paginaAtual').val());
            if (pagAtual == 1) return;

            modal.find('.pag').removeClass('ativo');
            modal.find('.numPag_1').addClass('ativo');
            modal.find('.paginaAtual').attr('value', 1);



            var dataFiltro = Modal.json(modal);
            Modal.update(modalElement, url, dataFiltro, updateCallback);
        });

        modalElement.delegate(".final", "click", function () {
            var pagAtual = parseInt(modal.find('.paginaAtual').val());
            var pagFinal = parseInt(modal.find('.paginaFinal').val());

            if (pagAtual == pagFinal) return;

            modal.find('.pag').removeClass('ativo');
            modal.find('.numPag_' + pagFinal).addClass('ativo');
            modal.find('.paginaAtual').attr('value', pagFinal);

            var dataFiltro = Modal.json(modal);
            Modal.update(modalElement, url, dataFiltro, updateCallback);
        });

        //ordenação das colunas
        modalElement.delegate(".ordenavel th:not(.semOrdenacao)", "click", function () {
            var index = $(this).parent().children().index(this);
            Listar.atualizarEstiloOrdenar(index, modalElement);

            modal.find('.ordenarPor').attr('value', index + 1);
            var dataFiltro = Modal.json(modal);
            Modal.update(modalElement, url, dataFiltro, updateCallback);
        });
    }
}

//Upload de arquivos	---------------------------------------------------------------
FileUpload = {

    Count: 0,

    setObjectArquivo: function (prefix, targetObj, arquivoObj) {
        if (typeof arquivoObj != 'undefined' && arquivoObj != null) {
            targetObj[prefix + 'Apagar'] = arquivoObj['Apagar'];
            targetObj[prefix + 'Nome'] = arquivoObj['Nome'];
            targetObj[prefix + 'TemporarioNome'] = arquivoObj['TemporarioNome'];
            targetObj[prefix + 'TemporarioPathNome'] = arquivoObj['TemporarioPathNome'];
            targetObj[prefix + 'Caminho'] = arquivoObj['Caminho'];
            targetObj[prefix + 'ContentLength'] = arquivoObj['ContentLength'];
            targetObj[prefix + 'ContentType'] = arquivoObj['ContentType'];
            targetObj[prefix + 'Raiz'] = arquivoObj['Raiz'];
            targetObj[prefix + 'Diretorio'] = arquivoObj['Diretorio'];
            targetObj[prefix + 'Extensao'] = arquivoObj['Extensao'];
            targetObj[prefix + 'Id'] = arquivoObj['Id'];
            targetObj[prefix + 'Tid'] = arquivoObj['Tid'];
        }
    },

    upload: function (url, ctr, callBack) {
        var fundoModal = Modal.getFundoModal(ctr);
        fundoModal.data('uploademandamento', 'true');

        var id = ctr.attr("id");
        var iframeAnt = $("body").find("#iframe_" + id);

        if (iframeAnt == null || iframeAnt.length > 0) {
            iframeAnt.remove();
            iframeAnt = null;
        }

        var iframe = $("<iframe id='iframe_" + id + "' name='iframe_" + id + "' style='display:none;'></iframe>");
        $("body").append(iframe);

        var parent = ctr.parent();

        var form = $("#formUpFile");
        form.empty();
        form.attr('action', url);
        form.attr("target", "iframe_" + id);
        form.append(ctr);

        var ctrRef = ctr.clone();
        parent.append(ctrRef);

        iframe.load(function () {
            fundoModal.data('uploademandamento', '');
            var data1 = iframe.contents().find("body").html();

            var isHtml = (data1 !== null && (data1.indexOf('<') > -1 && (data1.indexOf('/>') > -1 || data1.indexOf('</'))));

            if (isHtml) {
                Modal.abrirHtml(data1);
            } else {
                callBack(ctrRef, data1, isHtml);
            }

            FileUpload.Count--;
        });

        FileUpload.Count++;

        form.submit();
    },

    cancelar: function (uploadId) {
        var iframeAnt = $("body").find("#iframe_" + uploadId);

        if (iframeAnt == null || iframeAnt.length > 0) {
            iframeAnt.remove();
            iframeAnt = null;
            FileUpload.Count--;
        }
    }
}

Aux = {
	htmlEncode: function (value) {
		return $('<div/>').text(value || '').html();
	},

	scrollTop: function (container) {
		var modalBox = $(container).closest('.boxModal');
		if (modalBox.length > 0) { // dentro de modal
			modalBox.scrollTop(0, 'slow');
		} else { // na master
			$('html, body').animate({ scrollTop: 0 }, 'slow');
		}
	},

	scrollBottom: function (container) {
		var modalBox = $(container).closest('.boxModal');
		if (modalBox.length > 0) { // dentro de modal
			//modalBox.scrollTop(0, 'slow');
		} else { // na master
			$('html, body').animate({ scrollTop: container.height() }, 'slow');
		}
	},

	carregando: function (contentChild, isCarregando) {
		var content = MasterPage.getContent(contentChild);
		if (content.hasClass('modalContent')) {
			Modal.carregando(content, isCarregando);
		} else {
			MasterPage.carregando(isCarregando);
		}
	},

	error: function (jqXHR, status, errorThrown, container) {

		if (jqXHR.status == 401) {
			//if para evitar varias janelas
			if ($(".formLogon").length == 0) {
				Modal.abrir(MasterPage.urlLogin + '?msg=' + jqXHR.responseText, null, null, Modal.tamanhoModalMedia);
			}
			return;
		}

		if (!container) {
			container = $(".dialogContainer .fundoModal:last-child .modalContent");
			if (container.length === 0) {
				container = $("#central");
			}
		}

		//Manda para o Tratamento de erro no evento global do ajax
		if (jqXHR.status == 500 && jqXHR.responseText.indexOf('MsgPermissoes') >= 0) {
			return;
		}

		var msg = "Desculpe-nos, houve um erro ao carregar: " + jqXHR.status + " " + jqXHR.statusText;

		if (jqXHR.status == 500 || jqXHR.status == 404 || jqXHR.status == 400) {
			var resp = $(jqXHR.responseText);
			container.empty();
			container.append(resp.find("h2"));
			container.append(resp.find("table"));
		}

		var arrayMsg = [{ Tipo: 4, Texto: msg}];
		Mensagem.gerar(container, arrayMsg);

		if (container.attr("id") === "central") {
			MasterPage.carregando(false);
		} else {
			Modal.carregando(container, false);
			container.removeClass("hide");
			Modal.alinha();
		}

		return arrayMsg;
	},

	errorGetPost: function (response, textStatus, XMLHttpRequest, container) {

		if (XMLHttpRequest.status == 401) {
			//if para evitar varias janelas
			if ($(".formLogon").length == 0) {
				Modal.abrir(MasterPage.urlLogin, null, null, Modal.tamanhoModalMedia);
			}
			return;
		}

		if (textStatus === "error") {
			var msg = "Desculpe-nos, houve um erro ao carregar: " + XMLHttpRequest.status + " " + XMLHttpRequest.statusText;

			if (!container) {
				container = $(".dialogContainer .fundoModal:last-child .modalContent");
				if (container.length === 0) {
					container = $("#central");
				}
			}

			//Manda para o Tratamento de erro no evento global do ajax
			if (XMLHttpRequest.status == 500 && response.responseText.indexOf('MsgPermissoes') >= 0) {
				return;
			}

			if (XMLHttpRequest.status == 500 || XMLHttpRequest.status == 404 || XMLHttpRequest.status == 400) {
				var resp = $(response);
				container.empty();
				container.append(resp.find("h2"));
				container.append(resp.find("table"));
			}

			var arrayMsg = [{ Tipo: 4, Texto: msg}];
			Mensagem.gerar(container, arrayMsg);

			return true;
		}

		return false;
	},

	expandirLinha: function () {
		var tr = $(this);
		tr.next().toggle(0, function () {
			MasterPage.redimensionar();
		});
		tr.toggleClass('ativo');
	},

	expadirFieldSet: function () {
		var container = $(this).closest('fieldset');
		$('.titFiltros', container).toggleClass('fAberto');

		if ($('.titFiltro', container).parent().find('.fixado').length == 0) {
			if ($('.filtroCorpo', container).is(':animated')) {
				$('.filtroCorpo', container).stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.filtroCorpo', container).slideToggle('normal');
			}
		} else {
			if ($('.filtroCorpo > div', container).children().not('.fixado').is(':animated')) {
				$('.filtroCorpo > div', container).children().not('.fixado').stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.titFiltros > div', container).children().not('.fixado').slideToggle('normal');
			}
		}
	},

	parseJson: function (value) {
		value = value.replaceAll('\n', '\\n');
		return JSON.parse(value);
	},

	// Estrutura da seleção do estado
	/***********************************************************************************
	* divEndereco			"class obrigatória"
	* ddlMunicipio			"class obrigatória"

	* Exemplo de como chamar = $('.class do estado', container).change(Aux.onEnderecoEstadoChange);
	***********************************************************************************/
	onEnderecoEstadoChange: function () {
		var container = $($(this).closest('.divEndereco'));
		$municipioSelect = $($(this).closest('.divEndereco').find('.ddlMunicipio'), container);

		$estadoSelect = $(this, container);
		var estadoId = parseInt($estadoSelect.val());

		$municipioSelect.find('option').remove();
		var isMunicipioDisabled = $municipioSelect.is(':disabled');
		$municipioSelect
			.addClass('disabled')
			.attr('disabled', 'disabled');
		if (typeof estadoId == 'undefined' || isNaN(estadoId) || estadoId <= 0) {
			$municipioSelect.append('<option value="0">*** Selecione ***</option>');
		} else {
			$municipioSelect.append('<option value="0">*** Carregando municípios ***</option>');
			Aux.estadoChangeAjax = $.ajax({
				url: MasterPage.urlEnderecoMunicipio,
				data: { EstadoId: estadoId },
				async: false,
				success: function (municipios) {

					municipios.splice(0, 0, { Id: 0, Texto: '*** Selecione ***' });
					$municipioSelect
						.removeAttr('disabled')
						.removeClass('disabled');
					$municipioSelect.find('option').remove();

					$.each(municipios, function () {
						$municipioSelect.append('<option value="' + this.Id + '">' + this.Texto + '</option>');
					});
					if (isMunicipioDisabled) {
						$municipioSelect.focus();
					}
				},
				error: function (jqXHR, status, errorThrown) {

					if (jqXHR.status == 401) {
						//if para evitar varias janelas
						if ($(".formLogon").length == 0) {
							Modal.abrir(MasterPage.urlLogin, null, null, Modal.tamanhoModalMedia);
						}
						return;
					}

					var msg = "Desculpe-nos, houve um erro ao carregar: " + jqXHR.status + " " + jqXHR.statusText;
					container = container.closest('#central');

					if (jqXHR.status == 500 || jqXHR.status == 404 || jqXHR.status == 400) {
						var resp = $(jqXHR.responseText);
						container.empty();
						container.append(resp.find("h2"));
						container.append(resp.find("table"));
					}

					var arrayMsg = [{ Tipo: 4, Texto: msg}];
					Mensagem.gerar(container, arrayMsg);
				}
			});
		}
	},

	toFunction: function (functionOrName) {
		if (typeof functionOrName == 'string') return eval(functionOrName);
		if (typeof functionOrName == 'function') return functionOrName;
		else return null;
	},

	onChangeRadioCpfCnpjMask: function (campo) {
        var container = null;
		$('.txtCpfCnpj', container).unmask();

		if (typeof campo.length == 'undefined') {
			container = $(this).closest('div');
		} else {
			container = $(campo).closest('div');
		}

		$('.txtCpfCnpj', container).unmask();
		$('.txtCpfCnpj', container).val('');

		if ($('.radioCPF', container).attr('checked')) {
			$('.txtCpfCnpj', container).removeClass('maskCnpjParcial');
			$('.txtCpfCnpj', container).addClass('maskCpfParcial');
		} else {
			$('.txtCpfCnpj', container).removeClass('maskCpfParcial');
			$('.txtCpfCnpj', container).addClass('maskCnpjParcial');
		}

		if (typeof campo.length == 'undefined') {
			$('.txtCpfCnpj', container).focus();
		}

		Mascara.load(container);
	},

	isEmpty: function (obj) {
		return ($.trim(obj) == '' || typeof obj == "undefined" || obj == null);
	},

	setarFoco: function (container) {
		$('.setarFoco:enabled:first', container).focus();
	},

	isModal: function (container) {
		return MasterPage.getContent(container).hasClass('modalContent');
	},

	// fonte: http://www.filamentgroup.com/lab/jquery_plugin_for_requesting_ajax_like_file_downloads/
	downloadAjax: function (id, url, data, method) {
		var iframeAnt = $("body").find("#iframe_" + id);

		if (iframeAnt == null || iframeAnt.length > 0) {
			iframeAnt.remove();
			iframeAnt = null;
		}

		var iframe = $("<iframe id='iframe_" + id + "' name='iframe_" + id + "' style='display:none;'></iframe>");
		$("body").append(iframe);

		var form = $('<form action="' + url + '" method="' + (method || 'post') + '"></form>');
		form.appendTo('body');
		form.empty();
		form.attr('action', url);
		form.attr("target", "iframe_" + id);

		iframe.load(function () {
			
			var data1 = iframe.contents().find("body").html();
			var isJson = (data1 !== null && (data1.indexOf('{') > -1 && (data1.indexOf('}') > -1 || data1.indexOf('}'))));

			isJson = jQuery.parseJSON(data1);

			Mensagem.gerar($('#central'), isJson.Msgs);

			FileUpload.Count--;
		});

		FileUpload.Count++;

		form.submit();
		form.remove();
	},

	// fonte: http://www.filamentgroup.com/lab/jquery_plugin_for_requesting_ajax_like_file_downloads/
	download: function (url, data, method) {
		if (url && data) {
			data = typeof data == 'string' ? data : JSON.stringify(data);
			var inputParams = $('<input type="hidden" name="paramsJson" />').val(data);
			var form = $('<form action="' + url + '" method="' + (method || 'post') + '"></form>');
			form.appendTo('body');
			form.append(inputParams);
			form.submit();
			form.remove();
		};
	},

	marcarCheck: function () {
		var linha = $(this).closest('div,span');
		if ($('input[type=checkbox]', linha).attr('checked')) {
			$('input[type=checkbox]', linha).removeAttr('checked', 'checked');
		} else {
			$('input[type=checkbox]', linha).attr('checked', 'checked');
		}
	},

	selecionarCheckGrid: function (e) {
		var checkbox = $(this).find('input[type=checkbox]');
		if (e.target == checkbox.get(0))
			return;

		if (checkbox.is(':checked')) {
			$(this).find('input[type=checkbox]').removeAttr('checked');
		} else {
			$(this).find('input[type=checkbox]').attr('checked', 'checked');
		}
	},

	selecionarTodosCheckGrid: function () {
		var index = $(this).attr('cellIndex');
		var checkboxs = $(this).closest('table').find('tbody tr td').filter(function (i, e) { return $(e).attr('cellIndex') == index; }).find('input[type=checkbox]');

		if ($(this).hasClass('thTodos')) {
			$(this).removeClass('thTodos');
			checkboxs.removeAttr('checked');
		} else {
			$(this).addClass('thTodos');
			checkboxs.attr('checked', 'checked');
		}
	},

	validarData: function (data) {
		var matchData = new RegExp(/((0[1-9]|[12][0-9]|3[01])\/(0[13578]|1[02])\/[12][0-9]{3})|((0[1-9]|[12][0-9]|30)\/(0[469]|11)\/[12][0-9]{3})|((0[1-9]|1[0-9]|2[0-8])\/02\/[12][0-9]([02468][1235679]|[13579][01345789]))|((0[1-9]|[12][0-9])\/02\/[12][0-9]([02468][048]|[13579][26]))/gi);
		return data.match(matchData);
	}
}

String.prototype.lpad = function (padString, length) {
    var str = String(this);
    while (str.length < length)
        str = padString + str;
    return String(str);
}

String.prototype.rpad = function (padString, length) {
    var str = String(this);
    while (str.length < length)
        str = str + padString;
    return String(str);
}

String.prototype.ltrim = function (trimChar) {
    if (typeof trimChar == 'undefined') trimChar = ' ';
    var str = String(this);
    var l = 0;
    while (l < str.length && str[l] == trimChar) { l++; }
    return str.substring(l, str.length);
}

String.prototype.rtrim = function (trimChar) {
    if (typeof trimChar == 'undefined') trimChar = ' ';
    var str = String(this);
    var r = str.length - 1;
    while (r > 0 && str[r] == ' ')
    { r -= 1; }
    return str.substring(0, r + 1);
}

String.prototype.trim = function (trimChar) {
    return String(this).rtrim(trimChar).ltrim(trimChar);
}

String.prototype.startsWith = function (str) {
    return this.substring(0, str.length) === str;
}

String.prototype.replaceAll = function(de, para){
	var str = this;
	var pos = str.indexOf(de);
	while (pos > -1){
		str = str.replace(de, para);
		pos = str.indexOf(de);
	}
	return (str);
}