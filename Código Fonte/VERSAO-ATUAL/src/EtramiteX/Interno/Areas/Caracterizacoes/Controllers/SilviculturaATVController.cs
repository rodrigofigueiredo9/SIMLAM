using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class SilviculturaATVController : DefaultController
	{
		#region Propriedades

		SilviculturaATVBus _bus = new SilviculturaATVBus();
		ListaBus _listaBus = new ListaBus();
		SilviculturaATVValidar _validar = new SilviculturaATVValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empBus = new EmpreendimentoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVCriar })]
		public ActionResult Criar(int id)
		{

			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			SilviculturaATV caracterizacao = _bus.ObterDadosGeo(id);
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.SilviculturaATV, eCaracterizacaoDependenciaTipo.Caracterizacao);
			SilviculturaATVVM vm = new SilviculturaATVVM(caracterizacao, _listaBus.SilviculturaAtvCoberturaExitente, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SilviculturaAtvCaracteristicaFomento);

			/*var areaTotal = vm.ObterArea(eSilviculturaAreaATV.AA_TOTAL_FLORESTA);
			areaTotal.Valor = vm.ObterArea(eSilviculturaAreaATV.AVN).Valor + vm.ObterArea(eSilviculturaAreaATV.AA_FLORESTA_PLANTADA).Valor;
			areaTotal.ValorTexto = areaTotal.Valor.ToStringTrunc();*/

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);

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
		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVCriar })]
		public ActionResult Criar(SilviculturaATV caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.SilviculturaATV,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.silviculturaatv)]
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

			SilviculturaATV caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.SilviculturaATV,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			SilviculturaATVVM vm = new SilviculturaATVVM(caracterizacao, _listaBus.SilviculturaAtvCoberturaExitente, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SilviculturaAtvCaracteristicaFomento);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);

			var emp = _empBus.Obter(id);
			var enderecoEmp = emp.Enderecos.Find(x => x.ZonaLocalizacaoId == (int)eEmpreendimentoLocalizacao.ZonaRural);

			if (enderecoEmp != null)
			{
				if (!string.IsNullOrEmpty(textoMerge))
				{
					vm.TemARL = _bus.TemARL(id);
					vm.TemARLDesconhecida = _bus.TemARLDesconhecida(id);
				}
			}
			else
			{
				vm.TemARL = true;
				vm.TemARLDesconhecida = false;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVEditar })]
		public ActionResult Editar(SilviculturaATV caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.SilviculturaATV,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.silviculturaatv)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			SilviculturaATV caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.SilviculturaATV,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			SilviculturaATVVM vm = new SilviculturaATVVM(caracterizacao, _listaBus.SilviculturaAtvCoberturaExitente, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SilviculturaAtvCaracteristicaFomento, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);

			vm.TemARL = true;
			vm.TemARLDesconhecida = false;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.SilviculturaAtvMsg.ExcluirMensagem;
			vm.Titulo = "Excluir Silvicultura - Implantação da Atividade de Silvicultura (Fomento)";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			if (_bus.Excluir(id))
			{
				urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SilviculturaATVCriar, ePermissao.SilviculturaATVEditar })]
		public ActionResult GeoMergiar(SilviculturaATV silvicultura)
		{
			SilviculturaATVVM vm = new SilviculturaATVVM(_bus.MergiarGeo(silvicultura), _listaBus.SilviculturaAtvCoberturaExitente, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SilviculturaAtvCaracteristicaFomento);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "SilviculturaATV", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
