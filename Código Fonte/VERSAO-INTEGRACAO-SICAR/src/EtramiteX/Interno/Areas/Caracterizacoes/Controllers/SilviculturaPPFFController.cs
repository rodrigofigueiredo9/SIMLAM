using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
    public class SilviculturaPPFFController : DefaultController
    {
        #region Propriedades

        ListaBus _listaBus = new ListaBus();
        SilviculturaPPFFBus _bus = new SilviculturaPPFFBus();
        SilviculturaPPFFValidar _validar = new SilviculturaPPFFValidar();
        CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

        #endregion

        #region Criar

        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFCriar })]
        public ActionResult Criar(int id)
        {
            if (!_caracterizacaoValidar.Basicas(id))
            {
                return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
            }

            SilviculturaPPFF caracterizacao = new SilviculturaPPFF();
            caracterizacao.EmpreendimentoId = id;

            SilviculturaPPFFVM vm = new SilviculturaPPFFVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.Municipios(8));

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFCriar })]
        public ActionResult Criar(SilviculturaPPFF caracterizacao)
        {
            _bus.Salvar(caracterizacao);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Editar

        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.silviculturappff)]
        public ActionResult Editar(int id)
        {
            if (!_caracterizacaoValidar.Basicas(id))
            {
                return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
            }

            SilviculturaPPFF caracterizacao = _bus.ObterPorEmpreendimento(id);

            SilviculturaPPFFVM vm = new SilviculturaPPFFVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.Municipios(8), isVisualizar: false, isEditar: true);

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFEditar })]
        public ActionResult Editar(SilviculturaPPFF caracterizacao)
        {
            _bus.Salvar(caracterizacao);
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Visualizar

        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.silviculturappff)]
        public ActionResult Visualizar(int id)
        {
            SilviculturaPPFF caracterizacao = _bus.ObterPorEmpreendimento(id);
            SilviculturaPPFFVM vm = new SilviculturaPPFFVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.Municipios(8), true);
            return View(vm);
        }

        #endregion

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFExcluir })]
        public ActionResult ExcluirConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            vm.Id = id;
            vm.Mensagem = Mensagem.SilviculturaPPFF.ExcluirMensagem;
            vm.Titulo = "Excluir Silvicultura – Programa Produtor Florestal (Fomento) ";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.SilviculturaPPFFExcluir })]
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
