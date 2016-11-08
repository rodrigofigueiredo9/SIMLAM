using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.SharedVM;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using System.IO;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Pdf;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class UnidadeConsolidacaoController : DefaultController
	{

		#region Propriedades

		UnidadeConsolidacaoBus _bus = new UnidadeConsolidacaoBus();
		ListaBus _listaBus = new ListaBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoCredenciadoBus _empreendimentoBus = new EmpreendimentoCredenciadoBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		UnidadeConsolidacaoValidar _validar = new UnidadeConsolidacaoValidar();
		UnidadeProducaoValidar _validarUnidadeProducao = new UnidadeProducaoValidar();
		PragaInternoBus _pragaBus = new PragaInternoBus();
		CulturaInternoBus _culturaBus = new CulturaInternoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar })]
		public ActionResult Criar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();
			caracterizacao.Empreendimento.Id = id;

			if (!_validar.Acessar(caracterizacao.Empreendimento.Id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());
			vm.LstCultivar = ViewModelHelper.CriarSelectList(new List<Lista>());

			vm.UnidadeConsolidacao = caracterizacao;
			vm.UnidadeConsolidacao.Empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);

			UnidadeConsolidacaoInternoBus internoBus = new UnidadeConsolidacaoInternoBus();
			UnidadeConsolidacao caracterizacaoInterno = internoBus.ObterPorEmpreendimento(vm.UnidadeConsolidacao.Empreendimento.InternoID, true);

			if (caracterizacaoInterno.Id > 0)
			{
				vm.UnidadeConsolidacao.CodigoUC = caracterizacaoInterno.CodigoUC;
				vm.UnidadeConsolidacao.PossuiCodigoUC = caracterizacaoInterno.PossuiCodigoUC;
				vm.FoiCopiada = true;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoCriar })]
		public ActionResult Criar(UnidadeConsolidacao caracterizacao, int projetoDigitalId = 0)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_validar.Acessar(id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeConsolidacao caracterizacao = _bus.ObterPorEmpreendimento(id);
			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.UnidadeConsolidacao = caracterizacao;
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());
			vm.FoiCopiada = _bus.FoiCopiada(caracterizacao.Id);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoEditar })]
		public ActionResult Editar(UnidadeConsolidacao caracterizacao, int projetoDigitalId)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoVisualizar })]
		public ActionResult Visualizar(int id, int projetoDigitalId,bool retornarVisualizar = true)
		{
			if (!_validar.Acessar(id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeConsolidacao caracterizacao = _bus.ObterPorEmpreendimento(id, projetoDigitalId);

			UnidadeConsolidacaoVM vm = new UnidadeConsolidacaoVM();
			vm.IsVisualizar = true;
			vm.RetornarVisualizar = retornarVisualizar;
			vm.UnidadeConsolidacao = caracterizacao;
			vm.LstUnidadeMedida = ViewModelHelper.CriarSelectList(_bus.ObterListaUnidadeMedida());

			return View(vm);
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
		public ActionResult ExcluirConfirm(int id, int projetoDigitalId)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.AuxiliarID = projetoDigitalId;
			vm.Mensagem = Mensagem.UnidadeConsolidacao.ExcluirMensagem;
			vm.Titulo = "Excluir Unidade de Consolidação";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoExcluir })]
		public ActionResult Excluir(int id, int projetoDigitalId)
		{
			string urlRedireciona = string.Empty;

			if (_bus.Excluir(id))
			{
				urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeConsolidacaoVisualizar })]
		public ActionResult GerarFichaInscricaoPdf(int id)
		{
			try
			{
				MemoryStream resultado = null;

				resultado = new PdfFichaInscricaoUnidadeConsolidacao().Gerar(id);

				return ViewModelHelper.GerarArquivo("Ficha de Inscricao da Unidade de Consolidacao.pdf", resultado, "application/pdf");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = id }));
			}
		}
	}
}