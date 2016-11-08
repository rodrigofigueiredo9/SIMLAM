using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
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
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo
            );

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaCriar })]
        public ActionResult Criar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId = 0)
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

        #region Editar

        [Permite(RoleArray = new Object[] { ePermissao.BarragemDispensaLicencaEditar })]
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

            BarragemDispensaLicenca caracterizacao = _bus.ObterPorEmpreendimento(id);
            AtividadeInternoBus atividadeBus = new AtividadeInternoBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
                ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
                ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaFase,
                ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo
            );

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
        public ActionResult Visualizar(int id, int projetoDigitalId)
        {
            if (!_validar.Acessar(id, projetoDigitalId))
            {
                return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
            }

            if (!_validar.Acessar(id, projetoDigitalId))
            {
                return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
            }

            BarragemDispensaLicenca caracterizacao = _bus.ObterPorEmpreendimento(id, projetoDigitalId);
            AtividadeInternoBus atividadeBus = new AtividadeInternoBus();

            BarragemDispensaLicencaVM vm = new BarragemDispensaLicencaVM(
                caracterizacao,
                atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado),
                ListaCredenciadoBus.BarragemDispensaLicencaFinalidadeAtividade,
                ListaCredenciadoBus.BarragemDispensaLicencaFormacaoRT,
                ListaCredenciadoBus.BarragemDispensaLicencaBarragemTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaFase,
                ListaCredenciadoBus.BarragemDispensaLicencaMongeTipo,
                ListaCredenciadoBus.BarragemDispensaLicencaVertedouroTipo
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
    }
}