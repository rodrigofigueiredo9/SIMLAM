using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class FiscalizacaoNotificacaoController : DefaultController
	{
		#region Propriedades

		NotificacaoBus _bus = new NotificacaoBus();
		FiscalizacaoBus _busFiscalizacao = new FiscalizacaoBus();
		CobrancaBus _busCobranca = new CobrancaBus();
		ListaBus _busLista = new ListaBus();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
		public ActionResult Notificacao(int id)
		{
			if(!_bus.ValidarAcesso(_busFiscalizacao.Obter(id)))
				return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
			
			return View(this.GetNotificacaoVM(id, false));
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
		public ActionResult NotificacaoVisualizar(int id) => View(this.GetNotificacaoVM(id, true));

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
		public ActionResult NotificacaoCriar(Notificacao notificacao)
		{
			_bus.Salvar(notificacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("NotificacaoVisualizar", "FiscalizacaoNotificacao", new { id = notificacao.FiscalizacaoId, Msg = Validacao.QueryParam(new List<Mensagem>() { Mensagem.NotificacaoMsg.Salvar }) })
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
		public ActionResult GetNotificacaoId(int id) => Json(new { id = _bus.ObterId(id), Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);

		private NotificacaoVM GetNotificacaoVM(int id, bool visualizar)
		{
			var vm = new NotificacaoVM();

			vm.fiscalizacaoId = id;
			vm.Notificacao = _bus.Obter(id);
			vm.ArquivoVM.Anexos = vm.Notificacao.Anexos ?? new List<Anexo>();
			vm.IsVisualizar = visualizar;
			vm.ArquivoVM.IsVisualizar = visualizar;
			var cobranca = _busCobranca.ObterByFiscalizacao(vm.Notificacao.FiscalizacaoId);
			vm.UltimoParcelamento = cobranca?.Parcelamentos?.FindLast(x => x.Id > 0) ?? new CobrancaParcelamento();
			vm.PodeCriar = User.IsInRole(ePermissao.FiscalizacaoCriar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.FiscalizacaoEditar.ToString());

			return vm;
		}
	}
}
