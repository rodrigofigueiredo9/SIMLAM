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
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class InformacaoCorteController : DefaultController
	{
		CaracterizacaoBus _bus = new CaracterizacaoBus(new CaracterizacaoValidar());
		InformacaoCorteBus _informacaoCorteBus = new InformacaoCorteBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		InformacaoCorteValidar _validar = new InformacaoCorteValidar();

		#region Criar

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Criar(int id, int projetoDigitalId, bool visualizar = false)
		{
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var informacaoCorteVM = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>());
			return View(informacaoCorteVM);
		}

		[HttpPost]
		public ActionResult Criar(InformacaoCorte caracterizacao, int projetoDigitalId = 0)
		{
			_informacaoCorteBus.Salvar(caracterizacao, projetoDigitalId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
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

			var caracterizacao = _informacaoCorteBus.ObterPorEmpreendimento(id);
			var vm = new InformacaoCorteVM(caracterizacao.Empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				ProjetoDigitalId = projetoDigitalId
			};

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(InformacaoCorte caracterizacao, int projetoDigitalId)
		{
			_informacaoCorteBus.Salvar(caracterizacao, projetoDigitalId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}