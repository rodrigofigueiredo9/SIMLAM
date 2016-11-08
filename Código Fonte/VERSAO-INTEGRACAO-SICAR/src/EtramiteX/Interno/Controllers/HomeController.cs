using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSobre.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloIndicador.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMHome;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMManual;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMSobre;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class HomeController : DefaultController
	{
		RelatorioIndicadorBus _bus = new RelatorioIndicadorBus();
		PermissaoValidar _permissaoValidar = new PermissaoValidar();

		SobreBus _busSobre = new SobreBus();

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Index()
		{
			if (User == null || !User.Identity.IsAuthenticated)
			{
				return RedirectToAction("LogOn", "Autenticacao");
			}

			IndicadorPeriodoRelatorio indicadores = new IndicadorPeriodoRelatorio();
			IndicadoresVM viewModel = new IndicadoresVM();

			viewModel.Titulos = _bus.BuscarTitulosIndicadores();
			viewModel.Condicionantes = _bus.BuscarCondicionantesIndicadores();

			viewModel.Exibir = _permissaoValidar.ValidarAny(new[] {
			        ePermissao.TituloRelatorioIndicadoresTitulos,
			        ePermissao.TituloRelatorioIndicadoresTitulosCondicionantes	
			    }, false);

			viewModel.CalcularRelatorio();
			
			return View(viewModel);
		}

		public ActionResult SistemaMensagem()
		{			
			return View();
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Manual()
		{
			ManualVM vm = new ManualVM();

			System.Web.HttpContext.Current.Cache["_manuais"] = System.Web.HttpContext.Current.Cache["_manuais"] ?? Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("~/Content/_manuais"));
			string[] listManuais = System.Web.HttpContext.Current.Cache["_manuais"] as string[];

			foreach (var item in listManuais)
			{
				if (Path.GetExtension(item).ToLower().IndexOf("swf") > -1)
					continue;

				vm.Itens.Add(new ManualItemVM() { Titulo = Path.GetFileNameWithoutExtension(item), Arquivo = Path.GetFileName(item) });
			}

			return PartialView("ManualPartial", vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Sobre()
		{
			SobreVM vm = new SobreVM();

			vm.Sobre = _busSobre.Obter();

			return PartialView("SobrePartial", vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult SobreItens()
		{
			SobreVM vm = new SobreVM();

			vm.Sobre = _busSobre.Obter();

			vm.Sobre.Itens = _busSobre.ObterSobreItens(vm.Sobre.Id);

			vm.Versoes = _busSobre.ObterVersoes();
			
			return PartialView("SobreItensPartial", vm);
		}

		[HttpPost]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterSobreItens(int versaoId)
		{
			return Json(new { Itens = _busSobre.ObterSobreItens(versaoId) });
		}

		public ActionResult Log(int json = 0)
		{
			LogBus log = new LogBus();
			LogVM vm = new LogVM(log.ObterListSource());

			vm.Log.Resultados = log.Obter(vm.Log);

			if (json > 0)
			{
				return Json(vm, JsonRequestBehavior.AllowGet);
			}

			return View("Log", vm);
		}

		[HttpPost]
		public ActionResult Log(LogVM vm)
		{
			LogBus log = new LogBus();
			Blocos.Entities.Home.Log filtros = vm.Log;

			vm = new LogVM(log.ObterListSource());

			if (!string.IsNullOrEmpty(filtros.DataDe) && !ValidacoesGenericasBus.ValidarData(filtros.DataDe))
			{
				Validacao.Add(Mensagem.Padrao.DataInvalida("Log_DataDe", "\"Data de\""));
			}

			if (!string.IsNullOrEmpty(filtros.DataAte) && !ValidacoesGenericasBus.ValidarData(filtros.DataAte))
			{
				Validacao.Add(Mensagem.Padrao.DataInvalida("Log_DataAte", "\"Data até\""));
			}

			if (Validacao.EhValido)
			{
				vm.Log.Resultados = log.Obter(filtros);
			}

			return View("Log", vm);
		}


        private EtramiteX.Interno.Model.ModuloPTV.Business.PTVBus _busPTV = new EtramiteX.Interno.Model.ModuloPTV.Business.PTVBus();

        public ActionResult Teste(int id)
        {

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
            }, JsonRequestBehavior.AllowGet);

        }

		#region Teste

		public ActionResult ObterServicos()
		{
			string strCnf = HttpContext.Server.MapPath("~/Areas/Navegadores/");

			DirectoryInfo dir = new DirectoryInfo(strCnf);

			List<FileInfo> lstCnf = dir.GetFiles("*.xml", SearchOption.AllDirectories).ToList();
			List<dynamic> lstUrls = new List<dynamic>();

			lstCnf.ForEach(cnfFile => {

				XDocument doc = XDocument.Load(cnfFile.FullName);

				doc.Descendants("service").Elements()
					.Where(x => x.Name != "versaoMapa" && x.Name != "versaoMXD")
					.ToList().ForEach(x =>
				{
					lstUrls.Add(new { FileName = cnfFile.FullName, Name = x.Name.ToString(), Value = x.Value });
				});

			});

			lstUrls = lstUrls.Distinct().ToList();

			return Json(lstUrls, JsonRequestBehavior.AllowGet);
		}

		public string Clear()
		{
			List<string> lstKeys = new List<string>();

			var dic = System.Web.HttpContext.Current.Cache.GetEnumerator();
			while (dic.MoveNext())
			{
				if (dic.Key.ToString().IndexOf("Tecnomapas.EtramiteX.Configuracao") > -1)
				{
					lstKeys.Add(dic.Key.ToString());
					System.Web.HttpContext.Current.Cache.Remove(dic.Key.ToString());
				}
			}

			lstKeys.Sort();
			lstKeys.Add("[Limpo]");

			return  String.Join("</br>", lstKeys.ToArray());
		}

		public ActionResult ObterTID()
		{
			string tid = Tecnomapas.Blocos.Data.GerenciadorTransacao.ObterIDAtual().ToUpper();

			return Json(tid, JsonRequestBehavior.AllowGet);
		}

		/*public string MetodosS(string ns)
		{
			string str = string.Empty;
			
			System.Reflection.Assembly assembly = typeof(Tecnomapas.EtramiteX.Interno.Controllers.HomeController).Assembly;

			Type[] tipos = assembly.GetTypes();
			tipos = tipos.Where(x => x.BaseType.FullName == "System.Web.Mvc.Controller").ToArray();

			foreach (var item in tipos)
			{
				foreach (var method in item.GetMethods())
				{
					if (method.DeclaringType.FullName != item.FullName)
						continue;

					Object[] attrs = method.GetCustomAttributes(false);

					if (attrs.Any(x=> x is Tecnomapas.EtramiteX.Interno.Model.Security.PermiteAttribute))
						continue;

					str = String.Format("{0}</br>{1}-{2}", str, item.Name, method.Name);
				}
			}
			
			return str;
		}*/

		//[Permite()]
		//public ActionResult Teste()
		//{
		//    return View();
		//}

		//[Permite()]
		//public ActionResult Teste2()
		//{
		//    return View();
		//}

		//var q = from t in Assembly.GetExecutingAssembly().GetTypes()
		//where t.IsClass && t.Namespace == @namespace
		//select t;
		//q.ToList().ForEach(t => Console.WriteLine(t.Name));

		//[Permite()]
		//public ActionResult ObterTextoPdf(string ns)
		//{
		//	List<System.Reflection.Assembly> lstAssembly = new List<System.Reflection.Assembly>() { 
		//		typeof(Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Entities.Funcionario).Assembly,
		//		typeof(Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Entities.Especificidade).Assembly,
		//		typeof(Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.Entities.DadoFiltro).Assembly
		//	};

		//	IEnumerable<Type> lstTypes = lstAssembly.SelectMany(asm => asm.GetTypes().ToList());


		//	//Type tipo = lstAssembly.Find(x => x.GetType(ns, false, true) != null).GetType(ns, false, true);
		//	/*if (tipo == null)
		//	{
		//		throw new Exception("Tipo nao encontrado");
		//	}*/

		//	var gerador = new Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.AposeMergeFieldDoc();

		//	lstTypes = lstTypes.Where(x => !String.IsNullOrEmpty(x.Namespace) && x.Namespace.Contains(ns) && x.Namespace.Contains("Entities"));

		//	gerador.Abrir();

		//	foreach (var item in lstTypes)
		//	{
		//		gerador.Gerar(item);		 
		//	}

		//	System.IO.MemoryStream ms = gerador.Retornar();

		//	try
		//	{
		//		FileContentResult pdf = new FileContentResult(ms.ToArray(), "application/msword");
		//		pdf.FileDownloadName = "tagDocs.docx";
		//		return pdf;
		//	}
		//	catch (Exception exc)
		//	{
		//		Validacao.AddErro(exc);
		//	}
		//	finally
		//	{
		//		ms.Close();
		//		ms.Dispose();
		//	}
		//	return null;
		//}

		//[Permite()]
		//public string ValoresCache()
		//{
		//    //List<Configuracao.Situacao> lstSituacao = _confg.Obter<List<Configuracao.Situacao>>(Configuracao.ConfiguracaoFuncionario.KeySituacoes);

		//    //List<Configuracao.Situacao> lstSituacaoTitulo = _confgTitulo.Obter<List<Configuracao.Situacao>>(Configuracao.ConfiguracaoTitulo.KeySituacoes);

		//    return String.Empty;
		//}

		//[Permite()]
		//public string WebService()
		//{
		//    RequestJson requestJson = new RequestJson();

		//    GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		//    string urlGeoBasesWebService = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices);
		//    string geoBasesChave = _config.Obter<string>(ConfiguracaoSistema.KeyGeoBasesWebServicesAutencicacaoChave);

		//    requestJson.Executar(urlGeoBasesWebService + "/Autenticacao/LogOn", RequestJson.GET, new { chaveAutenticacao = geoBasesChave });

		//    Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ProjetoGeografico projeto = new Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ProjetoGeografico();

		//    string mbrWkt = String.Format("POLYGON (({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))",
		//        projeto.MenorX.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MenorY.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MaiorX.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MaiorY.ToString(NumberFormatInfo.InvariantInfo));

		//    ResponseJsonData<dynamic> resp = requestJson.Executar<dynamic>(urlGeoBasesWebService + "/Ortofoto/ObterOrtofoto", RequestJson.POST, new { wkt = "POLYGON ((392689.489822233 7962798.10302158, 392680.706953136 7961384.061097, 393119.850407976 7961360.64011274, 393523.862386431 7961331.36388242, 393913.236249723 7961299.16002906, 394027.413547982 7961293.30478299, 393907.381003657 7962748.33343003, 393828.335181788 7962768.82679126, 393433.10607243 7962780.53728339, 393049.587455203 7962792.24777552, 392689.489822233 7962798.10302158))" });

		//    projeto.ArquivosOrtofotos = new List<Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ArquivoProjeto>();

		//    foreach (var item in resp.Data)
		//    {
		//        projeto.ArquivosOrtofotos.Add(new Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ArquivoProjeto() { Nome = item["ArquivoNome"], Chave = item["ArquivoChave"] });
		//    }

		//    /*RequestJson request = new RequestJson();
			
		//    GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		//    string urlGeoBasesWebService = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices);


		//    Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ProjetoGeografico projeto = new Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Entities.ProjetoGeografico();

		//    projeto.MenorX = 1.0M;
		//    projeto.MenorY = 1.0M;
		//    projeto.MaiorX = 1.0M;
		//    projeto.MaiorY = 1.0M;


		//    string mbrWkt = String.Format("POLYGON (({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))", 
		//        projeto.MenorX.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MenorY.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MaiorX.ToString(NumberFormatInfo.InvariantInfo),
		//        projeto.MaiorY.ToString(NumberFormatInfo.InvariantInfo));

		//    request.Executar(urlGeoBasesWebService + "/Autenticacao/LogOn", RequestJson.GET, new { chaveAutenticacao = "A8AwSQesGQrpwpOyyFizGKynKeqaja8FWAny70mzZH90zWF9bF5izUCqykKSd9vO" });

		//    ResponseJsonData<dynamic> resp = request.Executar<dynamic>(urlGeoBasesWebService + "/Ortofoto/ObterOrtofoto", RequestJson.POST, new { wkt = "POLYGON ((392689.489822233 7962798.10302158, 392680.706953136 7961384.061097, 393119.850407976 7961360.64011274, 393523.862386431 7961331.36388242, 393913.236249723 7961299.16002906, 394027.413547982 7961293.30478299, 393907.381003657 7962748.33343003, 393828.335181788 7962768.82679126, 393433.10607243 7962780.53728339, 393049.587455203 7962792.24777552, 392689.489822233 7962798.10302158))" });
		//    */
		//    return String.Empty;
		//}

		//[Permite()]
		//public ActionResult ModeloTitulo()
		//{
		//    return View();
		//}

		//[Permite()]
		//public string Oracle()
		//{	
		//    //DaHelper.ObterLista(""

		//    for (int i = 0; i < 105; i++)
		//    {
		//        String retorno = String.Empty;

		//        IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.valor from cnf_sistema c where lower(c.campo) = 'geobaseswebservices'");

		//        foreach (var item in daReader)
		//        {
		//            retorno = item["valor"].ToString();
		//            break;
		//        }
		//    }
		//    return "Foi";
		//}

		//[Permite()]
		//public string Web()
		//{

		//    RequestJson req = new RequestJson();


		//    string url = req.Executar("http://idaf.simlam.com.br/webservices/DesenhadorWebServices/Projeto/BuscarDadosProjeto", RequestJson.POST, new { idProjeto=14 });


		//    return "Foi";
		//}

		//public String ObterNumeroAco()
		//{
		//	//char[] sufixoChars = "BBZ".ToArray();
		//	//byte[] sufixoBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sufixoChars);

		//	//double[] dbSufixo = new double[sufixoBytes.Length];

		//	//for (int i = 0; i < sufixoBytes.Length; i++)
		//	//{
		//	//	dbSufixo[i] = sufixoBytes[i]) % 26;
		//	//}

		//	//double a = dbSufixo.ToList().Sum();
		//	//a++;



		//	//char[] sufixoCharsConvert = a.ToString().ToArray();


		//	return null;
		//}

		#endregion
	}
}