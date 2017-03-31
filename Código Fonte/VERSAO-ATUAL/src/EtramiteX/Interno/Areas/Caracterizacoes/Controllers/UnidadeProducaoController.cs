using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Cred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class UnidadeProducaoController : DefaultController
	{
		#region Propriedades

		UnidadeProducaoBus _bus = new UnidadeProducaoBus();
		ListaBus _listaBus = new ListaBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empreendimentoBus = new EmpreendimentoBus();
		EmpreendimentoCredenciadoBus _empreendimentoCredenciadoBus = new EmpreendimentoCredenciadoBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		UnidadeProducaoValidar _validar = new UnidadeProducaoValidar();
		HabilitarEmissaoCFOCFOCBus _busHabilitacaoCFOCFOC = new HabilitarEmissaoCFOCFOCBus();
		PragaBus _pragaBus = new PragaBus();
		CulturaBus _culturaBus = new CulturaBus();
		Cred.UnidadeProducaoBus _busCredenciado = new Cred.UnidadeProducaoBus();
		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			UnidadeProducao caracterizacao = new UnidadeProducao();
			caracterizacao.Empreendimento.Id = id;

			if (!_validar.Acessar(caracterizacao.Empreendimento.Id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			UnidadeProducaoVM vm = new UnidadeProducaoVM();

			vm.UnidadeProducao.Empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar })]
		public ActionResult Criar(UnidadeProducao caracterizacao)
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

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.unidadeproducao)]
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

			UnidadeProducao caracterizacao = _bus.ObterPorEmpreendimento(id);
			UnidadeProducaoVM vm = new UnidadeProducaoVM();

			vm.UnidadeProducao = caracterizacao;
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(UnidadeProducao caracterizacao)
		{
			Cultura cultura = null;
			Cultivar cultivar = null;

			foreach (var unidade in caracterizacao.UnidadesProducao)
			{
				foreach (var responsavel in unidade.ResponsaveisTecnicos)
				{
					cultura = new Cultura() { Id = unidade.CulturaId, Nome = unidade.CulturaTexto };
					cultivar = new Cultivar() { Id = unidade.CultivarId.GetValueOrDefault(), Nome = unidade.CultivarTexto };

					var validacaoResponsavelTecnico = this.ObterResponsavelTecnico(responsavel.Id, cultura, cultivar);
					if (!Validacao.EhValido)
					{
						return validacaoResponsavelTecnico;
					}
				}
			}

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

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.unidadeproducao)]
		public ActionResult Visualizar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id, isVisualizar: true))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			UnidadeProducao caracterizacao = _bus.ObterPorEmpreendimento(id);
			UnidadeProducaoVM vm = new UnidadeProducaoVM() { IsVisualizar = true };

			vm.UnidadeProducao = caracterizacao;
			vm.UrlRetorno = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id });

//            vm.Situacao

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult VisualizarCredenciado(int projetoDigitalId, int protocoloId = 0) 
		{
			ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
			ProjetoDigital projeto = _busProjetoDigitalCredenciado.Obter(projetoDigitalId);

			UnidadeProducao caracterizacao = _busCredenciado.ObterPorEmpreendimento(projeto.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId);
			UnidadeProducaoVM vm = new UnidadeProducaoVM() { IsVisualizar = true };

			vm.UnidadeProducao = caracterizacao;
			vm.UrlRetorno = Url.Action("Analisar", "../AnaliseItens", new { protocoloId = protocoloId, requerimentoId = projeto.RequerimentoId });

			return View("Visualizar", vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.UnidadeProducao.ExcluirMensagem;
			vm.Titulo = "Excluir Unidade de Produção";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
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

		#region Auxiliares Unidade de Produção Item

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar, ePermissao.UnidadeProducaoEditar })]
		public ActionResult AdicionarUnidadeProducao(int empreendimento, UnidadeProducaoItem unidade = null, bool visualizar = false)
		{
			UnidadeProducaoItemVM vm = new UnidadeProducaoItemVM();

			if (unidade != null)
			{
				vm.UnidadeProducaoItem = unidade;
			}

			vm.TiposCoordenada = ViewModelHelper.CriarSelectList(_listaBus.TiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);
			vm.Datuns = ViewModelHelper.CriarSelectList(_listaBus.Datuns.Where(x => x.Id == 1).ToList(), true, false);
			vm.Fusos = ViewModelHelper.CriarSelectList(_listaBus.Fusos.Where(x => x.Id == 24).ToList(), true, false);
			vm.Hemisferios = ViewModelHelper.CriarSelectList(_listaBus.Hemisferios.Where(x => x.Id == 1).ToList(), true, false);
			vm.LstCultivar = ViewModelHelper.CriarSelectList(_culturaBus.ObterLstCultivar(unidade.CulturaId), false, true);
			vm.LstProdutores = ViewModelHelper.CriarSelectList(_empreendimentoBus.ObterResponsaveis(empreendimento));

			vm.IsVisualizar = visualizar;

			return PartialView("UnidadeProducaoPartialItem", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar, ePermissao.UnidadeProducaoEditar })]
		public ActionResult ObterResponsavelTecnico(int credenciadoId, Cultura cultura, Cultivar cultivar)
		{
			HabilitarEmissaoCFOCFOC credenciadoHabilitado = _busHabilitacaoCFOCFOC.ObterPorCredenciado(credenciadoId);
			if (Validacao.EhValido)
			{
				credenciadoHabilitado.Pragas.ForEach(x =>
				{
					x.Praga.Culturas = _pragaBus.ObterCulturas(x.Praga.Id);
				});

				_validar.ValidarResponsavelTecnicoHabilitado(credenciadoHabilitado, cultura);
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Habilitacao = credenciadoHabilitado
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar, ePermissao.UnidadeProducaoEditar })]
		public ActionResult ValidarUnidadeProducaoItem(UnidadeProducaoItem unidade, int empreendimento)
		{
			HabilitarEmissaoCFOCFOC credenciadoHabilitado = null;
			List<Mensagem> Validacoes = new List<Mensagem>();
			Cultura cultura = new Cultura() { Id = unidade.CulturaId, Nome = unidade.CulturaTexto };

			foreach (var responsavel in unidade.ResponsaveisTecnicos)
			{
				credenciadoHabilitado = _busHabilitacaoCFOCFOC.ObterPorCredenciado(responsavel.Id);

				if (Validacao.EhValido)
				{
					credenciadoHabilitado.Pragas.ForEach(x =>
					{
						x.Praga.Culturas = _pragaBus.ObterCulturas(x.Praga.Id);
					});

					_validar.ValidarResponsavelTecnicoHabilitado(credenciadoHabilitado, cultura);
				}
			}

			Validacao.Erros.AddRange(_validar.SalvarItemUnidadeProducao(unidade, empreendimento));

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
			});
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
	}
}