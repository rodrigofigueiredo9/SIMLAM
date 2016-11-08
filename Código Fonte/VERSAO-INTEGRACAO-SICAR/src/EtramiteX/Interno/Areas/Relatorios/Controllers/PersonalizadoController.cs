using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.RelatorioPersonalizado.Business;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PersonalizadoController : DefaultController
	{
		#region Propriedades

		FatoBus _fatoBus = new FatoBus();
		RelatorioPersonalizadoBus _bus = new RelatorioPersonalizadoBus();
		RelatorioPersonalizadoValidar _validar = new RelatorioPersonalizadoValidar();

		public EtramiteIdentity UsuarioLogado
		{
			get { return (HttpContext.User.Identity as EtramiteIdentity); }
		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoListar })]
		public ActionResult Index()
		{
			PersonalizadoListarVM vm = new PersonalizadoListarVM(_fatoBus.ObterFonteDados());
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoListar })]
		public ActionResult Filtrar(PersonalizadoListarVM vm)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PersonalizadoListarVM>(vm.UltimaBusca).Filtros;
			}
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));

			Resultados<Relatorio> resultados = _bus.Filtrar(vm.Filtros);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeExecutar = User.IsInRole(ePermissao.RelatorioPersonalizadoExecutar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.RelatorioPersonalizadoEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.RelatorioPersonalizadoExcluir.ToString());
			vm.PodeExportar = User.IsInRole(ePermissao.RelatorioPersonalizadoExportar.ToString());
			vm.PodeAtribuirExecutor = User.IsInRole(ePermissao.RelatorioPersonalizadoAtribuirExecutor.ToString());
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Executar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult Executar(int id)
		{
			FuncionarioBus funcionarioBus = new FuncionarioBus();
			PersonalizadoExecutarVM vm = new PersonalizadoExecutarVM(funcionarioBus.ObterSetoresFuncionario(UsuarioLogado.FuncionarioId));
			vm.Relatorio = _bus.Obter(id);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult Executar(string paramsJson)
		{
			PersonalizadoExecutarVME parametro = ViewModelHelper.JsSerializer.Deserialize<PersonalizadoExecutarVME>(paramsJson);
			Arquivo arquivo = _bus.Executar(parametro.Id, parametro.Tipo, parametro.Setor, parametro.Termos);

			if (Validacao.EhValido)
			{
				return ViewModelHelper.GerarArquivo(arquivo);
			}

			#region Erro

			FuncionarioBus funcionarioBus = new FuncionarioBus();
			PersonalizadoExecutarVM vm = new PersonalizadoExecutarVM(funcionarioBus.ObterSetoresFuncionario(UsuarioLogado.FuncionarioId), parametro.Setor);
			vm.Relatorio = _bus.Obter(parametro.Id);

			vm.Relatorio.ConfiguracaoRelatorio.Termos.Where(x => x.DefinirExecucao).ToList().ForEach(x =>
			{
				x.Valor = (parametro.Termos.SingleOrDefault(y => y.Ordem == x.Ordem) ?? new Termo()).Valor;
			});

			return View("Executar", vm);

			#endregion
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult ValidarExecutar(int id, int tipo, int setor, List<Termo> termos)
		{
			_bus.ValidarExecutar(id, tipo, setor, termos);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Atribuir Executor

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult AtribuirExecutor(int id)
		{
			Relatorio relatorio = _bus.Obter(id);

			if (relatorio.UsuariosPermitidos.Count > 0)
			{
				FuncionarioBus funcionarioBus = new FuncionarioBus();
				List<Funcionario> lista = funcionarioBus.ObterFuncionarios(relatorio.UsuariosPermitidos.Select(x => x.Id).ToList());

				relatorio.UsuariosPermitidos.ForEach(x =>
				{
					x.Nome = lista.Single(y => y.Id == x.Id).Nome;
				});
			}

			return View(relatorio);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult AtribuirExecutor(Relatorio relatorio)
		{
			relatorio.UsuariosPermitidos.ForEach(x => 
			{
				x.Tipo = (int)eUsuarioRelatorioTipo.Interno;
			});

			_bus.AtribuirExecutor(relatorio);

			string urlRedirecionar = Url.Action("Index", "Personalizado", Validacao.QueryParamSerializer(new { acaoId = relatorio.Id }));
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirecionar = urlRedirecionar });
		}

		#endregion

		#region Exportar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExportar })]
		public ActionResult Exportar(int id)
		{
			Arquivo arquivo = _bus.Exportar(id);
			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region Importar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoImportar })]
		public ActionResult Importar()
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoImportar })]
		public string Importar(HttpPostedFileBase file)
		{
			string retorno = string.Empty;

			try
			{
				StreamReader ss = new StreamReader(file.InputStream, Encoding.Default);
				retorno = ss.ReadToEnd();

				return ViewModelHelper.JsSerializer.Serialize(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, conteudo = retorno });
			}
			catch (Exception)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
			}

			return ViewModelHelper.JsSerializer.Serialize(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoImportar })]
		public ActionResult ImportarSalvar(Relatorio relatorio)
		{
			Relatorio rel = _validar.Deserializar(relatorio);

			if (Validacao.EhValido && rel != null)
			{
				rel.ConfiguracaoRelatorio.Nome = relatorio.Nome;
				rel.ConfiguracaoRelatorio.Descricao = relatorio.Descricao;
				_bus.Importar(rel);
			}
			else
			{
				Validacao.Erros.Clear();
				Validacao.Add(Mensagem.RelatorioPersonalizado.ConfiguracaoInvalida);
			}

			string urlRedirecionar = Url.Action("Index", "Personalizado", Validacao.QueryParamSerializer());
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirecionar = urlRedirecionar });
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			Relatorio relatorio = _bus.Obter(id, true);

			vm.Id = id;
			vm.Mensagem = Mensagem.RelatorioPersonalizado.MensagemExcluir(relatorio.Nome);
			vm.Titulo = "Excluir Relatório";

			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar/Editar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar })]
		public ActionResult Criar()
		{
			PersonalizadoVM vm = new PersonalizadoVM(_fatoBus.ObterFonteDados());
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult Editar(int id)
		{
			PersonalizadoVM vm = new PersonalizadoVM(_fatoBus.ObterFonteDados());
			vm.ConfiguracaoRelatorio = _bus.Obter(id).ConfiguracaoRelatorio;
			vm.ConfiguracaoRelatorio.FonteDados = _fatoBus.Obter(vm.ConfiguracaoRelatorio.FonteDados.Id);
			vm.Id = id;

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterOpcoes(ConfiguracaoRelatorio configuracao, bool obterCamposFato = false)
		{
			PersonalizadoVM vm = new PersonalizadoVM(_fatoBus.ObterFonteDados());
			vm.ConfiguracaoRelatorio = configuracao;
		    
			vm.ConfiguracaoRelatorio.FonteDados = _fatoBus.Obter(vm.ConfiguracaoRelatorio.FonteDados.Id);
            
			if (obterCamposFato)
			{
				vm.ConfiguracaoRelatorio.CamposSelecionados.Clear();
			}

			return PartialView("Opcoes", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterOrdenarColunas(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.CamposSelecionados = vm.ConfiguracaoRelatorio.CamposSelecionados.OrderBy(x => x.Posicao).ToList();
			return PartialView("OrdenarColunas", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterOrdenarValores(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.FonteDados = _fatoBus.Obter(vm.ConfiguracaoRelatorio.FonteDados.Id);
			vm.DimensoesLst = ViewModelHelper.CriarSelectList(configuracao.FonteDados.DimensoesLst, false, false);
			return PartialView("OrdenarValores", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterFiltros(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.FonteDados = _fatoBus.Obter(vm.ConfiguracaoRelatorio.FonteDados.Id);
			vm.OperadoresLst = ViewModelHelper.CriarSelectList(_bus.ObterOperadores(null), false, false);
			return PartialView("Filtros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterSumarizar(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.CamposSelecionados = vm.ConfiguracaoRelatorio.CamposSelecionados
				.Where(x => x.Campo.TipoDados == (int)eTipoDados.Inteiro || x.Campo.TipoDados == (int)eTipoDados.Real)
				.OrderBy(x => x.Campo.Alias).OrderBy(x => x.Campo.DimensaoNome).ToList();

			return PartialView("Sumarizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterDimensionar(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.CamposSelecionados = vm.ConfiguracaoRelatorio.CamposSelecionados.OrderBy(x => x.Posicao).ToList();

			int soma = vm.ConfiguracaoRelatorio.CamposSelecionados.Sum(x => x.Tamanho);
			if (soma != 100 || (vm.ConfiguracaoRelatorio.CamposSelecionados.Count != vm.ConfiguracaoRelatorio.CamposSelecionados.Where(x => x.Tamanho > 0).Count()))
			{
				int larguraInical = 100 / vm.ConfiguracaoRelatorio.CamposSelecionados.Count;
				vm.ConfiguracaoRelatorio.CamposSelecionados.ForEach(x =>
				{
					x.Tamanho = larguraInical;
				});

				soma = vm.ConfiguracaoRelatorio.CamposSelecionados.Sum(x => x.Tamanho);
				if (soma > 100)
				{
					larguraInical = larguraInical - (soma - 100);
				}
				else if (soma < 100)
				{
					larguraInical = larguraInical + (100 - soma);
				}
				vm.ConfiguracaoRelatorio.CamposSelecionados.Last().Tamanho = larguraInical;
			}

			return PartialView("Dimensionar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterAgrupar(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM(_fatoBus.ObterFonteDados());
			vm.CaminhoImagem = Url.Content("~/Content/_img/exemp_agrup_relatorios.jpg");
			vm.ConfiguracaoRelatorio = configuracao;
			vm.ConfiguracaoRelatorio.FonteDados = _fatoBus.Obter(vm.ConfiguracaoRelatorio.FonteDados.Id);
			List<Lista> lista = new List<Lista>();

			foreach (var item in vm.ConfiguracaoRelatorio.FonteDados.CamposExibicao)
			{
				lista.Add(new Lista()
				{
					Id = item.Id.ToString(),
					Texto = item.DimensaoNome + " - " + item.Alias
				});
			}

			int selecionado = 0;
			if (configuracao.Agrupamentos != null && configuracao.Agrupamentos.Count > 0)
			{
				selecionado = configuracao.Agrupamentos.First().Campo.Id;
			}

			vm.CamposLst = ViewModelHelper.CriarSelectList(lista.OrderBy(x => x.Texto).ToList(), false, true, selecionado.ToString());

			return PartialView("Agrupar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterFinalizar(ConfiguracaoRelatorio configuracao)
		{
			PersonalizadoVM vm = new PersonalizadoVM();
			vm.ConfiguracaoRelatorio = configuracao;
			return PartialView("Finalizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult Finalizar(Relatorio relatorio)
		{
			_bus.Salvar(relatorio);

			string urlRedirecionar = Url.Action("Index", "Personalizado", Validacao.QueryParamSerializer(new { acaoId = relatorio.Id }));
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirecionar = urlRedirecionar });
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ObterCamposFiltro(Campo campo)
		{
			#region Mascara

			string mascara = string.Empty;

			switch (campo.TipoDadosEnum)
			{
				case eTipoDados.Inteiro:
					mascara = "maskNumInt";
					break;

				case eTipoDados.Real:
					mascara = "maskDecimal";
					break;

				case eTipoDados.Data:
					mascara = "maskData";
					break;

				default:
					break;
			}

			#endregion

			return Json(new { Mascara = mascara, Operadores = _bus.ObterOperadores(campo) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoCriar, ePermissao.RelatorioPersonalizadoEditar })]
		public ActionResult ValidarFiltros(ConfiguracaoRelatorio configuracao)
		{
			_validar.AnalisarTermos(configuracao.Termos);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}