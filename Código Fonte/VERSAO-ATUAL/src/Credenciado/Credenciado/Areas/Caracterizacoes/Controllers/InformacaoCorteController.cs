using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class InformacaoCorteController : DefaultController
	{
		InformacaoCorteBus _bus = new InformacaoCorteBus();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus(new CaracterizacaoValidar());

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Criar(int id, int projetoDigitalId, bool visualizar = false)
		{
			InformacaoCorte caracterizacao = _bus.ObterPorEmpreendimento(id) ?? new InformacaoCorte();
			caracterizacao.EmpreendimentoId = id;

			if (caracterizacao.Id > 0)
			{
				return RedirectToAction("Editar", new { id = caracterizacao.EmpreendimentoId });
			}

			//if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			//{
			//	return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			//}

			//caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			var empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);
			InformacaoCorteVM vm = new InformacaoCorteVM(empreendimento);
			//vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#region InformacaoDeCorteInformacao
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult InformacaoCorteCriar(int ?id)
		{
			if (_bus.ValidarCriarInfCorte(0, 0))
			{
				var empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id??0);
				var informacaoCorteVM = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
					ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>());
				return View("InformacaoCorteCriar", informacaoCorteVM);
			}

			Validacao.Add(Mensagem.InformacaoCorte.AreaCorteObrigatoria);
			return Json(new
			{
				@Html = String.Empty,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}