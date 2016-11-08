using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.SharedVM;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class UnidadeProducaoController : DefaultController
	{
		#region Propriedades

		UnidadeProducaoBus _bus = new UnidadeProducaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoCredenciadoBus _empreendimentoBus = new EmpreendimentoCredenciadoBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		UnidadeProducaoValidar _validar = new UnidadeProducaoValidar();
		HabilitarEmissaoCFOCFOCBus _busHabilitacaoCFOCFOC = new HabilitarEmissaoCFOCFOCBus();
		PragaInternoBus _pragaBus = new PragaInternoBus();
		CulturaInternoBus _culturaBus = new CulturaInternoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar })]
		public ActionResult Criar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			UnidadeProducao caracterizacao = new UnidadeProducao();
			caracterizacao.Empreendimento.Id = id;

			if (!_validar.Acessar(caracterizacao.Empreendimento.Id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeProducaoVM vm = new UnidadeProducaoVM(caracterizacao);
			vm.UnidadeProducao.Empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);

			UnidadeProducaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeProducaoInternoBus();
			UnidadeProducao caracterizacaoInterno = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(vm.UnidadeProducao.Empreendimento.InternoID, true);

			if (caracterizacaoInterno.Id > 0)
			{
				vm.UnidadeProducao.PossuiCodigoPropriedade = caracterizacaoInterno.PossuiCodigoPropriedade;
				vm.UnidadeProducao.CodigoPropriedade = caracterizacaoInterno.CodigoPropriedade;
				vm.UnidadeProducao.InternoID = caracterizacaoInterno.Id;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar })]
		public ActionResult Criar(UnidadeProducao caracterizacao, int projetoDigitalId = 0)
		{
			_bus.Salvar(caracterizacao, projetoDigitalId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			if (!_validar.Acessar(id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeProducao caracterizacao = _bus.ObterPorEmpreendimento(id);
			UnidadeProducaoVM vm = new UnidadeProducaoVM(caracterizacao);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(UnidadeProducao caracterizacao, int projetoDigitalId)
		{
			Cultura cultura = null;
			Cultivar cultivar = null;

			foreach (var unidade in caracterizacao.UnidadesProducao)
			{
				foreach (var responsavel in unidade.ResponsaveisTecnicos)
				{
					cultura = new Cultura() { Id = unidade.CulturaId, Nome = unidade.CulturaTexto };
					cultivar = new Cultivar() { Id = unidade.CultivarId.GetValueOrDefault(), Nome = unidade.CultivarTexto };

					var validacaoResponsavelTecnico = ObterResponsavelTecnico(responsavel.Id, cultura, cultivar);
					if (!Validacao.EhValido)
					{
						return validacaoResponsavelTecnico;
					}
				}
			}

			_bus.Salvar(caracterizacao, projetoDigitalId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoVisualizar })]
		public ActionResult Visualizar(int id, int projetoDigitalId, bool retornarVisualizar = true)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			if (!_validar.Acessar(id, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			UnidadeProducao caracterizacao = _bus.ObterPorEmpreendimento(id, projetoDigitalId);
			UnidadeProducaoVM vm = new UnidadeProducaoVM(caracterizacao);
			vm.IsVisualizar = true;
			vm.RetornarVisualizar = retornarVisualizar;
			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
		public ActionResult ExcluirConfirm(int id, int projetoDigitalId)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.AuxiliarID = projetoDigitalId;
			vm.Mensagem = Mensagem.UnidadeProducao.ExcluirMensagem;
			vm.Titulo = "Excluir Unidade de Produção";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
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

		#region Auxiliares Unidade de Produção Item

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoCriar, ePermissao.UnidadeProducaoEditar })]
		public ActionResult AdicionarUnidadeProducao(int empreendimento, UnidadeProducaoItem unidade = null, bool visualizar = false)
		{
			UnidadeProducaoItemVM vm = new UnidadeProducaoItemVM(
				unidade, 
				ListaCredenciadoBus.TiposCoordenada, 
				ListaCredenciadoBus.Datuns, 
				ListaCredenciadoBus.Fusos, 
				ListaCredenciadoBus.Hemisferios, 
				_culturaBus.ObterLstCultivar(unidade.CulturaId),
				_empreendimentoBus.ObterResponsaveisLista(empreendimento));

			vm.IsVisualizar = visualizar;

			return PartialView("UnidadeProducaoItemPartial", vm);
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
		public ActionResult ValidarUnidadeProducaoItem(UnidadeProducaoItem unidade, int empreendimentoID)
		{
			HabilitarEmissaoCFOCFOC credenciadoHabilitado = null;

			List<Mensagem> Validacoes = new List<Mensagem>();

			Cultura cultura = new Cultura() { Id = unidade.CulturaId, Nome = unidade.CulturaTexto };

			CredenciadoPessoa credenciado = null;

			foreach (var responsavel in unidade.ResponsaveisTecnicos)
			{
				credenciadoHabilitado = _busHabilitacaoCFOCFOC.ObterPorCredenciado(responsavel.Id);

				if (Validacao.EhValido)
				{
					credenciadoHabilitado.Pragas.ForEach(x =>
					{
						x.Praga.Culturas = _pragaBus.ObterCulturas(x.Praga.Id);
					});
					credenciado = _credenciadoBus.Obter(responsavel.Id, true);
					_validar.ValidarResponsavelTecnicoHabilitado(credenciadoHabilitado, cultura);
				}
			}

			Validacao.Erros.AddRange(_validar.SalvarItemUnidadeProducao(unidade, empreendimentoID));

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

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult GerarFichaInscricaoPdf(int id)
		{
			try
			{
				MemoryStream resultado = null;

				resultado = new PdfFichaInscricaoUnidadeProducao().Gerar(id);

				return ViewModelHelper.GerarArquivo("Ficha de Inscricao da Unidade de Producao.pdf", resultado, "application/pdf");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = id }));
			}
		}
	}
}