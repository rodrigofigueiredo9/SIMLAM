using System;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class HomeController : DefaultController
	{
		public ActionResult Index()
		{
			if (User == null || !User.Identity.IsAuthenticated)
			{
				return RedirectToAction("LogOn", "Autenticacao");
			}

			return View();
		}

		public ActionResult SistemaMensagem()
		{
			return View();
		}

		#region Teste

		public ActionResult Teste()
		{
			Convert.ToInt64("");
			return View();
		}

        public ActionResult AvisoManutencao()
        {
            return PartialView("AvisoManutencaoPartial");
        }

            

		public string ValoresCache()
		{
			//Configuracao.GerenciadorConfiguracao _confg = new Configuracao.GerenciadorConfiguracao(new Configuracao.ConfiguracaoFuncionario());
			//Configuracao.GerenciadorConfiguracao _confgTitulo = new Configuracao.GerenciadorConfiguracao(new Configuracao.ConfiguracaoTitulo());
			//List<Configuracao.Situacao> lstSituacaoTitulo = _confgTitulo.Obter<List<Configuracao.Situacao>>(Configuracao.ConfiguracaoTitulo.KeySituacoes);
			return String.Empty;
		}

		#endregion
	}
}