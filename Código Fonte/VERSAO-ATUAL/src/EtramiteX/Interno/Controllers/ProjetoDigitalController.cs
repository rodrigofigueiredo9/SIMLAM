using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRequerimento.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ProjetoDigitalController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		ProjetoDigitalBus _bus = new ProjetoDigitalBus();
		ProjetoDigitalValidar _validar = new ProjetoDigitalValidar();
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult Index()
		{
			ProjetoDigitalListarVM vm = new ProjetoDigitalListarVM();
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult Filtrar(ProjetoDigitalListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ProjetoDigitalListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao);

			vm.Filtros.Situacao = (int)eRequerimentoSituacao.Finalizado;
            vm.Filtros.IsRemoverTituloDeclaratorio = true;
			vm.Filtros.ProjetoDigitalSituacoes = new List<eProjetoDigitalSituacao>() { eProjetoDigitalSituacao.AguardandoImportacao, eProjetoDigitalSituacao.AguardandoCorrecao };
			Resultados<Requerimento> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeImportar = User.IsInRole(ePermissao.ProjetoDigitalImportar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.ProjetoDigitalVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar })]
		public ActionResult Importar(int id)
		{
			Requerimento requerimento = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}

			requerimento.Pessoas.ForEach(r=>
			{
				if (r.InternoId <= 0)
				{
					r.SelecaoTipo = (int)eExecutorTipo.Credenciado;
				}
			});

			ProjetoDigitalVM vm = new ProjetoDigitalVM(requerimento);
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Requerimento requerimento = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			ProjetoDigitalVM vm = new ProjetoDigitalVM(requerimento, true);
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult ObterObjetivoPedido(int id)
		{
			ProjetoDigitalVM vm = new ProjetoDigitalVM();

			if (id != 0)
			{
				Requerimento requerimento = _bus.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);

					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
				}
			}

			return PartialView("ObjetivoPedido", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar })]
		public ActionResult ValidarObjetivoPedido(Requerimento requerimento)
		{
			requerimento.Atividades = _bus.Obter(requerimento.Id).Atividades;
			_validar.ObjetivoPedido(requerimento);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}
	

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult ObterPessoa(int id, string cnpfCnpj, bool isVisualizar = false, List<Pessoa> pessoas = null)
		{
			RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();
			Requerimento requerimento = requerimentoCredenciadoBus.ObterSimplificado(id);

			SalvarVM vm = new SalvarVM();
			PessoaCredenciadoBus credenciadoBus = new PessoaCredenciadoBus();
			Pessoa credenciado = credenciadoBus.Obter(cnpfCnpj, credenciadoId: requerimento.CredenciadoId);

			if (credenciado.Id > 0 && pessoas != null && pessoas.Exists(x => x.CPFCNPJ == credenciado.CPFCNPJ))
			{
				vm.PessoaCredenciado = credenciado.GerarLista();
			}

			vm.Pessoa.SelecaoTipo = (int)eExecutorTipo.Credenciado;
			vm.Pessoa.CPFCNPJ = cnpfCnpj;

			return PartialView("PessoaComparar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult ObterResponsavel(int id)
		{
			ProjetoDigitalVM vm = new ProjetoDigitalVM();

			if (id != 0)
			{
				Requerimento requerimento = _bus.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);
					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
				}
			}

			return PartialView("ResponsavelTecnico", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult ObterEmpreendimento(int id, bool isVisualizar = false)
		{
			ViewModels.VMEmpreendimento.SalvarVM vm = new ViewModels.VMEmpreendimento.SalvarVM();
			EmpreendimentoCredenciadoBus credenciadoBus = new EmpreendimentoCredenciadoBus();
			Empreendimento credenciado = credenciadoBus.Obter(id);
			EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();
			empreendimentoBus.ConfigurarResponsaveis(credenciado);

			vm.EmpreendimentoCredenciado = credenciado.GerarLista();
			vm.Empreendimento.SelecaoTipo = (int)eExecutorTipo.Credenciado;

			return PartialView("EmpreendimentoComparar", vm);
		}

		#region Finalizar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult ObterFinalizar(Requerimento requerimento)
		{
			requerimento = _bus.GerarObjeto(requerimento);
			ProjetoDigitalVM vm = new ProjetoDigitalVM(requerimento);
			vm.IsAbaFinalizar = true;
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return PartialView("Finalizar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult Finalizar(Requerimento requerimento)
		{
			_bus.Importar(requerimento);

			if (!Validacao.EhValido)
			{
				return Json(new { EhValido = false, Msg = Validacao.Erros });
			}

			string urlRedirecionar = Url.Action("", "ProjetoDigital", Validacao.QueryParamSerializer(new { acaoId = requerimento.Id }));
			return Json(new { EhValido = true, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar })]
		public ActionResult Recusar()
		{
			return View("MotivoRecusa", new MotivoRecusaVM() { IsVisualizar= false});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalImportar })]
		public ActionResult Recusar(int requerimentoId, string motivo)
		{
			Requerimento req = _bus.Obter(requerimentoId);
			req.ProjetoDigital.MotivoRecusa = motivo;

			_bus.Recusar(req.ProjetoDigital);
			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Index", "ProjetoDigital", new { Msg = Validacao.QueryParam() })
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult VisualizarNotificacao(int requerimentoId)
		{
			MotivoRecusaVM viewModel = new MotivoRecusaVM(true);
			viewModel.Motivo = (_bus.Obter(requerimentoId).ProjetoDigital ?? new ProjetoDigital()).MotivoRecusa;
			
			return View("MotivoRecusa", viewModel);
		}
		
		#endregion

		#region Pdfs

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar, ePermissao.ProjetoDigitalImportar, ePermissao.ProjetoDigitalVisualizar })]
		public ActionResult GerarPdfRequerimento(int id)
		{
			try
			{
				bool isCredenciado = false;
				PdfRequerimentoPadraoCredenciado pdf = new PdfRequerimentoPadraoCredenciado();

				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, out isCredenciado), "Requerimento Digital");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return RedirectToAction("", "ProjetoDigital", Validacao.QueryParamSerializer());
		}

		#endregion
	}
}