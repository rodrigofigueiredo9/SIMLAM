using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Interno.Areas.GeoProcessamento.ViewModels.VMMapa;
using Tecnomapas.EtramiteX.Interno.Model.ModuloGeoProcessamento.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class MapaController : DefaultController
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoord = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEnd = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());

		private List<Estado> Estados
		{
			get { return _configEnd.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados); }
		}

		private String EstadoDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault); }
		}

		#region Mapa de Coordenada

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult CoordenadaPartial()
		{
		    CoordenadaVM vm = CriarCoordenadaVM();
			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/Coordenada/");
			return PartialView(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult CoordenadaTela()
		{
            CoordenadaVM vm = CriarCoordenadaVM();

			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/Coordenada/");
			return View(vm);
		}

		[HttpGet]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public JsonResult LocalizarLogradouro(String logradouro)
		{
			MapaCoordenadaBus bus = new MapaCoordenadaBus();
			List<Logradouro> result = bus.ObterLogradouros(logradouro);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public JsonResult ObterEstadosMunicipiosPorCoordenada(String easting, String northing)
		{
			//CrossDomain case
			RequestJson requestJson = new RequestJson();
			ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();
			List<Estado> lstEstados = new List<Estado>();
			List<Municipio> lstMunicipios = new List<Municipio>();
			Municipio municipio = null;

			try
			{
				resposta = requestJson.Executar<dynamic>(_configCoord.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada) + "?easting=" + easting + "&northing=" + northing);

				if (resposta.Erros != null && resposta.Erros.Count > 0)
				{
					Validacao.Erros.AddRange(resposta.Erros);
					return Json(new { Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}

				var objJson = resposta.Data;
				if (objJson["EstaNoEstado"] && (objJson["Municipio"] == null || Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0) == 0))
				{
					Validacao.Add(Mensagem.Mapas.MunicipioSemRetorno);
				}

				if (!Validacao.EhValido)
				{
					return Json(new { Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}

				int codigoIbge = 0;
				if (objJson["Municipio"] != null)
				{
					codigoIbge = Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0);
				}

				ListaValoresDa _da = new ListaValoresDa();
				municipio = _da.ObterMunicipio(codigoIbge);

				if (municipio.Estado.Sigla != EstadoDefault)
				{
					lstEstados = Estados.Where(x => x.Texto != EstadoDefault).ToList();
					lstMunicipios = new List<Municipio>();
				}
				else
				{
					lstEstados = Estados;
					lstMunicipios = _configEnd.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios)[municipio.Estado.Id];
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Estados = lstEstados,
				@Municipios = lstMunicipios,
				@Municipio = municipio
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AreaAbrangenciaPartial()
		{
            CoordenadaVM vm = CriarCoordenadaVM();

			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/AreaAbrangencia/nav_baseref.swf");
			return PartialView(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AreaAbrangenciaFiscPartial()
		{
			CoordenadaVM vm = new CoordenadaVM();
			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/AreaAbrangenciaFisc/nav_baseref.swf");
			return PartialView(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult DesenhadorPartial(int modo = 1)
		{
			CoordenadaVM vm = new CoordenadaVM();
			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/Desenhador/DesenhadorIdaf.swf?ver=" + new System.Random().Next(1,2000).ToString() );
			vm.Modo = modo;
			return PartialView(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult DesenhadorTeste(int modo = 2)
		{
			CoordenadaVM vm = new CoordenadaVM();
			vm.CoordenadaGeoUrl = Url.Content("~/Areas/Navegadores/Desenhador/DesenhadorIdaf.swf");
			vm.Modo = modo;
			
			return View("DesenhadorTeste", vm);
		}

		[Permite(Tipo=ePermiteTipo.Logado)]
		public ActionResult ObterConfiguracoes()
		{
			var retorno = new { webserviceURL = "http://devap2/projetos/Etramite2010/IDAF/Desenvolvimento/DesenhadorWebServices"};
			return Json(retorno, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AtualizarSessao()
		{
			return Json(new { Msgs = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

        public CoordenadaVM CriarCoordenadaVM()
        {
            CoordenadaVM vm = new CoordenadaVM();

            vm.DesenhadorWebserviceURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyDesenhadorWebserviceURL);
            vm.MunicipioWebserviceURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyMunicipioWebserviceURL);
            vm.GeoprocessamentoWebserviceURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyGeoprocessamentoWebserviceURL);
            vm.MapaTematicoURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyMapaTematicoURL);
            vm.AeroLevantamentoMapaImagemURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyAeroLevantamentoMapaImagemURL);
            vm.DevEmpreendimentoMapaLoteURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyDevEmpreendimentoMapaLoteURL);
            vm.FiscalMapaLoteURL = _configSys.Obter<String>(ConfiguracaoSistema.KeyFiscalMapaLoteURL);

            return vm;
        }

		#endregion
	}
}