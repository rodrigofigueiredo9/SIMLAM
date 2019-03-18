using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
    public class BarragemDispensaLicencaController : DefaultController
    {
        #region Propriedades

        CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
        BarragemDispensaLicencaBus _bus = new BarragemDispensaLicencaBus();
        BarragemDispensaLicencaValidar _validar = new BarragemDispensaLicencaValidar();

        #endregion

        #region Criar

        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
        public ActionResult Criar(int id, int projetoDigitalId)
        {
            if (!_caracterizacaoValidar.Basicas(id))
            {
                return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
            }

            BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			caracterizacao.EmpreendimentoID = id;
			caracterizacao.responsaveisTecnicos = _bus.ObterResponsavelTecnicoRequerimento(caracterizacao.responsaveisTecnicos, projetoDigitalId);
			caracterizacao.barragemContiguaMesmoNivel = _bus.ObterBarragemContiguaMesmoNivel(projetoDigitalId);

			AtividadeInternoBus atividadeBus = new AtividadeInternoBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
                ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
                ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaFase,
                ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo,
				ListaCredenciadoBus.Profissoes
            );
			vm.rtElaborador = caracterizacao.responsaveisTecnicos[1].proprioDeclarante;
		
			return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
        public ActionResult Criar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId = 0)
        {
            _bus.Salvar(caracterizacao, projetoDigitalId);
			AssociarCaracterizacaoProjetoDigital(projetoDigitalId, caracterizacao.Id, caracterizacao.Tid);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult SalvarConfirm(BarragemDispensaLicenca caracterizacao, int projetoDigitalId = 0)
		{
			_bus.ValidarSalvar(caracterizacao, projetoDigitalId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}
		
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult Salvar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId = 0)
		{
			if(caracterizacao.Id <= 0)
				AssociarCaracterizacaoProjetoDigital(projetoDigitalId, caracterizacao.Id, caracterizacao.Tid);
			if(Validacao.EhValido)
				_bus.Salvar(caracterizacao, projetoDigitalId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaEditar })]
        public ActionResult Editar(int id, int empreendimentoId, int projetoDigitalId, int associada = 0)
        {
            if (!_caracterizacaoValidar.Basicas(empreendimentoId))
            {
                return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { empreendimentoId = projetoDigitalId, area = "" }));
            }

            if (!_validar.Acessar(empreendimentoId, projetoDigitalId))
            {
                return RedirectToAction("", "Caracterizacao", new { id = empreendimentoId, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
            }

            BarragemDispensaLicenca caracterizacao = _bus.Obter(id);

			var rtElaborador = _bus.ObterResponsavelTecnicoRequerimento(caracterizacao.responsaveisTecnicos, projetoDigitalId)[1].proprioDeclarante;
            AtividadeInternoBus atividadeBus = new AtividadeInternoBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
                ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
                ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaFase,
                ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo,
				ListaCredenciadoBus.Profissoes
            );
			vm.rtElaborador = rtElaborador;


			return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaEditar })]
        public ActionResult Editar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId)
        {
            _bus.Salvar(caracterizacao, projetoDigitalId);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoID, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Visualizar

        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaVisualizar })]
        public ActionResult Visualizar(int id, int empreendimentoId, int projetoDigitalId)
        {
			if (!_validar.Acessar(empreendimentoId, projetoDigitalId))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			if (!_validar.Acessar(empreendimentoId, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			BarragemDispensaLicenca caracterizacao = _bus.Obter(id);
            AtividadeInternoBus atividadeBus = new AtividadeInternoBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
                ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
                ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaFase,
                ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo,
				ListaCredenciadoBus.Profissoes
            );

            vm.IsVisualizar = true;

            return View(vm);
        }

        #endregion

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
        public ActionResult ExcluirConfirm(int id, int projetoDigitalId)
        {
            ConfirmarVM vm = new ConfirmarVM();
            vm.Id = id;
            vm.AuxiliarID = projetoDigitalId;
            vm.Mensagem = Mensagem.BarragemDispensaLicenca.ExcluirMensagem;
            vm.Titulo = "Excluir Barragem para Dispensa de Licença Ambiental";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaExcluir })]
        public ActionResult Excluir(int id, int empreendimento, int projetoDigitalId)
        {
            string urlRedireciona = string.Empty;
            if (_bus.ExcluirCarac(empreendimento) && _bus.Excluir(id))
            {
                urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = empreendimento, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
		public ActionResult Listar(int id, int projetoDigitalId, bool isVisualizar = false)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}
			var projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			List<BarragemDispensaLicenca> caracterizacoes = new List<BarragemDispensaLicenca>();
			caracterizacao.EmpreendimentoID = id;
			
			if (!_validar.Acessar(caracterizacao.EmpreendimentoID, projetoDigitalId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			AtividadeInternoBus atividadeBus = new AtividadeInternoBus();
			
			BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
				caracterizacao,
				atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
				ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
				ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
				ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
				ListaCredenciadoBus.BarragemDispensaLicencaFase,
				ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
				ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo,
				ListaCredenciadoBus.Profissoes
			);

			BarragemDispensaLicenca barragemAssociada = new BarragemDispensaLicenca();
			vm.IsVisualizar = isVisualizar;
			vm.Caracterizacao.PossuiAssociacaoExterna = _bus.PossuiAssociacaoExterna(id, projetoDigitalId);
			vm.CaracterizacoesCadastradas = _bus.ObterListar(id, projetoDigitalId);

			int result = _bus.ObterBarragemAssociada(projetoDigitalId).Count;
			if (result != 0)
				vm.CaracterizacoesAssociadas = _bus.ObterBarragemAssociada(projetoDigitalId);

			// Retira as barragens cadastradas que foram associadas a esse projeto digital
			vm.CaracterizacoesCadastradas = vm.CaracterizacoesCadastradas
							.Where(x => x.Id != vm.CaracterizacoesAssociadas.FirstOrDefault().Associado).ToList();
			return View(vm);
		}

		#endregion

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AssociarCaracterizacaoProjetoDigital(int projetoDigitalId, int caracterizacao, string tid)
		{
			var barragemId = _bus.AssociarNovaCaracterizacao(caracterizacao, projetoDigitalId);

			var projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			var projetoDigital = projetoDigitalBus.Obter(projetoDigitalId);
			projetoDigital.Dependencias.Add(new Dependencia()
			{
				DependenciaTipo = (int)eCaracterizacaoDependenciaTipo.Caracterizacao,
				DependenciaCaracterizacao = (int)eCaracterizacao.BarragemDispensaLicenca,
				DependenciaId = barragemId,
				DependenciaTid = tid
			});
			projetoDigitalBus.AssociarDependencias(projetoDigital, barragemId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Listar", "BarragemDispensaLicenca", new { id = projetoDigital.EmpreendimentoId, projetoDigitalId = projetoDigital.Id, associada = caracterizacao, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult DesassociarCaracterizacaoProjetoDigital(int projetoDigitalId, int caracterizacao)
		{
			var projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			var projetoDigital = projetoDigitalBus.Obter(projetoDigitalId);
			projetoDigitalBus.DesassociarDependencias(projetoDigital);
			if (!_bus.PossuiAssociacaoExterna(projetoDigital.EmpreendimentoId.GetValueOrDefault(0), projetoDigital.Id))
					_bus.Excluir(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Listar", "BarragemDispensaLicenca", new { id = projetoDigital.EmpreendimentoId, projetoDigitalId = projetoDigital.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}
	}
}