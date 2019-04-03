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
using System.Collections.Generic;

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

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.barragemdispensalicenca)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}
			
			BarragemDispensaLicenca caracterizacao = _bus.Obter(id);
			AtividadeBus atividadeBus = new AtividadeBus();

			//var rtElaborador = _bus.ObterResponsavelTecnicoRequerimento(caracterizacao.responsaveisTecnicos, caracterizacao.RequerimentoId)[1].proprioDeclarante;

			BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
				caracterizacao,
				atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
				_listaBus.BarragemDispensaLicencaFinalidadeAtividade,
				_listaBus.BarragemDispensaLicencaFormacaoRT,
				_listaBus.BarragemDispensaLicencaBarragemTipo,
				_listaBus.BarragemDispensaLicencaFase,
				_listaBus.BarragemDispensaLicencaMongeTipo,
				_listaBus.BarragemDispensaLicencaVertedouroTipo,
				_listaBus.Profissoes
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
                _listaBus.BarragemDispensaLicencaVertedouroTipo,
				_listaBus.Profissoes
			);

            vm.ProtocoloId = protocoloId;
            vm.ProjetoDigitalId = projeto.Id;
            vm.RequerimentoId = projeto.RequerimentoId;
            vm.UrlRetorno = Url.Action("Analisar", "../AnaliseItens", new { protocoloId = protocoloId, requerimentoId = projeto.RequerimentoId });

            return View("Visualizar", vm);
        }

        #endregion

		#region Excluir

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
		public ActionResult ExcluirConfirm(int titulo)
		{
			_validar.Excluir(titulo);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Titulo = "Excluir Barragem para Dispensa de Licença Ambiental",
				@Conteudo = Mensagem.BarragemDispensaLicenca.ExcluirMensagem.Texto,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
		public ActionResult Excluir(int barragemId, int empreendimento)
		{
			string urlRedirecionar = string.Empty;
			if (_bus.Excluir(barragemId))
			{
				urlRedirecionar = Url.Action("Listar", "BarragemDispensaLicenca", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult Listar(int id, bool isVisualizar = false)
		{
			if (!_validar.Acessar(id) || !_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			var projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			List<BarragemDispensaLicenca> caracterizacoes = new List<BarragemDispensaLicenca>();
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
				_listaBus.BarragemDispensaLicencaVertedouroTipo,
				_listaBus.Profissoes
			);

			vm.CaracterizacoesCadastradas = _bus.ObterListar(id);
			vm.IsVisualizar = isVisualizar;
			return View(vm);
		}

		#endregion
	}
}