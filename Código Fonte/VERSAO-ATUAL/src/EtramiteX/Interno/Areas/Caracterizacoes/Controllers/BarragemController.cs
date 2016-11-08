using System;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class BarragemController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		BarragemBus _bus = new BarragemBus();
		BarragemValidar _validar = new BarragemValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empBus = new EmpreendimentoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			Barragem barragem = new Barragem();
			barragem.EmpreendimentoId = id;

			if (!_validar.Acessar(barragem.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			barragem = _bus.ObterPorEmpreendimento(id);

			if (barragem.Id > 0)
			{
				return RedirectToAction("Editar", "Barragem", new { id = id, Msg = Validacao.QueryParam() });
			}

			barragem.EmpreendimentoId = id;

			barragem.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.Barragem, eCaracterizacaoDependenciaTipo.Caracterizacao);

			BarragemVM vm = new BarragemVM();
			vm.Barragem = barragem;
			vm.Atividades = ViewModelHelper.CriarSelectList(_listaBus.AtividadesSolicitada.Where(x => x.Id == barragem.AtividadeId).ToList(), true, true, selecionado: barragem.AtividadeId.ToString());
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			var emp = _empBus.Obter(id);
			var enderecoEmp = emp.Enderecos.Find(x => x.ZonaLocalizacaoId == (int)eEmpreendimentoLocalizacao.ZonaRural);

			if (enderecoEmp != null)
			{
				vm.TemARL = _bus.TemARL(id);
				vm.TemARLDesconhecida = _bus.TemARLDesconhecida(id);
			}
			else
			{
				vm.TemARL = true;
				vm.TemARLDesconhecida = false;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar })]
		public ActionResult Criar(Barragem barragem, int? barragemItemIdEdicao)
		{
			barragem.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(barragem.EmpreendimentoId, eCaracterizacao.Barragem, eCaracterizacaoDependenciaTipo.Caracterizacao);

			barragem.Id = _bus.Salvar(barragem);

			if (Validacao.EhValido)
			{
				barragem = _bus.Obter(barragem.Id);
				BarragemItem item = null;

				if (barragemItemIdEdicao.GetValueOrDefault(0) > 0)
				{
					item = barragem.Barragens.Find(x => x.Id == barragemItemIdEdicao);
				}
				else
				{
					item = barragem.Barragens.OrderByDescending(x => x.Id).First();
				}

				return Json(new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					@BarragemItem = item,
					@TotalLamina = barragem.TotalLamina,
					@TotalArmazenado = barragem.TotalArmazenado,
					@BarragemId = barragem.Id
				}, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros
				}, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar, ePermissao.BarragemVisualizar })]
		public ActionResult CriarBarragemItem(int empreendimentoId)
		{
			BarragemItemVM vm = new BarragemItemVM();
			BarragemItem barragemItem = new BarragemItem();
			vm.BarragemItem = barragemItem;
			vm.Finalidades = ViewModelHelper.CriarSelectList(_listaBus.BarragemFinalidades);
			vm.Outorgas = ViewModelHelper.CriarSelectList(_listaBus.BarragemOutorgas);
			vm.CoordenadaAtividadeVM = new CoordenadaAtividadeVM(barragemItem.CoordenadaAtividade, _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Barragem, (eTipoGeometria)barragemItem.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, false);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "BarragemItemPartial", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragem)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Barragem barragem = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				barragem.EmpreendimentoId,
				(int)eCaracterizacao.Barragem,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				barragem.Dependencias);

			BarragemVM vm = new BarragemVM();
			vm.Barragem = barragem;
			vm.Atividades = ViewModelHelper.CriarSelectList(_listaBus.AtividadesSolicitada.Where(x => x.Id == barragem.AtividadeId).ToList(), true, true, selecionado: barragem.AtividadeId.ToString());
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoMerge = textoMerge;

			var emp = _empBus.Obter(id);
			var enderecoEmp = emp.Enderecos.Find(x => x.ZonaLocalizacaoId == 2/*Rural*/);

			if (enderecoEmp != null)
			{
				vm.TemARL = _bus.TemARL(id);
				vm.TemARLDesconhecida = _bus.TemARLDesconhecida(id);
			}
			else
			{
				vm.TemARL = true;
				vm.TemARLDesconhecida = false;
			}

			return View(vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragem)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Barragem barragem = _bus.ObterPorEmpreendimento(id);

			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				barragem.EmpreendimentoId,
				(int)eCaracterizacao.Barragem,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				barragem.Dependencias, true);

			BarragemVM vm = new BarragemVM();
			vm.Barragem = barragem;
			vm.Atividades = ViewModelHelper.CriarSelectList(_listaBus.AtividadesSolicitada.Where(x => x.Id == barragem.AtividadeId).ToList(), true, true, selecionado: barragem.AtividadeId.ToString());
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.IsVisualizar = true;
			vm.IsEditar = false;

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoMerge = textoMerge;

			vm.TemARL = true;
			vm.TemARLDesconhecida = false;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.BarragemMsg.ExcluirMensagem;
			vm.Titulo = "Excluir Barragem";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			if (_bus.Excluir(id))
			{
				urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemExcluir })]
		public ActionResult ExcluirBarragemItem(int barragemItemId, int barragemId)
		{
			Barragem barragem = _bus.Obter(barragemId);
			if (barragem.Barragens.Count > 1)
			{
				_bus.ExcluirBarragemItem(barragemItemId);
				barragem = _bus.Obter(barragemId);
			}
			else
			{
				Validacao.Add(Mensagem.BarragemMsg.BarragemUltimoItemListaObrigatorio);
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@TotalLamina = barragem.TotalLamina,
				@TotalArmazenado = barragem.TotalArmazenado
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.BarragemExcluir })]
		public ActionResult ExcluirBarragemItemConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			vm.Id = id;
			vm.Mensagem = Mensagem.BarragemMsg.ExcluirBarragemItemConfirm;
			vm.Titulo = "Excluir Barragem";
			return PartialView("Excluir", vm);
		}

		#endregion

		#region Auxiliares

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar, ePermissao.BarragemVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragemitem)]
		public ActionResult EditarBarragemItem(int id, int empreendimentoId, int barragemId)
		{
			BarragemItemVM vm = new BarragemItemVM();
			BarragemItem barragemItem = _bus.Obter(barragemId).Barragens.Find(x => x.Id == id);
			vm.BarragemItem = barragemItem;
			vm.IsEditar = true;
			vm.BarragemItem = _bus.Obter(barragemId).Barragens.SingleOrDefault(x => x.Id == id);
			vm.Finalidades = ViewModelHelper.CriarSelectList(_listaBus.BarragemFinalidades);
			vm.Outorgas = ViewModelHelper.CriarSelectList(_listaBus.BarragemOutorgas);

			vm.CoordenadaAtividadeVM = new CoordenadaAtividadeVM(barragemItem.CoordenadaAtividade, _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Barragem, (eTipoGeometria)barragemItem.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Identificador = barragemItem.BarragensDados.Max(x => x.Identificador) + 1,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "BarragemItemPartial", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar, ePermissao.BarragemVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragemitem)]
		public ActionResult VisualizarBarragemItem(int id, int empreendimentoId, int barragemId)
		{
			BarragemItemVM vm = new BarragemItemVM();
			BarragemItem barragemItem = _bus.Obter(barragemId).Barragens.Find(x => x.Id == id);
			vm.BarragemItem = barragemItem;
			vm.IsVisualizar = true;
			vm.BarragemItem = _bus.Obter(barragemId).Barragens.SingleOrDefault(x => x.Id == id);
			vm.Finalidades = ViewModelHelper.CriarSelectList(_listaBus.BarragemFinalidades);
			vm.Outorgas = ViewModelHelper.CriarSelectList(_listaBus.BarragemOutorgas);

			vm.CoordenadaAtividadeVM = new CoordenadaAtividadeVM(barragemItem.CoordenadaAtividade, _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Barragem, (eTipoGeometria)barragemItem.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, true);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Identificador = barragemItem.BarragensDados.Max(x => x.Identificador) + 1,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "BarragemItemPartial", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar, ePermissao.BarragemVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.BarragemCriar, ePermissao.BarragemEditar, ePermissao.BarragemVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Barragem, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}