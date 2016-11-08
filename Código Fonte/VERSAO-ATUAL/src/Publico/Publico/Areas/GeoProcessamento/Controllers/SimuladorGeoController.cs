using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMSimuladorGeo;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using CoordenadaTipo = Tecnomapas.Blocos.Entities.Configuracao.Interno.CoordenadaTipo;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class SimuladorGeoController : DefaultController
	{
		#region Propriedade

		SimuladorGeoBus _bus = new SimuladorGeoBus();
		SimuladorGeoArquivoBus _busArquivoSimuladorGeo = new SimuladorGeoArquivoBus();

		#endregion

		#region Simulador

		public ActionResult Verificar()
		{
			SimuladorGeoVM vm = new SimuladorGeoVM();
			CarregarDdls(vm);

			return View(vm);
		}

		[HttpPost]
		public ActionResult VerificarCpf(string cpf)
		{
			SimuladorGeoVM vm = new SimuladorGeoVM();
			CarregarDdls(vm);

			vm.SimuladorGeo = _bus.VerificarCpf(cpf);

			CarregarArquivosProcessados(vm);

			return Json(new { EhValido = Validacao.EhValido, @Msg = Validacao.Erros, Vm = vm }, JsonRequestBehavior.AllowGet);
			//return PartialView("VerificarPartial", vm);
		}

		private void CarregarDdls(SimuladorGeoVM vm)
		{
			GerenciadorConfiguracao<ConfiguracaoCoordenada> config = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());

			vm.SistemaCoordenada = ViewModelHelper.CriarSelectList((List<CoordenadaTipo>)config.Obter(ConfiguracaoCoordenada.KeyTiposCoordenada), selecionado: ((int)eCoordenadaTipo.UTM).ToString());
			vm.Datuns = ViewModelHelper.CriarSelectList((List<Datum>)config.Obter(ConfiguracaoCoordenada.KeyDatuns), selecionado: ((int)eCoordenadaDatum.SIRGAS2000).ToString());
			vm.Fusos = ViewModelHelper.CriarSelectList((List<Fuso>)config.Obter(ConfiguracaoCoordenada.KeyFusos), selecionado: "24");
		}

		private void CarregarArquivosProcessados(SimuladorGeoVM vm)
		{
			if (vm.SimuladorGeo == null || vm.SimuladorGeo.Arquivos == null || vm.SimuladorGeo.Arquivos.Count == 0)
			{
				return;
			}

			if (vm.SimuladorGeo.Arquivos.Exists(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.DadosGEOBASES))
			{
				vm.ArquivosVetoriais.RemoveAll(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.DadosGEOBASES);
				vm.ArquivosVetoriais.Add(new ArquivoItemGridVM(vm.SimuladorGeo.Arquivos.Single(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.DadosGEOBASES)));
			}

			#region Arquivos processados desse projeto

			List<SimuladorGeoArquivo> arquivos = vm.SimuladorGeo.Arquivos.Where(x =>
					x.Tipo != (int)eSimuladorGeoArquivoTipo.DadosIDAF &&
					x.Tipo != (int)eSimuladorGeoArquivoTipo.DadosGEOBASES &&
					x.Tipo != (int)eSimuladorGeoArquivoTipo.CroquiFinal).ToList();

			if (arquivos != null && arquivos.Count > 0)
			{
				foreach (SimuladorGeoArquivo item in arquivos)
				{
					if (item.Tipo == (int)eSimuladorGeoArquivoTipo.ArquivoEnviado && item.Situacao <= 0)
					{
						SimuladorGeoArquivo arquivoAux = arquivos.FirstOrDefault(x => x.Tipo == 4);
						if (arquivoAux != null && arquivoAux.Id > 0)
						{
							item.Situacao = arquivoAux.Situacao;
							item.SituacaoTexto = arquivoAux.SituacaoTexto;
						}
					}

					vm.ArquivosProcessados.Add(new ArquivoItemGridVM(item, vm.ArquivoEnviado.Tipo, (eSimuladorGeoSituacao)vm.SimuladorGeo.SituacaoId));
				}
			}

			#endregion
		}

		public ActionResult ObterSituacao(List<ArquivoItemGridVM> arquivos, int projetoId = 0, int arquivoEnviadoTipo = 0)
		{
			foreach (ArquivoItemGridVM arquivoVetorial in arquivos)
			{
				_bus.ObterSituacaoFila(arquivoVetorial.ArquivoProcessamento);

				arquivoVetorial.RegraBotoesGridVetoriais();
			}

			List<ArquivoItemGridVM> arquivosProcessados = new List<ArquivoItemGridVM>();

			if (projetoId > 0)
			{
				List<SimuladorGeoArquivo> listas = _bus.ObterArquivos(projetoId);

				foreach (SimuladorGeoArquivo item in listas.Where(x =>
					x.Tipo != (int)eSimuladorGeoArquivoTipo.DadosIDAF &&
					x.Tipo != (int)eSimuladorGeoArquivoTipo.DadosGEOBASES &&
					x.Tipo != (int)eSimuladorGeoArquivoTipo.CroquiFinal))
				{
					arquivosProcessados.Add(new ArquivoItemGridVM(item, arquivoEnviadoTipo));
				}
			}

			return Json(new { @lista = arquivos, @arquivosProcessados = arquivosProcessados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult EnviarProcessar(SimuladorGeoVM projetoVM)
		{
			projetoVM.SimuladorGeo.ArquivoEnviado = projetoVM.ArquivoEnviado;
			var arquivo = _bus.EnviarArquivo(projetoVM.SimuladorGeo);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
		}

		#endregion

		#region Importador

		public ActionResult CancelarProcessamentoImportador(ArquivoItemGridVM arquivo)
		{
			_bus.CancelarProcessamento(arquivo.ArquivoProcessamento);

			return Json(new { @Arquivo = arquivo.ArquivoProcessamento, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		#endregion

		#region Arquivo Vetorial

		[HttpPost]
		public ActionResult GerarArquivoVetorial(ArquivoItemGridVM arquivoVetorial)
		{
			_bus.ReprocessarBaseReferencia(arquivoVetorial.ArquivoProcessamento);

			arquivoVetorial.RegraBotoesGridVetoriais();

			return Json(new { @arquivo = arquivoVetorial, @EhValido = Validacao.EhValido });
		}

		#endregion

		#region Arquivo

		[HttpPost]
		public string Arquivo(HttpPostedFileBase file)
		{
			string msg = string.Empty;
			Arquivo arquivo = null;
			try
			{
				SimuladorGeoArquivoBus _bus = new SimuladorGeoArquivoBus();
				arquivo = _bus.SalvarTemp(file);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return ViewModelHelper.JsSerializer.Serialize(new { Msg = Validacao.Erros, Arquivo = arquivo });
		}

		public FileResult Baixar(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivo(_busArquivoSimuladorGeo.Obter(id));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public FileResult BaixarTemporario(string nomeTemporario, string contentType)
		{
			try
			{
				return ViewModelHelper.GerarArquivo(_busArquivoSimuladorGeo.ObterTemporario(new Arquivo() { TemporarioNome = nomeTemporario, ContentType = contentType }));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion

		#region Baixar arquivo de referencia

		public FilePathResult BaixarArquivoRef(int tipo = 1)
		{
			try
			{
				FilePathResult file = null;

				if (tipo == 1)
				{
					file = new FilePathResult("~/Content/_zipModeloGeo/ModeloShape.zip", ContentType.ZIP);
					file.FileDownloadName = "ModeloShape.zip";
				}
				else if (tipo == 2)
				{
					file = new FilePathResult("~/Content/_manuais/ManualElaboracaoProjetoGeografico-Simulador.pdf", ContentType.PDF);
					file.FileDownloadName = "ManualElaboracaoProjetoGeografico.pdf";
				}

				return file;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion
	}
}