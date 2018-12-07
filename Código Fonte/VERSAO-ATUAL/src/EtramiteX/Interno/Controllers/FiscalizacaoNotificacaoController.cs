using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico;
using FiscalizacaoDa = Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data.FiscalizacaoDa;
using RelFiscalizacaoLib = Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;

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
			var cobranca = _busCobranca.Obter(vm.Notificacao.FiscalizacaoId);
			vm.UltimoParcelamento = cobranca?.Parcelamentos?.FindLast(x => x.Id > 0) ?? new CobrancaParcelamento();
			vm.PodeCriar = User.IsInRole(ePermissao.FiscalizacaoCriar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.FiscalizacaoEditar.ToString());

			return vm;
		}
	}
}
