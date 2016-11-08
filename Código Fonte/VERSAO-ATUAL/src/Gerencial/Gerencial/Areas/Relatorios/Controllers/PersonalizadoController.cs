using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.Security;
using Tecnomapas.Blocos.RelatorioPersonalizado.Business;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels;
using Tecnomapas.EtramiteX.Gerencial.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Gerencial.Controllers
{
	public class PersonalizadoController : DefaultController
	{
		#region Propriedades

		FatoBus _fatoBus = new FatoBus();
		RelatorioPersonalizadoBus _bus = new RelatorioPersonalizadoBus();

		public EtramiteIdentity UsuarioLogado
		{
			get { return (HttpContext.User.Identity as EtramiteIdentity); }
		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoListar })]
		public ActionResult Index()
		{
			PersonalizadoListarVM vm = new PersonalizadoListarVM(_fatoBus.ObterFonteDados());
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoListar })]
		public ActionResult Filtrar(PersonalizadoListarVM vm)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PersonalizadoListarVM>(vm.UltimaBusca).Filtros;
			}
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));

			Resultados<Relatorio> resultados = _bus.Filtrar(vm.Filtros);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeExecutar = User.IsInRole(ePermissao.RelatorioPersonalizadoExecutar.ToString());
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Executar

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult Executar(int id)
		{
			FuncionarioBus funcionarioBus = new FuncionarioBus();
			PersonalizadoExecutarVM vm = new PersonalizadoExecutarVM(funcionarioBus.ObterSetoresFuncionario(UsuarioLogado.FuncionarioId));
			vm.Relatorio = _bus.Obter(id);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult Executar(string paramsJson)
		{
			PersonalizadoExecutarVME parametro = ViewModelHelper.JsSerializer.Deserialize<PersonalizadoExecutarVME>(paramsJson);
			Arquivo arquivo = _bus.Executar(parametro.Id, parametro.Tipo, parametro.Setor, parametro.Termos);

			if (Validacao.EhValido)
			{
				return ViewModelHelper.GerarArquivo(arquivo);
			}

			#region Erro

			FuncionarioBus funcionarioBus = new FuncionarioBus();
			PersonalizadoExecutarVM vm = new PersonalizadoExecutarVM(funcionarioBus.ObterSetoresFuncionario(UsuarioLogado.FuncionarioId), parametro.Setor);
			vm.Relatorio = _bus.Obter(parametro.Id);

			vm.Relatorio.ConfiguracaoRelatorio.Termos.Where(x => x.DefinirExecucao).ToList().ForEach(x =>
			{
				x.Valor = (parametro.Termos.SingleOrDefault(y => y.Ordem == x.Ordem) ?? new Termo()).Valor;
			});

			return View("Executar", vm);

			#endregion
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioPersonalizadoExecutar })]
		public ActionResult ValidarExecutar(int id, int tipo, int setor, List<Termo> termos)
		{
			_bus.ValidarExecutar(id, tipo, setor, termos);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}