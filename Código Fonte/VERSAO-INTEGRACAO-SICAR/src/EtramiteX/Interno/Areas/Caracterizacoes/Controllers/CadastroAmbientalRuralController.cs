using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CadastroAmbientalRuralController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus(new CaracterizacaoValidar());
		CadastroAmbientalRuralBus _bus = new CadastroAmbientalRuralBus(new CadastroAmbientalRuralValidar());

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoBus.Validar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_bus.Validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			CadastroAmbientalRural caracterizacao = _bus.ObterTela(id);
			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);

			if (caracterizacao.Id <= 0)
			{
				EmpreendimentoCaracterizacao empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);
				caracterizacao.ModuloFiscalId = empreendimento.ModuloFiscalId;
				caracterizacao.ModuloFiscalHA = empreendimento.ModuloFiscalHA;
				caracterizacao.MunicipioId = empreendimento.MunicipioId;
				caracterizacao.EmpreendimentoId = id;
				caracterizacao.OcorreuAlteracaoApos2008 = -1;
			}

			if (!_bus.Validar.VerificarMunicipioForaES(caracterizacao.MunicipioId, _busLista.Municipios(ViewModelHelper.EstadoDefaultId())))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			CadastroAmbientalRuralVM vm = new CadastroAmbientalRuralVM(caracterizacao, _busLista.Municipios(ViewModelHelper.EstadoDefaultId()), _busLista.BooleanLista);
			vm.Abrangencia = _bus.ObterAgrangencia(id);

			if (caracterizacao.SituacaoProcessamento.Id == (int)eProjetoGeograficoSituacaoProcessamento.ProcessadoPDF)
			{
				_bus.ObterArquivosProjeto(caracterizacao.ProjetoGeoId).ForEach(arquivo =>
				{
					vm.ArquivosProcessamentoVM.Add(new ArquivoProcessamentoVM(arquivo, arquivo.Tipo, (int)eFilaTipoGeo.CAR));
				});
			}


			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			_bus.Validar.AbrirAcessar(caracterizacao);

			vm.UrlsArquivo = ViewModelHelper.Json(ObterUrlsArquivo());

			return View(vm);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoBus.Validar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_bus.Validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			CadastroAmbientalRural caracterizacao = _bus.ObterTela(id);

			string textoMerge = _caracterizacaoBus.Validar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.CadastroAmbientalRural,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}

			CadastroAmbientalRuralVM vm = new CadastroAmbientalRuralVM(caracterizacao, _busLista.Municipios(ViewModelHelper.EstadoDefaultId()), _busLista.BooleanLista);
			vm.Abrangencia = _bus.ObterAgrangencia(id);

			if (!_bus.Validar.VerificarMunicipioForaES(caracterizacao.MunicipioId, _busLista.Municipios(ViewModelHelper.EstadoDefaultId())))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			if (caracterizacao.SituacaoProcessamento.Id == (int)eProjetoGeograficoSituacaoProcessamento.ProcessadoPDF)
			{
				_bus.ObterArquivosProjeto(caracterizacao.ProjetoGeoId).ForEach(arquivo =>
				{
					vm.ArquivosProcessamentoVM.Add(new ArquivoProcessamentoVM(arquivo, arquivo.Tipo, (int)eFilaTipoGeo.CAR));
				});
			}

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			_bus.Validar.AbrirAcessar(caracterizacao);

			vm.UrlsArquivo = ViewModelHelper.Json(ObterUrlsArquivo());

			return View(vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public ActionResult Visualizar(int id)
		{
			if (!_bus.Validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			CadastroAmbientalRural caracterizacao = _bus.ObterTela(id);

			string textoMerge = _caracterizacaoBus.Validar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.CadastroAmbientalRural,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}

			CadastroAmbientalRuralVM vm = new CadastroAmbientalRuralVM(caracterizacao, _busLista.Municipios(ViewModelHelper.EstadoDefaultId()), _busLista.BooleanLista, isVisualizar: true);
			vm.Abrangencia = _bus.ObterAgrangencia(id);

			if (!_bus.Validar.VerificarMunicipioForaES(caracterizacao.MunicipioId, _busLista.Municipios(ViewModelHelper.EstadoDefaultId())))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			_bus.ObterArquivosProjeto(caracterizacao.ProjetoGeoId).ForEach(arquivo =>
			{
				vm.ArquivosProcessamentoVM.Add(new ArquivoProcessamentoVM(arquivo, arquivo.Tipo, (int)eFilaTipoGeo.CAR));
			});

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			vm.UrlsArquivo = ViewModelHelper.Json(ObterUrlsArquivo());

			return View(vm);
		}

		#endregion

		#region Finalizar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult Finalizar(CadastroAmbientalRural caracterizacao)
		{
			if (!_bus.Validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() });
			}

			string textoMerge = _caracterizacaoBus.Validar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.CadastroAmbientalRural,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Finalizar(caracterizacao);
			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.CadastroAmbientalRural.ExcluirMensagem;
			vm.Titulo = "Excluir Cadastro Ambiental Rural";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);

			string urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult GeoMergiar(CadastroAmbientalRural car)
		{
			CadastroAmbientalRuralVM vm = new CadastroAmbientalRuralVM(_bus.MergiarGeo(car), _busLista.Municipios(ViewModelHelper.EstadoDefaultId()), _busLista.BooleanLista);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "CadastroAmbientalRural", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Auxiliares

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult ObterModuloFiscal(int municipioID)
		{

			var retorno = _busLista.ModuloFiscal(municipioID);
			return Json(new { Retorno = new { Id = retorno.Id, Ha = retorno.Texto } }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult Processar(CadastroAmbientalRural caracterizacao)
		{
			_bus.Processar(caracterizacao);

			return Json(new
			{
				@arquivo = caracterizacao.Arquivo,
				@Caracterizacao = caracterizacao,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult ObterSituacaoProcessamento(int empreendimentoId)
		{
			Situacao situacao = _bus.ObterSituacaoProcessamento(empreendimentoId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Situacao = situacao,
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult ObterArquivosProcessamento(int projetoId, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = _bus.ObterArquivosProjeto(projetoId, finalizado) ?? new List<ArquivoProjeto>();

			arquivos = arquivos.Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui || x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoTrackMaker).ToList();

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Arquivos = arquivos,
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult Cancelar(CadastroAmbientalRural caracterizacao)
		{
			_bus.Cancelar(caracterizacao);

			return Json(new
			{
				@Situacao = caracterizacao.Situacao,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar })]
		public ActionResult ObterAreasProcessadas(int projetoId, decimal porcMaxRecuperar)
		{
			porcMaxRecuperar = (porcMaxRecuperar > 0) ? porcMaxRecuperar / 100 : porcMaxRecuperar;

			List<Area> areasProcessadas = _bus.ObterAreasProcessadas(projetoId, porcMaxRecuperar);

			return Json(new
			{
				AreasProcessadas = areasProcessadas,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
			}, JsonRequestBehavior.AllowGet);
		}

		public List<dynamic> ObterUrlsArquivo()
		{
			List<dynamic> list = new List<dynamic>();
			String[] nomes = Enum.GetNames(typeof(eProjetoGeograficoArquivoTipo));

			for (int i = 0; i < nomes.Length; i++)
			{
				list.Add(new { Tipo = (Int32)(Enum.Parse(typeof(eProjetoGeograficoArquivoTipo), nomes[i])), Url = Url.Action("Baixar" + nomes[i], "CadastroAmbientalRural", new { Area = "Caracterizacoes" }) });
			}

			return list;
		}

		#endregion

		#region Download Arquivos

		#region Croqui

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarcroquipdf, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarCroqui(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
		}

		#endregion

		#region BaseReferenciaInterna

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarbasereferencainterna, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarBaseReferenciaInterna(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region BaseReferenciaGeoBases

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarbasereferenciageobases, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarBaseReferenciaGeoBases(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region ArquivoEnviado

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoenviado, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarArquivoEnviado(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region RelatorioImportacao

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarrelatorioimportacao, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarRelatorioImportacao(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
		}

		#endregion

		#region ArquivoProcessadoGIS

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoprocessadogis, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarArquivoProcessadoGIS(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region ArquivoProcessadoTRACKMAKER

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralCriar, ePermissao.CadastroAmbientalRuralEditar, ePermissao.CadastroAmbientalRuralVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoprocessadotrackmaker, Artefato = (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural)]
		public FileResult BaixarArquivoProcessadoTRACKMAKER(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#endregion
	}
}