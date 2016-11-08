using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using BarragemDispensaLicencaCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business.BarragemDispensaLicencaBus;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class BarragemDispensaLicencaController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		BarragemDispensaLicencaBus _bus = new BarragemDispensaLicencaBus();
		BarragemDispensaLicencaValidar _validar = new BarragemDispensaLicencaValidar();
        BarragemDispensaLicencaCredenciadoBus _busCredenciado = new BarragemDispensaLicencaCredenciadoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			caracterizacao.EmpreendimentoID = id;
			AtividadeBus atividadeBus = new AtividadeBus();

			BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
				caracterizacao,
				atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
				_listaBus.BarragemDispensaLicencaFinalidadeAtividade,
				_listaBus.BarragemDispensaLicencaFormacaoRT,
				_listaBus.BarragemDispensaLicencaBarragemTipo,
				_listaBus.BarragemDispensaLicencaFase,
				_listaBus.BarragemDispensaLicencaMongeTipo,
				_listaBus.BarragemDispensaLicencaVertedouroTipo
			);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult Criar(BarragemDispensaLicenca caracterizacao)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragemdispensalicenca)]
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

			BarragemDispensaLicenca caracterizacao = _bus.ObterPorEmpreendimento(id);
			AtividadeBus atividadeBus = new AtividadeBus();

			BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
				caracterizacao,
				atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
				_listaBus.BarragemDispensaLicencaFinalidadeAtividade,
				_listaBus.BarragemDispensaLicencaFormacaoRT,
				_listaBus.BarragemDispensaLicencaBarragemTipo,
				_listaBus.BarragemDispensaLicencaFase,
				_listaBus.BarragemDispensaLicencaMongeTipo,
				_listaBus.BarragemDispensaLicencaVertedouroTipo
			);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaEditar })]
		public ActionResult Editar(BarragemDispensaLicenca caracterizacao)
		{
			_bus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragemdispensalicenca)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			BarragemDispensaLicenca caracterizacao = _bus.ObterPorEmpreendimento(id);
			AtividadeBus atividadeBus = new AtividadeBus();

			BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
				caracterizacao,
				atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
				_listaBus.BarragemDispensaLicencaFinalidadeAtividade,
				_listaBus.BarragemDispensaLicencaFormacaoRT,
				_listaBus.BarragemDispensaLicencaBarragemTipo,
				_listaBus.BarragemDispensaLicencaFase,
				_listaBus.BarragemDispensaLicencaMongeTipo,
				_listaBus.BarragemDispensaLicencaVertedouroTipo
			);

			vm.IsVisualizar = true;
            vm.UrlRetorno = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID });

			return View(vm);
		}

		#endregion

        #region Visualizar Credenciado

        [Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
        public ActionResult VisualizarCredenciado(int projetoDigitalId, int protocoloId = 0)
        {
            ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
            ProjetoDigital projeto = _busProjetoDigitalCredenciado.Obter(projetoDigitalId);

            BarragemDispensaLicenca caracterizacao = _busCredenciado.ObterPorEmpreendimento(projeto.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId);
            AtividadeBus atividadeBus = new AtividadeBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                _listaBus.BarragemDispensaLicencaFinalidadeAtividade,
                _listaBus.BarragemDispensaLicencaFormacaoRT,
                _listaBus.BarragemDispensaLicencaBarragemTipo,
                _listaBus.BarragemDispensaLicencaFase,
                _listaBus.BarragemDispensaLicencaMongeTipo,
                _listaBus.BarragemDispensaLicencaVertedouroTipo
            );

            vm.ProtocoloId = protocoloId;
            vm.ProjetoDigitalId = projeto.Id;
            vm.RequerimentoId = projeto.RequerimentoId;
            vm.UrlRetorno = Url.Action("Analisar", "../AnaliseItens", new { protocoloId = protocoloId, requerimentoId = projeto.RequerimentoId });

            return View("Visualizar", vm);
        }

        #endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.BarragemDispensaLicenca.ExcluirMensagem;
			vm.Titulo = "Excluir Barragem para Dispensa de Licença Ambiental";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
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