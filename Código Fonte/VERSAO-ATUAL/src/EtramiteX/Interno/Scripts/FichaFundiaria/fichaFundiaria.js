/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

FichaFundiaria = {
	settings: {
		urls: {
			salvar: ''
		}
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(FichaFundiaria.settings, options); }
		FichaFundiaria.container = MasterPage.getContent(container);

		FichaFundiaria.container.delegate('.btnSalvar', 'click', FichaFundiaria.salvar);
	},

	obter: function () {
		var container = FichaFundiaria.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			Codigo: $('.hdnCodigo', container).val(),
			ProtocoloGeral: $('.txtProtocoloGeral', container).val(),
			ProtocoloRegional: $('.txtProtocoloRegional', container).val(),
			ConfrontanteNorte: $('.txtConfrontanteNorte', container).val(),
			ConfrontanteSul: $('.txtConfrontanteSul', container).val(),
			ConfrontanteLeste: $('.txtConfrontanteLeste', container).val(),
			ConfrontanteOeste: $('.txtConfrontanteOeste', container).val(),
			Observacoes: $('.txtObservacoes', container).val(),
			Requerente: {},
			Terreno: {},
			EscrituraCondicional: {},
			EscrituraDefinitiva: {}
		};

		obj.Requerente.Nome = $('.txtRequerenteNome', container).val();
		obj.Requerente.DocumentoTipo = $('.txtRequerenteDocumentoTipo', container).val();
		obj.Requerente.DocumentoNumero = $('.txtRequerenteDocumentoNumero', container).val();
		obj.Requerente.NomePai = $('.txtRequerenteNomePai', container).val();
		obj.Requerente.NomeMae = $('.txtRequerenteNomeMae', container).val();
		obj.Requerente.Endereco = $('.txtRequerenteEndereco', container).val();

		obj.Terreno.Municipio = $('.txtTerrenoMunicipio', container).val();
		obj.Terreno.Distrito = $('.txtTerrenoDistrito', container).val();
		obj.Terreno.Lugar = $('.txtTerrenoLugar', container).val();
		obj.Terreno.Tipo = $('.txtTerrenoTipo', container).val();
		obj.Terreno.DataMedicao = $('.txtTerrenoDataMedicao', container).val();
		obj.Terreno.Area = $('.txtTerrenoArea', container).val();
		obj.Terreno.Perimetro = $('.txtTerrenoPerimetro', container).val();
		obj.Terreno.Lote = $('.txtTerrenoLote', container).val();
		obj.Terreno.Quadra = $('.txtTerrenoQuadra', container).val();
		obj.Terreno.NomeTopografo = $('.txtTerrenoNomeTopografo', container).val();

		obj.EscrituraCondicional.Data = $('.txtEscrituraCondicionalData', container).val();
		obj.EscrituraCondicional.Livro = $('.txtEscrituraCondicionalLivro', container).val();
		obj.EscrituraCondicional.Folha = $('.txtEscrituraCondicionalFolha', container).val();

		obj.EscrituraDefinitiva.Data = $('.txtEscrituraDefinitivaData', container).val();
		obj.EscrituraDefinitiva.Livro = $('.txtEscrituraDefinitivaLivro', container).val();
		obj.EscrituraDefinitiva.Folha = $('.txtEscrituraDefinitivaFolha', container).val();

		return obj;

	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: FichaFundiaria.settings.urls.salvar,
			data: JSON.stringify(FichaFundiaria.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, FichaFundiaria.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(FichaFundiaria.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}