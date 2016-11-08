using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMDominialidade;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.SharedVM;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class DominialidadeController : DefaultController
	{
		#region Propriedades

		DominialidadeBus _bus = new DominialidadeBus(new DominialidadeValidar());
		DominialidadeInternoBus _busInterno = new DominialidadeInternoBus();
		DominialidadeValidar _validar = new DominialidadeValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoCredenciadoBus _busEmpreendimento = new EmpreendimentoCredenciadoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar })]
		public ActionResult Criar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			Dominialidade caracterizacao = _bus.ObterDadosGeo(id);
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId: id, projetoDigitalId: projetoDigitalId, caracterizacaoTipo: (int)eCaracterizacao.Dominialidade) || !_validar.Dominios(caracterizacao.Dominios))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			int zona = _busEmpreendimento.Obter(id, simplificado: false).Enderecos
																			.Where(z => z.Correspondencia == 0)
																			.Select(z => z.ZonaLocalizacaoId).FirstOrDefault() ?? 0;

			caracterizacao.Dominios.ForEach(x => x.EmpreendimentoLocalizacao = zona);

			caracterizacao.EmpreendimentoId = id;
			caracterizacao.EmpreendimentoInternoId = _busEmpreendimento.ObterEmpreendimento(id, simplificado: true).InternoId.GetValueOrDefault();
			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, ListaCredenciadoBus.BooleanLista);
			vm.ProjetoDigitalId = projetoDigitalId;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar })]
		public ActionResult Criar(Dominialidade caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao, caracterizacao.ProjetoDigitalId, null);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = caracterizacao.ProjetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			if (!_caracterizacaoValidar.Dependencias(id, projetoDigitalId, (int)eCaracterizacao.Dominialidade))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			if (!_validar.ProjetoFinalizado(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			Dominialidade caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);

				if (!_validar.Dominios(caracterizacao.Dominios))
				{
					return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
				}
			}

			foreach (var dominio in caracterizacao.Dominios)
			{
				foreach (var reserva in dominio.ReservasLegais)
				{
					if (!LocalizacoesReserva(reserva, (int)dominio.Tipo).Any(x => x.Id == reserva.LocalizacaoId.ToString()))
					{
						reserva.LocalizacaoId = 0;
					}
				}
			}

			caracterizacao.EmpreendimentoInternoId = _busEmpreendimento.ObterEmpreendimento(id, simplificado: true).InternoId.GetValueOrDefault();
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, ListaCredenciadoBus.BooleanLista);
			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.ProjetoDigitalId = projetoDigitalId;
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeEditar })]
		public ActionResult Editar(Dominialidade caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			_bus.Salvar(caracterizacao, caracterizacao.ProjetoDigitalId, projetoDigitalCredenciadoBus.AlterarCaracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = caracterizacao.ProjetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		public ActionResult Visualizar(int id, int projetoDigitalId, bool retornarVisualizar = true)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			if (!_caracterizacaoValidar.Dependencias(id, projetoDigitalId, (int)eCaracterizacao.Dominialidade))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			Dominialidade caracterizacao = _bus.ObterPorEmpreendimento(id, projetoDigitalId);
			caracterizacao.EmpreendimentoInternoId = _busEmpreendimento.ObterEmpreendimento(id, simplificado: true).InternoId.GetValueOrDefault();
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, ListaCredenciadoBus.BooleanLista, true);
			vm.ProjetoDigitalId = projetoDigitalId;
			vm.RetornarVisualizar = retornarVisualizar;
			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.Dominialidade.ExcluirMensagem;
			vm.Titulo = "Excluir Dominialidade";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			//if (_bus.Excluir(id))
			//{
			//	urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			//}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Dominio/Reserva Legal

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult Dominio(Dominio dominio)
		{
			DominioVM vm = new DominioVM(ListaCredenciadoBus.DominialidadeComprovacoes, dominio, dominio.ComprovacaoId);
			return PartialView("DominioPartial", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		public ActionResult DominioVisualizar(Dominio dominio)
		{
			DominioVM vm = new DominioVM(ListaCredenciadoBus.DominialidadeComprovacoes, dominio, dominio.ComprovacaoId, true);
			return PartialView("DominioPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ReservaLegal(ReservaLegal reservaLegal, int? dominioTipo, int empInternoID)
		{
			List<Lista> dominiosCompensacao = new List<Lista>();
			List<Lista> lstIdentificacaoARL = new List<Lista>();

			if (reservaLegal.EmpreendimentoCompensacao.Id > 0)
			{
				dominiosCompensacao = _busInterno.ObterDominiosLista(reservaLegal.EmpreendimentoCompensacao.Id);
				lstIdentificacaoARL = _busInterno.ObterARLCompensacaoLista(empInternoID, reservaLegal.MatriculaId.GetValueOrDefault());
			}

			ReservaLegalVM vm =
				new ReservaLegalVM(
					ListaCredenciadoBus.DominialidadeReservaSituacoes,
					ListaCredenciadoBus.DominialidadeReservaLocalizacoes,
					ListaCredenciadoBus.DominialidadeReservaCartorios,
					reservaLegal, dominioTipo: dominioTipo.GetValueOrDefault(0),
					lstTiposCoordenada: ListaCredenciadoBus.TiposCoordenada,
					lstDatuns: ListaCredenciadoBus.Datuns,
					lstMatriculaCompensacao: dominiosCompensacao,
					lstIdentificacaoARLCompensacao: lstIdentificacaoARL,
					booleanLista: ListaCredenciadoBus.BooleanLista,
					lstSituacoesVegetal: ListaCredenciadoBus.DominialidadeReservaSituacaoVegetacao);
			return PartialView("ReservaLegalPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterDadosEmpreendimentoCompensacao(int empreendimentoInternoId)
		{
			List<Lista> dominios = _busInterno.ObterDominiosLista(empreendimentoInternoId);
			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @Dominios = dominios });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterCompensacaoARL(int empreendimentoId, int dominio)
		{
			List<Lista> compensacoesARL = _busInterno.ObterARLCompensacaoLista(empreendimentoId, dominio);
			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @Compensacoes = compensacoesARL });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		public ActionResult ReservaLegalVisualizar(ReservaLegal reservaLegal, int empInternoID)
		{
			List<Lista> dominiosCompensacao = new List<Lista>();
			List<Lista> lstIdentificacaoARL = new List<Lista>();

			if (reservaLegal.EmpreendimentoCompensacao.Id > 0)
			{
				dominiosCompensacao = _busInterno.ObterDominiosLista(reservaLegal.EmpreendimentoCompensacao.Id);
				lstIdentificacaoARL = _busInterno.ObterARLCompensacaoLista(empInternoID, reservaLegal.MatriculaId.GetValueOrDefault());
			}

			ReservaLegalVM vm = new ReservaLegalVM(
				ListaCredenciadoBus.DominialidadeReservaSituacoes,
				ListaCredenciadoBus.DominialidadeReservaLocalizacoes,
				ListaCredenciadoBus.DominialidadeReservaCartorios,
				reservaLegal,
				true,
				0,
				ListaCredenciadoBus.TiposCoordenada,
				ListaCredenciadoBus.Datuns,
				booleanLista: ListaCredenciadoBus.BooleanLista,
				lstMatriculaCompensacao: dominiosCompensacao,
				lstIdentificacaoARLCompensacao: lstIdentificacaoARL,
				lstSituacoesVegetal: ListaCredenciadoBus.DominialidadeReservaSituacaoVegetacao);
			return PartialView("ReservaLegalPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult DominioValidarSalvar(Dominio dominio)
		{
			_validar.DominioSalvar(dominio);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ReservaLegalValidarSalvar(ReservaLegal reserva, List<ReservaLegal> reservasAdicionadas)
		{
			if (reservasAdicionadas != null && reservasAdicionadas.Count > 0)
			{
				if (reserva.IdentificacaoARLCedente > 0 && reservasAdicionadas.Exists(r => r.IdentificacaoARLCedente == reserva.IdentificacaoARLCedente))
				{
					Validacao.Add(Mensagem.Dominialidade.RLAssociadaEmOutroEmpreendimentoCedente(0, ""));
				}
			}

			_validar.ReservaLegalSalvar(reserva);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult AtualizarGrupoARL(Dominialidade caracterizacao)
		{
			_bus.ObterDominialidadeARL(caracterizacao);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, new List<Lista>());

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DominialidadeARLPartial", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterARLCedente(int reservaCedenteId)
		{

			Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal reservaCedente = null;

			reservaCedente = _busInterno.ObterReservaLegal(reservaCedenteId);

			if (reservaCedente == null)
			{
				return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros });
			}

			var reservaJson = new
			{
				@SituacaoVegetalTexto = reservaCedente.SituacaoVegetalTexto,
				@SituacaoVegetalId = reservaCedente.SituacaoVegetalId,
				@Area = reservaCedente.ARLCroqui
			};

			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @ReservaCedenteJson = ViewModelHelper.Json(reservaJson) });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		private List<Lista> LocalizacoesReserva(ReservaLegal reserva, int dominioTipo = 0)
		{
			List<Lista> localizacoes = ListaCredenciadoBus.DominialidadeReservaLocalizacoes;

			if (!string.IsNullOrEmpty(reserva.Identificacao))
			{
				if (reserva.Compensada)
				{
					localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente).ToString() }).Contains(x.Id)).ToList(); ;
				}
				else
				{
					if (dominioTipo == (int)eDominioTipo.Posse)
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaPosse).ToString()).ToList(); ;
					}
					else if (dominioTipo == (int)eDominioTipo.Matricula)
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaMatricula).ToString()).ToList(); ;
					}
				}
			}
			else
			{
				localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora).ToString() }).Contains(x.Id)).ToList();
			}

			return localizacoes;
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult GeoMergiar(Dominialidade caracterizacao)
		{
			DominialidadeVM vm = new DominialidadeVM(_bus.MergiarGeo(caracterizacao), ListaCredenciadoBus.BooleanLista);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DominialidadePartial", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}