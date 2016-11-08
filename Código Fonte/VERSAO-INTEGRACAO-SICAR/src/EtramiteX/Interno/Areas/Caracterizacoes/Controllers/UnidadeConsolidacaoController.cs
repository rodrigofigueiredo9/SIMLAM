using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using UCBusCred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class UnidadeConsolidacaoController : DefaultController
	{
		#region Propriedades

		UnidadeConsolidacaoBus _bus = new UnidadeConsolidacaoBus();
		ListaBus _listaBus = new ListaBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empreendimentoBus = new EmpreendimentoBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		UnidadeConsolidacaoValidar _validar = new UnidadeConsolidacaoValidar();
		PragaBus _pragaBus = new PragaBus();
		UnidadeProducaoValidar _validarUnidadeProducao = new UnidadeProducaoValidar();
		CulturaBus _culturaBus = new CulturaBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();
			caracterizacao.Empreendimento.Id = id;

			if (!_validar.Acessar(caracterizacao.Empreendimento.Id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());
			vm.LstCultivar = ViewModelHelper.CriarSelectList(new List<Lista>());

			vm.UnidadeConsolidacao = caracterizacao;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar })]
		public ActionResult Criar(UnidadeConsolidacao caracterizacao)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.unidadeconsolidacao)]
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

			UnidadeConsolidacao caracterizacao = _bus.ObterPorEmpreendimento(id);
			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.UnidadeConsolidacao = caracterizacao;
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult Editar(UnidadeConsolidacao caracterizacao)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.unidadeconsolidacao)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			UnidadeConsolidacao caracterizacao = _bus.ObterPorEmpreendimento(id);

			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.IsVisualizar = true;
			vm.UnidadeConsolidacao = caracterizacao;
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());
			vm.UrlRetorno = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id });

			return View(vm);
		}

		#endregion

		#region VisualizarCredenciado

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult VisualizarCredenciado(int projetoDigitalId, int protocoloId = 0)
		{
			ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
			ProjetoDigital projeto = _busProjetoDigitalCredenciado.Obter(projetoDigitalId);

			UCBusCred.UnidadeConsolidacaoBus _busCredenciado = new UCBusCred.UnidadeConsolidacaoBus();
			UnidadeConsolidacao caracterizacao = _busCredenciado.ObterPorEmpreendimento(projeto.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId);

			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.IsVisualizar = true;
			vm.UnidadeConsolidacao = caracterizacao;
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());

			vm.ProtocoloId = protocoloId;
			vm.ProjetoDigitalId = projeto.Id;
			vm.RequerimentoId = projeto.RequerimentoId;
			vm.UrlRetorno = Url.Action("Analisar", "../AnaliseItens", new { protocoloId = protocoloId, requerimentoId = projeto.RequerimentoId });

			return View("Visualizar", vm);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar, ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult ObterResponsavelTecnico(int credenciadoId, List<Cultivar> cultivares)
		{
			HabilitarEmissaoCFOCFOC credenciadoHabilitado = new HabilitarEmissaoCFOCFOCBus().ObterPorCredenciado(credenciadoId);
			if (Validacao.EhValido)
			{
				credenciadoHabilitado.Pragas.ForEach(x =>
				{
					x.Praga.Culturas = _pragaBus.ObterCulturas(x.Praga.Id);
				});

				_validar.ValidarAssociarResponsavelTecnicoHabilitado(credenciadoHabilitado, cultivares);
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Habilitacao = credenciadoHabilitado
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar, ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult AdicionarCultivar(List<Cultivar> cultivarLista, Cultivar cultivar)
		{
			_validar.ValidarCultivar(cultivarLista, cultivar);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar, ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult AdicionarResponsavelTecnico(List<ResponsavelTecnico> responsavelTecnicoLista, ResponsavelTecnico responsavelTecnico)
		{
			_validar.ValidarResponsavelTecnico(responsavelTecnicoLista, responsavelTecnico);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ObterLstCultivares(int culturaId)
		{
			List<Lista> lstCultivares = _culturaBus.ObterLstCultivar(culturaId);

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Valido = Validacao.EhValido,
				@LstCultivar = lstCultivares
			});
		}


		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.UnidadeConsolidacao.ExcluirMensagem;
			vm.Titulo = "Excluir Unidade de Consolidação";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoExcluir })]
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
	}
}