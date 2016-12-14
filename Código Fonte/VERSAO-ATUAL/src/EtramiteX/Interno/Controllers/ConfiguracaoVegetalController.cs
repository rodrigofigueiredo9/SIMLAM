using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ConfiguracaoVegetalController : Controller
	{
		#region Propriedades

		GrupoQuimicoBus _grupoQuimicoBus = new GrupoQuimicoBus();
		ClasseUsoBus _classeUsoBus = new ClasseUsoBus();
		PericulosidadeAmbientalBus _periculosidadeAmbiental = new PericulosidadeAmbientalBus();
		ClassificacaoToxicologicaBus _classificacaoToxicologicaBus = new ClassificacaoToxicologicaBus();
		ModalidadeAplicacaoBus _modalidadeAplicacaoBus = new ModalidadeAplicacaoBus();
		FormaApresentacaoBus _formasApresentacao = new FormaApresentacaoBus();
		CulturaBus _culturaBus = new CulturaBus();
		PragaBus _pragaBus = new PragaBus();
		ListaBus _busLista = new ListaBus();
		CulturaValidar _validar = new CulturaValidar();
        DeclaracaoAdicionalBus _declaracao = new DeclaracaoAdicionalBus();

		#endregion

		public ActionResult Index()
		{
			return CadastrarModalidadeAplicacao();
		}

		#region Grupo Químico

		[Permite(RoleArray = new Object[] { ePermissao.GrupoQuimico })]
		public ActionResult CadastrarGrupoQuimico()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.GrupoQuimico);
			vm.UrlSalvar = Url.Action("SalvarGrupoQuimico", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarGrupoQuimico", "ConfiguracaoVegetal");
			vm.Itens = _grupoQuimicoBus.Listar();

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.GrupoQuimico })]
		public ActionResult EditarGrupoQuimico(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _grupoQuimicoBus.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.GrupoQuimico })]
		public ActionResult SalvarGrupoQuimico(ConfiguracaoVegetalItem grupoQuimico)
		{
			_grupoQuimicoBus.Salvar(grupoQuimico);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.GrupoQuimico) { Itens = _grupoQuimicoBus.Listar() };

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Classe de Uso

		[Permite(RoleArray = new Object[] { ePermissao.ClasseUso })]
		public ActionResult CadastrarClasseUso()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ClasseUso);
			vm.UrlSalvar = Url.Action("SalvarClasseUso", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarClasseUso", "ConfiguracaoVegetal");
			vm.Itens = _classeUsoBus.Listar();

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ClasseUso })]
		public ActionResult EditarClasseUso(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _classeUsoBus.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ClasseUso })]
		public ActionResult SalvarClasseUso(ConfiguracaoVegetalItem classeUso)
		{
			_classeUsoBus.Salvar(classeUso);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ClasseUso) { Itens = _classeUsoBus.Listar() };
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Periculosidade Ambiental

		[Permite(RoleArray = new Object[] { ePermissao.PericulosidadeAmbiental })]
		public ActionResult CadastrarPericulosidadeAmbiental()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.PericulosidadeAmbiental);
			vm.UrlSalvar = Url.Action("SalvarPericulosidadeAmbiental", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarPericulosidadeAmbiental", "ConfiguracaoVegetal");
			vm.Itens = _periculosidadeAmbiental.Listar();

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PericulosidadeAmbiental })]
		public ActionResult EditarPericulosidadeAmbiental(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _periculosidadeAmbiental.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PericulosidadeAmbiental })]
		public ActionResult SalvarPericulosidadeAmbiental(ConfiguracaoVegetalItem periculosidadeAmbiental)
		{
			_periculosidadeAmbiental.Salvar(periculosidadeAmbiental);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.PericulosidadeAmbiental) { Itens = _periculosidadeAmbiental.Listar() };
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Classificacao Toxicologica

		[Permite(RoleArray = new Object[] { ePermissao.ClassificacaoToxicologica })]
		public ActionResult CadastrarClassificacaoToxicologica()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ClassificacaoToxicologica);
			vm.Itens = _classificacaoToxicologicaBus.Listar();
			vm.UrlSalvar = Url.Action("SalvarClassificacaoToxicologica", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarClassificacaoToxicologica", "ConfiguracaoVegetal");

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ClassificacaoToxicologica })]
		public ActionResult EditarClassificacaoToxicologica(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _classificacaoToxicologicaBus.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ClassificacaoToxicologica })]
		public ActionResult SalvarClassificacaoToxicologica(ConfiguracaoVegetalItem classificacaoToxicologica)
		{
			_classificacaoToxicologicaBus.Salvar(classificacaoToxicologica);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ClassificacaoToxicologica) { Itens = _classificacaoToxicologicaBus.Listar() };
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Modalidade Aplicacao

		[Permite(RoleArray = new Object[] { ePermissao.ModalidadeAplicacao })]
		public ActionResult CadastrarModalidadeAplicacao()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ModalidadeAplicacao);
			vm.Itens = _modalidadeAplicacaoBus.Listar();
			vm.UrlSalvar = Url.Action("SalvarModalidadeAplicacao", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarModalidadeAplicacao", "ConfiguracaoVegetal");

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ModalidadeAplicacao })]
		public ActionResult EditarModalidadeAplicacao(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _modalidadeAplicacaoBus.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ModalidadeAplicacao })]
		public ActionResult SalvarModalidadeAplicacao(ConfiguracaoVegetalItem modalidadeAplicacao)
		{
			_modalidadeAplicacaoBus.Salvar(modalidadeAplicacao);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.ModalidadeAplicacao) { Itens = _modalidadeAplicacaoBus.Listar() };

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Formas Apresentacao

		[Permite(RoleArray = new Object[] { ePermissao.FormaApresentacao })]
		public ActionResult CadastrarFormasApresentacao()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.FormasApresentacao);
			vm.Itens = _formasApresentacao.Listar();
			vm.UrlSalvar = Url.Action("SalvarFormasApresentacao", "ConfiguracaoVegetal");
			vm.UrlEditar = Url.Action("EditarFormasApresentacao", "ConfiguracaoVegetal");

			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.FormaApresentacao })]
		public ActionResult EditarFormasApresentacao(int id)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @ConfiguracaoVegetalItem = _formasApresentacao.Obter(id) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FormaApresentacao })]
		public ActionResult SalvarFormasApresentacao(ConfiguracaoVegetalItem formasApresentacao)
		{
			_formasApresentacao.Salvar(formasApresentacao);
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.FormasApresentacao) { Itens = _formasApresentacao.Listar() };

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Grid = ViewModelHelper.RenderPartialViewToString(this.ControllerContext, "GridConfiguracaoVegetal", vm) });
		}

		#endregion

		#region Ingrediente Ativo

		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult IngredientesAtivos(bool associar = false)
		{
			IngredienteAtivoListarVM vm = new IngredienteAtivoListarVM(_busLista.QuantPaginacao, _busLista.IngredienteAtivoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			if (associar)
			{
				vm.PodeAssociar = true;
				return PartialView("IngredienteAtivo/ListarFiltros", vm);
			}

			return View("IngredienteAtivo/Listar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult FiltrarIngredientesAtivos(IngredienteAtivoListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<IngredienteAtivoListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();
			Resultados<ConfiguracaoVegetalItem> resultados = ingredienteAtivoBus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = true;
				vm.PodeAlterarSituacao = true;
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "IngredienteAtivo/ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult CadastrarIngredienteAtivo()
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.IngredienteAtivo);
			vm.MostrarGrid = false;
			vm.UrlSalvar = Url.Action("SalvarIngredienteAtivo", "ConfiguracaoVegetal");
			vm.UrlCancelar = Url.Action("IngredientesAtivos", "ConfiguracaoVegetal");
			return View("ConfiguracaoVegetal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult EditarIngredienteAtivo(int id)
		{
			IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();

			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.IngredienteAtivo, "Editar");
			vm.MostrarGrid = false;
			vm.UrlSalvar = Url.Action("SalvarIngredienteAtivo", "ConfiguracaoVegetal");
			vm.UrlCancelar = Url.Action("IngredientesAtivos", "ConfiguracaoVegetal");

			vm.ConfiguracaoVegetalItem = ingredienteAtivoBus.Obter(id);

			return View("ConfiguracaoVegetal", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult SalvarIngredienteAtivo(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			string urlRedirecionar = Url.Action((ingredienteAtivo.Id > 0) ? "IngredientesAtivos" : "CadastrarIngredienteAtivo", "ConfiguracaoVegetal");
			IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();
			ingredienteAtivoBus.Salvar(ingredienteAtivo);
			urlRedirecionar += "?Msg=" + Validacao.QueryParam();
			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				UrlRedirecionar = urlRedirecionar
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult AlterarSituacaoIngredienteAtivo(int id)
		{
			ConfiguracaoVegetalVM vm = new ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo.IngredienteAtivo);

			vm.UrlCancelar = Url.Action("IngredientesAtivos", "ConfiguracaoVegetal");
			IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();
			vm.ConfiguracaoVegetalItem = ingredienteAtivoBus.Obter(id);
			vm.SetListItens(_busLista.IngredienteAtivoSituacoes.Where(x => x.Id != vm.ConfiguracaoVegetalItem.SituacaoId).ToList());

			return View("IngredienteAtivo/AlterarSituacao", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.IngredienteAtivo })]
		public ActionResult AlterarSituacaoIngredienteAtivo(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();
			ingredienteAtivoBus.AlterarSituacao(ingredienteAtivo);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				UrlRedirecionar = Url.Action("IngredientesAtivos", "ConfiguracaoVegetal") + "?Msg=" + Validacao.QueryParam()
			});
		}

		#endregion

		#region Cultura

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		public ActionResult CadastrarCultura()
		{
			CulturaVM vm = new CulturaVM(new Cultura(), _busLista.CultivarTipos, _busLista.DeclaracaoAdicional);
			ViewBag.Titulo = "Cadastrar Cultura";

			return View("Cultura/Criar", vm);
		}

		public ActionResult DeclaracaoAdicional(Cultivar item)
		{
			item.LsCultivarConfiguracao = item.LsCultivarConfiguracao ?? new List<CultivarConfiguracao>();

			#region Lita Pragas
			List<ListaValor> listaPragas = new List<ListaValor>();
			_pragaBus.ObterPragas(item.IdRelacionamento).ForEach(x =>
			{
				listaPragas.Add(new ListaValor()
				{
					Id = x.Id,
					Texto = x.NomeCientifico + (string.IsNullOrEmpty(x.NomeComum) ? "" : " - " + x.NomeComum)
				});
			});
			#endregion

			CulturaVM vm = new CulturaVM(item.LsCultivarConfiguracao, listaPragas, _busLista.CultivarTipos, _busLista.DeclaracaoAdicional);
			vm.Cultura.Id = item.Id;
			vm.Cultura.Nome = item.CulturaTexto;
			vm.Cultura.NomeCultivar = item.Nome;			

			return View("Cultura/DeclaracaoAdicionalPartial", vm);
		}

		public ActionResult ValidarDeclaracaoAdicional(CultivarConfiguracao item, List<CultivarConfiguracao> lista)
		{
			_validar.ValidarConfiguracaoes(item, lista);
			return Json(new 
			{ 
				@EhValido = Validacao.EhValido, 
				@Msg = Validacao.Erros 
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		public ActionResult EditarCultura(int id)
		{
			CulturaVM vm = new CulturaVM(_culturaBus.Obter(id), _busLista.CultivarTipos, _busLista.DeclaracaoAdicional);
			ViewBag.Titulo = "Editar Cultura";

			return View("Cultura/Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		public ActionResult Culturas()
		{
			CulturaListarVM vm = new CulturaListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Cultura/Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PragaAssociarCultura })]
		public ActionResult AssociarCultura(bool straggCultivar = true)
		{
			CulturaListarVM vm = new CulturaListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.Associar = true;
			vm.StraggCultivar = straggCultivar;
			return PartialView("Cultura/ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		public ActionResult FiltrarCultura(CulturaListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<CulturaListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);
			vm.Filtros.StraggCultivar = vm.StraggCultivar;

			Resultados<CulturaListarResultado> resultados = _culturaBus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = !vm.Associar;

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Cultura/ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		public ActionResult SalvarCultura(Cultura cultura)
		{
			string urlRetorno = cultura.Id < 1 ? Url.Action("CadastrarCultura", "ConfiguracaoVegetal") : Url.Action("Culturas", "ConfiguracaoVegetal");

			_culturaBus.Salvar(cultura);
			urlRetorno += "?Msg=" + Validacao.QueryParam();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = urlRetorno });
		}

		[Permite(RoleArray = new Object[] { ePermissao.Cultura })]
		

		#endregion

		#region Praga

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult CadastrarPraga()
		{
			PragaVM vm = new PragaVM();
			ViewBag.Titulo = "Cadastrar Praga";
			return View("Praga/Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult EditarPraga(int id)
		{
			PragaVM vm = new PragaVM();

			vm.Praga = _pragaBus.Obter(id);
			ViewBag.Titulo = "Editar Praga";
			return View("Praga/Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult Pragas()
		{
			PragaListarVM vm = new PragaListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Praga/Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult AssociarPraga()
		{
			PragaListarVM vm = new PragaListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.PodeAssociar = false;
			vm.Associar = true;
			return PartialView("Praga/ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult FiltrarPraga(PragaListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PragaListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Praga> resultados = _pragaBus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = !vm.Associar;
			vm.PodeAssociar = User.IsInRole(ePermissao.PragaAssociarCultura.ToString()) && !vm.Associar;

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Praga/ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.Praga })]
		public ActionResult SalvarPraga(Praga praga)
		{
			string urlRetorno = praga.Id < 1 ? Url.Action("CadastrarPraga", "ConfiguracaoVegetal") : Url.Action("Pragas", "ConfiguracaoVegetal");
			_pragaBus.Salvar(praga);
			urlRetorno += "?Msg=" + Validacao.QueryParam();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = urlRetorno });
		}

		[Permite(RoleArray = new Object[] { ePermissao.PragaAssociarCultura })]
		public ActionResult AssociarCulturas(int pragaId)
		{
			PragaVM vm = new PragaVM();
			vm.Praga = _pragaBus.Obter(pragaId);
			vm.Praga.Culturas = _pragaBus.ObterCulturas(pragaId);
			return View("Praga/AssociarCulturas", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PragaAssociarCultura })]
		public ActionResult AssociarCulturasSalvar(Praga praga)
		{
			string urlRetorno = Url.Action("Pragas", "ConfiguracaoVegetal");
			_pragaBus.AssociarCulturas(praga);
			urlRetorno += "?Msg=" + Validacao.QueryParam();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = urlRetorno });
		}

		#endregion

        #region Declaração Adicional
        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult SalvarDeclaracaoAdicional(DeclaracaoAdicional declaracao)
        {
            string urlRetorno = declaracao.Id < 1 ? Url.Action("CadastrarDeclaracaoAdicional", "ConfiguracaoVegetal") : Url.Action("Declaracoes", "ConfiguracaoVegetal");
            _declaracao.Salvar(declaracao);
            urlRetorno += "?Msg=" + Validacao.QueryParam();
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = urlRetorno });
        }

        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult CadastrarDeclaracaoAdicional(DeclaracaoAdicional declaracao)
        {
            DeclaracaoAdicionalVM vm = new DeclaracaoAdicionalVM();
            ViewBag.Titulo = "Cadastrar Declaração Adicional";
            return View("DeclaracaoAdicional/Criar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult Declaracoes()
        {
            DeclaracaoAdicionalListarVM vm = new DeclaracaoAdicionalListarVM(_busLista.QuantPaginacao);
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            return View("DeclaracaoAdicional/Index", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult ExcluirConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            vm.Id = id;
            vm.Mensagem = Mensagem.Declaracao.MensagemExcluir(id.ToString());
            vm.Titulo = "Excluir Declaração Adicional";
            return PartialView("Excluir", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult Excluir(int id)
        {
            bool excluido = _declaracao.Excluir(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Excluido = excluido }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.Praga })]
        public ActionResult EditarDeclaracaoAdicional(int id)
        {
            DeclaracaoAdicionalVM vm = new DeclaracaoAdicionalVM();

            vm.DeclaracaoAdicional = _declaracao.Obter(id);
            ViewBag.Titulo = "Editar Declaração Adicional";
            return View("DeclaracaoAdicional/Criar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.DeclaracaoAdicional })]
        public ActionResult FiltrarDeclaracao(DeclaracaoAdicionalListarVM vm, Paginacao paginacao)
        {
            if (!String.IsNullOrEmpty(vm.UltimaBusca))
            {
                vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<DeclaracaoAdicionalListarVM>(vm.UltimaBusca).Filtros;
            }

            vm.Paginacao = paginacao;
            vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

            Resultados<DeclaracaoAdicional> resultados = _declaracao.Filtrar(vm.Filtros, vm.Paginacao);
            if (resultados == null)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }


            vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
            vm.Paginacao.EfetuarPaginacao();
            vm.Resultados = resultados.Itens;

            return Json(new
            {
                @Msg = Validacao.Erros,
                @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DeclaracaoAdicional/ListarResultados", vm)
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}